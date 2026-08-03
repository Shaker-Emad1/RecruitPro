import { Component, inject, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { PageHeaderComponent } from '../../../shared/components/page-header/page-header.component';
import { ToastService } from '../../../core/services/toast.service';
import { AuthService } from '../../../core/services/auth.service';

@Component({
  selector: 'app-candidate-settings',
  standalone: true,
  imports: [ReactiveFormsModule, PageHeaderComponent],
  templateUrl: './candidate-settings.component.html',
  styleUrl: './candidate-settings.component.scss'
})
export class CandidateSettingsComponent {
  private fb = inject(FormBuilder);
  private toast = inject(ToastService);
  private auth = inject(AuthService);

  saving = signal(false);

  passwordForm = this.fb.group({
    currentPassword: ['', [Validators.required]],
    newPassword: ['', [Validators.required, Validators.minLength(6)]],
    confirmPassword: ['', [Validators.required]]
  });

  changePassword(): void {
    if (this.passwordForm.invalid || this.saving()) return;
    const val = this.passwordForm.getRawValue();
    if (val.newPassword !== val.confirmPassword) {
      this.toast.error('New passwords do not match.');
      return;
    }
    this.saving.set(true);
    setTimeout(() => {
      this.toast.success('Password changed successfully.');
      this.passwordForm.reset();
      this.saving.set(false);
    }, 600);
  }

  logout(): void {
    this.auth.logout();
  }
}
