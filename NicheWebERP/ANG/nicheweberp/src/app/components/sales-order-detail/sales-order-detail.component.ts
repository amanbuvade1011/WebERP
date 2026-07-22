import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { forkJoin, of } from 'rxjs';
import { map } from 'rxjs/operators';
import { SalesOrderService } from '../../services/sales-order.service';
import { FirmService } from '../../services/firm.service';
import { LocationService } from '../../services/location.service';
import { StyleService } from '../../services/style.service';
import { ProductService } from '../../services/product.service';
import { PromotionService } from '../../services/promotion.service';
import { FreightService } from '../../services/freight.service';
import {
  SALES_ORDER_STATUS_VALUES,
  SalesOrderDetail,
  SalesOrderStatus
} from '../../models/sales-order.model';
import { FirmDetail } from '../../models/firm.model';
import { Location } from '../../models/location.model';
import { StyleDetail } from '../../models/style.model';
import { StockGridRow, StylePrice } from '../../models/product.model';
import { ValidateCouponResult } from '../../models/promotion.model';
import { EntityOption, EntityPickerComponent } from '../entity-picker/entity-picker.component';

// Mirrors SalesOrderService.AllowedTransitions on the backend - kept in sync manually since it's
// a small, stable, business-decided workflow (see docs/ai-plan/sprints/sprint-05-sales-orders.md).
// This only drives which buttons render; the server re-validates the transition regardless.
const ALLOWED_TRANSITIONS: Record<SalesOrderStatus, SalesOrderStatus[]> = {
  Draft: ['Confirmed', 'Cancelled'],
  Confirmed: ['Shipped', 'Cancelled'],
  Shipped: ['Delivered'],
  Delivered: [],
  Cancelled: []
};

interface PickerRow extends StockGridRow {
  qty: number;
  estUnitPriceExTax: number | null;
  estUnitPriceTax: number | null;
}

interface DraftLine {
  productId: string;
  styleCode: string;
  color: string;
  sizeDescription: string;
  quantity: number;
  estUnitPriceExTax: number | null;
  estUnitPriceTax: number | null;
}

@Component({
  selector: 'app-sales-order-detail',
  standalone: true,
  imports: [CommonModule, FormsModule, EntityPickerComponent],
  templateUrl: './sales-order-detail.component.html',
  styleUrls: ['./sales-order-detail.component.css', '../../shared/list-page.css']
})
export class SalesOrderDetailComponent implements OnInit {
  isNew = false;
  orderId: string | null = null;
  order: SalesOrderDetail | null = null;

  loading = true;
  error = false;
  saving = false;
  saveError = '';
  generatingInvoice = false;

  // New-order header state.
  locations: Location[] = [];
  selectedFirm: FirmDetail | null = null;
  selectedLocationId = '';
  customerReferenceNo = '';
  narration = '';
  draftLines: DraftLine[] = [];

  // Coupon + freight (Sprint 07) - new-order flow only. Both are estimates: the server
  // independently validates the coupon and computes freight at CreateSalesOrder time, same
  // "estimates only, server prices authoritatively" pattern already used for line pricing.
  couponCode = '';
  couponResult: ValidateCouponResult | null = null;
  couponError = '';
  applyingCoupon = false;
  freightAmount = 0;

  // Style -> color -> size/qty line picker, shared between "new order" and "add line to draft".
  selectedStyle: StyleDetail | null = null;
  selectedColorId = '';
  pickerRows: PickerRow[] = [];
  pickerLoading = false;
  pickerError = '';

  // Existing-draft-order inline line edits.
  lineEdits = new Map<string, number>();

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private salesOrderService: SalesOrderService,
    private firmService: FirmService,
    private locationService: LocationService,
    private styleService: StyleService,
    private productService: ProductService,
    private promotionService: PromotionService,
    private freightService: FreightService
  ) {}

  ngOnInit(): void {
    const idParam = this.route.snapshot.paramMap.get('id');
    this.isNew = idParam === 'new';
    this.orderId = this.isNew ? null : idParam;
    this.fetch();
  }

  private fetch(): void {
    this.loading = true;
    this.error = false;

    forkJoin({
      locations: this.locationService.getAllLocations(),
      order: this.isNew || !this.orderId ? of(null) : this.salesOrderService.getOrderById(this.orderId)
    }).subscribe({
      next: (result) => {
        this.locations = result.locations;
        this.order = result.order;
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
    this.router.navigate(['/sales-orders']);
  }

  // Passed into <app-entity-picker> for the new-order customer field.
  firmSearch = (term: string) =>
    this.firmService
      .getAllFirms({ page: 1, pageSize: 10, search: term })
      .pipe(map((result): EntityOption[] => result.items.map((f) => ({
        id: f.id,
        label: f.tradingName,
        sublabel: f.code || undefined
      }))));

  onFirmSelected(option: EntityOption): void {
    this.firmService.getFirmById(option.id).subscribe((firm) => {
      this.selectedFirm = firm;
      this.resetPicker();
    });
  }

  // Passed into <app-entity-picker> for the style field of the line picker.
  styleSearch = (term: string) =>
    this.styleService
      .getAllStyles({ page: 1, pageSize: 10, search: term })
      .pipe(map((result): EntityOption[] => result.items.map((s) => ({
        id: s.id,
        label: s.code,
        sublabel: s.description
      }))));

  onStyleSelected(option: EntityOption): void {
    this.styleService.getStyleById(option.id).subscribe((style) => {
      this.selectedStyle = style;
      this.selectedColorId = '';
      this.pickerRows = [];
      this.pickerError = '';
    });
  }

  get lineLocationId(): string {
    return this.isNew ? this.selectedLocationId : this.order?.locationId || '';
  }

  get lineFirmPricePointId(): string | undefined {
    return this.isNew ? this.selectedFirm?.pricePointId || undefined : this.order?.pricePointId;
  }

  onColorChange(): void {
    if (!this.selectedStyle || !this.selectedColorId) {
      this.pickerRows = [];
      return;
    }
    if (!this.lineLocationId) {
      this.pickerError = 'Choose a location first.';
      this.pickerRows = [];
      return;
    }

    this.pickerLoading = true;
    this.pickerError = '';

    forkJoin({
      stock: this.productService.getStockByStyleColor(this.selectedStyle.id, this.selectedColorId),
      prices: this.productService.getStylePrices(this.selectedStyle.id)
    }).subscribe({
      next: ({ stock, prices }) => {
        const matchedPrice = prices.find((p) => p.pricePointId === this.lineFirmPricePointId) || null;
        this.pickerRows = stock
          .filter((row) => row.locationId === this.lineLocationId)
          .map((row) => ({
            ...row,
            qty: 0,
            estUnitPriceExTax: matchedPrice?.localUnitPriceExTax1 ?? null,
            estUnitPriceTax: matchedPrice?.localUnitPriceTax1 ?? null
          }));
        if (!this.pickerRows.length) {
          this.pickerError = 'This style/color has no stock at the selected location.';
        }
        this.pickerLoading = false;
      },
      error: () => {
        this.pickerError = "Couldn't load stock for this style/color.";
        this.pickerLoading = false;
      }
    });
  }

  resetPicker(): void {
    this.selectedStyle = null;
    this.selectedColorId = '';
    this.pickerRows = [];
    this.pickerError = '';
  }

  addPickerRow(row: PickerRow): void {
    if (row.qty <= 0) {
      return;
    }

    if (this.isNew) {
      this.draftLines.push({
        productId: row.productId,
        styleCode: this.selectedStyle?.code || '',
        color: this.selectedStyle?.colors.find((c) => c.id === this.selectedColorId)?.color || '',
        sizeDescription: row.sizeDescription,
        quantity: row.qty,
        estUnitPriceExTax: row.estUnitPriceExTax,
        estUnitPriceTax: row.estUnitPriceTax
      });
      row.qty = 0;
      this.refreshFreightEstimate();
      if (this.couponResult) {
        this.applyCoupon();
      }
      return;
    }

    if (!this.orderId) {
      return;
    }
    this.saveError = '';
    this.salesOrderService.addLine(this.orderId, { productId: row.productId, quantity: row.qty }).subscribe({
      next: (updated) => {
        this.order = updated;
        row.qty = 0;
      },
      error: (err) => this.handleSaveError(err)
    });
  }

  removeDraftLine(index: number): void {
    this.draftLines.splice(index, 1);
    this.refreshFreightEstimate();
    if (this.couponResult) {
      this.applyCoupon();
    }
  }

  get draftSubTotalExTax(): number {
    return this.draftLines.reduce((sum, l) => sum + (l.estUnitPriceExTax ?? 0) * l.quantity, 0);
  }

  get draftTaxAmount(): number {
    return this.draftLines.reduce((sum, l) => sum + (l.estUnitPriceTax ?? 0) * l.quantity, 0);
  }

  get draftTotalQuantity(): number {
    return this.draftLines.reduce((sum, l) => sum + l.quantity, 0);
  }

  get draftDiscountAmount(): number {
    return this.couponResult?.valid ? this.couponResult.discountAmount : 0;
  }

  get draftTotal(): number {
    return this.draftSubTotalExTax + this.draftTaxAmount - this.draftDiscountAmount + this.freightAmount;
  }

  get hasUnpricedDraftLine(): boolean {
    return this.draftLines.some((l) => l.estUnitPriceExTax === null);
  }

  // Freight is estimated from the selected location + running item count as soon as both are
  // known; recomputed whenever either changes. No calculator configured -> 0, not an error, same
  // "free rather than blocking" behavior as the backend.
  refreshFreightEstimate(): void {
    if (!this.selectedLocationId || !this.draftLines.length) {
      this.freightAmount = 0;
      return;
    }
    this.freightService.calculateFreight(this.selectedLocationId, this.draftTotalQuantity).subscribe({
      next: (result) => (this.freightAmount = result.price),
      error: () => (this.freightAmount = 0)
    });
  }

  applyCoupon(): void {
    if (!this.selectedFirm || !this.couponCode) {
      this.couponError = 'Choose a customer and enter a coupon code first.';
      return;
    }

    this.applyingCoupon = true;
    this.couponError = '';
    this.promotionService
      .validateCoupon({ couponCode: this.couponCode, firmId: this.selectedFirm.id, orderSubTotal: this.draftSubTotalExTax })
      .subscribe({
        next: (result) => {
          this.applyingCoupon = false;
          this.couponResult = result;
          if (!result.valid) {
            this.couponError = result.message || 'This coupon can\'t be applied.';
          }
        },
        error: (err) => {
          this.applyingCoupon = false;
          this.couponResult = null;
          this.couponError = err?.error?.message || "Couldn't validate this coupon.";
        }
      });
  }

  clearCoupon(): void {
    this.couponCode = '';
    this.couponResult = null;
    this.couponError = '';
  }

  createOrder(): void {
    if (!this.selectedFirm) {
      this.saveError = 'Choose a customer.';
      return;
    }
    if (!this.selectedLocationId) {
      this.saveError = 'Choose a location.';
      return;
    }
    if (!this.draftLines.length) {
      this.saveError = 'Add at least one line.';
      return;
    }

    this.saving = true;
    this.saveError = '';

    this.salesOrderService
      .createOrder({
        firmId: this.selectedFirm.id,
        locationId: this.selectedLocationId,
        customerReferenceNo: this.customerReferenceNo || null,
        narration: this.narration || null,
        couponCode: this.couponResult?.valid ? this.couponCode : null,
        lines: this.draftLines.map((l) => ({ productId: l.productId, quantity: l.quantity }))
      })
      .subscribe({
        next: (created) => {
          this.saving = false;
          this.router.navigate(['/sales-orders', created.id]);
        },
        error: (err) => this.handleSaveError(err)
      });
  }

  // Existing draft-order line editing.
  lineEditValue(lineId: string, current: number): number {
    return this.lineEdits.has(lineId) ? this.lineEdits.get(lineId)! : current;
  }

  setLineEdit(lineId: string, value: number): void {
    this.lineEdits.set(lineId, value);
  }

  saveLineEdit(lineId: string): void {
    if (!this.orderId) return;
    const quantity = this.lineEdits.get(lineId);
    if (!quantity || quantity <= 0) return;

    this.saveError = '';
    this.salesOrderService.updateLine(this.orderId, lineId, { quantity }).subscribe({
      next: (updated) => {
        this.order = updated;
        this.lineEdits.delete(lineId);
      },
      error: (err) => this.handleSaveError(err)
    });
  }

  removeLine(lineId: string): void {
    if (!this.orderId) return;
    this.saveError = '';
    this.salesOrderService.removeLine(this.orderId, lineId).subscribe({
      next: (updated) => (this.order = updated),
      error: (err) => this.handleSaveError(err)
    });
  }

  // Status workflow.
  get availableTransitions(): SalesOrderStatus[] {
    if (!this.order) return [];
    return ALLOWED_TRANSITIONS[this.order.statusName] || [];
  }

  changeStatus(target: SalesOrderStatus): void {
    if (!this.orderId) return;
    this.saveError = '';
    this.salesOrderService.updateStatus(this.orderId, SALES_ORDER_STATUS_VALUES[target]).subscribe({
      next: (updated) => (this.order = updated),
      error: (err) => this.handleSaveError(err)
    });
  }

  // Generate Invoice - only offered once the order has reached a billable fulfillment status
  // and hasn't already been invoiced (see InvoiceService.InvoiceableOrderStatuses on the
  // backend, which re-validates this regardless of what the UI shows).
  get canGenerateInvoice(): boolean {
    if (!this.order || this.order.isInvoiced) return false;
    return ['Confirmed', 'Shipped', 'Delivered'].includes(this.order.statusName);
  }

  generateInvoice(): void {
    if (!this.orderId) return;
    this.generatingInvoice = true;
    this.saveError = '';
    this.salesOrderService.generateInvoice(this.orderId).subscribe({
      next: (invoice) => {
        this.generatingInvoice = false;
        this.router.navigate(['/invoices', invoice.id]);
      },
      error: (err) => {
        this.generatingInvoice = false;
        this.handleSaveError(err);
      }
    });
  }

  private handleSaveError(err: { error?: { message?: string } }): void {
    this.saving = false;
    this.saveError = err?.error?.message || 'Something went wrong. Please try again.';
  }

  currency(n: number): string {
    return n.toLocaleString('en-US', { style: 'currency', currency: 'USD' });
  }

  formatDate(iso: string | null): string {
    if (!iso) return '—';
    return new Date(iso).toLocaleDateString('en-US', { day: '2-digit', month: 'short', year: 'numeric' });
  }
}
