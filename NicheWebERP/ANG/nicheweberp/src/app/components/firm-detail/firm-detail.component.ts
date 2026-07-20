import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { forkJoin, of } from 'rxjs';
import { FirmService } from '../../services/firm.service';
import { ProductService } from '../../services/product.service';
import { FirmDetail, TradingTermsOption } from '../../models/firm.model';
import { PricePoint } from '../../models/product.model';

interface FirmFormModel {
  tradingName: string;
  companyName: string;
  code: string;
  entityClassName: string;
  address: string;
  suburb: string;
  state: string;
  postcode: string;
  phone1: string;
  phone2: string;
  generalEmail: string;
  termsId: string;
  pricePointId: string;
  creditLimit: number;
  discountPercent1: number;
  allowOrder: boolean;
  allowInvoice: boolean;
  depositPercent: number;
  inactive: boolean;
}

@Component({
  selector: 'app-firm-detail',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './firm-detail.component.html',
  styleUrls: ['./firm-detail.component.css', '../../shared/list-page.css']
})
export class FirmDetailComponent implements OnInit {
  isNew = false;
  firmId: string | null = null;
  firm: FirmDetail | null = null;

  terms: TradingTermsOption[] = [];
  pricePoints: PricePoint[] = [];

  loading = true;
  error = false;
  saving = false;
  saveError = '';

  form: FirmFormModel = this.emptyForm();

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private firmService: FirmService,
    private productService: ProductService
  ) {}

  ngOnInit(): void {
    const idParam = this.route.snapshot.paramMap.get('id');
    this.isNew = idParam === 'new';
    this.firmId = this.isNew ? null : idParam;
    this.fetch();
  }

  private fetch(): void {
    this.loading = true;
    this.error = false;

    forkJoin({
      terms: this.firmService.getAllTradingTerms(),
      pricePoints: this.productService.getAllPricePoints(),
      firm: this.isNew || !this.firmId ? of(null) : this.firmService.getFirmById(this.firmId)
    }).subscribe({
      next: (result) => {
        this.terms = result.terms;
        this.pricePoints = result.pricePoints;

        if (result.firm) {
          this.firm = result.firm;
          this.form = {
            tradingName: this.firm.tradingName,
            companyName: this.firm.companyName || '',
            code: this.firm.code || '',
            entityClassName: this.firm.entityClassName,
            address: this.firm.address || '',
            suburb: this.firm.suburb || '',
            state: this.firm.state || '',
            postcode: this.firm.postcode || '',
            phone1: this.firm.phone1 || '',
            phone2: this.firm.phone2 || '',
            generalEmail: this.firm.generalEmail || '',
            termsId: this.firm.termsId || '',
            pricePointId: this.firm.pricePointId || '',
            creditLimit: this.firm.creditLimit,
            discountPercent1: this.firm.discountPercent1,
            allowOrder: this.firm.allowOrder,
            allowInvoice: this.firm.allowInvoice,
            depositPercent: this.firm.depositPercent,
            inactive: this.firm.inactive
          };
        } else {
          this.form = this.emptyForm();
        }

        this.loading = false;
      },
      error: () => {
        this.error = true;
        this.loading = false;
      }
    });
  }

  save(): void {
    if (!this.form.tradingName) {
      this.saveError = 'Trading name is required.';
      return;
    }

    this.saving = true;
    this.saveError = '';

    if (this.isNew) {
      this.firmService
        .createFirm({
          tradingName: this.form.tradingName,
          companyName: this.form.companyName || null,
          code: this.form.code || null,
          entityClassName: this.form.entityClassName,
          address: this.form.address || null,
          suburb: this.form.suburb || null,
          state: this.form.state || null,
          postcode: this.form.postcode || null,
          countryId: null,
          phone1: this.form.phone1 || null,
          phone2: this.form.phone2 || null,
          generalEmail: this.form.generalEmail || null,
          termsId: this.form.termsId || null,
          pricePointId: this.form.pricePointId || null,
          creditLimit: this.form.creditLimit,
          discountPercent1: this.form.discountPercent1,
          allowOrder: this.form.allowOrder,
          allowInvoice: this.form.allowInvoice,
          depositPercent: this.form.depositPercent
        })
        .subscribe({
          next: (created) => {
            this.saving = false;
            this.router.navigate(['/firms', created.id]);
          },
          error: (err) => this.handleSaveError(err)
        });
    } else if (this.firmId) {
      this.firmService
        .updateFirm(this.firmId, {
          tradingName: this.form.tradingName,
          companyName: this.form.companyName || null,
          address: this.form.address || null,
          suburb: this.form.suburb || null,
          state: this.form.state || null,
          postcode: this.form.postcode || null,
          countryId: null,
          phone1: this.form.phone1 || null,
          phone2: this.form.phone2 || null,
          generalEmail: this.form.generalEmail || null,
          termsId: this.form.termsId || null,
          pricePointId: this.form.pricePointId || null,
          creditLimit: this.form.creditLimit,
          discountPercent1: this.form.discountPercent1,
          allowOrder: this.form.allowOrder,
          allowInvoice: this.form.allowInvoice,
          depositPercent: this.form.depositPercent,
          inactive: this.form.inactive
        })
        .subscribe({
          next: (updated) => {
            this.firm = updated;
            this.saving = false;
          },
          error: (err) => this.handleSaveError(err)
        });
    }
  }

  private handleSaveError(err: { error?: { message?: string } }): void {
    this.saving = false;
    this.saveError = err?.error?.message || 'Something went wrong. Please try again.';
  }

  goBack(): void {
    this.router.navigate(['/firms']);
  }

  private emptyForm(): FirmFormModel {
    return {
      tradingName: '',
      companyName: '',
      code: '',
      entityClassName: 'WholesaleCustomer',
      address: '',
      suburb: '',
      state: '',
      postcode: '',
      phone1: '',
      phone2: '',
      generalEmail: '',
      termsId: '',
      pricePointId: '',
      creditLimit: 0,
      discountPercent1: 0,
      allowOrder: true,
      allowInvoice: true,
      depositPercent: 0,
      inactive: false
    };
  }
}
