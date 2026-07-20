import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { ApiURLService } from './ApiURLService.service';
import { Company, UpdateCompanyRequest } from '../models/company.model';

@Injectable({ providedIn: 'root' })
export class CompanyService {
  constructor(private http: HttpClient) {}

  getCurrentCompany(): Observable<Company> {
    return this.http.get<Company>(`${ApiURLService.BASE_URL}/Company/GetCurrentCompany`);
  }

  updateCompany(request: UpdateCompanyRequest): Observable<Company> {
    return this.http.put<Company>(`${ApiURLService.BASE_URL}/Company/UpdateCompany`, request);
  }
}
