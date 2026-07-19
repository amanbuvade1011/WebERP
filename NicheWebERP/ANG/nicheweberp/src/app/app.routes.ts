import { Routes } from '@angular/router';
import { DashboardComponent } from './components/dashboard/dashboard.component';
import { ProductsListComponent } from './components/products-list/products-list.component';
import { SalesOrdersComponent } from './components/sales-orders/sales-orders.component';
import { InvoicesComponent } from './components/invoices/invoices.component';
import { CompanyComponent } from './components/company/company.component';

export const routes: Routes = [
  { path: '', redirectTo: 'dashboard', pathMatch: 'full' },
  { path: 'dashboard', component: DashboardComponent },
  { path: 'products', component: ProductsListComponent },
  { path: 'sales-orders', component: SalesOrdersComponent },
  { path: 'invoices', component: InvoicesComponent },
  { path: 'company', component: CompanyComponent }
];
