import { Component, inject, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { PageHeaderComponent } from '../../../shared/components/page-header/page-header.component';
import { BadgeComponent } from '../../../shared/components/badge/badge.component';
import { ToastService } from '../../../core/services/toast.service';

export interface EmailTemplateItem {
  id: string;
  name: string;
  subject: string;
  body: string;
}

@Component({
  selector: 'app-admin-templates',
  standalone: true,
  imports: [ReactiveFormsModule, PageHeaderComponent],
  templateUrl: './admin-templates.component.html',
  styleUrl: './admin-templates.component.scss'
})
export class AdminTemplatesComponent {
  private fb = inject(FormBuilder);
  private toast = inject(ToastService);

  templates = signal<EmailTemplateItem[]>([
    {
      id: '1',
      name: 'Application Received',
      subject: 'Thank you for applying to RecruitPro',
      body: 'Dear {{CandidateName}},\n\nThank you for submitting your application for {{JobTitle}}. Our hiring team is reviewing your profile and will be in touch soon.\n\nBest regards,\nRecruitPro Talent Team'
    },
    {
      id: '2',
      name: 'Interview Invitation',
      subject: 'Interview Invitation: {{JobTitle}}',
      body: 'Dear {{CandidateName}},\n\nWe are pleased to invite you to a {{InterviewType}} interview. Please join using the Google Meet link below:\n{{MeetingLink}}\n\nBest regards,\nRecruitPro Talent Team'
    },
    {
      id: '3',
      name: 'Offer Letter',
      subject: 'Formal Job Offer: {{JobTitle}} at RecruitPro',
      body: 'Dear {{CandidateName}},\n\nWe are thrilled to extend a formal offer of employment for the {{JobTitle}} position.\n\nBest regards,\nRecruitPro HR Team'
    },
    {
      id: '4',
      name: 'Rejection Letter',
      subject: 'Update regarding your application for {{JobTitle}}',
      body: 'Dear {{CandidateName}},\n\nThank you for taking the time to interview with us. After careful consideration, we have decided to move forward with another candidate whose experience closely aligns with our current needs.\n\nBest regards,\nRecruitPro Team'
    }
  ]);

  selectedTemplate = signal<EmailTemplateItem>(this.templates()[0]);
  saving = signal(false);

  form = this.fb.group({
    subject: ['', [Validators.required]],
    body: ['', [Validators.required]]
  });

  selectTemplate(tpl: EmailTemplateItem): void {
    this.selectedTemplate.set(tpl);
    this.form.patchValue({
      subject: tpl.subject,
      body: tpl.body
    });
  }

  saveTemplate(): void {
    if (this.form.invalid || this.saving()) return;
    this.saving.set(true);

    setTimeout(() => {
      const val = this.form.getRawValue();
      const curr = this.selectedTemplate();
      curr.subject = val.subject!;
      curr.body = val.body!;

      this.toast.success(`Email template "${curr.name}" saved successfully.`);
      this.saving.set(false);
    }, 500);
  }
}
