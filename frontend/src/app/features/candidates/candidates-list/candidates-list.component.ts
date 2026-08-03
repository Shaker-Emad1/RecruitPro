import { Component, inject, OnInit, signal, OnDestroy } from '@angular/core';
import { RouterLink } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { Subject, debounceTime, distinctUntilChanged, takeUntil } from 'rxjs';
import { CandidateService } from '../../../core/services/candidate.service';
import { Candidate, PaginatedResult } from '../../../core/models/api.models';
import { PageHeaderComponent } from '../../../shared/components/page-header/page-header.component';
import { BadgeComponent } from '../../../shared/components/badge/badge.component';

@Component({
  selector: 'app-candidates-list',
  imports: [RouterLink, FormsModule, PageHeaderComponent, BadgeComponent],
  templateUrl: './candidates-list.component.html',
  styleUrl: './candidates-list.component.scss',
})
export class CandidatesListComponent implements OnInit, OnDestroy {
  private service = inject(CandidateService);
  private destroy$ = new Subject<void>();
  private searchSubject = new Subject<string>();

  candidates = signal<Candidate[]>([]);
  loading = signal(true);
  error = signal('');
  searchQuery = '';

  ngOnInit(): void {
    this.searchSubject.pipe(
      debounceTime(300),
      distinctUntilChanged(),
      takeUntil(this.destroy$),
    ).subscribe((q) => this.load(q));

    this.load('');
  }

  onSearch(value: string): void {
    this.searchSubject.next(value);
  }

  badgeClass(status: string): string {
    const map: Record<string, string> = {
      New: 'badge-slate', Screening: 'badge-blue', Shortlisted: 'badge-teal',
      Hired: 'badge-green', Rejected: 'badge-red',
    };
    return map[status] ?? 'badge-slate';
  }

  private load(search: string): void {
    this.loading.set(true);
    this.service.getAll(1, 50, search).pipe(takeUntil(this.destroy$)).subscribe({
      next: (data: PaginatedResult<Candidate>) => { this.candidates.set(data.items); this.loading.set(false); },
      error: () => { this.error.set('Failed to load candidates.'); this.loading.set(false); },
    });
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }
}
