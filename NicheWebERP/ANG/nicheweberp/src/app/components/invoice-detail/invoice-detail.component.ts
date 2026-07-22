import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { forkJoin } from 'rxjs';
import { InvoiceService } from '../../services/invoice.service';
import { PaymentMethodService } from '../../services/payment-method.service';
import { InvoiceDetail } from '../../models/invoice.model';
import { PaymentMethod } from '../../models/payment-method.model';

@Component({
  selector: 'app-invoice-detail',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './invoice-detail.component.html',
  styleUrls: ['./invoice-detail.component.css', '../../shared/list-page.css']
})
export class InvoiceDetailComponent implements OnInit {
  invoiceId!: string;
  invoice: InvoiceDetail | null = null;
  paymentMethods: PaymentMethod[] = [];

  loading = true;
  error = false;

  paymentAmount: number | null = null;
  paymentMethodId = '';
  paymentNarration = '';
  recording = false;
  recordError = '';

  constructor(private route: ActivatedRoute, private router: Router, private invoiceService: InvoiceService, private paymentMethodService: PaymentMethodService) {}

  ngOnInit(): void {
    this.invoiceId = this.route.snapshot.paramMap.get('id')!;
    this.fetch();
  }

  private fetch(): void {
    this.loading = true;
    this.error = false;

    forkJoin({
      invoice: this.invoiceService.getInvoiceById(this.invoiceId),
      paymentMethods: this.paymentMethodService.getAllPaymentMethods()
    }).subscribe({
      next: ({ invoice, paymentMethods }) => {
        this.invoice = invoice;
        this.paymentMethods = paymentMethods;
        this.paymentAmount = invoice.remainingAmount > 0 ? invoice.remainingAmount : null;
        this.loading = false;
      },
      error: () => {
        this.error = true;
        this.loading = false;
      }
    });
  }

  refresh(): void {
    this.fetch();
  }

  goBack(): void {
    this.router.navigate(['/invoices']);
  }

  openOrder(): void {
    if (this.invoice?.salesOrderId) {
      this.router.navigate(['/sales-orders', this.invoice.salesOrderId]);
    }
  }

  recordPayment(): void {
    if (!this.invoice) return;
    if (!this.paymentAmount || this.paymentAmount <= 0) {
      this.recordError = 'Enter a payment amount greater than zero.';
      return;
    }
    if (!this.paymentMethodId) {
      this.recordError = 'Choose a payment method.';
      return;
    }

    this.recording = true;
    this.recordError = '';

    this.invoiceService
      .recordPayment({
        invoiceId: this.invoiceId,
        amount: this.paymentAmount,
        paymentMethodId: this.paymentMethodId,
        narration: this.paymentNarration || null
      })
      .subscribe({
        next: (updated) => {
          this.invoice = updated;
          this.recording = false;
          this.paymentAmount = updated.remainingAmount > 0 ? updated.remainingAmount : null;
          this.paymentMethodId = '';
          this.paymentNarration = '';
        },
        error: (err) => {
          this.recording = false;
          this.recordError = err?.error?.message || 'Something went wrong. Please try again.';
        }
      });
  }

  statusClass(status: string): string {
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

  currency(n: number): string {
    return n.toLocaleString('en-US', { style: 'currency', currency: 'USD' });
  }

  formatDate(iso: string | null): string {
    if (!iso) return '—';
    return new Date(iso).toLocaleDateString('en-US', { day: '2-digit', month: 'short', year: 'numeric' });
  }
}
