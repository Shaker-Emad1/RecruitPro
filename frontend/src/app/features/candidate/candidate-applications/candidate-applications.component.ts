import { Component, inject, OnInit, signal } from '@angular/core';
import { RouterLink } from '@angular/router';
import { ApplicationService } from '../../../core/services/application.service';
import { Application, PaginatedResult } from '../../../core/models/api.models';
import { PageHeaderComponent } from '../../../shared/components/page-header/page-header.component';
import { BadgeComponent } from '../../../shared/components/badge/badge.component';
import { DatePipe } from '@angular/common';
import { IconComponent } from '../../../shared/components/icon/icon.component';

@Component({
  selector: 'app-candidate-applications',
  standalone: true,
  imports: [RouterLink, PageHeaderComponent, BadgeComponent, DatePipe, IconComponent ],
  templateUrl: './candidate-applications.component.html',
  styleUrl: './candidate-applications.component.scss'
})
export class CandidateApplicationsComponent implements OnInit {
  private appService = inject(ApplicationService);

  applications = signal<Application[]>([]);
  loading = signal(true);

  ngOnInit(): void {
    this.appService.getAll(1, 20).subscribe({
      next: (res: PaginatedResult<Application>) => {
        this.applications.set(res.items || []);
        this.loading.set(false);
      },
      error: () => this.loading.set(false)
    });
  }

  isStepCompleted(currentStatus: string, step: string): boolean {
    const steps = ['Applied', 'Screening', 'Interview', 'Offered', 'Hired'];
    const currIndex = steps.indexOf(currentStatus);
    const stepIndex = steps.indexOf(step);
    return currIndex >= stepIndex;
  }
}
