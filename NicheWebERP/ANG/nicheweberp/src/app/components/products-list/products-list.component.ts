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
  items: ProductListItem[] = [];
  totalCount = 0;
  page = 1;
  pageSize = 25;
  searchTerm = '';
  loading = true;
  error = false;

  private readonly categoryColorCache = new Map<string, string>();
  private searchDebounce: ReturnType<typeof setTimeout> | null = null;

  constructor(private productService: ProductService) {}

  ngOnInit(): void {
    this.fetchProducts();
  }

  refresh(): void {
    this.fetchProducts();
  }

  onSearchChange(): void {
    if (this.searchDebounce) {
      clearTimeout(this.searchDebounce);
    }
    this.searchDebounce = setTimeout(() => {
      this.page = 1;
      this.fetchProducts();
    }, 300);
  }

  clearSearch(): void {
    this.searchTerm = '';
    this.page = 1;
    this.fetchProducts();
  }

  private fetchProducts(): void {
    this.loading = true;
    this.error = false;
    this.productService.getAllProducts({ page: this.page, pageSize: this.pageSize, search: this.searchTerm || undefined }).subscribe({
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
    this.fetchProducts();
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
