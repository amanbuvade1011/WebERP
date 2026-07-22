import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { DashboardService } from '../../services/dashboard.service';
import { ArAgingBucket, AttentionItem, CategoryCount, DashboardSummary, KpiStat, MonthlyPoint, OrderStageCount, RecentActivityItem, TopCustomer } from '../../models/dashboard.model';

// Chart geometry constants for the revenue line/area chart (see dataviz method: line for trend
// over time, single series -> sequential/accent hue, hairline gridlines, hover crosshair+tooltip,
// value at the line's end). Fixed logical viewBox, independent of rendered size.
const CHART_W = 600;
const CHART_H = 200;
const PAD_L = 8;
const PAD_R = 16;
const PAD_T = 16;
const PAD_B = 28;
const INNER_W = CHART_W - PAD_L - PAD_R;
const INNER_H = CHART_H - PAD_T - PAD_B;

interface LinePoint {
  x: number;
  y: number;
  month: string;
  amount: number;
}

interface HoverTooltip {
  x: number;
  y: number;
  lines: { label: string; value: string }[];
}

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './dashboard.component.html',
  styleUrls: ['./dashboard.component.css', '../../shared/list-page.css']
})
export class DashboardComponent implements OnInit {
  data: DashboardSummary | null = null;
  loading = true;
  error = false;

  hoveredMonth: number | null = null;
  hoveredStage: number | null = null;
  hoveredCategory: number | null = null;
  hoveredInvoiceStatus: number | null = null;

  readonly chartW = CHART_W;
  readonly chartH = CHART_H;

  constructor(private dashboardService: DashboardService, private router: Router) {}

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

  // ---------- KPI tiles ----------

  kpiValueDisplay(kpi: KpiStat): string {
    return kpi.format === 'currency' ? this.compactCurrency(kpi.value) : this.compactCount(kpi.value);
  }

  openKpi(kpi: KpiStat): void {
    if (kpi.linkType) {
      this.router.navigate(['/' + kpi.linkType]);
    }
  }

  deltaClass(kpi: KpiStat): string {
    if (kpi.deltaPercent === null) return 'delta-neutral';
    const good = kpi.positiveIsGood ? kpi.deltaPercent >= 0 : kpi.deltaPercent < 0;
    return good ? 'delta-up' : 'delta-down';
  }

  sparklinePoints(trend: number[]): string {
    if (!trend.length) return '';
    const max = Math.max(...trend, 1);
    const min = Math.min(...trend, 0);
    const range = max - min || 1;
    const stepX = 100 / Math.max(trend.length - 1, 1);
    return trend.map((v, i) => `${(i * stepX).toFixed(1)},${(28 - ((v - min) / range) * 26).toFixed(1)}`).join(' ');
  }

  // ---------- Revenue trend (line + area) ----------

  linePoints(d: DashboardSummary): LinePoint[] {
    const values = d.revenueTrend.map((m) => m.amount);
    const max = Math.max(...values, 1);
    const min = Math.min(...values, 0);
    const range = max - min || 1;
    const n = d.revenueTrend.length;
    const stepX = n > 1 ? INNER_W / (n - 1) : 0;
    return d.revenueTrend.map((m, i) => ({
      x: PAD_L + i * stepX,
      y: PAD_T + INNER_H - ((m.amount - min) / range) * INNER_H,
      month: m.month,
      amount: m.amount
    }));
  }

  linePath(d: DashboardSummary): string {
    const pts = this.linePoints(d);
    if (!pts.length) return '';
    return pts.map((p, i) => `${i === 0 ? 'M' : 'L'} ${p.x.toFixed(1)},${p.y.toFixed(1)}`).join(' ');
  }

  areaPath(d: DashboardSummary): string {
    const pts = this.linePoints(d);
    if (!pts.length) return '';
    const baseline = PAD_T + INNER_H;
    const first = pts[0];
    const last = pts[pts.length - 1];
    const line = pts.map((p, i) => `${i === 0 ? 'M' : 'L'} ${p.x.toFixed(1)},${p.y.toFixed(1)}`).join(' ');
    return `${line} L ${last.x.toFixed(1)},${baseline} L ${first.x.toFixed(1)},${baseline} Z`;
  }

  yAxisTicks(d: DashboardSummary): { y: number; label: string }[] {
    const values = d.revenueTrend.map((m) => m.amount);
    const max = Math.max(...values, 1);
    const step = max / 3;
    return [0, 1, 2, 3].map((i) => ({
      y: PAD_T + INNER_H - (i / 3) * INNER_H,
      label: this.compactCurrency(step * i)
    }));
  }

  onHoverMonth(i: number | null): void {
    this.hoveredMonth = i;
  }

  revenueTooltip(d: DashboardSummary): HoverTooltip | null {
    if (this.hoveredMonth === null) return null;
    const pts = this.linePoints(d);
    const p = pts[this.hoveredMonth];
    if (!p) return null;
    return { x: p.x, y: p.y, lines: [{ label: p.month, value: this.compactCurrency(p.amount) }] };
  }

  // ---------- Order pipeline (ordinal ramp) ----------

  stagePct(count: number, all: OrderStageCount[]): number {
    const max = Math.max(...all.map((s) => s.count), 1);
    return max === 0 ? 0 : Math.max((count / max) * 100, count > 0 ? 3 : 0);
  }

  // ---------- Category breakdown (sequential, magnitude) ----------

  categoryPct(count: number, all: CategoryCount[]): number {
    const max = Math.max(...all.map((c) => c.count), 1);
    return max === 0 ? 0 : Math.max((count / max) * 100, count > 0 ? 3 : 0);
  }

  // ---------- AR aging (ordinal severity ramp: older = darker) ----------

  get arAgingTotal(): number {
    return this.data ? this.data.arAging.reduce((s, b) => s + b.amount, 0) : 0;
  }

  agingPct(amount: number, all: ArAgingBucket[]): number {
    const max = Math.max(...all.map((b) => b.amount), 1);
    return max === 0 ? 0 : Math.max((amount / max) * 100, amount > 0 ? 3 : 0);
  }

  // ---------- Top customers (magnitude bars, click-through to firm) ----------

  customerPct(revenue: number, all: TopCustomer[]): number {
    const max = Math.max(...all.map((c) => c.revenue), 1);
    return max === 0 ? 0 : Math.max((revenue / max) * 100, revenue > 0 ? 3 : 0);
  }

  openCustomer(c: TopCustomer): void {
    this.router.navigate(['/firms', c.firmId]);
  }

  // ---------- Invoice status (part-to-whole, stacked bar) ----------

  invoiceStatusSegments(d: DashboardSummary): { status: string; pct: number; offset: number; count: number; amount: number }[] {
    const total = d.invoiceStatus.reduce((s, i) => s + i.amount, 0) || 1;
    let offset = 0;
    return d.invoiceStatus.map((s) => {
      const pct = (s.amount / total) * 100;
      const seg = { status: s.status, pct, offset, count: s.count, amount: s.amount };
      offset += pct;
      return seg;
    });
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

  // ---------- Needs Attention ----------

  attentionIconPath(item: AttentionItem): string {
    return item.severity === 'critical'
      ? 'M10 3.5l7.5 13h-15l7.5-13z M10 8v3.5 M10 13.5h.01'
      : 'M10 2.5a7.5 7.5 0 1 0 0 15 7.5 7.5 0 0 0 0-15z M10 6.5v4 M10 13.5h.01';
  }

  openAttentionItem(item: AttentionItem): void {
    switch (item.linkType) {
      case 'invoices':
        this.router.navigate(['/invoices']);
        break;
      case 'firm':
        if (item.linkId) this.router.navigate(['/firms', item.linkId]);
        break;
      case 'reconciliation':
        this.router.navigate(['/reconciliations']);
        break;
    }
  }

  // ---------- Recent activity ----------

  activityIconClass(kind: RecentActivityItem['kind']): string {
    return kind === 'order' ? 'activity-icon-order' : 'activity-icon-invoice';
  }

  openActivityItem(item: RecentActivityItem): void {
    if (item.kind === 'order') {
      this.router.navigate(['/sales-orders', item.id.replace('SO-', '')]);
    }
  }

  relativeTime(iso: string): string {
    const days = Math.floor((Date.now() - new Date(iso).getTime()) / 86400000);
    if (days <= 0) return 'Today';
    if (days === 1) return 'Yesterday';
    if (days < 30) return `${days}d ago`;
    return `${Math.floor(days / 30)}mo ago`;
  }

  // ---------- Formatting ----------

  compactCurrency(n: number): string {
    const abs = Math.abs(n);
    if (abs >= 1_000_000) return (n < 0 ? '-$' : '$') + (abs / 1_000_000).toFixed(1) + 'M';
    if (abs >= 1_000) return (n < 0 ? '-$' : '$') + (abs / 1_000).toFixed(1) + 'K';
    return n.toLocaleString('en-US', { style: 'currency', currency: 'USD', maximumFractionDigits: 0 });
  }

  compactCount(n: number): string {
    const abs = Math.abs(n);
    if (abs >= 1_000_000) return (abs / 1_000_000).toFixed(1) + 'M';
    if (abs >= 10_000) return (abs / 1_000).toFixed(1) + 'K';
    return n.toLocaleString('en-US');
  }

  currency(n: number): string {
    return n.toLocaleString('en-US', { style: 'currency', currency: 'USD' });
  }
}
