import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { map } from 'rxjs/operators';
import { SalesOrderService } from '../../services/sales-order.service';
import { FirmService } from '../../services/firm.service';
import { SALES_ORDER_STATUS_NAMES, SALES_ORDER_STATUS_VALUES, SalesOrderListItem, SalesOrderStatus } from '../../models/sales-order.model';
import { EntityOption, EntityPickerComponent } from '../entity-picker/entity-picker.component';

const STATUS_FILTERS: readonly (SalesOrderStatus | 'All')[] = ['All', ...SALES_ORDER_STATUS_NAMES];

@Component({
  selector: 'app-sales-orders',
  standalone: true,
  imports: [CommonModule, FormsModule, EntityPickerComponent],
  templateUrl: './sales-orders.component.html',
  styleUrls: ['./sales-orders.component.css', '../../shared/list-page.css']
})
export class SalesOrdersComponent implements OnInit {
  items: SalesOrderListItem[] = [];
  totalCount = 0;
  page = 1;
  pageSize = 25;
  statusFilter: SalesOrderStatus | 'All' = 'All';
  statusOptions = STATUS_FILTERS;
  firmFilterId: string | null = null;
  firmFilterLabel = '';
  dateFrom = '';
  dateTo = '';
  loading = true;
  error = false;

  constructor(private salesOrderService: SalesOrderService, private firmService: FirmService, private router: Router) {}

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

  // GetAllSalesOrders only filters by status/firmId/date range (no free-text search) - so
  // "customer" filtering is a firm quick-jump, same generic contract as FirmsListComponent's.
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
    this.salesOrderService
      .getAllOrders({
        page: this.page,
        pageSize: this.pageSize,
        status: this.statusFilter === 'All' ? undefined : SALES_ORDER_STATUS_VALUES[this.statusFilter],
        firmId: this.firmFilterId || undefined,
        dateFrom: this.dateFrom || undefined,
        dateTo: this.dateTo || undefined
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

  openOrder(id: string): void {
    this.router.navigate(['/sales-orders', id]);
  }

  createOrder(): void {
    this.router.navigate(['/sales-orders', 'new']);
  }

  statusClass(status: SalesOrderStatus): string {
    switch (status) {
      case 'Draft':
        return 'status-warning';
      case 'Confirmed':
        return 'cat-sky';
      case 'Shipped':
        return 'cat-violet';
      case 'Delivered':
        return 'status-good';
      case 'Cancelled':
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
