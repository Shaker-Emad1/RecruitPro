import { Component, inject, OnInit, signal } from '@angular/core';
import { RouterLink } from '@angular/router';
import { ApplicationService } from '../../../core/services/application.service';
import { InterviewService } from '../../../core/services/interview.service';
import { Application, Interview, PaginatedResult } from '../../../core/models/api.models';
import { StatCardComponent } from '../../../shared/components/stat-card/stat-card.component';
import { PageHeaderComponent } from '../../../shared/components/page-header/page-header.component';
import { BadgeComponent } from '../../../shared/components/badge/badge.component';
import { DatePipe } from '@angular/common';

@Component({
  selector: 'app-candidate-dashboard',
  standalone: true,
  imports: [RouterLink, StatCardComponent, PageHeaderComponent, BadgeComponent, DatePipe],
  templateUrl: './candidate-dashboard.component.html',
  styleUrl: './candidate-dashboard.component.scss'
})
export class CandidateDashboardComponent implements OnInit {
  private appService = inject(ApplicationService);
  private interviewService = inject(InterviewService);

  applications = signal<Application[]>([]);
  interviews = signal<Interview[]>([]);
  loading = signal(true);

  ngOnInit(): void {
    this.appService.getAll(1, 10).subscribe({
      next: (res: PaginatedResult<Application>) => {
        this.applications.set(res.items || []);
        this.loading.set(false);
      },
      error: () => this.loading.set(false)
    });

    this.interviewService.getAll(1, 5).subscribe({
      next: (res: PaginatedResult<Interview>) => {
        this.interviews.set(res.items || []);
      }
    });
  }
}
