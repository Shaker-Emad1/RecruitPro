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
    .badge-blue    { background: #eff6ff; color: #1d4ed8; }
    .badge-teal    { background: #f0fdfa; color: #0f766e; }
    .badge-slate   { background: #f1f5f9; color: #334155; }
    .badge-dark    { background: #0f172a; color: #fff; }
    .badge-green   { background: #f0fdf4; color: #166534; }
    .badge-red     { background: #fef2f2; color: #991b1b; }
    .badge-amber   { background: #fffbeb; color: #92400e; }
  `],
})
export class BadgeComponent {
  text = input.required<string>();
  colorClass = input<string>('badge-slate');
}
