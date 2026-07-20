export interface RetailCustomerListItem {
  id: string;
  firstName: string | null;
  lastName: string | null;
  email: string | null;
  phoneNumber: string | null;
  isSuspended: boolean;
}

export interface CreateRetailCustomerRequest {
  firstName: string;
  lastName: string;
  email: string | null;
  phoneNumber: string | null;
  address: string | null;
  suburb: string | null;
  state: string | null;
  postCode: string | null;
  countryId: string | null;
}
