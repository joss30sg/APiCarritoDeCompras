import { Routes } from '@angular/router';
import { LoginComponent } from './presentation/pages/login/login.component';
import { RegisterComponent } from './presentation/pages/register/register.component';
import { HomeComponent } from './presentation/pages/home/home.component';
import { ShoppingCartPageComponent } from './presentation/pages/shopping-cart-page/shopping-cart-page.component';
import { OrderHistoryComponent } from './presentation/components/order-history/order-history.component';
import { authGuard } from './infrastructure/guards/auth.guard';
import { noAuthGuard } from './infrastructure/guards/no-auth.guard';

/**
 * Application Routes
 * Rutas de la aplicación con guards de autenticación
 */
export const routes: Routes = [
  {
    path: 'login',
    component: LoginComponent,
    canActivate: [noAuthGuard]
  },
  {
    path: 'register',
    component: RegisterComponent,
    canActivate: [noAuthGuard]
  },
  {
    path: '',
    component: HomeComponent,
    canActivate: [authGuard]
  },
  {
    path: 'cart',
    component: ShoppingCartPageComponent,
    canActivate: [authGuard]
  },
  {
    path: 'order-history',
    component: OrderHistoryComponent,
    canActivate: [authGuard]
  },
  {
    path: '**',
    redirectTo: ''
  }
];
