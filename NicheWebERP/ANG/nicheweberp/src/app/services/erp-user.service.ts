import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { ApiURLService } from './ApiURLService.service';
import {
  CreateErpUserRequest,
  ErpRole,
  ErpUser,
  UpdateErpUserRequest
} from '../models/erp-user.model';

@Injectable({ providedIn: 'root' })
export class ErpUserService {
  constructor(private http: HttpClient) {}

  getAllUsers(): Observable<ErpUser[]> {
    return this.http.get<ErpUser[]>(`${ApiURLService.BASE_URL}/ErpUsers/GetAllUsers`);
  }

  createUser(request: CreateErpUserRequest): Observable<ErpUser> {
    return this.http.post<ErpUser>(`${ApiURLService.BASE_URL}/ErpUsers/CreateUser`, request);
  }

  updateUser(id: number, request: UpdateErpUserRequest): Observable<ErpUser> {
    return this.http.put<ErpUser>(`${ApiURLService.BASE_URL}/ErpUsers/UpdateUser/${id}`, request);
  }

  resetPassword(id: number, newPassword: string): Observable<void> {
    return this.http.put<void>(`${ApiURLService.BASE_URL}/ErpUsers/${id}/ResetPassword`, { newPassword });
  }

  getAllRoles(): Observable<ErpRole[]> {
    return this.http.get<ErpRole[]>(`${ApiURLService.BASE_URL}/ErpRoles/GetAllRoles`);
  }
}
