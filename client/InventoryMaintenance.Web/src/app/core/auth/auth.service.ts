import { isPlatformBrowser } from '@angular/common';
import { HttpClient } from '@angular/common/http';
import { Injectable, PLATFORM_ID, computed, inject, signal } from '@angular/core';
import { Router } from '@angular/router';
import { Observable, catchError, map, of, tap } from 'rxjs';
import { environment } from '../../../environments/environment';
import type { LoginResponse, UserSummary } from '../models/api.models';

const STORAGE_ACCESS = 'im.accessToken';
const STORAGE_REFRESH = 'im.refreshToken';
const STORAGE_USER = 'im.user';

@Injectable({ providedIn: 'root' })
export class AuthService {
  private readonly http = inject(HttpClient);
  private readonly router = inject(Router);
  private readonly platformId = inject(PLATFORM_ID);

  private readonly userSignal = signal<UserSummary | null>(null);
  private readonly accessTokenSignal = signal<string | null>(null);
  private readonly refreshTokenSignal = signal<string | null>(null);

  readonly user = this.userSignal.asReadonly();
  readonly accessToken = this.accessTokenSignal.asReadonly();
  readonly isLoggedIn = computed(() => !!this.accessTokenSignal() && !!this.userSignal());

  constructor() {
    if (!isPlatformBrowser(this.platformId)) return;
    const access = localStorage.getItem(STORAGE_ACCESS);
    const refresh = localStorage.getItem(STORAGE_REFRESH);
    const userJson = localStorage.getItem(STORAGE_USER);
    if (access && refresh && userJson) {
      try {
        const user = JSON.parse(userJson) as UserSummary;
        this.accessTokenSignal.set(access);
        this.refreshTokenSignal.set(refresh);
        this.userSignal.set(user);
      } catch {
        this.clearStorage();
      }
    }
  }

  login(userName: string, password: string): Observable<LoginResponse> {
    return this.http
      .post<LoginResponse>(`${environment.apiUrl}/api/auth/login`, { userName, password })
      .pipe(tap((r) => this.persistSession(r)));
  }

  logout(): void {
    this.clearStorage();
    void this.router.navigateByUrl('/login');
  }

  refreshSession(): Observable<LoginResponse | null> {
    const refresh = this.refreshTokenSignal();
    if (!refresh) return of(null);
    return this.http
      .post<LoginResponse>(`${environment.apiUrl}/api/auth/refresh`, { refreshToken: refresh })
      .pipe(
        tap((r) => this.persistSession(r)),
        catchError(() => {
          this.clearStorage();
          return of(null);
        }),
      );
  }

  /** Used by interceptor: returns new access token or throws. */
  refreshSessionOrThrow(): Observable<string> {
    return this.refreshSession().pipe(
      map((r) => {
        if (!r?.accessToken) throw new Error('Refresh failed');
        return r.accessToken;
      }),
    );
  }

  private persistSession(response: LoginResponse): void {
    this.accessTokenSignal.set(response.accessToken);
    this.refreshTokenSignal.set(response.refreshToken);
    this.userSignal.set(response.user);
    if (!isPlatformBrowser(this.platformId)) return;
    localStorage.setItem(STORAGE_ACCESS, response.accessToken);
    localStorage.setItem(STORAGE_REFRESH, response.refreshToken);
    localStorage.setItem(STORAGE_USER, JSON.stringify(response.user));
  }

  private clearStorage(): void {
    this.accessTokenSignal.set(null);
    this.refreshTokenSignal.set(null);
    this.userSignal.set(null);
    if (!isPlatformBrowser(this.platformId)) return;
    localStorage.removeItem(STORAGE_ACCESS);
    localStorage.removeItem(STORAGE_REFRESH);
    localStorage.removeItem(STORAGE_USER);
  }
}
