/**
 * AddressService — Angular-сервис для работы с REST API адресов.
 *
 * CQRS на клиенте:
 *   getAll / getById — читают AddressDto (модель чтения)
 *   create           — отправляет CreateAddressCommand (модель записи),
 *                      получает в ответ AddressDto созданного адреса
 */

import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Address } from '../models/address.model';
import { CreateAddressCommand } from '../models/create-address.command';
import { environment } from '../../../environments/environment';

@Injectable({ providedIn: 'root' })
export class AddressService {
  private readonly http = inject(HttpClient);
  private readonly url = `${environment.apiUrl}/addresses`;

  /** GET /api/addresses — список всех адресов (модель чтения). */
  getAll(): Observable<Address[]> {
    return this.http.get<Address[]>(this.url);
  }

  /** GET /api/addresses/{id} — один адрес (модель чтения). */
  getById(id: number): Observable<Address> {
    return this.http.get<Address>(`${this.url}/${id}`);
  }

  /**
   * POST /api/addresses — создать адрес.
   * Отправляет CreateAddressCommand (модель записи с cityId),
   * получает Address (модель чтения с полной иерархией) — 201 Created.
   */
  create(command: CreateAddressCommand): Observable<Address> {
    return this.http.post<Address>(this.url, command);
  }
}
