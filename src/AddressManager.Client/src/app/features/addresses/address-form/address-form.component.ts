/**
 * AddressFormComponent — форма создания нового адреса.
 *
 * Использует ReactiveFormsModule для валидации полей.
 * Загружает список городов из CityService при инициализации.
 *
 * Взаимодействие с родителем через Output-события:
 *   saved  — пользователь сохранил адрес, передаёт созданный Address
 *   cancel — пользователь отменил заполнение формы
 */

import { Component, OnInit, Output, EventEmitter, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { AddressService } from '../../../core/services/address.service';
import { CityService } from '../../../core/services/city.service';
import { Address } from '../../../core/models/address.model';
import { City } from '../../../core/models/city.model';

@Component({
  selector: 'app-address-form',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './address-form.component.html',
  styleUrl: './address-form.component.scss'
})
export class AddressFormComponent implements OnInit {
  private readonly fb             = inject(FormBuilder);
  private readonly addressService = inject(AddressService);
  private readonly cityService    = inject(CityService);

  /** Генерируется при успешном сохранении — передаёт созданный адрес родителю. */
  @Output() saved  = new EventEmitter<Address>();

  /** Генерируется при нажатии «Отмена». */
  @Output() cancel = new EventEmitter<void>();

  /** Список городов для выпадающего списка. */
  cities: City[] = [];

  /** true пока идёт загрузка городов. */
  loadingCities = true;

  /** true пока идёт отправка формы на сервер. */
  submitting = false;

  /** Сообщение об ошибке при отправке. */
  submitError: string | null = null;

  /** Реактивная форма с валидацией. */
  form: FormGroup = this.fb.group({
    street:          ['', [Validators.required, Validators.maxLength(200)]],
    houseNumber:     ['', [Validators.required, Validators.maxLength(20)]],
    apartmentNumber: ['', Validators.maxLength(20)],   // необязательное
    postalCode:      ['', [Validators.required, Validators.maxLength(20)]],
    cityId:          [null, Validators.required]
  });

  ngOnInit(): void {
    this.cityService.getAll().subscribe({
      next:  cities => { this.cities = cities; this.loadingCities = false; },
      error: ()     => { this.loadingCities = false; }
    });
  }

  /** Вспомогательный геттер для удобного доступа к полям формы в шаблоне. */
  get f() { return this.form.controls; }

  onSubmit(): void {
    if (this.form.invalid) {
      // Помечаем все поля как "тронутые", чтобы показать ошибки валидации
      this.form.markAllAsTouched();
      return;
    }

    this.submitting  = true;
    this.submitError = null;

    const command = {
      street:          this.f['street'].value.trim(),
      houseNumber:     this.f['houseNumber'].value.trim(),
      apartmentNumber: this.f['apartmentNumber'].value?.trim() || null,
      postalCode:      this.f['postalCode'].value.trim(),
      cityId:          Number(this.f['cityId'].value)
    };

    this.addressService.create(command).subscribe({
      next: created => {
        this.submitting = false;
        this.saved.emit(created); // передаём созданный адрес родителю
      },
      error: err => {
        this.submitting  = false;
        this.submitError = 'Не удалось сохранить адрес: ' + err.message;
      }
    });
  }

  onCancel(): void {
    this.cancel.emit();
  }
}
