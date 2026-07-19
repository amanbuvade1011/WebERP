export interface Company {
  code: string;
  name: string;
  taxId: string;
  location: string;
  contactEmail: string;
  contactPhone: string;
  status: 'Active' | 'Inactive';
  employees: number;
}
