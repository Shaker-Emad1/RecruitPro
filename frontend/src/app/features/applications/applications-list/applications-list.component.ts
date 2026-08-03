import { Component, inject, OnInit, signal } from '@angular/core';
import { ApplicationService } from '../../../core/services/application.service';
import { ToastService } from '../../../core/services/toast.service';
import { Application, PaginatedResult } from '../../../core/models/api.models';
import { PageHeaderComponent } from '../../../shared/components/page-header/page-header.component';
import { BadgeComponent } from '../../../shared/components/badge/badge.component';
import { DatePipe } from '@angular/common';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-applications-list',
  standalone: true,
  imports: [PageHeaderComponent, BadgeComponent, DatePipe, FormsModule],
  templateUrl: './applications-list.component.html',
  styleUrl: './applications-list.component.scss',
})
export class ApplicationsListComponent implements OnInit {
  private service = inject(ApplicationService);
  private toast = inject(ToastService);

  applications = signal<Application[]>([]);
  loading = signal(true);
  error = signal('');
  selectedStatus = '';
  viewMode: 'pipeline' | 'table' = 'pipeline';

  stages = ['Applied', 'Screening', 'Interview', 'Offered', 'Hired'];

  ngOnInit(): void {
    this.loadApplications();
  }

  loadApplications(): void {
    this.loading.set(true);
    this.service.getAll().subscribe({
      next: (data: PaginatedResult<Application>) => {
        this.applications.set(data.items || []);
        this.loading.set(false);
      },
      error: () => {
        this.error.set('Failed to load applications.');
        this.loading.set(false);
      },
    });
  }

  getFilteredApplications(): Application[] {
    if (!this.selectedStatus) return this.applications();
    return this.applications().filter(a => a.status.toLowerCase() === this.selectedStatus.toLowerCase());
  }

  getApplicationsByStage(stage: string): Application[] {
    return this.applications().filter(a => {
      if (stage === 'Interview') {
        return a.status === 'Interview' || a.status === 'Shortlisted';
      }
      return a.status.toLowerCase() === stage.toLowerCase();
    });
  }

  updateStatus(app: Application, newStatus: string): void {
    this.service.updateStatus(app.id, newStatus).subscribe({
      next: () => {
        this.toast.success(`Application updated to "${newStatus}" status.`);
        this.loadApplications();
      },
      error: () => this.toast.error('Failed to update status.')
    });
  }

  statusClass(status: string): string {
    const map: Record<string, string> = {
      Applied: 'badge-blue', Screening: 'badge-slate', Shortlisted: 'badge-teal',
      Interview: 'badge-amber', Offered: 'badge-green', Rejected: 'badge-red', Hired: 'badge-green',
    };
    return map[status] ?? 'badge-slate';
  }
}
