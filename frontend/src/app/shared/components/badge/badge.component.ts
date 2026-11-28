import { Component, input } from '@angular/core';
import { NgClass } from '@angular/common';

@Component({
  selector: 'app-badge',
  imports: [NgClass],
  template: `
    <span class="badge" [ngClass]="colorClass()">{{ text() }}</span>
  `,
  styles: [`
    .badge {
      display: inline-flex;
      align-items: center;
      padding: 3px 12px;
      border-radius: 999px;
      font-size: 0.75rem;
      font-weight: 600;
      white-space: nowrap;
    }
    .badge-blue    { background: rgba(11,107,79,.1); color: var(--primary); }
    .badge-teal    { background: rgba(15,118,110,.1); color: var(--success); }
    .badge-slate   { background: var(--surface-muted); color: var(--muted-strong); }
    .badge-dark    { background: var(--ink); color: #f7f5f0; }
    .badge-green   { background: var(--success-bg); color: var(--success); }
    .badge-red     { background: var(--danger-bg); color: var(--danger); }
    .badge-amber   { background: var(--warning-bg); color: var(--warning); }
  `],
})
export class BadgeComponent {
  text = input.required<string>();
  colorClass = input<string>('badge-slate');
}
