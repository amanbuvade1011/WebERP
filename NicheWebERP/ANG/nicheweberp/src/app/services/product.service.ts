import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { ApiURLService } from './ApiURLService.service';
import { ProductListItem } from '../models/product.model';

@Injectable({ providedIn: 'root' })
export class ProductService {
  constructor(private http: HttpClient) {}

  getAllProducts(): Observable<ProductListItem[]> {
    return this.http.get<ProductListItem[]>(`${ApiURLService.BASE_URL}/Products/GetAllProducts`);
  }
}
