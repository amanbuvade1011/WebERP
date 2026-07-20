import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { CatalogLookupService } from '../../services/catalog-lookup.service';
import { CategoryNode } from '../../models/category.model';

@Component({
  selector: 'app-category-tree',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './category-tree.component.html',
  styleUrls: ['./category-tree.component.css', '../../shared/list-page.css']
})
export class CategoryTreeComponent implements OnInit {
  tree: CategoryNode[] = [];
  loading = true;
  error = false;

  addingUnderId: string | null = null;
  newCategoryName = '';
  adding = false;
  addError = '';

  constructor(private catalogLookupService: CatalogLookupService) {}

  ngOnInit(): void {
    this.fetch();
  }

  refresh(): void {
    this.fetch();
  }

  private fetch(): void {
    this.loading = true;
    this.error = false;
    this.catalogLookupService.getCategoryTree().subscribe({
      next: (tree) => {
        this.tree = tree;
        this.loading = false;
      },
      error: () => {
        this.error = true;
        this.loading = false;
      }
    });
  }

  startAdd(parentId: string | null): void {
    this.addingUnderId = parentId ?? 'root';
    this.newCategoryName = '';
    this.addError = '';
  }

  cancelAdd(): void {
    this.addingUnderId = null;
  }

  submitAdd(): void {
    if (!this.newCategoryName) {
      this.addError = 'Name is required.';
      return;
    }
    const parentId = this.addingUnderId === 'root' ? null : this.addingUnderId;

    this.adding = true;
    this.addError = '';
    this.catalogLookupService.createCategory({ description: this.newCategoryName, parentId }).subscribe({
      next: () => {
        this.adding = false;
        this.addingUnderId = null;
        this.fetch();
      },
      error: (err) => {
        this.adding = false;
        this.addError = err?.error?.message || 'Could not create the category.';
      }
    });
  }
}
