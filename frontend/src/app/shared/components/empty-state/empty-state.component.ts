import { Component, input, output } from '@angular/core';

@Component({
  selector: 'app-empty-state',
  standalone: true,
  template: `
    <div class="empty-state-card card-ui text-center py-5">
      <div class="empty-icon">{{ icon() }}</div>
      <h3 class="empty-title">{{ title() }}</h3>
      <p class="empty-description">{{ description() }}</p>
      @if (actionLabel()) {
        <button class="btn-ui-primary mt-3" (click)="action.emit()">
          {{ actionLabel() }}
        </button>
      }
    </div>
  `,
  styles: [`
    .empty-state-card {
      display: flex;
      flex-direction: column;
      align-items: center;
      justify-content: center;
    }
    .empty-icon { font-size: 3rem; margin-bottom: 12px; }
    .empty-title { font-size: 1.2rem; font-weight: 800; margin: 0 0 6px; }
    .empty-description { font-size: 0.875rem; color: var(--muted); max-width: 420px; margin: 0 auto; }
  `]
})
export class EmptyStateComponent {
  icon = input<string>('📭');
  title = input.required<string>();
  description = input.required<string>();
  actionLabel = input<string>();
  action = output<void>();
}
