import { Injectable } from '@angular/core';
import { Observable, of } from 'rxjs';
import { delay } from 'rxjs/operators';
import { Company } from '../models/company.model';

const MOCK_COMPANIES: Company[] = [
  {
    code: 'MC-AU',
    name: 'Muhan Corporate Pty Ltd',
    taxId: 'ABN 45 812 934 110',
    location: 'Melbourne, Australia',
    contactEmail: 'accounts@muhancorporate.com.au',
    contactPhone: '+61 3 9555 0142',
    status: 'Active',
    employees: 214
  },
  {
    code: 'MC-NZ',
    name: 'Muhan Corporate (NZ) Ltd',
    taxId: 'NZBN 942 100 233 07',
    location: 'Auckland, New Zealand',
    contactEmail: 'accounts@muhancorporate.co.nz',
    contactPhone: '+64 9 555 0198',
    status: 'Active',
    employees: 46
  },
  {
    code: 'IPA-HO',
    name: 'IPA Healthcare Outfitters',
    taxId: 'ABN 71 220 445 903',
    location: 'Sydney, Australia',
    contactEmail: 'finance@ipahealthcare.com',
    contactPhone: '+61 2 8210 6633',
    status: 'Active',
    employees: 88
  },
  {
    code: 'WPH-DIST',
    name: 'WPH Distribution Group',
    taxId: 'ABN 33 004 118 776',
    location: 'Brisbane, Australia',
    contactEmail: 'ops@wphdistribution.com',
    contactPhone: '+61 7 3040 2277',
    status: 'Inactive',
    employees: 12
  }
];

@Injectable({ providedIn: 'root' })
export class CompanyService {
  getAllCompanies(): Observable<Company[]> {
    return of(MOCK_COMPANIES).pipe(delay(300));
  }
}
