import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { AuthService } from '../services/auth.service';

export const guestGuard: CanActivateFn = () => {
  const auth = inject(AuthService);
  const router = inject(Router);

  if (!auth.isAuthenticated()) {
    return true;
  }

  const role = auth.userRole();
  if (role === 'Candidate') {
    return router.createUrlTree(['/candidate/dashboard']);
  }

  if (role === 'Admin') {
    return router.createUrlTree(['/admin/dashboard']);
  }

  return router.createUrlTree(['/hr/dashboard']);
};
