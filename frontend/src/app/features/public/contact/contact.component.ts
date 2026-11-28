import { Component, inject, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { ToastService } from '../../../core/services/toast.service';
import { IconComponent } from '../../../shared/components/icon/icon.component';

@Component({
  selector: 'app-contact',
  standalone: true,
  imports: [ReactiveFormsModule, IconComponent ],
  templateUrl: './contact.component.html',
  styleUrl: './contact.component.scss'
})
export class ContactComponent {
  private fb = inject(FormBuilder);
  private toast = inject(ToastService);

  submitting = signal(false);

  form = this.fb.group({
    fullName: ['', [Validators.required]],
    email: ['', [Validators.required, Validators.email]],
    subject: ['', [Validators.required]],
    message: ['', [Validators.required, Validators.minLength(10)]]
  });

  submitForm(): void {
    if (this.form.invalid || this.submitting()) return;
    this.submitting.set(true);

    setTimeout(() => {
      this.toast.success('Thank you! Your message has been sent to our recruitment team.');
      this.form.reset();
      this.submitting.set(false);
    }, 600);
  }
}
