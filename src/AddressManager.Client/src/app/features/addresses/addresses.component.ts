/**
 * AddressesComponent — главный экран приложения.
 *
 * Возможности:
 *   - таблица адресов с фильтрацией и сортировкой
 *   - кнопка «Добавить адрес» показывает inline-форму
 *   - после сохранения новый адрес добавляется в таблицу без перезагрузки
 */

import { Component, OnInit, inject, signal, computed } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { AddressService } from '../../core/services/address.service';
import { Address } from '../../core/models/address.model';
import { AddressFormComponent } from './address-form/address-form.component';

@Component({
  selector: 'app-addresses',
  standalone: true,
  imports: [CommonModule, FormsModule, AddressFormComponent],
  templateUrl: './addresses.component.html',
  styleUrl: './addresses.component.scss'
})
export class AddressesComponent implements OnInit {
  private readonly service = inject(AddressService);

  // ── Состояние таблицы ────────────────────────────────────────────────────────

  /** Полный список адресов, загруженных с сервера. */
  addresses = signal<Address[]>([]);

  /** true пока идёт первый запрос к API. */
  loading = signal(true);

  /** Сообщение об ошибке загрузки. null — ошибок нет. */
  error = signal<string | null>(null);

  /** Текущее значение строки поиска. */
  search = signal('');

  /** Поле, по которому выполняется сортировка. */
  sortField = signal<keyof Address>('country');

  /** true — ASC (А→Я), false — DESC (Я→А). */
  sortAsc = signal(true);

  // ── Состояние формы ──────────────────────────────────────────────────────────

  /** true — форма создания адреса видима. */
  showForm = signal(false);

  // ── Производное состояние ────────────────────────────────────────────────────

  /**
   * Отфильтрованный и отсортированный список для таблицы.
   * Пересчитывается автоматически при изменении addresses / search / sortField / sortAsc.
   */
  filtered = computed(() => {
    const term = this.search().toLowerCase().trim();
    let list = this.addresses();

    if (term) {
      list = list.filter(a =>
        a.street.toLowerCase().includes(term)    ||
        a.city.toLowerCase().includes(term)      ||
        a.region.toLowerCase().includes(term)    ||
        a.country.toLowerCase().includes(term)   ||
        a.postalCode.toLowerCase().includes(term)
      );
    }

    const field = this.sortField();
    const asc   = this.sortAsc() ? 1 : -1;

    return [...list].sort((a, b) => {
      const va = (a[field] ?? '') as string;
      const vb = (b[field] ?? '') as string;
      return va.localeCompare(vb) * asc;
    });
  });

  // ── Жизненный цикл ───────────────────────────────────────────────────────────

  ngOnInit(): void {
    this.service.getAll().subscribe({
      next:  data => { this.addresses.set(data); this.loading.set(false); },
      error: err  => { this.error.set('Не удалось загрузить данные: ' + err.message); this.loading.set(false); }
    });
  }

  // ── Управление формой ────────────────────────────────────────────────────────

  /** Показывает форму создания адреса. */
  openForm(): void {
    this.showForm.set(true);
  }

  /**
   * Вызывается когда форма успешно сохранила адрес.
   * Добавляем его в начало списка (он окажется сверху таблицы до пересортировки)
   * и скрываем форму — без повторного запроса к API.
   */
  onAddressSaved(created: Address): void {
    this.addresses.update(list => [created, ...list]);
    this.showForm.set(false);
  }

  /** Вызывается при нажатии «Отмена» в форме. */
  onFormCancel(): void {
    this.showForm.set(false);
  }

  // ── Сортировка ───────────────────────────────────────────────────────────────

  sort(field: keyof Address): void {
    if (this.sortField() === field) {
      this.sortAsc.update(v => !v);
    } else {
      this.sortField.set(field);
      this.sortAsc.set(true);
    }
  }

  sortIcon(field: keyof Address): string {
    if (this.sortField() !== field) return '↕';
    return this.sortAsc() ? '↑' : '↓';
  }

  // ── Вспомогательные методы ───────────────────────────────────────────────────

  fullAddress(a: Address): string {
    const apt = a.apartmentNumber ? `, кв. ${a.apartmentNumber}` : '';
    return `${a.street}, ${a.houseNumber}${apt}`;
  }

  flagEmoji(code: string): string {
    return code.toUpperCase().replace(/./g, c =>
      String.fromCodePoint(c.charCodeAt(0) + 127397)
    );
  }
}
