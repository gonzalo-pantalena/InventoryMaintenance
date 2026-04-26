import { HttpErrorResponse, HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { Router } from '@angular/router';
import { catchError, switchMap, throwError } from 'rxjs';
import { AuthService } from './auth.service';

function isAuthEndpoint(url: string): boolean {
  return url.includes('/api/auth/login') || url.includes('/api/auth/refresh');
}

export const authInterceptor: HttpInterceptorFn = (req, next) => {
  const auth = inject(AuthService);
  const router = inject(Router);

  let outgoing = req;
  const token = auth.accessToken();
  if (token && !isAuthEndpoint(req.url)) {
    outgoing = req.clone({ setHeaders: { Authorization: `Bearer ${token}` } });
  }

  return next(outgoing).pipe(
    catchError((err: unknown) => {
      if (!(err instanceof HttpErrorResponse) || err.status !== 401) {
        return throwError(() => err);
      }
      if (isAuthEndpoint(req.url) || !auth.accessToken()) {
        auth.logout();
        return throwError(() => err);
      }

      return auth.refreshSessionOrThrow().pipe(
        switchMap((newToken) => {
          const retry = req.clone({ setHeaders: { Authorization: `Bearer ${newToken}` } });
          return next(retry);
        }),
        catchError(() => {
          auth.logout();
          void router.navigateByUrl('/login');
          return throwError(() => err);
        }),
      );
    }),
  );
};
