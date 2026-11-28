import { Component, inject, OnInit, signal } from '@angular/core';
import { RouterLink } from '@angular/router';
import { AnalyticsService } from '../../core/services/analytics.service';
import { DashboardAnalytics, MonthlyTrend } from '../../core/models/api.models';
import { StatCardComponent } from '../../shared/components/stat-card/stat-card.component';
import { PageHeaderComponent } from '../../shared/components/page-header/page-header.component';
import { TranslatePipe } from '../../core/i18n/t.pipe';

@Component({
  selector: 'app-dashboard',
  imports: [RouterLink, StatCardComponent, PageHeaderComponent, TranslatePipe ],
  templateUrl: './dashboard.component.html',
  styleUrl: './dashboard.component.scss',
})
export class DashboardComponent implements OnInit {
  private analyticsService = inject(AnalyticsService);

  analytics = signal<DashboardAnalytics | null>(null);
  loading = signal(true);
  error = signal('');

  get trends(): MonthlyTrend[] {
    return (this.analytics()?.monthlyTrends ?? []).slice(-6);
  }

  get maxTrend(): number {
    const vals = this.trends.flatMap((t) => [t.candidates, t.interviews, t.hires]);
    return Math.max(...vals, 1);
  }

  get funnelRows(): { label: string; count: number; total: number }[] {
    const a = this.analytics();
    if (!a) return [];
    return [
      { label: 'dash.stage.qualified',   count: a.qualifiedCandidates,  total: a.totalCandidates },
      { label: 'dash.stage.rejected',    count: a.rejectedCandidates,   total: a.totalCandidates },
      { label: 'dash.stage.interviews',  count: a.interviewsScheduled,  total: a.totalCandidates },
    ];
  }

  barHeight(val: number): string {
    return `${(val / this.maxTrend) * 100}%`;
  }

  funnelPct(count: number, total: number): number {
    return total > 0 ? Math.round((count / total) * 100) : 0;
  }

  ngOnInit(): void {
    this.analyticsService.getDashboard().subscribe({
      next: (data) => {
        this.analytics.set(data);
        this.loading.set(false);
      },
      error: () => {
        this.error.set('Failed to load dashboard analytics.');
        this.loading.set(false);
      },
    });
  }
}
