import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { ApiURLService } from './ApiURLService.service';
import {
  CreateSizewayRequest,
  Size,
  Sizeway,
  UpdateSizeSequenceRequest
} from '../models/sizeway.model';

@Injectable({ providedIn: 'root' })
export class SizewayService {
  constructor(private http: HttpClient) {}

  getAllSizeways(): Observable<Sizeway[]> {
    return this.http.get<Sizeway[]>(`${ApiURLService.BASE_URL}/Sizeways/GetAllSizeways`);
  }

  createSizeway(request: CreateSizewayRequest): Observable<Sizeway> {
    return this.http.post<Sizeway>(`${ApiURLService.BASE_URL}/Sizeways/CreateSizeway`, request);
  }

  updateSizeSequence(id: string, request: UpdateSizeSequenceRequest): Observable<Sizeway> {
    return this.http.put<Sizeway>(`${ApiURLService.BASE_URL}/Sizeways/${id}/UpdateSizeSequence`, request);
  }

  getAllSizes(): Observable<Size[]> {
    return this.http.get<Size[]>(`${ApiURLService.BASE_URL}/Sizes/GetAllSizes`);
  }
}
