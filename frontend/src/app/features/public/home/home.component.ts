import { Component, inject, OnInit, signal } from '@angular/core';
import { RouterLink } from '@angular/router';
import { DecimalPipe } from '@angular/common';
import { JobService } from '../../../core/services/job.service';
import { Job, PaginatedResult } from '../../../core/models/api.models';
import { BadgeComponent } from '../../../shared/components/badge/badge.component';

@Component({
  selector: 'app-home',
  standalone: true,
  imports: [RouterLink, BadgeComponent, DecimalPipe],
  templateUrl: './home.component.html',
  styleUrl: './home.component.scss'
})
export class HomeComponent implements OnInit {
  private jobService = inject(JobService);

  featuredJobs = signal<Job[]>([]);
  loading = signal(true);

  ngOnInit(): void {
    this.jobService.getAll(1, 6, '', 'Open').subscribe({
      next: (res: PaginatedResult<Job>) => {
        this.featuredJobs.set(res.items || []);
        this.loading.set(false);
      },
      error: () => this.loading.set(false)
    });
  }
}
