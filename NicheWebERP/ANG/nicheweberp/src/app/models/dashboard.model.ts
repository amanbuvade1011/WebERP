export interface KpiStat {
  label: string;
  value: string;
  delta: number;
  deltaLabel: string;
  trend: number[];
  positiveIsGood: boolean;
}

export interface MonthlyRevenuePoint {
  month: string;
  amount: number;
}

export interface CategoryBreakdownItem {
  category: string;
  value: number;
}

export interface OrderStageBreakdown {
  stage: 'Draft' | 'Confirmed' | 'Shipped' | 'Delivered';
  count: number;
}

export interface InvoiceStatusBreakdown {
  status: 'Paid' | 'Pending' | 'Overdue';
  count: number;
  amount: number;
}

export interface RecentActivityItem {
  id: string;
  kind: 'order' | 'invoice' | 'product';
  title: string;
  subtitle: string;
  timestamp: string;
  amount?: number;
}
