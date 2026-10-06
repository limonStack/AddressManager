/**
 * Команда создания адреса — модель ЗАПИСИ (CQRS Command).
 *
 * Намеренно отличается от Address (модели чтения):
 * - Содержит cityId вместо строк city/region/country
 * - Клиент выбирает город из списка и отправляет только его Id
 *
 * Соответствует CreateAddressCommand на бэкенде.
 */
export interface CreateAddressCommand {
  street: string;
  houseNumber: string;
  apartmentNumber: string | null;
  postalCode: string;

  /** Id города из GET /api/cities */
  cityId: number;
}
