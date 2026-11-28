import { Component, inject, OnInit, signal } from '@angular/core';
import { Router, RouterLink } from '@angular/router';
import { DecimalPipe } from '@angular/common';
import { JobService } from '../../../core/services/job.service';
import { Job, PaginatedResult } from '../../../core/models/api.models';
import { IconComponent } from '../../../shared/components/icon/icon.component';

export interface RadarBlip {
  job: Job;
  x: number; // percent 0-100
  y: number;
  confidence: number;
  cyan: boolean;
}

@Component({
  selector: 'app-home',
  standalone: true,
  imports: [RouterLink, DecimalPipe, IconComponent],
  templateUrl: './home.component.html',
  styleUrl: './home.component.scss'
})
export class HomeComponent implements OnInit {
  private jobService = inject(JobService);
  private router = inject(Router);

  featuredJobs = signal<Job[]>([]);
  blips = signal<RadarBlip[]>([]);
  activeBlip = signal<RadarBlip | null>(null);
  openCount = signal(0);
  loading = signal(true);
  tickerItems = signal<string[]>([]);

  ngOnInit(): void {
    this.jobService.getAll(1, 9, '', 'Open').subscribe({
      next: (res: PaginatedResult<Job>) => {
        const jobs = res.items || [];
        this.featuredJobs.set(jobs.slice(0, 6));
        this.openCount.set(res.totalCount || jobs.length);
        this.blips.set(jobs.map((job, i) => this.toBlip(job, i)));
        this.tickerItems.set(jobs.slice(0, 6).map(j =>
          `▲ ${j.title.toUpperCase()} — ${j.location.toUpperCase()} — $${Math.round(j.salaryMin / 1000)}–${Math.round(j.salaryMax / 1000)}K`
        ));
        this.loading.set(false);
      },
      error: () => this.loading.set(false)
    });
  }

  private toBlip(job: Job, i: number): RadarBlip {
    // Golden-angle spread so blips never overlap; radius reflects live demand.
    const angle = i * 137.5 * (Math.PI / 180);
    const demand = Math.min(job.applicationCount || 0, 25) / 25;
    const r = 0.30 + ((i * 37) % 45) / 100 + demand * 0.18;
    return {
      job,
      x: 50 + Math.min(r, 0.93) * 46 * Math.cos(angle),
      y: 50 + Math.min(r, 0.93) * 46 * Math.sin(angle),
      confidence: this.confidence(job),
      cyan: i % 3 === 1
    };
  }

  confidence(job: Job): number {
    return 82 + ((job.applicationCount || 0) * 7) % 17;
  }

  hover(job: Job | null): void {
    if (!job) { this.activeBlip.set(null); return; }
    this.activeBlip.set(this.blips().find(b => b.job.id === job.id) || null);
  }

  open(job: Job): void {
    this.router.navigate(['/jobs', job.id]);
  }
}
