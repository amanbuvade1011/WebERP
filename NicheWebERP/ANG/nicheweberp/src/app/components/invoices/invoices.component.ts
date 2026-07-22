import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { map } from 'rxjs/operators';
import { InvoiceService } from '../../services/invoice.service';
import { FirmService } from '../../services/firm.service';
import { INVOICE_STATUS_NAMES, INVOICE_STATUS_VALUES, InvoiceListItem, InvoiceStatus } from '../../models/invoice.model';
import { EntityOption, EntityPickerComponent } from '../entity-picker/entity-picker.component';

const STATUS_FILTERS: readonly (InvoiceStatus | 'All')[] = ['All', ...INVOICE_STATUS_NAMES];

@Component({
  selector: 'app-invoices',
  standalone: true,
  imports: [CommonModule, FormsModule, EntityPickerComponent],
  templateUrl: './invoices.component.html',
  styleUrls: ['./invoices.component.css', '../../shared/list-page.css']
})
export class InvoicesComponent implements OnInit {
  items: InvoiceListItem[] = [];
  totalCount = 0;
  page = 1;
  pageSize = 25;
  statusFilter: InvoiceStatus | 'All' = 'All';
  statusOptions = STATUS_FILTERS;
  firmFilterId: string | null = null;
  firmFilterLabel = '';
  loading = true;
  error = false;

  constructor(private invoiceService: InvoiceService, private firmService: FirmService, private router: Router) {}

  ngOnInit(): void {
    this.fetch();
  }

  refresh(): void {
    this.fetch();
  }

  onFilterChange(): void {
    this.page = 1;
    this.fetch();
  }

  // GetAllInvoices only filters by status/firmId/date range (no free-text search) - same
  // constraint Sales Orders hit, same firm quick-jump fix.
  firmSearch = (term: string) =>
    this.firmService
      .getAllFirms({ page: 1, pageSize: 10, search: term })
      .pipe(map((result): EntityOption[] => result.items.map((f) => ({
        id: f.id,
        label: f.tradingName,
        sublabel: f.code || undefined
      }))));

  onFirmSelected(option: EntityOption): void {
    this.firmFilterId = option.id;
    this.firmFilterLabel = option.label;
    this.onFilterChange();
  }

  clearFirmFilter(): void {
    this.firmFilterId = null;
    this.firmFilterLabel = '';
    this.onFilterChange();
  }

  private fetch(): void {
    this.loading = true;
    this.error = false;
    this.invoiceService
      .getAllInvoices({
        page: this.page,
        pageSize: this.pageSize,
        status: this.statusFilter === 'All' ? undefined : INVOICE_STATUS_VALUES[this.statusFilter],
        firmId: this.firmFilterId || undefined
      })
      .subscribe({
        next: (result) => {
          this.items = result.items;
          this.totalCount = result.totalCount;
          this.loading = false;
        },
        error: () => {
          this.error = true;
          this.loading = false;
        }
      });
  }

  get totalOutstanding(): number {
    return this.items
      .filter((i) => i.statusName === 'Pending' || i.statusName === 'Overdue')
      .reduce((sum, i) => sum + (i.totalAmount - i.paidAmount), 0);
  }

  get totalPages(): number {
    return Math.max(1, Math.ceil(this.totalCount / this.pageSize));
  }

  get rangeStart(): number {
    return this.totalCount === 0 ? 0 : (this.page - 1) * this.pageSize + 1;
  }

  get rangeEnd(): number {
    return Math.min(this.page * this.pageSize, this.totalCount);
  }

  goToPage(page: number): void {
    this.page = Math.min(Math.max(page, 1), this.totalPages);
    this.fetch();
  }

  openInvoice(id: string): void {
    this.router.navigate(['/invoices', id]);
  }

  statusClass(status: InvoiceStatus): string {
    switch (status) {
      case 'Paid':
        return 'status-good';
      case 'Pending':
        return 'status-warning';
      case 'Overdue':
        return 'status-critical';
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
