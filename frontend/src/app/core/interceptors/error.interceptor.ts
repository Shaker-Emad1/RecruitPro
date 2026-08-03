import { HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { catchError, throwError } from 'rxjs';
import { AuthService } from '../services/auth.service';
import { ToastService } from '../services/toast.service';

export const errorInterceptor: HttpInterceptorFn = (req, next) => {
  const auth = inject(AuthService);
  const toast = inject(ToastService);

  return next(req).pipe(
    catchError((err) => {
      if (err.status === 401) {
        auth.logout();
      } else if (err.status === 403) {
        toast.error('You do not have permission to perform this action.');
      } else if (err.status >= 500) {
        toast.error('A server error occurred. Please try again later.');
      } else if (err.status >= 400) {
        const msg = err.error?.message ?? 'An unexpected error occurred.';
        toast.error(msg);
      }
      return throwError(() => err);
    }),
  );
};
