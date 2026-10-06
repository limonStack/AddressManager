import { Routes } from '@angular/router';
import { AddressesComponent } from './features/addresses/addresses.component';

export const routes: Routes = [
  { path: '', redirectTo: 'addresses', pathMatch: 'full' },
  { path: 'addresses', component: AddressesComponent }
];
