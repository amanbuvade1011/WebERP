import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { CompanyService } from '../../services/company.service';
import { Company } from '../../models/company.model';

const AVATAR_PALETTE = ['cat-indigo', 'cat-emerald', 'cat-amber', 'cat-pink', 'cat-sky', 'cat-violet', 'cat-teal', 'cat-orange'];

@Component({
  selector: 'app-company',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './company.component.html',
  styleUrls: ['./company.component.css', '../../shared/list-page.css']
})
export class CompanyComponent implements OnInit {
  companies: Company[] = [];
  filteredCompanies: Company[] = [];
  searchTerm = '';
  loading = true;
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
    this.companyService.getAllCompanies().subscribe({
      next: (data) => {
        this.companies = data;
        this.applyFilter();
        this.loading = false;
      },
      error: () => {
        this.error = true;
        this.loading = false;
      }
    });
  }

  applyFilter(): void {
    const term = this.searchTerm.trim().toLowerCase();
    this.filteredCompanies = term
      ? this.companies.filter(
          (c) =>
            c.name.toLowerCase().includes(term) ||
            c.code.toLowerCase().includes(term) ||
            c.location.toLowerCase().includes(term)
        )
      : this.companies;
  }

  clearSearch(): void {
    this.searchTerm = '';
    this.applyFilter();
  }

  initials(name: string): string {
    const words = name.replace(/[()]/g, '').split(' ').filter(Boolean);
    return (words[0]?.[0] ?? '') + (words[1]?.[0] ?? '');
  }

  avatarColor(code: string): string {
    let hash = 0;
    for (let i = 0; i < code.length; i++) {
      hash = (hash * 31 + code.charCodeAt(i)) >>> 0;
    }
    return AVATAR_PALETTE[hash % AVATAR_PALETTE.length];
  }
}
