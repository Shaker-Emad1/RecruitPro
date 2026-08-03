import { Component, inject, signal } from '@angular/core';
import { RouterOutlet, RouterLink, RouterLinkActive } from '@angular/router';
import { AuthService } from '../../core/services/auth.service';

interface NavItem {
  path: string;
  label: string;
  icon: string;
  adminOnly?: boolean;
}

@Component({
  selector: 'app-main-layout',
  standalone: true,
  imports: [RouterOutlet, RouterLink, RouterLinkActive],
  templateUrl: './main-layout.component.html',
  styleUrl: './main-layout.component.scss',
})
export class MainLayoutComponent {
  auth = inject(AuthService);
  sidebarOpen = signal(false);

  readonly navItems: NavItem[] = [
    { path: '/dashboard',           label: 'Overview',           icon: '📊' },
    { path: '/candidates',          label: 'Candidates',         icon: '👤' },
    { path: '/portal-jobs',         label: 'Jobs',               icon: '💼' },
    { path: '/applications',        label: 'Applications',       icon: '📄' },
    { path: '/interviews',          label: 'Interviews',         icon: '📅' },
    { path: '/analytics',           label: 'Analytics',          icon: '📈' },
    { path: '/admin/users',         label: 'User Management',    icon: '👥', adminOnly: true },
    { path: '/admin/roles',         label: 'Roles & Access',     icon: '🔐', adminOnly: true },
    { path: '/admin/settings',      label: 'System Settings',    icon: '⚙️', adminOnly: true },
    { path: '/admin/templates',     label: 'Email Templates',    icon: '✉️', adminOnly: true },
    { path: '/admin/audit',         label: 'Audit Logs',         icon: '📜', adminOnly: true },
    { path: '/admin/notifications', label: 'Broadcast Center',   icon: '🔔', adminOnly: true },
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
