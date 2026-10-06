/**
 * AppComponent — корневой компонент приложения.
 *
 * Является "оболочкой": сам ничего не рендерит, только предоставляет
 * <router-outlet> — точку монтирования, куда Angular Router вставляет
 * компонент текущего маршрута (например AddressesComponent для /addresses).
 */

import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';

@Component({
  selector: 'app-root',       // тег в index.html: <app-root></app-root>
  standalone: true,
  imports: [RouterOutlet],    // нужен для работы директивы <router-outlet>
  template: `<router-outlet />` // весь шаблон — один тег, остальное в дочерних компонентах
})
export class AppComponent {}
