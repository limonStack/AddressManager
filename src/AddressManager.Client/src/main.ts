/**
 * Точка входа Angular-приложения.
 *
 * bootstrapApplication() — способ запуска standalone-приложения без AppModule.
 * Принимает корневой компонент и конфигурацию провайдеров (appConfig).
 */

import { bootstrapApplication } from '@angular/platform-browser';
import { AppComponent } from './app/app.component';
import { appConfig } from './app/app.config';

bootstrapApplication(AppComponent, appConfig)
  .catch(err => console.error(err)); // если bootstrap упал — выводим в консоль
