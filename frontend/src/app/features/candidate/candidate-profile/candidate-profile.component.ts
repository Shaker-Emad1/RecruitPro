import { Component, inject, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { PageHeaderComponent } from '../../../shared/components/page-header/page-header.component';
import { ToastService } from '../../../core/services/toast.service';
import { IconComponent } from '../../../shared/components/icon/icon.component';

@Component({
  selector: 'app-candidate-profile',
  standalone: true,
  imports: [ReactiveFormsModule, PageHeaderComponent, IconComponent ],
  templateUrl: './candidate-profile.component.html',
  styleUrl: './candidate-profile.component.scss'
})
export class CandidateProfileComponent {
  private fb = inject(FormBuilder);
  private toast = inject(ToastService);

  saving = signal(false);
  uploading = signal(false);

  resumeFileName = signal<string | null>('My_Software_Engineer_Resume.pdf');
  resumeUploadError = signal('');

  form = this.fb.group({
    fullName: ['Alex Morgan', [Validators.required]],
    phone: ['+1-555-019-2831', [Validators.required]],
    location: ['San Francisco, CA', [Validators.required]],
    yearsOfExperience: [6, [Validators.required, Validators.min(0)]],
    education: ['B.S. Computer Science'],
    certifications: ['AWS Certified Developer, Scrum Master'],
    linkedInUrl: ['https://linkedin.com/in/alexmorgan'],
    gitHubUrl: ['https://github.com/alexmorgan']
  });

  saveProfile(): void {
    if (this.form.invalid || this.saving()) return;
    this.saving.set(true);

    setTimeout(() => {
      this.toast.success('Candidate profile updated successfully!');
      this.saving.set(false);
    }, 500);
  }

  onFileSelected(event: Event): void {
    const input = event.target as HTMLInputElement;
    if (!input.files || input.files.length === 0) return;

    const file = input.files[0];
    this.resumeUploadError.set('');

    if (file.size > 5 * 1024 * 1024) {
      this.resumeUploadError.set('File size exceeds maximum 5MB limit.');
      return;
    }

    const ext = file.name.split('.').pop()?.toLowerCase();
    if (ext !== 'pdf' && ext !== 'docx' && ext !== 'doc') {
      this.resumeUploadError.set('Only PDF and DOCX file formats are allowed.');
      return;
    }

    this.uploading.set(true);
    setTimeout(() => {
      this.resumeFileName.set(file.name);
      this.toast.success(`Resume "${file.name}" uploaded successfully!`);
      this.uploading.set(false);
    }, 700);
  }

  deleteResume(): void {
    this.resumeFileName.set(null);
    this.toast.info('Resume file removed.');
  }
}
