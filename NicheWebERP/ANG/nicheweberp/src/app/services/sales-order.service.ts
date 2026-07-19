import { Injectable } from '@angular/core';
import { Observable, of } from 'rxjs';
import { delay } from 'rxjs/operators';
import { SalesOrder, SalesOrderStatus } from '../models/sales-order.model';
import { CUSTOMERS, daysAgoIso, mulberry32, pick } from './mock-data.util';

const STATUS_WEIGHTS: readonly SalesOrderStatus[] = [
  'Pending', 'Pending',
  'Confirmed', 'Confirmed', 'Confirmed',
  'Shipped', 'Shipped', 'Shipped',
  'Delivered', 'Delivered', 'Delivered', 'Delivered', 'Delivered',
  'Cancelled'
];

function buildOrders(): SalesOrder[] {
  const rng = mulberry32(20260712);
  const orders: SalesOrder[] = [];
  for (let i = 1; i <= 138; i++) {
    const items = 1 + Math.floor(rng() * 24);
    const unitAvg = 18 + rng() * 42;
    orders.push({
      orderNo: `SO-${(20260000 + i).toString()}`,
      customer: pick(rng, CUSTOMERS),
      orderDate: daysAgoIso(rng, 120),
      items,
      amount: Math.round(items * unitAvg * 100) / 100,
      status: pick(rng, STATUS_WEIGHTS)
    });
  }
  return orders.sort((a, b) => (a.orderDate < b.orderDate ? 1 : -1));
}

const MOCK_ORDERS = buildOrders();

@Injectable({ providedIn: 'root' })
export class SalesOrderService {
  getAllOrders(): Observable<SalesOrder[]> {
    return of(MOCK_ORDERS).pipe(delay(350));
  }
}
