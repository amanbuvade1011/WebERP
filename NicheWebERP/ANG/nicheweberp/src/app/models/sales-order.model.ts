export type SalesOrderStatus = 'Draft' | 'Confirmed' | 'Shipped' | 'Delivered' | 'Cancelled';

// Mirrors Dtos/SalesOrderDto.cs's SalesOrderStatus enum (Status1 workflow decided in Sprint 05).
export const SALES_ORDER_STATUS_VALUES: Record<SalesOrderStatus, number> = {
  Draft: 0,
  Confirmed: 1,
  Shipped: 2,
  Delivered: 3,
  Cancelled: 4
};

export const SALES_ORDER_STATUS_NAMES: readonly SalesOrderStatus[] = [
  'Draft', 'Confirmed', 'Shipped', 'Delivered', 'Cancelled'
];

export interface SalesOrderListItem {
  id: string;
  documentNumber: number;
  firmId: string;
  firmName: string;
  orderDate: string | null;
  customerReferenceNo: string | null;
  status: number;
  statusName: SalesOrderStatus;
  totalQuantities: number;
  totalAmount: number;
}

export interface SalesOrderLine {
  lineId: string;
  productId: string;
  styleCode: string;
  color: string;
  sizeDescription: string;
  quantity: number;
  unitPriceExTax: number;
  unitPriceTax: number;
  lineTotalExTax: number;
  lineTotalTax: number;
}

export interface SalesOrderDetail {
  id: string;
  documentNumber: number;
  firmId: string;
  firmName: string;
  locationId: string;
  locationName: string | null;
  pricePointId: string;
  pricePointName: string | null;
  orderDate: string | null;
  customerReferenceNo: string | null;
  narration: string | null;
  status: number;
  statusName: SalesOrderStatus;
  subTotalExTax: number;
  taxAmount: number;
  totalAmount: number;
  totalQuantities: number;
  lines: SalesOrderLine[];
}

// Deliberately has no price field - the API always prices from StylePrice server-side.
export interface CreateSalesOrderLineRequest {
  productId: string;
  quantity: number;
}

export interface CreateSalesOrderRequest {
  firmId: string;
  locationId: string;
  customerReferenceNo: string | null;
  narration: string | null;
  lines: CreateSalesOrderLineRequest[];
}

export interface AddSalesOrderLineRequest {
  productId: string;
  quantity: number;
}

export interface UpdateSalesOrderLineRequest {
  quantity: number;
}
