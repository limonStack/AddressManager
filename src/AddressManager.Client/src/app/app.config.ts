/**
 * Глобальная конфигурация приложения.
 *
 * Передаётся в bootstrapApplication() и регистрирует провайдеры,
 * которые доступны во всём приложении (аналог корневого NgModule.providers).
 */

import { ApplicationConfig, provideZoneChangeDetection } from '@angular/core';
import { provideRouter } from '@angular/router';
import { provideHttpClient } from '@angular/common/http';
import { routes } from './app.routes';

export const appConfig: ApplicationConfig = {
  providers: [
    // provideZoneChangeDetection — настраивает Zone.js для обнаружения изменений.
    // eventCoalescing: true — объединяет несколько событий в один цикл обнаружения,
    // что уменьшает количество лишних проверок и повышает производительность.
    provideZoneChangeDetection({ eventCoalescing: true }),

    // Подключаем роутер с маршрутами из app.routes.ts
    provideRouter(routes),

    // Регистрируем HttpClient — нужен для HTTP-запросов в AddressService.
    // Без этого провайдера inject(HttpClient) в сервисах выбросит ошибку.
    provideHttpClient()
  ]
};
