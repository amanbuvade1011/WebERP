import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { ApiURLService } from './ApiURLService.service';
import { DashboardSummary } from '../models/dashboard.model';

// Sprint 12 - single server-computed summary, replacing the former client-side
// forkJoin-across-three-200-row-pages aggregation. See docs/ai-plan/sprints/sprint-12-reports-dashboard.md.
@Injectable({ providedIn: 'root' })
export class DashboardService {
  constructor(private http: HttpClient) {}

  getDashboardData(): Observable<DashboardSummary> {
    return this.http.get<DashboardSummary>(`${ApiURLService.BASE_URL}/Dashboard/GetSummary`);
  }
}
