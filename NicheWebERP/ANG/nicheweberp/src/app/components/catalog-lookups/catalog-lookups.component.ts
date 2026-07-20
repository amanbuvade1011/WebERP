import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { forkJoin } from 'rxjs';
import { CatalogLookupService } from '../../services/catalog-lookup.service';
import { Label, RangeNode, Season } from '../../models/category.model';

@Component({
  selector: 'app-catalog-lookups',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './catalog-lookups.component.html',
  styleUrls: ['./catalog-lookups.component.css', '../../shared/list-page.css']
})
export class CatalogLookupsComponent implements OnInit {
  labels: Label[] = [];
  ranges: RangeNode[] = [];
  seasons: Season[] = [];
  loading = true;
  error = false;

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
    forkJoin({
      labels: this.catalogLookupService.getAllLabels(),
      ranges: this.catalogLookupService.getRangeTree(),
      seasons: this.catalogLookupService.getAllSeasons()
    }).subscribe({
      next: ({ labels, ranges, seasons }) => {
        this.labels = labels;
        this.ranges = ranges;
        this.seasons = seasons;
        this.loading = false;
      },
      error: () => {
        this.error = true;
        this.loading = false;
      }
    });
  }

  flattenRange(node: RangeNode, depth = 0): { label: string; depth: number }[] {
    const result = [{ label: node.description || '(unnamed)', depth }];
    for (const child of node.children) {
      result.push(...this.flattenRange(child, depth + 1));
    }
    return result;
  }

  get flatRanges(): { label: string; depth: number }[] {
    return this.ranges.flatMap((r) => this.flattenRange(r));
  }
}
