/**
 * DTO города — соответствует CityDto из API (GET /api/cities).
 * Используется для заполнения выпадающего списка в форме создания адреса.
 */
export interface City {
  id: number;
  name: string;
  region: string;
  country: string;

  /**
   * Отображаемое название: "Киев (Киевская область, Украина)".
   * Вычисляется на сервере в CityDto.DisplayName.
   */
  displayName: string;
}
