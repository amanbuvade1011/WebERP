import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { map } from 'rxjs/operators';
import { FirmService } from '../../services/firm.service';
import { FirmListItem } from '../../models/firm.model';
import { EntityOption, EntityPickerComponent } from '../entity-picker/entity-picker.component';

@Component({
  selector: 'app-firms-list',
  standalone: true,
  imports: [CommonModule, FormsModule, EntityPickerComponent],
  templateUrl: './firms-list.component.html',
  styleUrls: ['./firms-list.component.css', '../../shared/list-page.css']
})
export class FirmsListComponent implements OnInit {
  items: FirmListItem[] = [];
  totalCount = 0;
  page = 1;
  pageSize = 25;
  searchTerm = '';
  loading = true;
  error = false;

  private searchDebounce: ReturnType<typeof setTimeout> | null = null;

  constructor(private firmService: FirmService, private router: Router) {}

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
    this.firmService
      .getAllFirms({ page: this.page, pageSize: this.pageSize, search: this.searchTerm || undefined })
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

  openFirm(id: string): void {
    this.router.navigate(['/firms', id]);
  }

  createFirm(): void {
    this.router.navigate(['/firms', 'new']);
  }

  // Passed straight into <app-entity-picker> - the generic contract every future picker
  // consumer (Sprint 05, Sprint 08) follows: a search term in, EntityOption[] out.
  firmSearch = (term: string) =>
    this.firmService
      .getAllFirms({ page: 1, pageSize: 10, search: term })
      .pipe(map((result): EntityOption[] => result.items.map((f) => ({
        id: f.id,
        label: f.tradingName,
        sublabel: f.code || undefined
      }))));

  onQuickJump(option: EntityOption): void {
    this.openFirm(option.id);
  }
}
