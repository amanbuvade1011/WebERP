import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { ApiURLService } from './ApiURLService.service';
import { PagedResult } from '../models/paged-result.model';
import { InvoiceDetail, InvoiceListItem, RecordPaymentRequest } from '../models/invoice.model';

export interface InvoiceQuery {
  page: number;
  pageSize: number;
  status?: number;
  firmId?: string;
  dateFrom?: string;
  dateTo?: string;
}

@Injectable({ providedIn: 'root' })
export class InvoiceService {
  constructor(private http: HttpClient) {}

  getAllInvoices(query: InvoiceQuery): Observable<PagedResult<InvoiceListItem>> {
    let params = new HttpParams().set('page', query.page).set('pageSize', query.pageSize);
    if (query.status !== undefined) params = params.set('status', query.status);
    if (query.firmId) params = params.set('firmId', query.firmId);
    if (query.dateFrom) params = params.set('dateFrom', query.dateFrom);
    if (query.dateTo) params = params.set('dateTo', query.dateTo);

    return this.http.get<PagedResult<InvoiceListItem>>(`${ApiURLService.BASE_URL}/Invoices/GetAllInvoices`, { params });
  }

  getInvoiceById(id: string): Observable<InvoiceDetail> {
    return this.http.get<InvoiceDetail>(`${ApiURLService.BASE_URL}/Invoices/GetInvoiceById/${id}`);
  }

  recordPayment(request: RecordPaymentRequest): Observable<InvoiceDetail> {
    return this.http.post<InvoiceDetail>(`${ApiURLService.BASE_URL}/Payments/RecordPayment`, request);
  }
}
