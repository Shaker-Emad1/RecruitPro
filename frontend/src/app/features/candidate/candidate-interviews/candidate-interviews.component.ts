import { Component, inject, OnInit, signal } from '@angular/core';
import { InterviewService } from '../../../core/services/interview.service';
import { Interview, PaginatedResult } from '../../../core/models/api.models';
import { PageHeaderComponent } from '../../../shared/components/page-header/page-header.component';
import { BadgeComponent } from '../../../shared/components/badge/badge.component';
import { DatePipe } from '@angular/common';

@Component({
  selector: 'app-candidate-interviews',
  standalone: true,
  imports: [PageHeaderComponent, BadgeComponent, DatePipe],
  templateUrl: './candidate-interviews.component.html',
  styleUrl: './candidate-interviews.component.scss'
})
export class CandidateInterviewsComponent implements OnInit {
  private interviewService = inject(InterviewService);

  interviews = signal<Interview[]>([]);
  loading = signal(true);

  ngOnInit(): void {
    this.interviewService.getAll(1, 20).subscribe({
      next: (res: PaginatedResult<Interview>) => {
        this.interviews.set(res.items || []);
        this.loading.set(false);
      },
      error: () => this.loading.set(false)
    });
  }
}
