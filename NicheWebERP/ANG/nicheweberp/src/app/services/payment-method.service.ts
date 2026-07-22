import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { ApiURLService } from './ApiURLService.service';
import { PaymentMethod } from '../models/payment-method.model';

@Injectable({ providedIn: 'root' })
export class PaymentMethodService {
  constructor(private http: HttpClient) {}

  getAllPaymentMethods(): Observable<PaymentMethod[]> {
    return this.http.get<PaymentMethod[]>(`${ApiURLService.BASE_URL}/PaymentMethods/GetAllPaymentMethods`);
  }
}
