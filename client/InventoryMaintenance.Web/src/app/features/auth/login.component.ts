import { CommonModule } from '@angular/common';
import { ChangeDetectionStrategy, Component, inject, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { AuthService } from '../../core/auth/auth.service';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './login.component.html',
  styleUrl: './login.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class LoginComponent {
  private readonly fb = inject(FormBuilder);
  private readonly auth = inject(AuthService);
  private readonly router = inject(Router);

  readonly busy = signal(false);
  readonly error = signal<string | null>(null);

  readonly form = this.fb.nonNullable.group({
    userName: ['', [Validators.required, Validators.maxLength(128)]],
    password: ['', [Validators.required, Validators.maxLength(256)]],
  });

  constructor() {
    if (this.auth.isLoggedIn()) {
      void this.router.navigateByUrl('/equipment');
    }
  }

  submit(): void {
    this.error.set(null);
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }
    const { userName, password } = this.form.getRawValue();
    this.busy.set(true);
    this.auth.login(userName.trim(), password).subscribe({
      next: () => void this.router.navigateByUrl('/equipment'),
      error: () => {
        this.busy.set(false);
        this.error.set('Invalid credentials.');
      },
      complete: () => this.busy.set(false),
    });
  }
}
