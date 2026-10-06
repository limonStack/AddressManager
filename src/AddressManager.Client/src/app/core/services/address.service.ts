/**
 * AddressService — Angular-сервис для работы с REST API адресов.
 *
 * Тонкая обёртка над HttpClient: инкапсулирует базовый URL и типизирует ответы.
 * Зарегистрирован глобально (providedIn: 'root') — синглтон на всё приложение.
 */

import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Address } from '../models/address.model';
import { environment } from '../../../environments/environment';

@Injectable({ providedIn: 'root' })
export class AddressService {
  // inject() — современный способ DI в Angular 14+, альтернатива constructor-инъекции
  private readonly http = inject(HttpClient);

  // Базовый URL эндпоинта — в dev: http://localhost:5017/api/addresses
  //                         в prod: /api/addresses (относительный, проксируется сервером)
  private readonly url = `${environment.apiUrl}/addresses`;

  /**
   * Возвращает список всех адресов с полной географической иерархией.
   * Соответствует GET /api/addresses.
   */
  getAll(): Observable<Address[]> {
    return this.http.get<Address[]>(this.url);
  }

  /**
   * Возвращает один адрес по идентификатору.
   * Соответствует GET /api/addresses/{id}.
   * Если адрес не найден — сервер вернёт 404, RxJS пробросит ошибку в subscribe.error.
   */
  getById(id: number): Observable<Address> {
    return this.http.get<Address>(`${this.url}/${id}`);
  }
}
