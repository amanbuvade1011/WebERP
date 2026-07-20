import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RetailCustomerService } from '../../services/retail-customer.service';
import { RetailCustomerListItem } from '../../models/retail-customer.model';

interface CustomerFormModel {
  firstName: string;
  lastName: string;
  email: string;
  phoneNumber: string;
  address: string;
  suburb: string;
  state: string;
  postCode: string;
}

@Component({
  selector: 'app-retail-customers-list',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './retail-customers-list.component.html',
  styleUrls: ['./retail-customers-list.component.css', '../../shared/list-page.css']
})
export class RetailCustomersListComponent implements OnInit {
  items: RetailCustomerListItem[] = [];
  totalCount = 0;
  page = 1;
  pageSize = 25;
  searchTerm = '';
  loading = true;
  error = false;

  showCreateForm = false;
  creating = false;
  createError = '';
  form: CustomerFormModel = this.emptyForm();

  private searchDebounce: ReturnType<typeof setTimeout> | null = null;

  constructor(private retailCustomerService: RetailCustomerService) {}

  ngOnInit(): void {
    this.fetch();
  }

  refresh(): void {
    this.fetch();
  }

  onSearchChange(): void {
    if (this.searchDebounce) {
      clearTimeout(this.searchDebounce);
    }
    this.searchDebounce = setTimeout(() => {
      this.page = 1;
      this.fetch();
    }, 300);
  }

  private fetch(): void {
    this.loading = true;
    this.error = false;
    this.retailCustomerService
      .getAllRetailCustomers({ page: this.page, pageSize: this.pageSize, search: this.searchTerm || undefined })
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

  openCreateForm(): void {
    this.form = this.emptyForm();
    this.createError = '';
    this.showCreateForm = true;
  }

  cancelCreate(): void {
    this.showCreateForm = false;
  }

  submitCreate(): void {
    if (!this.form.firstName || !this.form.lastName) {
      this.createError = 'First name and last name are required.';
      return;
    }

    this.creating = true;
    this.createError = '';
    this.retailCustomerService
      .createRetailCustomer({
        firstName: this.form.firstName,
        lastName: this.form.lastName,
        email: this.form.email || null,
        phoneNumber: this.form.phoneNumber || null,
        address: this.form.address || null,
        suburb: this.form.suburb || null,
        state: this.form.state || null,
        postCode: this.form.postCode || null,
        countryId: null
      })
      .subscribe({
        next: () => {
          this.creating = false;
          this.showCreateForm = false;
          this.page = 1;
          this.fetch();
        },
        error: (err) => {
          this.creating = false;
          this.createError = err?.error?.message || 'Could not create the customer.';
        }
      });
  }

  private emptyForm(): CustomerFormModel {
    return { firstName: '', lastName: '', email: '', phoneNumber: '', address: '', suburb: '', state: '', postCode: '' };
  }
}
