import { Component, inject, signal } from '@angular/core';
import { RouterOutlet, RouterLink, RouterLinkActive } from '@angular/router';
import { AuthService } from '../../core/services/auth.service';
import { IconComponent } from '../../shared/components/icon/icon.component';

interface NavItem {
  path: string;
  label: string;
  icon: string;
  adminOnly?: boolean;
}

@Component({
  selector: 'app-main-layout',
  standalone: true,
  imports: [RouterOutlet, RouterLink, RouterLinkActive, IconComponent],
  templateUrl: './main-layout.component.html',
  styleUrl: './main-layout.component.scss',
})
export class MainLayoutComponent {
  auth = inject(AuthService);
  sidebarOpen = signal(false);

  readonly navItems: NavItem[] = [
    { path: '/dashboard',           label: 'Overview',           icon: 'grid' },
    { path: '/candidates',          label: 'Candidates',         icon: 'user' },
    { path: '/portal-jobs',         label: 'Jobs',               icon: 'briefcase' },
    { path: '/applications',        label: 'Applications',       icon: 'file-text' },
    { path: '/interviews',          label: 'Interviews',         icon: 'calendar' },
    { path: '/analytics',           label: 'Analytics',          icon: 'bar-chart' },
    { path: '/admin/users',         label: 'User Management',    icon: 'users', adminOnly: true },
    { path: '/admin/roles',         label: 'Roles & Access',     icon: 'lock', adminOnly: true },
    { path: '/admin/settings',      label: 'System Settings',    icon: 'settings', adminOnly: true },
    { path: '/admin/templates',     label: 'Email Templates',    icon: 'mail', adminOnly: true },
    { path: '/admin/audit',         label: 'Audit Logs',         icon: 'file-text', adminOnly: true },
    { path: '/admin/notifications', label: 'Broadcast Center',   icon: 'bell', adminOnly: true },
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
