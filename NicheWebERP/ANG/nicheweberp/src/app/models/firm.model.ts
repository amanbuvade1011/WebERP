export interface FirmListItem {
  id: string;
  code: string | null;
  tradingName: string;
  entityClassName: string;
  creditLimit: number;
  inactive: boolean;
}

export interface FirmDetail {
  id: string;
  code: string | null;
  tradingName: string;
  companyName: string | null;
  entityClassName: string;
  address: string | null;
  suburb: string | null;
  state: string | null;
  postcode: string | null;
  countryId: string | null;
  phone1: string | null;
  phone2: string | null;
  generalEmail: string | null;
  fax: string | null;
  termsId: string | null;
  termsName: string | null;
  pricePointId: string | null;
  pricePointName: string | null;
  creditLimit: number;
  discountPercent1: number;
  allowOrder: boolean;
  allowInvoice: boolean;
  depositPercent: number;
  inactive: boolean;
}

export interface CreateFirmRequest {
  tradingName: string;
  companyName: string | null;
  code: string | null;
  entityClassName: string;
  address: string | null;
  suburb: string | null;
  state: string | null;
  postcode: string | null;
  countryId: string | null;
  phone1: string | null;
  phone2: string | null;
  generalEmail: string | null;
  termsId: string | null;
  pricePointId: string | null;
  creditLimit: number;
  discountPercent1: number;
  allowOrder: boolean;
  allowInvoice: boolean;
  depositPercent: number;
}

export interface UpdateFirmRequest {
  tradingName: string;
  companyName: string | null;
  address: string | null;
  suburb: string | null;
  state: string | null;
  postcode: string | null;
  countryId: string | null;
  phone1: string | null;
  phone2: string | null;
  generalEmail: string | null;
  termsId: string | null;
  pricePointId: string | null;
  creditLimit: number;
  discountPercent1: number;
  allowOrder: boolean;
  allowInvoice: boolean;
  depositPercent: number;
  inactive: boolean;
}

export interface TradingTermsOption {
  id: string;
  description: string | null;
  numberOfDays: number;
  discountDays: number;
  settlementDiscountPercent: number;
}
