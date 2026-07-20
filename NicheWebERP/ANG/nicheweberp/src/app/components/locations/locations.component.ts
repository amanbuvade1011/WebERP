import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { LocationService } from '../../services/location.service';
import { CreateLocationRequest, Location } from '../../models/location.model';

@Component({
  selector: 'app-locations',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './locations.component.html',
  styleUrls: ['./locations.component.css', '../../shared/list-page.css']
})
export class LocationsComponent implements OnInit {
  locations: Location[] = [];
  loading = true;
  error = false;

  showCreateForm = false;
  creating = false;
  createError = '';
  form: CreateLocationRequest = this.emptyForm();

  constructor(private locationService: LocationService) {}

  ngOnInit(): void {
    this.fetch();
  }

  refresh(): void {
    this.fetch();
  }

  private fetch(): void {
    this.loading = true;
    this.error = false;
    this.locationService.getAllLocations().subscribe({
      next: (data) => {
        this.locations = data;
        this.loading = false;
      },
      error: () => {
        this.error = true;
        this.loading = false;
      }
    });
  }

  parentName(parentId: string | null): string {
    if (!parentId) {
      return '—';
    }
    return this.locations.find((l) => l.id === parentId)?.name || '—';
  }

  openCreateForm(): void {
    this.form = this.emptyForm();
    this.createError = '';
    this.showCreateForm = true;
  }

  cancelCreate(): void {
    this.showCreateForm = false;
  }

  submitCreate(): void {
    if (!this.form.name || !this.form.parentId) {
      this.createError = 'Name and parent location are required.';
      return;
    }

    this.creating = true;
    this.createError = '';
    this.locationService.createLocation(this.form).subscribe({
      next: () => {
        this.creating = false;
        this.showCreateForm = false;
        this.fetch();
      },
      error: () => {
        this.creating = false;
        this.createError = 'Could not create the location. Please try again.';
      }
    });
  }

  private emptyForm(): CreateLocationRequest {
    return {
      name: '',
      code: null,
      parentId: this.locations.find((l) => !l.parentId)?.id || '',
      countryId: null
    };
  }
}
