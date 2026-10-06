/**
 * AddressesComponent — главный (и единственный) экран приложения.
 *
 * Отображает таблицу адресов с возможностью:
 *   - фильтрации по любому текстовому полю (улица, город, регион, страна, индекс)
 *   - сортировки по любому столбцу (клик по заголовку)
 *
 * Использует Angular Signals (signal / computed) вместо классических Subject/BehaviorSubject,
 * что даёт автоматическое реактивное обновление шаблона без явного ChangeDetection.
 */

import { Component, OnInit, inject, signal, computed } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { AddressService } from '../../core/services/address.service';
import { Address } from '../../core/models/address.model';

@Component({
  selector: 'app-addresses',
  standalone: true,                           // Standalone-компонент: не нужен NgModule
  imports: [CommonModule, FormsModule],       // CommonModule — *ngIf, *ngFor; FormsModule — [(ngModel)]
  templateUrl: './addresses.component.html',
  styleUrl: './addresses.component.scss'
})
export class AddressesComponent implements OnInit {
  // inject() внутри класса — современный аналог constructor(private service: AddressService)
  private readonly service = inject(AddressService);

  // ── Состояние (Signals) ─────────────────────────────────────────────────────

  /** Полный список адресов, загруженных с сервера. */
  addresses = signal<Address[]>([]);

  /** true пока идёт первый запрос к API. */
  loading = signal(true);

  /** Сообщение об ошибке, если запрос к API упал. null — ошибок нет. */
  error = signal<string | null>(null);

  /** Текущее значение строки поиска (привязано к input через ngModel). */
  search = signal('');

  /** Поле Address, по которому в данный момент выполняется сортировка. */
  sortField = signal<keyof Address>('country');

  /** true — сортировка по возрастанию (А→Я), false — по убыванию (Я→А). */
  sortAsc = signal(true);

  // ── Производное состояние ───────────────────────────────────────────────────

  /**
   * Отфильтрованный и отсортированный список адресов для отображения в таблице.
   *
   * computed() автоматически пересчитывается при изменении любого из signals,
   * на которые он ссылается: addresses, search, sortField, sortAsc.
   */
  filtered = computed(() => {
    const term = this.search().toLowerCase().trim();
    let list = this.addresses();

    // Фильтрация: оставляем только те адреса, где хотя бы одно поле содержит term
    if (term) {
      list = list.filter(a =>
        a.street.toLowerCase().includes(term)     ||
        a.city.toLowerCase().includes(term)       ||
        a.region.toLowerCase().includes(term)     ||
        a.country.toLowerCase().includes(term)    ||
        a.postalCode.toLowerCase().includes(term)
      );
    }

    const field = this.sortField();
    const asc   = this.sortAsc() ? 1 : -1; // множитель: 1 для ASC, -1 для DESC

    // Сортировка: spread [...list] чтобы не мутировать исходный массив сигнала
    return [...list].sort((a, b) => {
      const va = (a[field] ?? '') as string;
      const vb = (b[field] ?? '') as string;
      return va.localeCompare(vb) * asc; // localeCompare — корректное сравнение строк с учётом языка
    });
  });

  // ── Жизненный цикл ──────────────────────────────────────────────────────────

  /** Загружаем данные при инициализации компонента. */
  ngOnInit(): void {
    this.service.getAll().subscribe({
      next:  data => { this.addresses.set(data); this.loading.set(false); },
      error: err  => { this.error.set('Не удалось загрузить данные: ' + err.message); this.loading.set(false); }
    });
  }

  // ── Методы сортировки ───────────────────────────────────────────────────────

  /**
   * Обрабатывает клик по заголовку столбца:
   * - повторный клик по тому же полю — переключает направление
   * - клик по новому полю — сортируем по нему по возрастанию
   */
  sort(field: keyof Address): void {
    if (this.sortField() === field) {
      this.sortAsc.update(v => !v); // инвертируем направление
    } else {
      this.sortField.set(field);
      this.sortAsc.set(true);       // новое поле — всегда с ASC
    }
  }

  /**
   * Возвращает символ-стрелку для заголовка столбца:
   * ↕ — столбец не активен, ↑ — ASC, ↓ — DESC.
   */
  sortIcon(field: keyof Address): string {
    if (this.sortField() !== field) return '↕';
    return this.sortAsc() ? '↑' : '↓';
  }

  // ── Вспомогательные методы ──────────────────────────────────────────────────

  /**
   * Форматирует строку адреса: "улица, дом" или "улица, дом, кв. N".
   */
  fullAddress(a: Address): string {
    const apt = a.apartmentNumber ? `, кв. ${a.apartmentNumber}` : '';
    return `${a.street}, ${a.houseNumber}${apt}`;
  }

  /**
   * Конвертирует ISO 3166-1 alpha-2 код страны в флаг-эмодзи.
   *
   * Принцип: буквы A-Z имеют региональные индикаторы в Unicode (U+1F1E6..U+1F1FF).
   * Смещение от ASCII 'A' (65) до первого регионального символа (127462) = 127397.
   * Два таких символа подряд браузер рендерит как флаг: "UA" → 🇺🇦
   */
  flagEmoji(code: string): string {
    return code.toUpperCase().replace(/./g, c =>
      String.fromCodePoint(c.charCodeAt(0) + 127397)
    );
  }
}
