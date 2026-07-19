import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ProductService } from '../../services/product.service';
import { ProductListItem } from '../../models/product.model';

const CATEGORY_PALETTE = [
  'cat-indigo',
  'cat-emerald',
  'cat-amber',
  'cat-pink',
  'cat-sky',
  'cat-violet',
  'cat-teal',
  'cat-orange'
];

@Component({
  selector: 'app-products-list',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './products-list.component.html',
  styleUrls: ['./products-list.component.css', '../../shared/list-page.css']
})
export class ProductsListComponent implements OnInit {
  products: ProductListItem[] = [];
  filteredProducts: ProductListItem[] = [];
  searchTerm = '';
  loading = true;
  error = false;

  pageSize = 25;
  page = 1;

  private readonly categoryColorCache = new Map<string, string>();

  constructor(private productService: ProductService) {}

  ngOnInit(): void {
    this.fetchProducts();
  }

  refresh(): void {
    this.fetchProducts();
  }

  private fetchProducts(): void {
    this.loading = true;
    this.error = false;
    this.productService.getAllProducts().subscribe({
      next: (data) => {
        this.products = data;
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
    this.filteredProducts = term
      ? this.products.filter((p) =>
          [p.styleCode, p.garment, p.category, p.label]
            .filter((v): v is string => !!v)
            .some((v) => v.toLowerCase().includes(term))
        )
      : this.products;
    this.page = 1;
  }

  get totalPages(): number {
    return Math.max(1, Math.ceil(this.filteredProducts.length / this.pageSize));
  }

  get pagedProducts(): ProductListItem[] {
    const start = (this.page - 1) * this.pageSize;
    return this.filteredProducts.slice(start, start + this.pageSize);
  }

  get rangeStart(): number {
    return this.filteredProducts.length === 0 ? 0 : (this.page - 1) * this.pageSize + 1;
  }

  get rangeEnd(): number {
    return Math.min(this.page * this.pageSize, this.filteredProducts.length);
  }

  goToPage(page: number): void {
    this.page = Math.min(Math.max(page, 1), this.totalPages);
  }

  clearSearch(): void {
    this.searchTerm = '';
    this.applyFilter();
  }

  categoryColor(category: string | null): string {
    if (!category) {
      return 'cat-neutral';
    }
    let color = this.categoryColorCache.get(category);
    if (!color) {
      let hash = 0;
      for (let i = 0; i < category.length; i++) {
        hash = (hash * 31 + category.charCodeAt(i)) >>> 0;
      }
      color = CATEGORY_PALETTE[hash % CATEGORY_PALETTE.length];
      this.categoryColorCache.set(category, color);
    }
    return color;
  }
}
