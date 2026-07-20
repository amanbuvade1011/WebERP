import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router } from '@angular/router';
import { forkJoin } from 'rxjs';
import { ProductService } from '../../services/product.service';
import { StyleService } from '../../services/style.service';
import { StockGridRow } from '../../models/product.model';
import { StyleDetail } from '../../models/style.model';

interface GridCell {
  held: number;
  allocated: number;
  available: number;
}

@Component({
  selector: 'app-stock-grid',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './stock-grid.component.html',
  styleUrls: ['./stock-grid.component.css', '../../shared/list-page.css']
})
export class StockGridComponent implements OnInit {
  styleId!: string;
  colorId!: string;
  style: StyleDetail | null = null;
  colorName = '';

  sizes: string[] = [];
  locations: string[] = [];
  grid = new Map<string, GridCell>();

  loading = true;
  error = false;

  constructor(private route: ActivatedRoute, private router: Router, private productService: ProductService, private styleService: StyleService) {}

  ngOnInit(): void {
    this.styleId = this.route.snapshot.paramMap.get('id')!;
    this.colorId = this.route.snapshot.paramMap.get('colorId')!;
    this.fetch();
  }

  private fetch(): void {
    this.loading = true;
    this.error = false;

    forkJoin({
      style: this.styleService.getStyleById(this.styleId),
      rows: this.productService.getStockByStyleColor(this.styleId, this.colorId)
    }).subscribe({
      next: ({ style, rows }) => {
        this.style = style;
        this.colorName = style.colors.find((c) => c.id === this.colorId)?.color || '';
        this.buildGrid(rows);
        this.loading = false;
      },
      error: () => {
        this.error = true;
        this.loading = false;
      }
    });
  }

  private buildGrid(rows: StockGridRow[]): void {
    const sizeOrder: string[] = [];
    const locationSet = new Set<string>();
    this.grid = new Map();

    for (const row of rows) {
      if (!sizeOrder.includes(row.sizeDescription)) {
        sizeOrder.push(row.sizeDescription);
      }
      const locationLabel = row.locationName || 'Unassigned';
      locationSet.add(locationLabel);
      this.grid.set(`${row.sizeDescription}|${locationLabel}`, {
        held: row.held,
        allocated: row.allocated,
        available: row.available
      });
    }

    this.sizes = sizeOrder;
    this.locations = Array.from(locationSet);
  }

  cell(size: string, location: string): GridCell {
    return this.grid.get(`${size}|${location}`) || { held: 0, allocated: 0, available: 0 };
  }

  refresh(): void {
    this.fetch();
  }

  goBack(): void {
    this.router.navigate(['/styles', this.styleId]);
  }
}
