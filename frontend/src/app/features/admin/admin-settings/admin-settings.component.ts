import { Component, inject, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { PageHeaderComponent } from '../../../shared/components/page-header/page-header.component';
import { ToastService } from '../../../core/services/toast.service';
import { IconComponent } from '../../../shared/components/icon/icon.component';

@Component({
  selector: 'app-admin-settings',
  standalone: true,
  imports: [ReactiveFormsModule, PageHeaderComponent, IconComponent ],
  templateUrl: './admin-settings.component.html',
  styleUrl: './admin-settings.component.scss'
})
export class AdminSettingsComponent {
  private fb = inject(FormBuilder);
  private toast = inject(ToastService);

  saving = signal(false);

  form = this.fb.group({
    companyName: ['RecruitPro Enterprise', [Validators.required]],
    supportEmail: ['support@recruitpro.com', [Validators.required, Validators.email]],
    jwtExpiryHours: [24, [Validators.required, Validators.min(1)]],
    smtpHost: ['smtp.mailtrap.io', [Validators.required]],
    smtpPort: [587, [Validators.required]],
    maxResumeSizeMb: [5, [Validators.required, Validators.min(1)]],
    allowedResumeFormats: ['.pdf,.docx,.doc', [Validators.required]],
    enableAiScoring: [true]
  });

  saveSettings(): void {
    if (this.form.invalid || this.saving()) return;
    this.saving.set(true);

    setTimeout(() => {
      this.toast.success('System configuration & settings updated successfully!');
      this.saving.set(false);
    }, 500);
  }
}
