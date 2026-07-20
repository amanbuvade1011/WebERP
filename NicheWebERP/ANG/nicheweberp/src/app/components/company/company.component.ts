import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { CompanyService } from '../../services/company.service';
import { Company, UpdateCompanyRequest } from '../../models/company.model';

@Component({
  selector: 'app-company',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './company.component.html',
  styleUrls: ['./company.component.css', '../../shared/list-page.css']
})
export class CompanyComponent implements OnInit {
  company: Company | null = null;
  form: UpdateCompanyRequest = this.emptyForm();
  editing = false;
  loading = true;
  saving = false;
  error = false;

  constructor(private companyService: CompanyService) {}

  ngOnInit(): void {
    this.fetch();
  }

  refresh(): void {
    this.fetch();
  }

  private fetch(): void {
    this.loading = true;
    this.error = false;
    this.companyService.getCurrentCompany().subscribe({
      next: (data) => {
        this.company = data;
        this.loading = false;
      },
      error: () => {
        this.error = true;
        this.loading = false;
      }
    });
  }

  startEdit(): void {
    if (!this.company) {
      return;
    }
    this.form = {
      name: this.company.name,
      address: this.company.address,
      suburb: this.company.suburb,
      state: this.company.state,
      postcode: this.company.postcode,
      countryId: this.company.countryId,
      phone1: this.company.phone1,
      phone2: this.company.phone2,
      fax: this.company.fax,
      generalEmail: this.company.generalEmail,
      companyNumber1: this.company.companyNumber1,
      companyNumber2: this.company.companyNumber2
    };
    this.editing = true;
  }

  cancelEdit(): void {
    this.editing = false;
  }

  save(): void {
    this.saving = true;
    this.companyService.updateCompany(this.form).subscribe({
      next: (data) => {
        this.company = data;
        this.saving = false;
        this.editing = false;
      },
      error: () => {
        this.saving = false;
      }
    });
  }

  private emptyForm(): UpdateCompanyRequest {
    return {
      name: null,
      address: null,
      suburb: null,
      state: null,
      postcode: null,
      countryId: null,
      phone1: null,
      phone2: null,
      fax: null,
      generalEmail: null,
      companyNumber1: null,
      companyNumber2: null
    };
  }
}
