import { Injectable } from '@angular/core';
import { Observable, of } from 'rxjs';
import { delay } from 'rxjs/operators';
import { Invoice, InvoiceStatus } from '../models/invoice.model';
import { CUSTOMERS, daysAgoIso, mulberry32, pick } from './mock-data.util';

const STATUS_WEIGHTS: readonly InvoiceStatus[] = [
  'Paid', 'Paid', 'Paid', 'Paid', 'Paid', 'Paid',
  'Pending', 'Pending', 'Pending',
  'Overdue', 'Overdue',
  'Draft'
];

function addDaysIso(iso: string, days: number): string {
  const d = new Date(iso);
  d.setDate(d.getDate() + days);
  return d.toISOString();
}

function buildInvoices(): Invoice[] {
  const rng = mulberry32(20260713);
  const invoices: Invoice[] = [];
  for (let i = 1; i <= 112; i++) {
    const issueDate = daysAgoIso(rng, 110);
    invoices.push({
      invoiceNo: `INV-${(20260000 + i).toString()}`,
      customer: pick(rng, CUSTOMERS),
      orderRef: `SO-${(20260000 + Math.ceil(rng() * 138)).toString()}`,
      issueDate,
      dueDate: addDaysIso(issueDate, 30),
      amount: Math.round((150 + rng() * 4200) * 100) / 100,
      status: pick(rng, STATUS_WEIGHTS)
    });
  }
  return invoices.sort((a, b) => (a.issueDate < b.issueDate ? 1 : -1));
}

const MOCK_INVOICES = buildInvoices();

@Injectable({ providedIn: 'root' })
export class InvoiceService {
  getAllInvoices(): Observable<Invoice[]> {
    return of(MOCK_INVOICES).pipe(delay(350));
  }
}
