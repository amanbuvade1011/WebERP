import { Injectable } from '@angular/core';
import { Observable, forkJoin } from 'rxjs';
import { map } from 'rxjs/operators';
import { SalesOrderService } from './sales-order.service';
import { InvoiceService } from './invoice.service';
import { ProductService } from './product.service';
import {
  CategoryBreakdownItem,
  InvoiceStatusBreakdown,
  KpiStat,
  MonthlyRevenuePoint,
  OrderStageBreakdown,
  RecentActivityItem
} from '../models/dashboard.model';
import { SalesOrder } from '../models/sales-order.model';
import { Invoice } from '../models/invoice.model';

export interface DashboardData {
  kpis: KpiStat[];
  monthlyRevenue: MonthlyRevenuePoint[];
  categoryBreakdown: CategoryBreakdownItem[];
  orderStages: OrderStageBreakdown[];
  cancelledOrders: number;
  invoiceStatus: InvoiceStatusBreakdown[];
  recentActivity: RecentActivityItem[];
}

const CATEGORY_SAMPLE: readonly string[] = [
  'Shirts', 'Knitwear', 'Tunics', 'Pants', 'Pharmacy Assistant', 'Maternity', 'Tops', 'Accessories'
];

function daysBetween(iso: string, from: Date): number {
  return Math.floor((from.getTime() - new Date(iso).getTime()) / 86400000);
}

function monthLabel(d: Date): string {
  return d.toLocaleDateString('en-US', { month: 'short' });
}

@Injectable({ providedIn: 'root' })
export class DashboardService {
  constructor(
    private salesOrderService: SalesOrderService,
    private invoiceService: InvoiceService,
    private productService: ProductService
  ) {}

  getDashboardData(): Observable<DashboardData> {
    return forkJoin({
      orders: this.salesOrderService.getAllOrders(),
      invoices: this.invoiceService.getAllInvoices(),
      products: this.productService.getAllProducts()
    }).pipe(
      map(({ orders, invoices, products }) => {
        const now = new Date();

        const revenueLast30 = orders
          .filter((o) => daysBetween(o.orderDate, now) < 30)
          .reduce((sum, o) => sum + o.amount, 0);
        const revenuePrev30 = orders
          .filter((o) => {
            const d = daysBetween(o.orderDate, now);
            return d >= 30 && d < 60;
          })
          .reduce((sum, o) => sum + o.amount, 0);

        const openOrders = orders.filter((o) => ['Pending', 'Confirmed', 'Shipped'].includes(o.status));
        const openOrdersPrev = orders.filter((o) => {
          const d = daysBetween(o.orderDate, now);
          return d >= 30 && d < 60 && ['Pending', 'Confirmed', 'Shipped'].includes(o.status);
        });

        const outstandingInvoices = invoices.filter((i) => i.status === 'Pending' || i.status === 'Overdue');
        const outstandingAmount = outstandingInvoices.reduce((sum, i) => sum + i.amount, 0);
        const overdueAmount = invoices.filter((i) => i.status === 'Overdue').reduce((sum, i) => sum + i.amount, 0);

        const weeklyTrend = this.bucketWeekly(orders, now, 8);

        const kpis: KpiStat[] = [
          {
            label: 'Revenue (30d)',
            value: this.currency(revenueLast30),
            delta: this.pctDelta(revenueLast30, revenuePrev30),
            deltaLabel: 'vs prior 30d',
            trend: weeklyTrend,
            positiveIsGood: true
          },
          {
            label: 'Open Sales Orders',
            value: openOrders.length.toLocaleString(),
            delta: this.pctDelta(openOrders.length, openOrdersPrev.length || openOrders.length),
            deltaLabel: 'vs prior 30d',
            trend: this.bucketWeeklyCount(orders, now, 8),
            positiveIsGood: true
          },
          {
            label: 'Outstanding Invoices',
            value: this.currency(outstandingAmount),
            delta: overdueAmount > 0 ? Math.round((overdueAmount / Math.max(outstandingAmount, 1)) * 100) : 0,
            deltaLabel: 'overdue share',
            trend: this.bucketWeekly(invoices.map((i) => ({ orderDate: i.issueDate, amount: i.amount })), now, 8),
            positiveIsGood: false
          },
          {
            label: 'Active Products',
            value: products.length.toLocaleString(),
            delta: 0,
            deltaLabel: 'live from catalog',
            trend: [],
            positiveIsGood: true
          }
        ];

        const monthlyRevenue = this.bucketMonthly(orders, now, 6);

        const categoryCounts = new Map<string, number>();
        for (const p of products) {
          const key = p.category ?? 'Uncategorized';
          categoryCounts.set(key, (categoryCounts.get(key) ?? 0) + 1);
        }
        const categoryBreakdown: CategoryBreakdownItem[] = (
          categoryCounts.size
            ? Array.from(categoryCounts, ([category, value]) => ({ category, value }))
            : CATEGORY_SAMPLE.map((category, i) => ({ category, value: 40 - i * 3 }))
        )
          .sort((a, b) => b.value - a.value)
          .slice(0, 6);

        const stageOrder: OrderStageBreakdown['stage'][] = ['Pending', 'Confirmed', 'Shipped', 'Delivered'];
        const orderStages: OrderStageBreakdown[] = stageOrder.map((stage) => ({
          stage,
          count: orders.filter((o) => o.status === stage).length
        }));
        const cancelledOrders = orders.filter((o) => o.status === 'Cancelled').length;

        const invStatusOrder: InvoiceStatusBreakdown['status'][] = ['Paid', 'Pending', 'Overdue', 'Draft'];
        const invoiceStatus: InvoiceStatusBreakdown[] = invStatusOrder.map((status) => {
          const matching = invoices.filter((i) => i.status === status);
          return {
            status,
            count: matching.length,
            amount: matching.reduce((sum, i) => sum + i.amount, 0)
          };
        });

        const recentActivity = this.buildRecentActivity(orders, invoices);

        return {
          kpis,
          monthlyRevenue,
          categoryBreakdown,
          orderStages,
          cancelledOrders,
          invoiceStatus,
          recentActivity
        };
      })
    );
  }

  private currency(n: number): string {
    return n.toLocaleString('en-US', { style: 'currency', currency: 'USD', maximumFractionDigits: 0 });
  }

  private pctDelta(current: number, previous: number): number {
    if (!previous) {
      return current > 0 ? 100 : 0;
    }
    return Math.round(((current - previous) / previous) * 100);
  }

  private bucketWeekly(rows: { orderDate: string; amount: number }[], now: Date, weeks: number): number[] {
    const buckets = new Array(weeks).fill(0);
    for (const r of rows) {
      const week = Math.floor(daysBetween(r.orderDate, now) / 7);
      if (week >= 0 && week < weeks) {
        buckets[weeks - 1 - week] += r.amount;
      }
    }
    return buckets;
  }

  private bucketWeeklyCount(rows: { orderDate: string }[], now: Date, weeks: number): number[] {
    const buckets = new Array(weeks).fill(0);
    for (const r of rows) {
      const week = Math.floor(daysBetween(r.orderDate, now) / 7);
      if (week >= 0 && week < weeks) {
        buckets[weeks - 1 - week] += 1;
      }
    }
    return buckets;
  }

  private bucketMonthly(orders: SalesOrder[], now: Date, months: number): MonthlyRevenuePoint[] {
    const points: MonthlyRevenuePoint[] = [];
    for (let i = months - 1; i >= 0; i--) {
      const monthStart = new Date(now.getFullYear(), now.getMonth() - i, 1);
      const monthEnd = new Date(now.getFullYear(), now.getMonth() - i + 1, 1);
      const amount = orders
        .filter((o) => {
          const d = new Date(o.orderDate);
          return d >= monthStart && d < monthEnd;
        })
        .reduce((sum, o) => sum + o.amount, 0);
      points.push({ month: monthLabel(monthStart), amount: Math.round(amount) });
    }
    return points;
  }

  private buildRecentActivity(orders: SalesOrder[], invoices: Invoice[]): RecentActivityItem[] {
    const orderItems: RecentActivityItem[] = orders.slice(0, 5).map((o) => ({
      id: o.orderNo,
      kind: 'order',
      title: `${o.orderNo} · ${o.customer}`,
      subtitle: `${o.items} items · ${o.status}`,
      timestamp: o.orderDate,
      amount: o.amount
    }));
    const invoiceItems: RecentActivityItem[] = invoices.slice(0, 5).map((i) => ({
      id: i.invoiceNo,
      kind: 'invoice',
      title: `${i.invoiceNo} · ${i.customer}`,
      subtitle: `${i.status} · due ${new Date(i.dueDate).toLocaleDateString()}`,
      timestamp: i.issueDate,
      amount: i.amount
    }));
    return [...orderItems, ...invoiceItems]
      .sort((a, b) => (a.timestamp < b.timestamp ? 1 : -1))
      .slice(0, 8);
  }
}
