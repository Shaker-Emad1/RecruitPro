import { Component, inject, OnInit, signal } from '@angular/core';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { forkJoin } from 'rxjs';
import { CandidateService } from '../../../core/services/candidate.service';
import { ApplicationService } from '../../../core/services/application.service';
import { Candidate, Application, PaginatedResult } from '../../../core/models/api.models';
import { PageHeaderComponent } from '../../../shared/components/page-header/page-header.component';
import { BadgeComponent } from '../../../shared/components/badge/badge.component';

@Component({
  selector: 'app-candidate-detail',
  imports: [RouterLink, PageHeaderComponent, BadgeComponent],
  templateUrl: './candidate-detail.component.html',
  styleUrl: './candidate-detail.component.scss',
})
export class CandidateDetailComponent implements OnInit {
  private route = inject(ActivatedRoute);
  private candidateService = inject(CandidateService);
  private applicationService = inject(ApplicationService);

  candidate = signal<Candidate | null>(null);
  applications = signal<Application[]>([]);
  loading = signal(true);
  error = signal('');

  ngOnInit(): void {
    const id = this.route.snapshot.paramMap.get('id') ?? '';
    forkJoin([
      this.candidateService.getById(id),
      this.applicationService.getAll(1, 100),
    ]).subscribe({
      next: ([candidateData, appResult]: [Candidate, PaginatedResult<Application>]) => {
        this.candidate.set(candidateData);
        this.applications.set(appResult.items.filter((a) => a.candidateId === id));
        this.loading.set(false);
      },
      error: () => {
        this.error.set('Failed to load candidate details.');
        this.loading.set(false);
      },
    });
  }
}
