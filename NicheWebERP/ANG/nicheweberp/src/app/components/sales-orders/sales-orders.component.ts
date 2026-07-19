import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { SalesOrderService } from '../../services/sales-order.service';
import { SalesOrder, SalesOrderStatus } from '../../models/sales-order.model';

const STATUS_FILTERS: readonly (SalesOrderStatus | 'All')[] = [
  'All', 'Pending', 'Confirmed', 'Shipped', 'Delivered', 'Cancelled'
];

@Component({
  selector: 'app-sales-orders',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './sales-orders.component.html',
  styleUrls: ['./sales-orders.component.css', '../../shared/list-page.css']
})
export class SalesOrdersComponent implements OnInit {
  orders: SalesOrder[] = [];
  filteredOrders: SalesOrder[] = [];
  searchTerm = '';
  statusFilter: SalesOrderStatus | 'All' = 'All';
  statusOptions = STATUS_FILTERS;
  loading = true;
  error = false;

  pageSize = 20;
  page = 1;

  constructor(private salesOrderService: SalesOrderService) {}

  ngOnInit(): void {
    this.fetch();
  }

  refresh(): void {
    this.fetch();
  }

  private fetch(): void {
    this.loading = true;
    this.error = false;
    this.salesOrderService.getAllOrders().subscribe({
      next: (data) => {
        this.orders = data;
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
    this.filteredOrders = this.orders.filter((o) => {
      const matchesStatus = this.statusFilter === 'All' || o.status === this.statusFilter;
      const matchesTerm =
        !term ||
        o.orderNo.toLowerCase().includes(term) ||
        o.customer.toLowerCase().includes(term);
      return matchesStatus && matchesTerm;
    });
    this.page = 1;
  }

  clearSearch(): void {
    this.searchTerm = '';
    this.applyFilter();
  }

  get totalPages(): number {
    return Math.max(1, Math.ceil(this.filteredOrders.length / this.pageSize));
  }

  get pagedOrders(): SalesOrder[] {
    const start = (this.page - 1) * this.pageSize;
    return this.filteredOrders.slice(start, start + this.pageSize);
  }

  get rangeStart(): number {
    return this.filteredOrders.length === 0 ? 0 : (this.page - 1) * this.pageSize + 1;
  }

  get rangeEnd(): number {
    return Math.min(this.page * this.pageSize, this.filteredOrders.length);
  }

  goToPage(page: number): void {
    this.page = Math.min(Math.max(page, 1), this.totalPages);
  }

  statusClass(status: SalesOrderStatus): string {
    switch (status) {
      case 'Pending':
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

  formatDate(iso: string): string {
    return new Date(iso).toLocaleDateString('en-US', { day: '2-digit', month: 'short', year: 'numeric' });
  }
}
