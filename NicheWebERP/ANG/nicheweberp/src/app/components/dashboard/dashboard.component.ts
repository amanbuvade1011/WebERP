import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { DashboardService, DashboardData } from '../../services/dashboard.service';
import { KpiStat, RecentActivityItem } from '../../models/dashboard.model';

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './dashboard.component.html',
  styleUrls: ['./dashboard.component.css', '../../shared/list-page.css']
})
export class DashboardComponent implements OnInit {
  data: DashboardData | null = null;
  loading = true;
  error = false;

  constructor(private dashboardService: DashboardService) {}

  ngOnInit(): void {
    this.fetch();
  }

  refresh(): void {
    this.fetch();
  }

  private fetch(): void {
    this.loading = true;
    this.error = false;
    this.dashboardService.getDashboardData().subscribe({
      next: (data) => {
        this.data = data;
        this.loading = false;
      },
      error: () => {
        this.error = true;
        this.loading = false;
      }
    });
  }

  sparklinePoints(trend: number[]): string {
    if (!trend.length) {
      return '';
    }
    const max = Math.max(...trend, 1);
    const min = Math.min(...trend, 0);
    const range = max - min || 1;
    const stepX = 100 / Math.max(trend.length - 1, 1);
    return trend
      .map((v, i) => {
        const x = i * stepX;
        const y = 28 - ((v - min) / range) * 26;
        return `${x.toFixed(1)},${y.toFixed(1)}`;
      })
      .join(' ');
  }

  barHeightPct(value: number, all: number[]): number {
    const max = Math.max(...all, 1);
    return max === 0 ? 0 : Math.max((value / max) * 100, value > 0 ? 4 : 0);
  }

  stagePct(count: number, all: number[]): number {
    const total = all.reduce((s, v) => s + v, 0);
    return total === 0 ? 0 : Math.round((count / total) * 100);
  }

  revenueValues(d: DashboardData): number[] {
    return d.monthlyRevenue.map((m) => m.amount);
  }

  stageValues(d: DashboardData): number[] {
    return d.orderStages.map((s) => s.count);
  }

  categoryValues(d: DashboardData): number[] {
    return d.categoryBreakdown.map((c) => c.value);
  }

  deltaClass(kpi: KpiStat): string {
    const good = kpi.positiveIsGood ? kpi.delta >= 0 : kpi.delta < 0;
    return good ? 'delta-up' : 'delta-down';
  }

  invoiceStatusClass(status: string): string {
    switch (status) {
      case 'Paid':
        return 'status-good';
      case 'Pending':
        return 'status-warning';
      case 'Overdue':
        return 'status-critical';
      default:
        return 'status-neutral';
    }
  }

  activityIconClass(kind: RecentActivityItem['kind']): string {
    switch (kind) {
      case 'order':
        return 'activity-icon-order';
      case 'invoice':
        return 'activity-icon-invoice';
      default:
        return 'activity-icon-product';
    }
  }

  relativeTime(iso: string): string {
    const diffMs = Date.now() - new Date(iso).getTime();
    const days = Math.floor(diffMs / 86400000);
    if (days <= 0) {
      return 'Today';
    }
    if (days === 1) {
      return 'Yesterday';
    }
    if (days < 30) {
      return `${days}d ago`;
    }
    const months = Math.floor(days / 30);
    return `${months}mo ago`;
  }

  currency(n: number): string {
    return n.toLocaleString('en-US', { style: 'currency', currency: 'USD', maximumFractionDigits: 0 });
  }
}
