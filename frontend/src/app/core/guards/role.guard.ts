import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { AuthService } from '../services/auth.service';

export const roleGuard = (allowedRoles: string[]): CanActivateFn => {
  return () => {
    const auth = inject(AuthService);
    const router = inject(Router);

    if (!auth.isAuthenticated()) {
      return router.createUrlTree(['/login']);
    }

    const currentRole = auth.userRole();

    // Admin has universal permission to access all modules
    if (currentRole === 'Admin') {
      return true;
    }

    if (allowedRoles.includes(currentRole)) {
      return true;
    }

    // Role unauthorized -> redirect to appropriate portal landing
    if (currentRole === 'Candidate') {
      return router.createUrlTree(['/candidate/dashboard']);
    }

    if (currentRole === 'HRManager' || currentRole === 'Recruiter') {
      return router.createUrlTree(['/hr/dashboard']);
    }

    return router.createUrlTree(['/403']);
  };
};
