import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { ApiURLService } from './ApiURLService.service';

export interface CalculateFreightResult {
  price: number;
}

@Injectable({ providedIn: 'root' })
export class FreightService {
  constructor(private http: HttpClient) {}

  calculateFreight(locationId: string, quantity: number): Observable<CalculateFreightResult> {
    const params = new HttpParams().set('locationId', locationId).set('quantity', quantity);
    return this.http.get<CalculateFreightResult>(`${ApiURLService.BASE_URL}/Freight/CalculateFreight`, { params });
  }
}
