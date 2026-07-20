import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { forkJoin, of } from 'rxjs';
import { StyleService } from '../../services/style.service';
import { SizewayService } from '../../services/sizeway.service';
import { CatalogLookupService } from '../../services/catalog-lookup.service';
import { ProductService } from '../../services/product.service';
import { LocationService } from '../../services/location.service';
import { StyleDetail } from '../../models/style.model';
import { Sizeway } from '../../models/sizeway.model';
import { CategoryNode, Label, RangeNode } from '../../models/category.model';
import { PricePoint, StyleSellLocation } from '../../models/product.model';
import { Location } from '../../models/location.model';

interface FlatOption {
  id: string;
  label: string;
  depth: number;
}

interface StyleFormModel {
  code: string;
  description: string;
  webDescription: string;
  weight: number;
  sizewayId: string;
  categoryId: string;
  labelId: string;
  rangeId: string;
  deliveryPeriod: string;
  inactive: boolean;
}

interface PriceRow {
  pricePointId: string;
  pricePointName: string | null;
  localUnitPriceExTax1: number;
  localUnitPriceTax1: number;
  internationalUnitPriceExTax1: number;
  internationalUnitPriceTax1: number;
}

@Component({
  selector: 'app-style-detail',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './style-detail.component.html',
  styleUrls: ['./style-detail.component.css', '../../shared/list-page.css']
})
export class StyleDetailComponent implements OnInit {
  isNew = false;
  styleId: string | null = null;
  style: StyleDetail | null = null;

  sizeways: Sizeway[] = [];
  labels: Label[] = [];
  flatCategories: FlatOption[] = [];
  flatRanges: FlatOption[] = [];

  loading = true;
  error = false;
  saving = false;
  saveError = '';

  form: StyleFormModel = this.emptyForm();

  newColorName = '';
  newColorRgb = '';
  addingColor = false;
  addColorError = '';

  priceRows: PriceRow[] = [];
  savingPrices = false;
  priceError = '';

  sellLocationRows: StyleSellLocation[] = [];
  savingSellLocations = false;
  sellLocationError = '';

  generatingColorId: string | null = null;
  generateMessage = '';
  generateError = '';

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private styleService: StyleService,
    private sizewayService: SizewayService,
    private catalogLookupService: CatalogLookupService,
    private productService: ProductService,
    private locationService: LocationService
  ) {}

  ngOnInit(): void {
    const idParam = this.route.snapshot.paramMap.get('id');
    this.isNew = idParam === 'new';
    this.styleId = this.isNew ? null : idParam;
    this.fetch();
  }

  private fetch(): void {
    this.loading = true;
    this.error = false;

    forkJoin({
      sizeways: this.sizewayService.getAllSizeways(),
      labels: this.catalogLookupService.getAllLabels(),
      categories: this.catalogLookupService.getCategoryTree(),
      ranges: this.catalogLookupService.getRangeTree(),
      style: this.isNew || !this.styleId ? of(null) : this.styleService.getStyleById(this.styleId),
      pricePoints: this.isNew ? of([]) : this.productService.getAllPricePoints(),
      existingPrices: this.isNew || !this.styleId ? of([]) : this.productService.getStylePrices(this.styleId),
      allLocations: this.isNew ? of([]) : this.locationService.getAllLocations(),
      existingSellLocations: this.isNew || !this.styleId ? of([]) : this.productService.getSellLocations(this.styleId)
    }).subscribe({
      next: (result) => {
        this.sizeways = result.sizeways;
        this.labels = result.labels;
        this.flatCategories = this.flattenCategories(result.categories);
        this.flatRanges = this.flattenRanges(result.ranges);

        if (result.style) {
          this.style = result.style as StyleDetail;
          this.form = {
            code: this.style.code,
            description: this.style.description,
            webDescription: this.style.webDescription || '',
            weight: this.style.weight,
            sizewayId: this.style.sizewayId,
            categoryId: this.style.categoryId || '',
            labelId: this.style.labelId || '',
            rangeId: this.style.rangeId,
            deliveryPeriod: this.style.deliveryPeriod || '',
            inactive: this.style.inactive
          };
        } else {
          this.form = this.emptyForm();
        }

        this.priceRows = this.mergePrices(result.pricePoints as PricePoint[], result.existingPrices as PriceRow[]);
        this.sellLocationRows = this.mergeSellLocations(
          result.allLocations as Location[],
          result.existingSellLocations as StyleSellLocation[]
        );

        this.loading = false;
      },
      error: () => {
        this.error = true;
        this.loading = false;
      }
    });
  }

  private mergePrices(pricePoints: PricePoint[], existing: PriceRow[]): PriceRow[] {
    const byId = new Map(existing.map((p) => [p.pricePointId, p]));
    return pricePoints.map(
      (pp) =>
        byId.get(pp.id) || {
          pricePointId: pp.id,
          pricePointName: pp.name,
          localUnitPriceExTax1: 0,
          localUnitPriceTax1: 0,
          internationalUnitPriceExTax1: 0,
          internationalUnitPriceTax1: 0
        }
    );
  }

  private mergeSellLocations(locations: Location[], existing: StyleSellLocation[]): StyleSellLocation[] {
    const byId = new Map(existing.map((l) => [l.locationId, l]));
    return locations.map(
      (loc) =>
        byId.get(loc.id) || {
          locationId: loc.id,
          locationName: loc.name,
          allowRetail: false,
          allowWebRetail: false,
          allowRental: false,
          allowWholesaleIndent: false
        }
    );
  }

  private flattenCategories(nodes: CategoryNode[], depth = 0): FlatOption[] {
    const result: FlatOption[] = [];
    for (const n of nodes) {
      result.push({ id: n.id, label: `${'—'.repeat(depth)} ${n.description}`.trim(), depth });
      result.push(...this.flattenCategories(n.children, depth + 1));
    }
    return result;
  }

  private flattenRanges(nodes: RangeNode[], depth = 0): FlatOption[] {
    const result: FlatOption[] = [];
    for (const n of nodes) {
      result.push({ id: n.id, label: `${'—'.repeat(depth)} ${n.description || '(unnamed)'}`.trim(), depth });
      result.push(...this.flattenRanges(n.children, depth + 1));
    }
    return result;
  }

  save(): void {
    if (!this.form.description || !this.form.sizewayId || !this.form.rangeId) {
      this.saveError = 'Description, sizeway, and range are required.';
      return;
    }

    this.saving = true;
    this.saveError = '';

    if (this.isNew) {
      if (!this.form.code) {
        this.saveError = 'Code is required.';
        this.saving = false;
        return;
      }
      this.styleService
        .createStyle({
          code: this.form.code,
          description: this.form.description,
          webDescription: this.form.webDescription || null,
          weight: this.form.weight,
          sizewayId: this.form.sizewayId,
          categoryId: this.form.categoryId || null,
          labelId: this.form.labelId || null,
          rangeId: this.form.rangeId,
          deliveryPeriod: this.form.deliveryPeriod || null
        })
        .subscribe({
          next: (created) => {
            this.saving = false;
            this.router.navigate(['/styles', created.id]);
          },
          error: (err) => this.handleSaveError(err)
        });
    } else if (this.styleId) {
      this.styleService
        .updateStyle(this.styleId, {
          description: this.form.description,
          webDescription: this.form.webDescription || null,
          weight: this.form.weight,
          sizewayId: this.form.sizewayId,
          categoryId: this.form.categoryId || null,
          labelId: this.form.labelId || null,
          rangeId: this.form.rangeId,
          deliveryPeriod: this.form.deliveryPeriod || null,
          inactive: this.form.inactive
        })
        .subscribe({
          next: (updated) => {
            this.style = updated;
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

  addColor(): void {
    if (!this.styleId || !this.newColorName) {
      return;
    }
    this.addingColor = true;
    this.addColorError = '';
    this.styleService.addColor(this.styleId, { color: this.newColorName, rgbValue: this.newColorRgb || null }).subscribe({
      next: () => {
        this.addingColor = false;
        this.newColorName = '';
        this.newColorRgb = '';
        this.fetch();
      },
      error: (err) => {
        this.addingColor = false;
        this.addColorError = err?.error?.message || 'Could not add the color.';
      }
    });
  }

  savePrices(): void {
    if (!this.styleId) {
      return;
    }
    this.savingPrices = true;
    this.priceError = '';
    this.productService.updateStylePrices(this.styleId, this.priceRows).subscribe({
      next: (updated) => {
        this.savingPrices = false;
        this.priceRows = updated.map((p) => ({ ...p }));
      },
      error: (err) => {
        this.savingPrices = false;
        this.priceError = err?.error?.message || 'Could not save prices.';
      }
    });
  }

  saveSellLocations(): void {
    if (!this.styleId) {
      return;
    }
    this.savingSellLocations = true;
    this.sellLocationError = '';
    this.productService.updateSellLocations(this.styleId, this.sellLocationRows).subscribe({
      next: (updated) => {
        this.savingSellLocations = false;
        this.sellLocationRows = updated.map((l) => ({ ...l }));
      },
      error: (err) => {
        this.savingSellLocations = false;
        this.sellLocationError = err?.error?.message || 'Could not save sell locations.';
      }
    });
  }

  generateProducts(colorId: string): void {
    if (!this.styleId) {
      return;
    }
    this.generatingColorId = colorId;
    this.generateMessage = '';
    this.generateError = '';
    this.productService.generateProducts(this.styleId, colorId).subscribe({
      next: (result) => {
        this.generatingColorId = null;
        this.generateMessage = `${result.productsCreated} product(s) and ${result.productLocationsCreated} stock row(s) created (${result.totalProductsForColor} total for this color).`;
      },
      error: (err) => {
        this.generatingColorId = null;
        this.generateError = err?.error?.message || 'Could not generate products.';
      }
    });
  }

  viewStock(colorId: string): void {
    if (!this.styleId) {
      return;
    }
    this.router.navigate(['/styles', this.styleId, 'stock', colorId]);
  }

  goBack(): void {
    this.router.navigate(['/styles']);
  }

  private emptyForm(): StyleFormModel {
    return {
      code: '',
      description: '',
      webDescription: '',
      weight: 0,
      sizewayId: '',
      categoryId: '',
      labelId: '',
      rangeId: '',
      deliveryPeriod: '',
      inactive: false
    };
  }
}
