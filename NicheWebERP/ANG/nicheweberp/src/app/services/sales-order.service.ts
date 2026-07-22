import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { ApiURLService } from './ApiURLService.service';
import { PagedResult } from '../models/paged-result.model';
import { InvoiceDetail } from '../models/invoice.model';
import {
  AddSalesOrderLineRequest,
  CreateSalesOrderRequest,
  SalesOrderDetail,
  SalesOrderListItem,
  UpdateSalesOrderLineRequest
} from '../models/sales-order.model';

export interface SalesOrderQuery {
  page: number;
  pageSize: number;
  status?: number;
  firmId?: string;
  dateFrom?: string;
  dateTo?: string;
}

@Injectable({ providedIn: 'root' })
export class SalesOrderService {
  constructor(private http: HttpClient) {}

  getAllOrders(query: SalesOrderQuery): Observable<PagedResult<SalesOrderListItem>> {
    let params = new HttpParams().set('page', query.page).set('pageSize', query.pageSize);
    if (query.status !== undefined) params = params.set('status', query.status);
    if (query.firmId) params = params.set('firmId', query.firmId);
    if (query.dateFrom) params = params.set('dateFrom', query.dateFrom);
    if (query.dateTo) params = params.set('dateTo', query.dateTo);

    return this.http.get<PagedResult<SalesOrderListItem>>(`${ApiURLService.BASE_URL}/SalesOrders/GetAllSalesOrders`, { params });
  }

  getOrderById(id: string): Observable<SalesOrderDetail> {
    return this.http.get<SalesOrderDetail>(`${ApiURLService.BASE_URL}/SalesOrders/GetSalesOrderById/${id}`);
  }

  createOrder(request: CreateSalesOrderRequest): Observable<SalesOrderDetail> {
    return this.http.post<SalesOrderDetail>(`${ApiURLService.BASE_URL}/SalesOrders/CreateSalesOrder`, request);
  }

  updateStatus(id: string, status: number): Observable<SalesOrderDetail> {
    return this.http.put<SalesOrderDetail>(`${ApiURLService.BASE_URL}/SalesOrders/${id}/UpdateStatus`, { status });
  }

  addLine(id: string, request: AddSalesOrderLineRequest): Observable<SalesOrderDetail> {
    return this.http.post<SalesOrderDetail>(`${ApiURLService.BASE_URL}/SalesOrders/${id}/AddLine`, request);
  }

  updateLine(id: string, lineId: string, request: UpdateSalesOrderLineRequest): Observable<SalesOrderDetail> {
    return this.http.put<SalesOrderDetail>(`${ApiURLService.BASE_URL}/SalesOrders/${id}/UpdateLine/${lineId}`, request);
  }

  removeLine(id: string, lineId: string): Observable<SalesOrderDetail> {
    return this.http.delete<SalesOrderDetail>(`${ApiURLService.BASE_URL}/SalesOrders/${id}/RemoveLine/${lineId}`);
  }

  generateInvoice(id: string): Observable<InvoiceDetail> {
    return this.http.post<InvoiceDetail>(`${ApiURLService.BASE_URL}/SalesOrders/${id}/GenerateInvoice`, {});
  }
}
