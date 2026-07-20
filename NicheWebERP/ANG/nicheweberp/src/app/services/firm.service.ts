import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { ApiURLService } from './ApiURLService.service';
import { CreateFirmRequest, FirmDetail, FirmListItem, TradingTermsOption, UpdateFirmRequest } from '../models/firm.model';
import { PagedResult } from '../models/paged-result.model';

export interface FirmQuery {
  page: number;
  pageSize: number;
  search?: string;
  entityClassName?: string;
}

@Injectable({ providedIn: 'root' })
export class FirmService {
  constructor(private http: HttpClient) {}

  getAllFirms(query: FirmQuery): Observable<PagedResult<FirmListItem>> {
    let params = new HttpParams().set('page', query.page).set('pageSize', query.pageSize);
    if (query.search) params = params.set('search', query.search);
    if (query.entityClassName) params = params.set('entityClassName', query.entityClassName);

    return this.http.get<PagedResult<FirmListItem>>(`${ApiURLService.BASE_URL}/Firms/GetAllFirms`, { params });
  }

  getFirmById(id: string): Observable<FirmDetail> {
    return this.http.get<FirmDetail>(`${ApiURLService.BASE_URL}/Firms/GetFirmById/${id}`);
  }

  createFirm(request: CreateFirmRequest): Observable<FirmDetail> {
    return this.http.post<FirmDetail>(`${ApiURLService.BASE_URL}/Firms/CreateFirm`, request);
  }

  updateFirm(id: string, request: UpdateFirmRequest): Observable<FirmDetail> {
    return this.http.put<FirmDetail>(`${ApiURLService.BASE_URL}/Firms/UpdateFirm/${id}`, request);
  }

  getAllTradingTerms(): Observable<TradingTermsOption[]> {
    return this.http.get<TradingTermsOption[]>(`${ApiURLService.BASE_URL}/TradingTerms/GetAllTerms`);
  }
}
