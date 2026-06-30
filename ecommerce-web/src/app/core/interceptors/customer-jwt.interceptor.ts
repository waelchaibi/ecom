import { HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { CustomerAuthService } from '../services/customer-auth.service';
import { jwtHasRole } from '../utils/jwt.util';

export const customerJwtInterceptor: HttpInterceptorFn = (req, next) => {
  const auth = inject(CustomerAuthService);
  const token = auth.token();
  const url = req.url;
  const needsAuth = url.includes('/api/me') || url.includes('/api/orders');
  if (token && needsAuth && jwtHasRole(token, 'Customer')) {
    req = req.clone({ setHeaders: { Authorization: `Bearer ${token}` } });
  }
  return next(req);
};
