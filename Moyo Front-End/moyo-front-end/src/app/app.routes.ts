import { Routes } from '@angular/router';
import { LoginComponent } from './pages/login/login.component';
import { RegistrationComponent } from './pages/registration/registration.component';
import { ProductListing } from './pages/product-listing/product-listing.component';
import { authGuard } from './guards/auth-guard';

export const routes: Routes = [
  { path: '', redirectTo: '/products', pathMatch: 'full' },
  { path: 'login', component: LoginComponent },
  { path: 'register', component: RegistrationComponent },
  { path: 'registration', component: RegistrationComponent },
  { path: 'products', component: ProductListing, canActivate: [authGuard] },
  { path: 'landing', component: ProductListing, canActivate: [authGuard] }
];
