import { ApplicationConfig } from '@angular/core';
import { provideRouter } from '@angular/router';
import { provideHttpClient, HTTP_INTERCEPTORS } from '@angular/common/http';
import { provideStore } from '@ngrx/store';
import { provideEffects } from '@ngrx/effects';
import { provideStoreDevtools } from '@ngrx/store-devtools';
import { environment } from '../environments/environment';

import { routes } from './app.routes';
import { JwtInterceptor } from './infrastructure/interceptors/jwt.interceptor';
import { authReducer, AuthEffects } from './store/auth';

/**
 * Application Configuration
 * Configuración de la aplicación Angular con dependencias inyectadas
 * 
 * NOTA: Requires @ngrx packages:
 * npm install @ngrx/store @ngrx/effects @ngrx/store-devtools @ngrx/schematics --save
 */
export const appConfig: ApplicationConfig = {
  providers: [
    provideRouter(routes),
    provideHttpClient(),
    
    // NgRx Store
    provideStore(
      { auth: authReducer },
      {
        runtimeChecks: {
          strictStateImmutability: true,
          strictActionImmutability: true,
          strictStateSerializability: true,
          strictActionSerializability: true,
          strictActionWithinNgZone: true,
          strictActionTypeUniqueness: true
        }
      }
    ),
    
    // NgRx Effects
    provideEffects([AuthEffects]),
    
    // NgRx Store Devtools (solo en development)
    provideStoreDevtools({ maxAge: 25, logOnly: environment.production }),
    
    // HTTP Interceptor para JWT
    {
      provide: HTTP_INTERCEPTORS,
      useClass: JwtInterceptor,
      multi: true
    }
  ]
};

