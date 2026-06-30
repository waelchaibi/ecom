import { HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { AdminAuthService } from '../services/admin-auth.service';

import { jwtHasRole } from '../utils/jwt.util';

export const adminJwtInterceptor: HttpInterceptorFn = (req, next) => {
  const auth = inject(AdminAuthService);
  const token = auth.token();
  const url = req.url;
  const needsAuth =
    url.includes('/api/admin') || url.includes('/api/analytics');
  if (token && needsAuth && jwtHasRole(token, 'Admin')) {
    req = req.clone({ setHeaders: { Authorization: `Bearer ${token}` } });
  }
  return next(req);
};
