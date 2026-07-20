export interface Location {
  id: string;
  name: string | null;
  code: string | null;
  parentId: string | null;
  inactive: boolean;
}

export interface CreateLocationRequest {
  name: string;
  code: string | null;
  parentId: string;
  countryId: string | null;
}
