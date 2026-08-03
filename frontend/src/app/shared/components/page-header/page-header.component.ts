import { Component, input } from '@angular/core';

@Component({
  selector: 'app-page-header',
  template: `
    <header class="page-header">
      <div class="page-header__meta">
        @if (eyebrow()) {
          <p class="page-header__eyebrow">{{ eyebrow() }}</p>
        }
        <h1 class="page-header__title">{{ title() }}</h1>
        @if (description()) {
          <p class="page-header__desc">{{ description() }}</p>
        }
      </div>
      <div class="page-header__actions">
        <ng-content />
      </div>
    </header>
  `,
  styles: [`
    .page-header {
      display: flex;
      flex-direction: column;
      gap: 16px;
      background: var(--surface);
      border: 1px solid var(--border-strong);
      border-radius: 28px;
      padding: 24px;
      box-shadow: var(--shadow-soft);
      margin-bottom: 24px;
    }
    @media (min-width: 1024px) {
      .page-header {
        flex-direction: row;
        align-items: flex-end;
        justify-content: space-between;
      }
    }
    .page-header__eyebrow {
      font-size: 0.7rem;
      font-weight: 700;
      text-transform: uppercase;
      letter-spacing: 0.24em;
      color: var(--muted);
      margin: 0 0 6px;
    }
    .page-header__title {
      font-size: 1.875rem;
      font-weight: 700;
      color: var(--foreground);
      letter-spacing: -0.025em;
      margin: 0;
    }
    .page-header__desc {
      font-size: 0.875rem;
      line-height: 1.6;
      color: var(--muted);
      margin: 8px 0 0;
      max-width: 600px;
    }
    .page-header__actions {
      display: flex;
      flex-wrap: wrap;
      gap: 10px;
      flex-shrink: 0;
    }
  `],
})
export class PageHeaderComponent {
  eyebrow = input<string>('');
  title = input.required<string>();
  description = input<string>('');
}
