/**
 * Конфигурация для режима разработки (ng serve / npm start).
 *
 * Angular CLI автоматически подставляет этот файл при сборке без флага --configuration production.
 * В production вместо него используется environment.prod.ts (замена настраивается в angular.json).
 */
export const environment = {
  production: false,

  // Абсолютный URL API — нужен потому, что Angular dev-сервер (порт 4200)
  // и ASP.NET API (порт 5017) — разные серверы. CORS на API настроен для этого URL.
  apiUrl: 'http://localhost:5017/api'
};
