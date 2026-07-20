import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { ApiURLService } from './ApiURLService.service';
import {
  GenerateProductsResult,
  PricePoint,
  ProductListItem,
  StockGridRow,
  StylePrice,
  StylePriceLine,
  StyleSellLocation
} from '../models/product.model';
import { PagedResult } from '../models/paged-result.model';

export interface ProductQuery {
  page: number;
  pageSize: number;
  search?: string;
  categoryId?: string;
  labelId?: string;
}

@Injectable({ providedIn: 'root' })
export class ProductService {
  constructor(private http: HttpClient) {}

  getAllProducts(query: ProductQuery): Observable<PagedResult<ProductListItem>> {
    let params = new HttpParams().set('page', query.page).set('pageSize', query.pageSize);
    if (query.search) params = params.set('search', query.search);
    if (query.categoryId) params = params.set('categoryId', query.categoryId);
    if (query.labelId) params = params.set('labelId', query.labelId);

    return this.http.get<PagedResult<ProductListItem>>(`${ApiURLService.BASE_URL}/Products/GetAllProducts`, { params });
  }

  getAllPricePoints(): Observable<PricePoint[]> {
    return this.http.get<PricePoint[]>(`${ApiURLService.BASE_URL}/PricePoints/GetAllPricePoints`);
  }

  getStylePrices(styleId: string): Observable<StylePrice[]> {
    return this.http.get<StylePrice[]>(`${ApiURLService.BASE_URL}/Styles/${styleId}/GetPrices`);
  }

  updateStylePrices(styleId: string, prices: StylePriceLine[]): Observable<StylePrice[]> {
    return this.http.put<StylePrice[]>(`${ApiURLService.BASE_URL}/Styles/${styleId}/UpdatePrices`, { prices });
  }

  getSellLocations(styleId: string): Observable<StyleSellLocation[]> {
    return this.http.get<StyleSellLocation[]>(`${ApiURLService.BASE_URL}/Styles/${styleId}/SellLocations`);
  }

  updateSellLocations(styleId: string, locations: StyleSellLocation[]): Observable<StyleSellLocation[]> {
    return this.http.put<StyleSellLocation[]>(`${ApiURLService.BASE_URL}/Styles/${styleId}/SellLocations`, { locations });
  }

  generateProducts(styleId: string, colorId: string): Observable<GenerateProductsResult> {
    return this.http.post<GenerateProductsResult>(
      `${ApiURLService.BASE_URL}/Styles/${styleId}/Colors/${colorId}/GenerateProducts`, {});
  }

  getStockByStyleColor(styleId: string, colorId: string): Observable<StockGridRow[]> {
    return this.http.get<StockGridRow[]>(`${ApiURLService.BASE_URL}/Products/GetStockByStyleColor/${styleId}/${colorId}`);
  }
}
