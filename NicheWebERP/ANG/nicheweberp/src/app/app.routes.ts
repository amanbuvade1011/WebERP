import { Routes } from '@angular/router';
import { DashboardComponent } from './components/dashboard/dashboard.component';
import { ProductsListComponent } from './components/products-list/products-list.component';
import { SalesOrdersComponent } from './components/sales-orders/sales-orders.component';
import { InvoicesComponent } from './components/invoices/invoices.component';
import { CompanyComponent } from './components/company/company.component';
import { LocationsComponent } from './components/locations/locations.component';
import { ErpUsersComponent } from './components/erp-users/erp-users.component';
import { StylesListComponent } from './components/styles-list/styles-list.component';
import { StyleDetailComponent } from './components/style-detail/style-detail.component';
import { SizewayBuilderComponent } from './components/sizeway-builder/sizeway-builder.component';
import { CategoryTreeComponent } from './components/category-tree/category-tree.component';
import { CatalogLookupsComponent } from './components/catalog-lookups/catalog-lookups.component';
import { StockGridComponent } from './components/stock-grid/stock-grid.component';
import { FirmsListComponent } from './components/firms-list/firms-list.component';
import { FirmDetailComponent } from './components/firm-detail/firm-detail.component';
import { RetailCustomersListComponent } from './components/retail-customers-list/retail-customers-list.component';
import { LoginComponent } from './components/login/login.component';
import { SalesOrderDetailComponent } from './components/sales-order-detail/sales-order-detail.component';
import { authGuard } from './guards/auth.guard';

export const routes: Routes = [
  { path: '', redirectTo: 'dashboard', pathMatch: 'full' },
  { path: 'login', component: LoginComponent },
  { path: 'dashboard', component: DashboardComponent, canActivate: [authGuard] },
  { path: 'products', component: ProductsListComponent, canActivate: [authGuard] },
  { path: 'styles', component: StylesListComponent, canActivate: [authGuard] },
  { path: 'styles/:id', component: StyleDetailComponent, canActivate: [authGuard] },
  { path: 'styles/:id/stock/:colorId', component: StockGridComponent, canActivate: [authGuard] },
  { path: 'sizeways', component: SizewayBuilderComponent, canActivate: [authGuard] },
  { path: 'categories', component: CategoryTreeComponent, canActivate: [authGuard] },
  { path: 'catalog-lookups', component: CatalogLookupsComponent, canActivate: [authGuard] },
  { path: 'firms', component: FirmsListComponent, canActivate: [authGuard] },
  { path: 'firms/:id', component: FirmDetailComponent, canActivate: [authGuard] },
  { path: 'retail-customers', component: RetailCustomersListComponent, canActivate: [authGuard] },
  { path: 'sales-orders', component: SalesOrdersComponent, canActivate: [authGuard] },
  { path: 'sales-orders/:id', component: SalesOrderDetailComponent, canActivate: [authGuard] },
  { path: 'invoices', component: InvoicesComponent, canActivate: [authGuard] },
  { path: 'company', component: CompanyComponent, canActivate: [authGuard] },
  { path: 'locations', component: LocationsComponent, canActivate: [authGuard] },
  { path: 'users', component: ErpUsersComponent, canActivate: [authGuard] }
];
