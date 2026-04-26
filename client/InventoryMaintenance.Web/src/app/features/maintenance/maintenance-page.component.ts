import { CommonModule } from '@angular/common';
import { HttpClient, HttpParams } from '@angular/common/http';
import { ChangeDetectionStrategy, Component, computed, inject, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { AuthService } from '../../core/auth/auth.service';
import type { EquipmentDto, LookupBundleDto, MaintenanceEventDto } from '../../core/models/api.models';
import { environment } from '../../../environments/environment';

@Component({
  selector: 'app-maintenance-page',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterLink],
  templateUrl: './maintenance-page.component.html',
  styleUrl: './maintenance-page.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class MaintenancePageComponent {
  private readonly http = inject(HttpClient);
  private readonly fb = inject(FormBuilder);
  private readonly route = inject(ActivatedRoute);
  private readonly router = inject(Router);
  private readonly auth = inject(AuthService);

  readonly equipmentId = signal(0);
  readonly equipment = signal<EquipmentDto | null>(null);
  readonly events = signal<MaintenanceEventDto[]>([]);
  readonly lookups = signal<LookupBundleDto | null>(null);
  readonly loading = signal(true);
  readonly saving = signal(false);
  readonly error = signal<string | null>(null);

  readonly isTechnician = computed(() => this.auth.user()?.staffRoleId === 2);

  readonly openRecord = computed(() => this.events().filter((r) => !r.completedAt).at(-1) ?? null);

  readonly createForm = this.fb.nonNullable.group({
    maintenanceTypeId: [0, [Validators.required, Validators.min(1)]],
    description: ['', [Validators.required, Validators.maxLength(4000)]],
    scheduledFor: [''],
  });

  readonly updateForm = this.fb.nonNullable.group({
    description: ['', [Validators.required, Validators.maxLength(4000)]],
    markCompleted: [false],
  });

  constructor() {
    const id = Number(this.route.snapshot.paramMap.get('id'));
    if (!Number.isFinite(id) || id <= 0) {
      void this.router.navigateByUrl('/equipment');
      return;
    }
    this.equipmentId.set(id);
    this.bootstrap();
  }

  private bootstrap(): void {
    this.loading.set(true);
    this.error.set(null);
    const eid = this.equipmentId();
    this.http.get<EquipmentDto[]>(`${environment.apiUrl}/api/equipment`).subscribe({
      next: (list) => {
        const found = list.find((x) => x.id === eid) ?? null;
        this.equipment.set(found);
        if (!found) {
          this.loading.set(false);
          this.error.set('Equipment not found.');
          return;
        }
        this.loadEvents();
      },
      error: () => {
        this.loading.set(false);
        this.error.set('Could not load equipment.');
      },
    });
  }

  private loadLookupsForForm(): void {
    this.http.get<LookupBundleDto>(`${environment.apiUrl}/api/lookups/bundle`).subscribe({
      next: (b) => {
        this.lookups.set(b);
        const mt = b.maintenanceTypes[0]?.id ?? 0;
        this.createForm.patchValue({ maintenanceTypeId: mt, description: '', scheduledFor: '' });
      },
      error: () => this.error.set('Could not load maintenance types.'),
    });
  }

  private loadEvents(): void {
    const eid = this.equipmentId();
    const params = new HttpParams().set('equipmentId', String(eid));
    this.http.get<MaintenanceEventDto[]>(`${environment.apiUrl}/api/maintenance`, { params }).subscribe({
      next: (rows) => {
        this.events.set(rows);
        this.loading.set(false);
        this.loadLookupsForForm();
        this.updateForm.reset({ description: '', markCompleted: false });
      },
      error: () => {
        this.loading.set(false);
        this.error.set('Could not load maintenance history.');
      },
    });
  }

  create(): void {
    if (this.createForm.invalid) {
      this.createForm.markAllAsTouched();
      return;
    }
    const v = this.createForm.getRawValue();
    const scheduled = v.scheduledFor?.trim();
    const body = {
      equipmentId: this.equipmentId(),
      maintenanceTypeId: Number(v.maintenanceTypeId),
      description: v.description.trim(),
      scheduledFor: scheduled ? new Date(scheduled).toISOString() : null,
    };
    this.saving.set(true);
    this.error.set(null);
    this.http.post<MaintenanceEventDto[]>(`${environment.apiUrl}/api/maintenance`, body).subscribe({
      next: (rows) => {
        this.events.set(rows);
        this.saving.set(false);
        this.createForm.patchValue({ description: '', scheduledFor: '' });
        this.updateForm.reset({ description: '', markCompleted: false });
      },
      error: () => {
        this.saving.set(false);
        this.error.set('Could not create maintenance request.');
      },
    });
  }

  update(): void {
    const open = this.openRecord();
    if (!open) {
      this.error.set('There is no open maintenance record to update.');
      return;
    }
    const v = this.updateForm.getRawValue();
    if (this.updateForm.controls.description.invalid) {
      this.updateForm.markAllAsTouched();
      return;
    }
    this.saving.set(true);
    this.error.set(null);
    const body = {
      id: open.id,
      description: v.description.trim(),
      markCompleted: !!v.markCompleted,
    };
    this.http.patch<MaintenanceEventDto[]>(`${environment.apiUrl}/api/maintenance`, body).subscribe({
      next: (rows) => {
        this.events.set(rows);
        this.saving.set(false);
        this.updateForm.reset({ description: '', markCompleted: false });
      },
      error: (e: { status?: number }) => {
        this.saving.set(false);
        if (e.status === 403) this.error.set('Only technicians may mark work as completed.');
        else this.error.set('Could not update maintenance.');
      },
    });
  }
}
