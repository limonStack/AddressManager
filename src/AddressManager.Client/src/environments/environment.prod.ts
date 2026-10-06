/**
 * Конфигурация для production-сборки (ng build --configuration production).
 *
 * В production Angular-приложение раздаётся самим ASP.NET-сервером
 * как статика, поэтому API доступен по относительному пути /api —
 * никакого CORS не нужно, оба на одном домене.
 */
export const environment = {
  production: true,

  // Относительный URL — работает когда фронтенд и бэкенд на одном домене/порту.
  apiUrl: '/api'
};
