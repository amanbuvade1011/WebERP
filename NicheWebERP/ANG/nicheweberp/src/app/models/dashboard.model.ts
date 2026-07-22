// Mirrors NicheWebErpAPI.Dtos.DashboardSummaryDto (Sprint 12) - server-computed, replacing the
// former client-side forkJoin-and-page-200-rows aggregation.

export interface KpiStat {
  label: string;
  value: number;
  format: 'currency' | 'count';
  deltaPercent: number | null;
  deltaLabel: string | null;
  positiveIsGood: boolean;
  trend: number[];
  // Route name this tile links to when clicked (e.g. "sales-orders"); null = not clickable.
  linkType: string | null;
}

export interface ArAgingBucket {
  label: string;
  count: number;
  amount: number;
}

export interface TopCustomer {
  firmId: string;
  firmName: string;
  revenue: number;
  orderCount: number;
}

export interface MonthlyPoint {
  month: string;
  amount: number;
}

export interface OrderStageCount {
  stage: 'Draft' | 'Confirmed' | 'Shipped' | 'Delivered';
  count: number;
}

export interface InvoiceStatusCount {
  status: 'Paid' | 'Pending' | 'Overdue';
  count: number;
  amount: number;
}

export interface CategoryCount {
  category: string;
  count: number;
}

export interface RecentActivityItem {
  id: string;
  kind: 'order' | 'invoice';
  title: string;
  subtitle: string;
  timestamp: string;
  amount?: number;
}

// "invoices" | "firm" | "reconciliation" - which list page this item should link to
export type AttentionLinkType = 'invoices' | 'firm' | 'reconciliation';

export interface AttentionItem {
  type: string;
  severity: 'critical' | 'warning';
  message: string;
  linkType: AttentionLinkType;
  linkId: string | null;
}

export interface DashboardSummary {
  kpis: KpiStat[];
  revenueTrend: MonthlyPoint[];
  orderStages: OrderStageCount[];
  cancelledOrders: number;
  invoiceStatus: InvoiceStatusCount[];
  categoryBreakdown: CategoryCount[];
  recentActivity: RecentActivityItem[];
  needsAttention: AttentionItem[];
  arAging: ArAgingBucket[];
  topCustomers: TopCustomer[];
}
