import { Component, inject, OnInit, signal } from '@angular/core';
import { RouterLink } from '@angular/router';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { JobService } from '../../../core/services/job.service';
import { ToastService } from '../../../core/services/toast.service';
import { Job, PaginatedResult } from '../../../core/models/api.models';
import { PageHeaderComponent } from '../../../shared/components/page-header/page-header.component';
import { BadgeComponent } from '../../../shared/components/badge/badge.component';
import { CurrencyPipe, DatePipe } from '@angular/common';
import { IconComponent } from '../../../shared/components/icon/icon.component';

@Component({
  selector: 'app-jobs-list',
  standalone: true,
  imports: [RouterLink, ReactiveFormsModule, PageHeaderComponent, BadgeComponent, CurrencyPipe, IconComponent ],
  templateUrl: './jobs-list.component.html',
  styleUrl: './jobs-list.component.scss',
})
export class JobsListComponent implements OnInit {
  private service = inject(JobService);
  private toast = inject(ToastService);
  private fb = inject(FormBuilder);

  jobs = signal<Job[]>([]);
  loading = signal(true);
  error = signal('');
  showCreateModal = signal(false);
  submitting = signal(false);

  form = this.fb.group({
    title: ['', [Validators.required]],
    department: ['Engineering', [Validators.required]],
    location: ['San Francisco, CA', [Validators.required]],
    salaryMin: [110000, [Validators.required, Validators.min(0)]],
    salaryMax: [165000, [Validators.required, Validators.min(0)]],
    minimumExperience: [3, [Validators.required, Validators.min(0)]],
    requiredSkills: ['C#, ASP.NET Core, Angular, SQL', [Validators.required]],
    description: ['We are seeking an experienced engineer to join our high-growth team.', [Validators.required]],
    requirements: ['3+ years software engineering experience with C# and Web APIs.', [Validators.required]]
  });

  statusClass(status: string): string {
    const map: Record<string, string> = {
      Open: 'badge-green', Closed: 'badge-red', Draft: 'badge-slate', Paused: 'badge-amber',
    };
    return map[status] ?? 'badge-slate';
  }

  ngOnInit(): void {
    this.loadJobs();
  }

  loadJobs(): void {
    this.loading.set(true);
    this.service.getAll().subscribe({
      next: (data: PaginatedResult<Job>) => {
        this.jobs.set(data.items || []);
        this.loading.set(false);
      },
      error: () => {
        this.error.set('Failed to load jobs.');
        this.loading.set(false);
      },
    });
  }

  createJob(): void {
    if (this.form.invalid || this.submitting()) return;
    this.submitting.set(true);

    setTimeout(() => {
      this.toast.success('Job posting created successfully!');
      this.showCreateModal.set(false);
      this.form.reset();
      this.submitting.set(false);
      this.loadJobs();
    }, 600);
  }

  deleteJob(id: string): void {
    if (confirm('Are you sure you want to delete this job position?')) {
      this.toast.info('Job position removed.');
      this.jobs.update(list => list.filter(j => j.id !== id));
    }
  }
}
