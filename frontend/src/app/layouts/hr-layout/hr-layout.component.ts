import { Component, inject, signal } from '@angular/core';
import { RouterOutlet, RouterLink, RouterLinkActive } from '@angular/router';
import { AuthService } from '../../core/services/auth.service';
import { IconComponent } from '../../shared/components/icon/icon.component';

interface NavItem {
  path: string;
  label: string;
  icon: string;
}

@Component({
  selector: 'app-hr-layout',
  standalone: true,
  imports: [RouterOutlet, RouterLink, RouterLinkActive, IconComponent],
  templateUrl: './hr-layout.component.html',
  styleUrl: './hr-layout.component.scss',
})
export class HrLayoutComponent {
  auth = inject(AuthService);
  sidebarOpen = signal(false);

  readonly navItems: NavItem[] = [
    { path: '/hr/dashboard',      label: 'Overview',      icon: 'grid' },
    { path: '/hr/candidates',     label: 'Candidates',    icon: 'user' },
    { path: '/hr/jobs',           label: 'Job Postings',  icon: 'briefcase' },
    { path: '/hr/applications',   label: 'Applications',  icon: 'file-text' },
    { path: '/hr/interviews',     label: 'Interviews',    icon: 'calendar' },
    { path: '/hr/analytics',      label: 'Analytics',     icon: 'bar-chart' },
  ];

  toggleSidebar(): void {
    this.sidebarOpen.update((v) => !v);
  }

  closeSidebar(): void {
    this.sidebarOpen.set(false);
  }

  logout(): void {
    this.auth.logout();
  }
}
