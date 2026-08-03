import { Component, input } from '@angular/core';

@Component({
  selector: 'app-stat-card',
  template: `
    <article class="stat-card">
      <p class="stat-label">{{ label() }}</p>
      <p class="stat-value">{{ value() }}</p>
    </article>
  `,
  styles: [`
    .stat-card {
      background: var(--surface);
      border: 1px solid var(--border);
      border-radius: 28px;
      padding: 20px 24px;
      box-shadow: var(--shadow-soft);
    }
    .stat-label {
      font-size: 0.8125rem;
      color: var(--muted);
      margin: 0;
    }
    .stat-value {
      font-size: 2rem;
      font-weight: 700;
      color: var(--foreground);
      margin: 12px 0 0;
      letter-spacing: -0.025em;
    }
  `],
})
export class StatCardComponent {
  label = input.required<string>();
  value = input.required<string | number>();
}
