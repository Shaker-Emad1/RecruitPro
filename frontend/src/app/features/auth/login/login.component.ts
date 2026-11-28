import { Component, inject, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { AuthService } from '../../../core/services/auth.service';
import { TranslatePipe } from '../../../core/i18n/t.pipe';

@Component({
  selector: 'app-login',
  imports: [ReactiveFormsModule, TranslatePipe ],
  templateUrl: './login.component.html',
  styleUrl: './login.component.scss',
})
export class LoginComponent {
  private fb = inject(FormBuilder);
  private auth = inject(AuthService);
  private router = inject(Router);

  submitting = signal(false);
  error = signal('');

  form = this.fb.group({
    email:    ['admin@recruit.com', [Validators.required, Validators.email]],
    password: ['Admin123!',         [Validators.required, Validators.minLength(6)]],
  });

  fillAccount(email: string, pass: string): void {
    this.form.patchValue({ email, password: pass });
    this.error.set('');
  }

  submit(): void {
    if (this.form.invalid || this.submitting()) return;
    this.error.set('');
    this.submitting.set(true);

    this.auth.login(this.form.getRawValue() as { email: string; password: string }).subscribe({
      next: () => this.router.navigate(['/dashboard']),
      error: (err) => {
        this.error.set(err.error?.message ?? 'Unable to sign in. Check your credentials and try again.');
        this.submitting.set(false);
      },
    });
  }
}
