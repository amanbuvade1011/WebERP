import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { StyleService } from '../../services/style.service';
import { StyleListItem } from '../../models/style.model';

@Component({
  selector: 'app-styles-list',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './styles-list.component.html',
  styleUrls: ['./styles-list.component.css', '../../shared/list-page.css']
})
export class StylesListComponent implements OnInit {
  items: StyleListItem[] = [];
  totalCount = 0;
  page = 1;
  pageSize = 25;
  searchTerm = '';
  loading = true;
  error = false;

  private searchDebounce: ReturnType<typeof setTimeout> | null = null;

  constructor(private styleService: StyleService, private router: Router) {}

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

  clearSearch(): void {
    this.searchTerm = '';
    this.page = 1;
    this.fetch();
  }

  private fetch(): void {
    this.loading = true;
    this.error = false;
    this.styleService
      .getAllStyles({ page: this.page, pageSize: this.pageSize, search: this.searchTerm || undefined })
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

  openStyle(id: string): void {
    this.router.navigate(['/styles', id]);
  }

  createStyle(): void {
    this.router.navigate(['/styles', 'new']);
  }
}
