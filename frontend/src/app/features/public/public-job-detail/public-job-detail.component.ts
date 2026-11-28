import { Component, inject, OnInit, signal } from '@angular/core';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { CurrencyPipe, DatePipe, DecimalPipe } from '@angular/common';
import { JobService } from '../../../core/services/job.service';
import { Job } from '../../../core/models/api.models';
import { IconComponent } from '../../../shared/components/icon/icon.component';
import { TranslatePipe } from '../../../core/i18n/t.pipe';

@Component({
  selector: 'app-public-job-detail',
  standalone: true,
  imports: [RouterLink, CurrencyPipe, DatePipe, DecimalPipe, IconComponent, TranslatePipe ],
  templateUrl: './public-job-detail.component.html',
  styleUrl: './public-job-detail.component.scss'
})
export class PublicJobDetailComponent implements OnInit {
  private route = inject(ActivatedRoute);
  private jobService = inject(JobService);

  job = signal<Job | null>(null);
  loading = signal(true);
  error = signal('');

  ngOnInit(): void {
    const id = this.route.snapshot.paramMap.get('id') || '';
    this.jobService.getById(id).subscribe({
      next: (data: Job) => {
        this.job.set(data);
        this.loading.set(false);
      },
      error: () => {
        this.error.set('Job position not found or unavailable.');
        this.loading.set(false);
      }
    });
  }
}
