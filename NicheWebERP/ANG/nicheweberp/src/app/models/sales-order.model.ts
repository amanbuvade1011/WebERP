export type SalesOrderStatus = 'Pending' | 'Confirmed' | 'Shipped' | 'Delivered' | 'Cancelled';

export interface SalesOrder {
  orderNo: string;
  customer: string;
  orderDate: string;
  items: number;
  amount: number;
  status: SalesOrderStatus;
}
