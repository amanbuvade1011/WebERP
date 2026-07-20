import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { ApiURLService } from './ApiURLService.service';
import { CreateRetailCustomerRequest, RetailCustomerListItem } from '../models/retail-customer.model';
import { PagedResult } from '../models/paged-result.model';

export interface RetailCustomerQuery {
  page: number;
  pageSize: number;
  search?: string;
}

@Injectable({ providedIn: 'root' })
export class RetailCustomerService {
  constructor(private http: HttpClient) {}

  getAllRetailCustomers(query: RetailCustomerQuery): Observable<PagedResult<RetailCustomerListItem>> {
    let params = new HttpParams().set('page', query.page).set('pageSize', query.pageSize);
    if (query.search) params = params.set('search', query.search);

    return this.http.get<PagedResult<RetailCustomerListItem>>(`${ApiURLService.BASE_URL}/Customers/GetAllRetailCustomers`, { params });
  }

  createRetailCustomer(request: CreateRetailCustomerRequest): Observable<RetailCustomerListItem> {
    return this.http.post<RetailCustomerListItem>(`${ApiURLService.BASE_URL}/Customers/CreateRetailCustomer`, request);
  }
}
