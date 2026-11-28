import { Component, inject, OnInit, signal, OnDestroy } from '@angular/core';
import { RouterLink } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { Subject, debounceTime, distinctUntilChanged, takeUntil } from 'rxjs';
import { JobService } from '../../../core/services/job.service';
import { Job, PaginatedResult } from '../../../core/models/api.models';
import { BadgeComponent } from '../../../shared/components/badge/badge.component';
import { DatePipe, CurrencyPipe } from '@angular/common';
import { IconComponent } from '../../../shared/components/icon/icon.component';
import { TranslatePipe } from '../../../core/i18n/t.pipe';

@Component({
  selector: 'app-public-jobs',
  standalone: true,
  imports: [RouterLink, FormsModule, BadgeComponent, DatePipe, CurrencyPipe, IconComponent, TranslatePipe ],
  templateUrl: './public-jobs.component.html',
  styleUrl: './public-jobs.component.scss'
})
export class PublicJobsComponent implements OnInit, OnDestroy {
  private jobService = inject(JobService);
  private destroy$ = new Subject<void>();
  private searchSubject = new Subject<string>();

  jobs = signal<Job[]>([]);
  totalCount = signal(0);
  loading = signal(true);
  currentPage = signal(1);
  pageSize = 9;
  searchQuery = '';
  selectedDepartment = '';

  departments = ['Engineering', 'Product', 'Design', 'Data Science', 'DevOps', 'QA', 'Security'];

  ngOnInit(): void {
    this.searchSubject.pipe(
      debounceTime(300),
      distinctUntilChanged(),
      takeUntil(this.destroy$)
    ).subscribe((query) => {
      this.currentPage.set(1);
      this.loadJobs(query);
    });

    this.loadJobs('');
  }

  onSearch(value: string): void {
    this.searchSubject.next(value);
  }

  onDepartmentChange(dept: string): void {
    this.selectedDepartment = dept;
    this.currentPage.set(1);
    this.loadJobs(this.searchQuery);
  }

  changePage(page: number): void {
    if (page < 1 || page > this.totalPages) return;
    this.currentPage.set(page);
    this.loadJobs(this.searchQuery);
  }

  get totalPages(): number {
    return Math.ceil(this.totalCount() / this.pageSize);
  }

  private loadJobs(search: string): void {
    this.loading.set(true);
    this.jobService.getAll(this.currentPage(), this.pageSize, search, 'Open').pipe(
      takeUntil(this.destroy$)
    ).subscribe({
      next: (res: PaginatedResult<Job>) => {
        let items = res.items || [];
        if (this.selectedDepartment) {
          items = items.filter(j => j.department.toLowerCase() === this.selectedDepartment.toLowerCase());
        }
        this.jobs.set(items);
        this.totalCount.set(res.totalCount || items.length);
        this.loading.set(false);
      },
      error: () => this.loading.set(false)
    });
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }
}
