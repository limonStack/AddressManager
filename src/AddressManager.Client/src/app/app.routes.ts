/**
 * Маршруты приложения.
 *
 * Текущие маршруты:
 *   /            → редирект на /addresses
 *   /addresses   → AddressesComponent (список адресов)
 *
 * При необходимости сюда добавляются новые маршруты:
 *   { path: 'addresses/:id', component: AddressDetailComponent }
 */

import { Routes } from '@angular/router';
import { AddressesComponent } from './features/addresses/addresses.component';

export const routes: Routes = [
  // Корневой маршрут: пустой путь перенаправляет на /addresses.
  // pathMatch: 'full' — важно! Без него '/' матчится частично и редирект срабатывает для любого пути.
  { path: '', redirectTo: 'addresses', pathMatch: 'full' },

  // Страница списка адресов
  { path: 'addresses', component: AddressesComponent }
];
