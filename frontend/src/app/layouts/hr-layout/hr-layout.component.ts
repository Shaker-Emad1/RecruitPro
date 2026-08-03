import { Component, inject, signal } from '@angular/core';
import { RouterOutlet, RouterLink, RouterLinkActive } from '@angular/router';
import { AuthService } from '../../core/services/auth.service';

interface NavItem {
  path: string;
  label: string;
  icon: string;
}

@Component({
  selector: 'app-hr-layout',
  standalone: true,
  imports: [RouterOutlet, RouterLink, RouterLinkActive],
  templateUrl: './hr-layout.component.html',
  styleUrl: './hr-layout.component.scss',
})
export class HrLayoutComponent {
  auth = inject(AuthService);
  sidebarOpen = signal(false);

  readonly navItems: NavItem[] = [
    { path: '/hr/dashboard',      label: 'Overview',      icon: '📊' },
    { path: '/hr/candidates',     label: 'Candidates',    icon: '👤' },
    { path: '/hr/jobs',           label: 'Job Postings',  icon: '💼' },
    { path: '/hr/applications',   label: 'Applications',  icon: '📄' },
    { path: '/hr/interviews',     label: 'Interviews',    icon: '📅' },
    { path: '/hr/analytics',      label: 'Analytics',     icon: '📈' },
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
