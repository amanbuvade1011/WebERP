import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { InvoiceService } from '../../services/invoice.service';
import { Invoice, InvoiceStatus } from '../../models/invoice.model';

const STATUS_FILTERS: readonly (InvoiceStatus | 'All')[] = ['All', 'Paid', 'Pending', 'Overdue', 'Draft'];

@Component({
  selector: 'app-invoices',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './invoices.component.html',
  styleUrls: ['./invoices.component.css', '../../shared/list-page.css']
})
export class InvoicesComponent implements OnInit {
  invoices: Invoice[] = [];
  filteredInvoices: Invoice[] = [];
  searchTerm = '';
  statusFilter: InvoiceStatus | 'All' = 'All';
  statusOptions = STATUS_FILTERS;
  loading = true;
  error = false;

  pageSize = 20;
  page = 1;

  constructor(private invoiceService: InvoiceService) {}

  ngOnInit(): void {
    this.fetch();
  }

  refresh(): void {
    this.fetch();
  }

  private fetch(): void {
    this.loading = true;
    this.error = false;
    this.invoiceService.getAllInvoices().subscribe({
      next: (data) => {
        this.invoices = data;
        this.applyFilter();
        this.loading = false;
      },
      error: () => {
        this.error = true;
        this.loading = false;
      }
    });
  }

  applyFilter(): void {
    const term = this.searchTerm.trim().toLowerCase();
    this.filteredInvoices = this.invoices.filter((i) => {
      const matchesStatus = this.statusFilter === 'All' || i.status === this.statusFilter;
      const matchesTerm =
        !term ||
        i.invoiceNo.toLowerCase().includes(term) ||
        i.customer.toLowerCase().includes(term) ||
        i.orderRef.toLowerCase().includes(term);
      return matchesStatus && matchesTerm;
    });
    this.page = 1;
  }

  clearSearch(): void {
    this.searchTerm = '';
    this.applyFilter();
  }

  get totalOutstanding(): number {
    return this.invoices
      .filter((i) => i.status === 'Pending' || i.status === 'Overdue')
      .reduce((sum, i) => sum + i.amount, 0);
  }

  get totalPages(): number {
    return Math.max(1, Math.ceil(this.filteredInvoices.length / this.pageSize));
  }

  get pagedInvoices(): Invoice[] {
    const start = (this.page - 1) * this.pageSize;
    return this.filteredInvoices.slice(start, start + this.pageSize);
  }

  get rangeStart(): number {
    return this.filteredInvoices.length === 0 ? 0 : (this.page - 1) * this.pageSize + 1;
  }

  get rangeEnd(): number {
    return Math.min(this.page * this.pageSize, this.filteredInvoices.length);
  }

  goToPage(page: number): void {
    this.page = Math.min(Math.max(page, 1), this.totalPages);
  }

  statusClass(status: InvoiceStatus): string {
    switch (status) {
      case 'Paid':
        return 'status-good';
      case 'Pending':
        return 'status-warning';
      case 'Overdue':
        return 'status-critical';
      case 'Draft':
        return 'status-neutral';
    }
  }

  currency(n: number): string {
    return n.toLocaleString('en-US', { style: 'currency', currency: 'USD' });
  }

  formatDate(iso: string): string {
    return new Date(iso).toLocaleDateString('en-US', { day: '2-digit', month: 'short', year: 'numeric' });
  }
}
