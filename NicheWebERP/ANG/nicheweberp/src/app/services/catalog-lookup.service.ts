import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { ApiURLService } from './ApiURLService.service';
import { CategoryNode, CreateCategoryRequest, Label, RangeNode, Season } from '../models/category.model';

@Injectable({ providedIn: 'root' })
export class CatalogLookupService {
  constructor(private http: HttpClient) {}

  getCategoryTree(): Observable<CategoryNode[]> {
    return this.http.get<CategoryNode[]>(`${ApiURLService.BASE_URL}/Categories/GetCategoryTree`);
  }

  createCategory(request: CreateCategoryRequest): Observable<CategoryNode> {
    return this.http.post<CategoryNode>(`${ApiURLService.BASE_URL}/Categories/CreateCategory`, request);
  }

  getAllLabels(): Observable<Label[]> {
    return this.http.get<Label[]>(`${ApiURLService.BASE_URL}/Labels/GetAllLabels`);
  }

  getRangeTree(): Observable<RangeNode[]> {
    return this.http.get<RangeNode[]>(`${ApiURLService.BASE_URL}/Ranges/GetRangeTree`);
  }

  getAllSeasons(): Observable<Season[]> {
    return this.http.get<Season[]>(`${ApiURLService.BASE_URL}/Seasons/GetAllSeasons`);
  }
}
