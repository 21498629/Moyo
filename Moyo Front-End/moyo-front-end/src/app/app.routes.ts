import { Routes } from '@angular/router';
import { LoginComponent } from './pages/login/login.component';
import { RegistrationComponent } from './pages/registration/registration.component';
import { ProductListing } from './pages/product-listing/product-listing.component';
import { ProductManagementComponent } from './pages/product-management/product-management.component';
import { VendorManagementComponent } from './pages/vendor-management/vendor-management.component';
import { authGuard } from './guards/auth-guard';

export const routes: Routes = [
  { path: '', redirectTo: '/products', pathMatch: 'full' },
  { path: 'login', component: LoginComponent },
  { path: 'register', component: RegistrationComponent },
  { path: 'registration', component: RegistrationComponent },
  { path: 'products', component: ProductListing, canActivate: [authGuard] },
  { path: 'landing', component: ProductListing, canActivate: [authGuard] },
  { path: 'product-management', component: ProductManagementComponent, canActivate: [authGuard] },
  { path: 'vendor-management', component: VendorManagementComponent, canActivate: [authGuard] }
];
