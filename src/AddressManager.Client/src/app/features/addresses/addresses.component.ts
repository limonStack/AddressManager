import { Component, OnInit, inject, signal, computed } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { AddressService } from '../../core/services/address.service';
import { Address } from '../../core/models/address.model';

@Component({
  selector: 'app-addresses',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './addresses.component.html',
  styleUrl: './addresses.component.scss'
})
export class AddressesComponent implements OnInit {
  private readonly service = inject(AddressService);

  addresses = signal<Address[]>([]);
  loading   = signal(true);
  error     = signal<string | null>(null);
  search    = signal('');
  sortField = signal<keyof Address>('country');
  sortAsc   = signal(true);

  filtered = computed(() => {
    const term = this.search().toLowerCase().trim();
    let list = this.addresses();

    if (term) {
      list = list.filter(a =>
        a.street.toLowerCase().includes(term)        ||
        a.city.toLowerCase().includes(term)          ||
        a.region.toLowerCase().includes(term)        ||
        a.country.toLowerCase().includes(term)       ||
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

  ngOnInit(): void {
    this.service.getAll().subscribe({
      next:  data => { this.addresses.set(data); this.loading.set(false); },
      error: err  => { this.error.set('Не удалось загрузить данные: ' + err.message); this.loading.set(false); }
    });
  }

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

  fullAddress(a: Address): string {
    const apt = a.apartmentNumber ? `, кв. ${a.apartmentNumber}` : '';
    return `${a.street}, ${a.houseNumber}${apt}`;
  }

  flagEmoji(code: string): string {
    // Конвертируем ISO 3166-1 alpha-2 в emoji флага
    return code.toUpperCase().replace(/./g, c =>
      String.fromCodePoint(c.charCodeAt(0) + 127397)
    );
  }
}
