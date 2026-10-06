/**
 * CityService — сервис для получения списка городов.
 * Используется формой создания адреса для заполнения выпадающего списка.
 */

import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { City } from '../models/city.model';
import { environment } from '../../../environments/environment';

@Injectable({ providedIn: 'root' })
export class CityService {
  private readonly http = inject(HttpClient);
  private readonly url = `${environment.apiUrl}/cities`;

  /** GET /api/cities — все города с регионом и страной. */
  getAll(): Observable<City[]> {
    return this.http.get<City[]>(this.url);
  }
}
