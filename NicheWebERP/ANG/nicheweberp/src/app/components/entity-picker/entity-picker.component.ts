import { Component, EventEmitter, Input, Output } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Observable, Subject, of } from 'rxjs';
import { catchError, debounceTime, switchMap, tap } from 'rxjs/operators';

export interface EntityOption {
  id: string;
  label: string;
  sublabel?: string;
}

// Generic typeahead picker - NOT hardcoded to any one entity type. The caller supplies a
// searchFn (e.g. wrapping FirmService.getAllFirms, RetailCustomerService.getAllRetailCustomers,
// or a future ProductService search) that maps a search term to EntityOption[]. Built in
// Sprint 04 for the Firms list; Sprint 05 (order line product/customer picking) and Sprint 08
// (cutting sheet) should reuse this component as-is, not copy it - see
// docs/ai-plan/sprints/sprint-04-sales-customers.md's acceptance criteria.
@Component({
  selector: 'app-entity-picker',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './entity-picker.component.html',
  styleUrls: ['./entity-picker.component.css']
})
export class EntityPickerComponent {
  @Input() placeholder = 'Search…';
  @Input() searchFn!: (term: string) => Observable<EntityOption[]>;
  @Output() selected = new EventEmitter<EntityOption>();

  term = '';
  results: EntityOption[] = [];
  loading = false;
  open = false;

  private readonly search$ = new Subject<string>();

  constructor() {
    this.search$
      .pipe(
        debounceTime(250),
        tap(() => (this.loading = true)),
        switchMap((term) =>
          term.length < 2
            ? of<EntityOption[]>([])
            : this.searchFn(term).pipe(catchError(() => of<EntityOption[]>([])))
        )
      )
      .subscribe((results) => {
        this.results = results;
        this.loading = false;
      });
  }

  onInput(): void {
    this.open = true;
    this.search$.next(this.term);
  }

  choose(option: EntityOption): void {
    this.selected.emit(option);
    this.term = option.label;
    this.open = false;
    this.results = [];
  }

  onBlur(): void {
    // Slight delay so a click on a result fires before the blur closes the dropdown.
    setTimeout(() => (this.open = false), 150);
  }
}
