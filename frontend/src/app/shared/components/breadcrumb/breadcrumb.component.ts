import { Component, input } from '@angular/core';
import { RouterLink } from '@angular/router';

export interface BreadcrumbItem {
  label: string;
  url?: string;
}

@Component({
  selector: 'app-breadcrumb',
  standalone: true,
  imports: [RouterLink],
  template: `
    <nav class="breadcrumb-nav" aria-label="breadcrumb">
      <ol class="breadcrumb-list">
        @for (item of items(); track item.label; let last = $last) {
          <li class="breadcrumb-item" [class.active]="last">
            @if (item.url && !last) {
              <a [routerLink]="item.url" class="breadcrumb-link">{{ item.label }}</a>
              <span class="breadcrumb-separator">/</span>
            } @else {
              <span class="breadcrumb-current">{{ item.label }}</span>
            }
          </li>
        }
      </ol>
    </nav>
  `,
  styles: [`
    .breadcrumb-nav { margin-bottom: 12px; }
    .breadcrumb-list {
      display: flex;
      align-items: center;
      gap: 6px;
      padding: 0;
      margin: 0;
      list-style: none;
      font-size: 0.8125rem;
    }
    .breadcrumb-item {
      display: flex;
      align-items: center;
      gap: 6px;
      color: var(--muted);
    }
    .breadcrumb-link {
      color: var(--primary);
      text-decoration: none;
      font-weight: 600;
      &:hover { text-decoration: underline; }
    }
    .breadcrumb-current {
      color: var(--foreground);
      font-weight: 700;
    }
    .breadcrumb-separator {
      color: var(--border-strong);
      font-size: 0.75rem;
    }
  `]
})
export class BreadcrumbComponent {
  items = input.required<BreadcrumbItem[]>();
}
