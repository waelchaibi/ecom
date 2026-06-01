import { ApplicationConfig, provideZoneChangeDetection } from '@angular/core';
import { provideHttpClient, withFetch, withInterceptors } from '@angular/common/http';
import { provideRouter } from '@angular/router';

import { routes } from './app.routes';
import { adminJwtInterceptor } from './core/interceptors/admin-jwt.interceptor';
import { customerJwtInterceptor } from './core/interceptors/customer-jwt.interceptor';

export const appConfig: ApplicationConfig = {
  providers: [
    provideZoneChangeDetection({ eventCoalescing: true }),
    provideRouter(routes),
    provideHttpClient(withFetch(), withInterceptors([adminJwtInterceptor, customerJwtInterceptor]))
  ]
};
