import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { ApiURLService } from './ApiURLService.service';
import { CreateLocationRequest, Location } from '../models/location.model';

@Injectable({ providedIn: 'root' })
export class LocationService {
  constructor(private http: HttpClient) {}

  getAllLocations(): Observable<Location[]> {
    return this.http.get<Location[]>(`${ApiURLService.BASE_URL}/Locations/GetAllLocations`);
  }

  createLocation(request: CreateLocationRequest): Observable<Location> {
    return this.http.post<Location>(`${ApiURLService.BASE_URL}/Locations/CreateLocation`, request);
  }
}
