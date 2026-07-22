import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { ApiURLService } from './ApiURLService.service';
import { CreatePromotionRequest, Promotion, ValidateCouponRequest, ValidateCouponResult } from '../models/promotion.model';

@Injectable({ providedIn: 'root' })
export class PromotionService {
  constructor(private http: HttpClient) {}

  getAllPromotions(): Observable<Promotion[]> {
    return this.http.get<Promotion[]>(`${ApiURLService.BASE_URL}/Promotions/GetAllPromotions`);
  }

  createPromotion(request: CreatePromotionRequest): Observable<Promotion> {
    return this.http.post<Promotion>(`${ApiURLService.BASE_URL}/Promotions/CreatePromotion`, request);
  }

  validateCoupon(request: ValidateCouponRequest): Observable<ValidateCouponResult> {
    return this.http.post<ValidateCouponResult>(`${ApiURLService.BASE_URL}/Promotions/ValidateCoupon`, request);
  }
}
