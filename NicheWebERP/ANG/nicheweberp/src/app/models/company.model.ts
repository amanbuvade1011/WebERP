export interface Company {
  companyId: string;
  entityId: string;
  name: string | null;
  code: string | null;
  address: string | null;
  suburb: string | null;
  state: string | null;
  postcode: string | null;
  countryId: string | null;
  phone1: string | null;
  phone2: string | null;
  fax: string | null;
  generalEmail: string | null;
  companyNumber1: string | null;
  companyNumber2: string | null;
}

export interface UpdateCompanyRequest {
  name: string | null;
  address: string | null;
  suburb: string | null;
  state: string | null;
  postcode: string | null;
  countryId: string | null;
  phone1: string | null;
  phone2: string | null;
  fax: string | null;
  generalEmail: string | null;
  companyNumber1: string | null;
  companyNumber2: string | null;
}
