import { Component, inject, OnInit, signal } from '@angular/core';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { JobService } from '../../../core/services/job.service';
import { Job } from '../../../core/models/api.models';
import { PageHeaderComponent } from '../../../shared/components/page-header/page-header.component';
import { BadgeComponent } from '../../../shared/components/badge/badge.component';

@Component({
  selector: 'app-job-detail',
  imports: [RouterLink, PageHeaderComponent, BadgeComponent],
  templateUrl: './job-detail.component.html',
  styleUrl: './job-detail.component.scss',
})
export class JobDetailComponent implements OnInit {
  private route = inject(ActivatedRoute);
  private service = inject(JobService);

  job = signal<Job | null>(null);
  loading = signal(true);
  error = signal('');

  ngOnInit(): void {
    const id = this.route.snapshot.paramMap.get('id') ?? '';
    this.service.getById(id).subscribe({
      next: (data: Job) => { this.job.set(data); this.loading.set(false); },
      error: () => { this.error.set('Failed to load job details.'); this.loading.set(false); },
    });
  }
}
