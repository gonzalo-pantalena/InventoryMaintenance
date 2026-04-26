import { CommonModule } from '@angular/common';
import { HttpClient } from '@angular/common/http';
import { ChangeDetectionStrategy, Component, computed, inject, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { AuthService } from '../../core/auth/auth.service';
import type { EquipmentDto, LookupBundleDto } from '../../core/models/api.models';
import { environment } from '../../../environments/environment';

@Component({
  selector: 'app-equipment-list',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './equipment-list.component.html',
  styleUrl: './equipment-list.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class EquipmentListComponent {
  private readonly http = inject(HttpClient);
  private readonly fb = inject(FormBuilder);
  private readonly auth = inject(AuthService);
  private readonly router = inject(Router);

  readonly items = signal<EquipmentDto[]>([]);
  readonly lookups = signal<LookupBundleDto | null>(null);
  readonly loading = signal(true);
  readonly saving = signal(false);
  readonly showAdd = signal(false);
  readonly error = signal<string | null>(null);

  readonly isAdmin = computed(() => (this.auth.user()?.staffRoleId ?? 0) >= 3);

  readonly addTypeId = signal(0);

  readonly filteredApplications = computed(() => {
    const bundle = this.lookups();
    const typeId = this.addTypeId();
    if (!bundle || !typeId) return [];
    return bundle.equipmentApplications.filter((a) => a.equipmentTypeId === typeId);
  });

  readonly addForm = this.fb.nonNullable.group({
    departmentId: [0, [Validators.required, Validators.min(1)]],
    equipmentTypeId: [0, [Validators.required, Validators.min(1)]],
    equipmentApplicationId: [0, [Validators.required, Validators.min(1)]],
    name: ['', [Validators.required, Validators.maxLength(200)]],
    brand: ['', [Validators.required, Validators.maxLength(120)]],
    model: ['', [Validators.required, Validators.maxLength(120)]],
    year: [new Date().getFullYear(), [Validators.required, Validators.min(1900), Validators.max(2100)]],
    serialNumber: ['', [Validators.required, Validators.maxLength(120)]],
    ecriCode: ['', [Validators.required, Validators.maxLength(64)]],
    commissionedOn: [new Date().toISOString().slice(0, 10), Validators.required],
    decommissionedOn: this.fb.control<string | null>(null),
  });

  constructor() {
    this.reload();
  }

  reload(): void {
    this.loading.set(true);
    this.error.set(null);
    this.http.get<EquipmentDto[]>(`${environment.apiUrl}/api/equipment`).subscribe({
      next: (rows) => {
        this.items.set(rows);
        this.loading.set(false);
      },
      error: () => {
        this.loading.set(false);
        this.error.set('Could not load equipment.');
      },
    });
  }

  openAdd(): void {
    this.error.set(null);
    this.http.get<LookupBundleDto>(`${environment.apiUrl}/api/lookups/bundle`).subscribe({
      next: (bundle) => {
        this.lookups.set(bundle);
        const dept = bundle.departments[0]?.id ?? 0;
        const type = bundle.equipmentTypes[0]?.id ?? 0;
        this.addTypeId.set(type);
        const app =
          bundle.equipmentApplications.find((a) => a.equipmentTypeId === type)?.id ??
          bundle.equipmentApplications[0]?.id ??
          0;
        this.addForm.patchValue({
          departmentId: dept,
          equipmentTypeId: type,
          equipmentApplicationId: app,
          name: '',
          brand: '',
          model: '',
          year: new Date().getFullYear(),
          serialNumber: '',
          ecriCode: '',
          commissionedOn: new Date().toISOString().slice(0, 10),
          decommissionedOn: null,
        });
        this.onTypeChange(type);
        this.showAdd.set(true);
      },
      error: () => this.error.set('Could not load lookups.'),
    });
  }

  cancelAdd(): void {
    this.showAdd.set(false);
  }

  onTypeChange(typeId: number): void {
    this.addTypeId.set(typeId);
    const apps = this.lookups()?.equipmentApplications.filter((a) => a.equipmentTypeId === typeId) ?? [];
    const first = apps[0]?.id ?? 0;
    this.addForm.patchValue({ equipmentTypeId: typeId, equipmentApplicationId: first });
  }

  saveNew(): void {
    if (this.addForm.invalid) {
      this.addForm.markAllAsTouched();
      return;
    }
    const v = this.addForm.getRawValue();
    this.saving.set(true);
    this.error.set(null);
    const body = {
      id: 0,
      departmentId: Number(v.departmentId),
      equipmentTypeId: Number(v.equipmentTypeId),
      equipmentApplicationId: Number(v.equipmentApplicationId),
      name: v.name.trim(),
      brand: v.brand.trim(),
      model: v.model.trim(),
      year: Number(v.year),
      serialNumber: v.serialNumber.trim(),
      ecriCode: v.ecriCode.trim(),
      commissionedOn: v.commissionedOn,
      decommissionedOn: v.decommissionedOn ? v.decommissionedOn : null,
    };
    this.http.post<EquipmentDto>(`${environment.apiUrl}/api/equipment`, body).subscribe({
      next: () => {
        this.saving.set(false);
        this.showAdd.set(false);
        this.reload();
      },
      error: (e: { status?: number }) => {
        this.saving.set(false);
        if (e.status === 409) this.error.set('Serial number already exists for this tenant.');
        else this.error.set('Could not save equipment.');
      },
    });
  }

  openMaintenance(item: EquipmentDto): void {
    void this.router.navigate(['/equipment', item.id, 'maintenance']);
  }
}
