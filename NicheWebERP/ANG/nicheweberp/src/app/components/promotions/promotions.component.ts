import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { PromotionService } from '../../services/promotion.service';
import { Promotion } from '../../models/promotion.model';

interface PromotionFormModel {
  description: string;
  startDate: string;
  endDate: string;
  isCoupon: boolean;
  couponCode: string;
  couponDiscountPrintedValue: number;
  couponIsDollar: boolean;
  couponDiscountMinimumSpend: number;
  couponMaxUses: number;
  couponMaxUsesPerson: number;
}

@Component({
  selector: 'app-promotions',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './promotions.component.html',
  styleUrls: ['./promotions.component.css', '../../shared/list-page.css']
})
export class PromotionsComponent implements OnInit {
  items: Promotion[] = [];
  loading = true;
  error = false;

  showCreateForm = false;
  creating = false;
  createError = '';
  form: PromotionFormModel = this.emptyForm();

  constructor(private promotionService: PromotionService) {}

  ngOnInit(): void {
    this.fetch();
  }

  refresh(): void {
    this.fetch();
  }

  private fetch(): void {
    this.loading = true;
    this.error = false;
    this.promotionService.getAllPromotions().subscribe({
      next: (items) => {
        this.items = items;
        this.loading = false;
      },
      error: () => {
        this.error = true;
        this.loading = false;
      }
    });
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
    if (!this.form.description) {
      this.createError = 'Description is required.';
      return;
    }
    if (this.form.isCoupon && !this.form.couponCode) {
      this.createError = 'Coupon code is required for a coupon promotion.';
      return;
    }

    this.creating = true;
    this.createError = '';
    this.promotionService
      .createPromotion({
        description: this.form.description,
        startDate: this.form.startDate || new Date().toISOString(),
        endDate: this.form.endDate || null,
        isCoupon: this.form.isCoupon,
        couponCode: this.form.isCoupon ? this.form.couponCode : null,
        couponDiscountPrintedValue: this.form.couponDiscountPrintedValue,
        couponIsDollar: this.form.couponIsDollar,
        couponDiscountMinimumSpend: this.form.couponDiscountMinimumSpend,
        couponMaxUses: this.form.couponMaxUses,
        couponMaxUsesPerson: this.form.couponMaxUsesPerson
      })
      .subscribe({
        next: () => {
          this.creating = false;
          this.showCreateForm = false;
          this.fetch();
        },
        error: (err) => {
          this.creating = false;
          this.createError = err?.error?.message || 'Could not create the promotion.';
        }
      });
  }

  currency(n: number): string {
    return n.toLocaleString('en-US', { style: 'currency', currency: 'USD' });
  }

  formatDate(iso: string | null): string {
    if (!iso) return '—';
    return new Date(iso).toLocaleDateString('en-US', { day: '2-digit', month: 'short', year: 'numeric' });
  }

  private emptyForm(): PromotionFormModel {
    return {
      description: '',
      startDate: new Date().toISOString().slice(0, 10),
      endDate: '',
      isCoupon: true,
      couponCode: '',
      couponDiscountPrintedValue: 0,
      couponIsDollar: false,
      couponDiscountMinimumSpend: 0,
      couponMaxUses: 0,
      couponMaxUsesPerson: 0
    };
  }
}
