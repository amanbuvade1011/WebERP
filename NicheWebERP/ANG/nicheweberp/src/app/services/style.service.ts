import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { ApiURLService } from './ApiURLService.service';
import {
  AddColorRequest,
  CreateStyleRequest,
  PagedResult,
  StyleColor,
  StyleDetail,
  StyleListItem,
  UpdateStyleRequest
} from '../models/style.model';

export interface StyleQuery {
  page: number;
  pageSize: number;
  search?: string;
  categoryId?: string;
  labelId?: string;
  rangeId?: string;
  inactive?: boolean;
}

@Injectable({ providedIn: 'root' })
export class StyleService {
  constructor(private http: HttpClient) {}

  getAllStyles(query: StyleQuery): Observable<PagedResult<StyleListItem>> {
    let params = new HttpParams().set('page', query.page).set('pageSize', query.pageSize);
    if (query.search) params = params.set('search', query.search);
    if (query.categoryId) params = params.set('categoryId', query.categoryId);
    if (query.labelId) params = params.set('labelId', query.labelId);
    if (query.rangeId) params = params.set('rangeId', query.rangeId);
    if (query.inactive !== undefined) params = params.set('inactive', query.inactive);

    return this.http.get<PagedResult<StyleListItem>>(`${ApiURLService.BASE_URL}/Styles/GetAllStyles`, { params });
  }

  getStyleById(id: string): Observable<StyleDetail> {
    return this.http.get<StyleDetail>(`${ApiURLService.BASE_URL}/Styles/GetStyleById/${id}`);
  }

  createStyle(request: CreateStyleRequest): Observable<StyleDetail> {
    return this.http.post<StyleDetail>(`${ApiURLService.BASE_URL}/Styles/CreateStyle`, request);
  }

  updateStyle(id: string, request: UpdateStyleRequest): Observable<StyleDetail> {
    return this.http.put<StyleDetail>(`${ApiURLService.BASE_URL}/Styles/UpdateStyle/${id}`, request);
  }

  addColor(styleId: string, request: AddColorRequest): Observable<StyleColor> {
    return this.http.post<StyleColor>(`${ApiURLService.BASE_URL}/Styles/${styleId}/Colors/AddColor`, request);
  }
}
