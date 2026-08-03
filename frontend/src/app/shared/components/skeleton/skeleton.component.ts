import { Component, input } from '@angular/core';

@Component({
  selector: 'app-skeleton',
  standalone: true,
  template: `
    <div class="skeleton-loader" [style.height]="height()" [style.width]="width()" [style.border-radius]="radius()"></div>
  `,
  styles: [`
    .skeleton-loader {
      background: linear-gradient(90deg, var(--surface-muted) 25%, var(--border) 50%, var(--surface-muted) 75%);
      background-size: 200% 100%;
      animation: skeleton-shimmer 1.5s infinite;
      display: inline-block;
    }
    @keyframes skeleton-shimmer {
      0% { background-position: 200% 0; }
      100% { background-position: -200% 0; }
    }
  `]
})
export class SkeletonComponent {
  height = input<string>('20px');
  width = input<string>('100%');
  radius = input<string>('8px');
}
