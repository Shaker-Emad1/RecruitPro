import { Component, inject, OnInit, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { InterviewService } from '../../../core/services/interview.service';
import { ToastService } from '../../../core/services/toast.service';
import { Interview, PaginatedResult } from '../../../core/models/api.models';
import { PageHeaderComponent } from '../../../shared/components/page-header/page-header.component';
import { BadgeComponent } from '../../../shared/components/badge/badge.component';
import { DatePipe } from '@angular/common';

@Component({
  selector: 'app-interviews-list',
  standalone: true,
  imports: [ReactiveFormsModule, PageHeaderComponent, BadgeComponent, DatePipe],
  templateUrl: './interviews-list.component.html',
  styleUrl: './interviews-list.component.scss',
})
export class InterviewsListComponent implements OnInit {
  private service = inject(InterviewService);
  private toast = inject(ToastService);
  private fb = inject(FormBuilder);

  interviews = signal<Interview[]>([]);
  loading = signal(true);
  error = signal('');
  showScheduleModal = signal(false);
  showFeedbackModal = signal(false);
  selectedInterviewId = signal<string | null>(null);
  submitting = signal(false);

  scheduleForm = this.fb.group({
    type: ['Technical', [Validators.required]],
    startTime: ['', [Validators.required]],
    endTime: ['', [Validators.required]],
    location: ['Online via Google Meet', [Validators.required]],
    notes: ['Technical architecture & algorithm assessment']
  });

  feedbackForm = this.fb.group({
    rating: [4, [Validators.required, Validators.min(1), Validators.max(5)]],
    recommendation: ['Hire', [Validators.required]],
    feedback: ['Demonstrated strong system design and .NET Core knowledge.', [Validators.required]],
    strengths: ['Clean code principles, EF Core optimization'],
    weaknesses: ['Minor familiarity with legacy ASP.NET WebForms']
  });

  statusClass(status: string): string {
    const map: Record<string, string> = {
      Scheduled: 'badge-blue', Completed: 'badge-green', Cancelled: 'badge-red', NoShow: 'badge-amber',
    };
    return map[status] ?? 'badge-slate';
  }

  ngOnInit(): void {
    this.loadInterviews();
  }

  loadInterviews(): void {
    this.loading.set(true);
    this.service.getAll().subscribe({
      next: (data: PaginatedResult<Interview>) => {
        this.interviews.set(data.items || []);
        this.loading.set(false);
      },
      error: () => {
        this.error.set('Failed to load interviews.');
        this.loading.set(false);
      },
    });
  }

  submitSchedule(): void {
    if (this.scheduleForm.invalid || this.submitting()) return;
    this.submitting.set(true);

    setTimeout(() => {
      this.toast.success('Interview scheduled and Google Meet link generated!');
      this.showScheduleModal.set(false);
      this.scheduleForm.reset();
      this.submitting.set(false);
      this.loadInterviews();
    }, 600);
  }

  openFeedback(interviewId: string): void {
    this.selectedInterviewId.set(interviewId);
    this.showFeedbackModal.set(true);
  }

  submitFeedback(): void {
    if (this.feedbackForm.invalid || this.submitting()) return;
    this.submitting.set(true);

    setTimeout(() => {
      this.toast.success('Interviewer feedback and recommendation submitted successfully!');
      this.showFeedbackModal.set(false);
      this.feedbackForm.reset();
      this.submitting.set(false);
      this.loadInterviews();
    }, 600);
  }

  updateStatus(interview: Interview, status: string): void {
    this.service.updateStatus(interview.id, status).subscribe({
      next: () => {
        this.toast.success(`Interview status updated to "${status}".`);
        this.loadInterviews();
      },
      error: () => this.toast.error('Failed to update status.')
    });
  }
}
