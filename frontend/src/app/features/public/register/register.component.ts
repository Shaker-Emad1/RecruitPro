import { Component, inject, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { AuthService } from '../../../core/services/auth.service';
import { ToastService } from '../../../core/services/toast.service';
import { IconComponent } from '../../../shared/components/icon/icon.component';
import { TranslatePipe } from '../../../core/i18n/t.pipe';

@Component({
  selector: 'app-register',
  standalone: true,
  imports: [ReactiveFormsModule, RouterLink, IconComponent, TranslatePipe ],
  templateUrl: './register.component.html',
  styleUrl: './register.component.scss'
})
export class RegisterComponent {
  private fb = inject(FormBuilder);
  private auth = inject(AuthService);
  private router = inject(Router);
  private toast = inject(ToastService);

  submitting = signal(false);
  error = signal('');

  form = this.fb.group({
    firstName: ['', [Validators.required]],
    lastName: ['', [Validators.required]],
    email: ['', [Validators.required, Validators.email]],
    password: ['', [Validators.required, Validators.minLength(6)]],
    role: ['Candidate']
  });

  submit(): void {
    if (this.form.invalid || this.submitting()) return;
    this.error.set('');
    this.submitting.set(true);

    const val = this.form.getRawValue();
    this.auth.login(val as any).subscribe({
      next: () => {
        this.toast.success('Account created successfully! Welcome to RecruitPro.');
        this.router.navigate(['/jobs']);
      },
      error: (err) => {
        this.error.set(err.error?.message || 'Registration failed. Please check details.');
        this.submitting.set(false);
      }
    });
  }
}
