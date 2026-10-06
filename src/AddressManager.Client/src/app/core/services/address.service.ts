import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Address } from '../models/address.model';
import { environment } from '../../../environments/environment';

@Injectable({ providedIn: 'root' })
export class AddressService {
  private readonly http = inject(HttpClient);
  private readonly url = `${environment.apiUrl}/addresses`;

  getAll(): Observable<Address[]> {
    return this.http.get<Address[]>(this.url);
  }

  getById(id: number): Observable<Address> {
    return this.http.get<Address>(`${this.url}/${id}`);
  }
}
