import { Routes } from '@angular/router';
import { authGuard } from './core/auth/auth.guard';

export const routes: Routes = [
  { path: 'login', loadComponent: () => import('./features/auth/login.component').then((m) => m.LoginComponent) },
  {
    path: '',
    canActivate: [authGuard],
    loadComponent: () => import('./layout/app-layout.component').then((m) => m.AppLayoutComponent),
    children: [
      { path: '', pathMatch: 'full', redirectTo: 'equipment' },
      {
        path: 'equipment',
        loadComponent: () =>
          import('./features/equipment/equipment-list.component').then((m) => m.EquipmentListComponent),
      },
      {
        path: 'equipment/:id/maintenance',
        loadComponent: () =>
          import('./features/maintenance/maintenance-page.component').then((m) => m.MaintenancePageComponent),
      },
    ],
  },
  { path: '**', redirectTo: '' },
];
