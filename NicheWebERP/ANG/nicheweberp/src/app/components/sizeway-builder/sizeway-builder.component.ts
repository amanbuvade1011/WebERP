import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { forkJoin } from 'rxjs';
import { SizewayService } from '../../services/sizeway.service';
import { Size, Sizeway, SizewayItem } from '../../models/sizeway.model';

@Component({
  selector: 'app-sizeway-builder',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './sizeway-builder.component.html',
  styleUrls: ['./sizeway-builder.component.css', '../../shared/list-page.css']
})
export class SizewayBuilderComponent implements OnInit {
  sizeways: Sizeway[] = [];
  allSizes: Size[] = [];
  loading = true;
  error = false;

  selectedSizewayId: string | null = null;
  isCreating = false;
  builderName = '';
  builderSizes: SizewayItem[] = [];
  sizeToAdd = '';

  saving = false;
  saveError = '';

  constructor(private sizewayService: SizewayService) {}

  ngOnInit(): void {
    this.fetch();
  }

  refresh(): void {
    this.fetch();
  }

  private fetch(): void {
    this.loading = true;
    this.error = false;
    forkJoin({
      sizeways: this.sizewayService.getAllSizeways(),
      sizes: this.sizewayService.getAllSizes()
    }).subscribe({
      next: ({ sizeways, sizes }) => {
        this.sizeways = sizeways;
        this.allSizes = sizes;
        this.loading = false;
      },
      error: () => {
        this.error = true;
        this.loading = false;
      }
    });
  }

  selectSizeway(sw: Sizeway): void {
    this.isCreating = false;
    this.selectedSizewayId = sw.id;
    this.builderName = sw.description;
    this.builderSizes = [...sw.sizes].sort((a, b) => a.sequence - b.sequence);
    this.saveError = '';
  }

  startCreate(): void {
    this.isCreating = true;
    this.selectedSizewayId = null;
    this.builderName = '';
    this.builderSizes = [];
    this.saveError = '';
  }

  cancelBuilder(): void {
    this.isCreating = false;
    this.selectedSizewayId = null;
    this.builderSizes = [];
  }

  get availableSizes(): Size[] {
    const usedIds = new Set(this.builderSizes.map((s) => s.sizeId));
    return this.allSizes.filter((s) => !usedIds.has(s.id));
  }

  addSize(): void {
    if (!this.sizeToAdd) {
      return;
    }
    const size = this.allSizes.find((s) => s.id === this.sizeToAdd);
    if (!size) {
      return;
    }
    this.builderSizes.push({ sizeId: size.id, sizeDescription: size.description, sequence: this.builderSizes.length + 1 });
    this.sizeToAdd = '';
  }

  removeSize(sizeId: string): void {
    this.builderSizes = this.builderSizes.filter((s) => s.sizeId !== sizeId);
  }

  moveUp(index: number): void {
    if (index <= 0) {
      return;
    }
    [this.builderSizes[index - 1], this.builderSizes[index]] = [this.builderSizes[index], this.builderSizes[index - 1]];
  }

  moveDown(index: number): void {
    if (index >= this.builderSizes.length - 1) {
      return;
    }
    [this.builderSizes[index], this.builderSizes[index + 1]] = [this.builderSizes[index + 1], this.builderSizes[index]];
  }

  save(): void {
    if (this.builderSizes.length === 0) {
      this.saveError = 'Add at least one size.';
      return;
    }

    const sizeIds = this.builderSizes.map((s) => s.sizeId);
    this.saving = true;
    this.saveError = '';

    if (this.isCreating) {
      if (!this.builderName) {
        this.saveError = 'Name is required.';
        this.saving = false;
        return;
      }
      this.sizewayService.createSizeway({ description: this.builderName, excludeRetailSearch: false, sizeIds }).subscribe({
        next: (created) => {
          this.saving = false;
          this.fetch();
          this.selectedSizewayId = created.id;
          this.isCreating = false;
        },
        error: (err) => this.handleError(err)
      });
    } else if (this.selectedSizewayId) {
      this.sizewayService.updateSizeSequence(this.selectedSizewayId, { sizeIds }).subscribe({
        next: () => {
          this.saving = false;
          this.fetch();
        },
        error: (err) => this.handleError(err)
      });
    }
  }

  private handleError(err: { error?: { message?: string } }): void {
    this.saving = false;
    this.saveError = err?.error?.message || 'Something went wrong. Please try again.';
  }
}
