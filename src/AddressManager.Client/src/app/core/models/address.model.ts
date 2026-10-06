export interface Address {
  id: number;
  street: string;
  houseNumber: string;
  apartmentNumber: string | null;
  postalCode: string;
  city: string;
  region: string;
  country: string;
  countryCode: string;
}
