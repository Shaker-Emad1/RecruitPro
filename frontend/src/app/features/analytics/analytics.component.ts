import { Component, inject, OnInit, signal } from '@angular/core';
import { DecimalPipe } from '@angular/common';
import { PageHeaderComponent } from '../../shared/components/page-header/page-header.component';
import { StatCardComponent } from '../../shared/components/stat-card/stat-card.component';
import { AnalyticsService, ExtendedAnalytics, HiringFunnel } from '../../core/services/analytics.service';

@Component({
  selector: 'app-analytics',
  standalone: true,
  imports: [PageHeaderComponent, StatCardComponent, DecimalPipe],
  templateUrl: './analytics.component.html',
  styleUrl: './analytics.component.scss'
})
export class AnalyticsComponent implements OnInit {
  private service = inject(AnalyticsService);

  analytics = signal<ExtendedAnalytics | null>(null);
  funnel = signal<HiringFunnel | null>(null);
  loading = signal(true);
  error = signal('');

  ngOnInit(): void {
    this.service.getExtended().subscribe({
      next: (data) => {
        this.analytics.set(data);
        this.funnel.set(data.funnel);
        this.loading.set(false);
      },
      error: () => {
        this.error.set('Failed to load analytics data. Ensure you are connected to the API.');
        this.loading.set(false);
      }
    });
  }

  getMonthLabel(month: string): string {
    const [year, m] = month.split('-');
    const names = ['Jan','Feb','Mar','Apr','May','Jun','Jul','Aug','Sep','Oct','Nov','Dec'];
    return `${names[parseInt(m) - 1]} ${year}`;
  }

  getFunnelBarWidth(value: number, total: number): string {
    if (!total) return '0%';
    return `${Math.round((value / total) * 100)}%`;
  }

  getFunnelPercent(value: number, total: number): string {
    if (!total) return '0%';
    return `${Math.round((value / total) * 100)}%`;
  }

  getBarHeight(value: number): string {
    const max = this.analytics()?.monthlyTrends.reduce((acc, t) => Math.max(acc, t.candidates), 1) ?? 1;
    return `${Math.max(4, Math.round((value / max) * 100))}%`;
  }

  getDeptBarWidth(value: number): string {
    const max = this.analytics()?.departmentBreakdown.reduce((acc, d) => Math.max(acc, d.applications), 1) ?? 1;
    return `${Math.max(4, Math.round((value / max) * 100))}%`;
  }

  exportCSV(): void {
    const data = this.analytics();
    if (!data) return;

    const rows = [
      ['Metric', 'Value'],
      ['Total Jobs', data.totalJobs],
      ['Open Jobs', data.openJobs],
      ['Closed Jobs', data.closedJobs],
      ['Total Applications', data.totalApplications],
      ['Total Interviews', data.totalInterviews],
      ['Hired Candidates', data.hiredCandidates],
      ['Rejected Candidates', data.rejectedCandidates],
      ['Offer Acceptance Rate', `${data.offerAcceptanceRate}%`],
      ['Job Fill Rate', `${data.jobFillRate}%`],
      ['Candidate Conversion Rate', `${data.candidateConversionRate}%`],
      ['Average Interview Score', `${data.averageInterviewScore}/5`],
    ];

    const csv = rows.map(r => r.join(',')).join('\n');
    const blob = new Blob([csv], { type: 'text/csv' });
    const url = URL.createObjectURL(blob);
    const a = document.createElement('a');
    a.href = url;
    a.download = `RecruitPro-Analytics-${new Date().toISOString().slice(0,10)}.csv`;
    a.click();
    URL.revokeObjectURL(url);
  }
}
