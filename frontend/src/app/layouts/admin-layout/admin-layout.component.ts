import { Component, inject, signal } from '@angular/core';
import { RouterOutlet, RouterLink, RouterLinkActive } from '@angular/router';
import { AuthService } from '../../core/services/auth.service';

interface NavItem {
  path: string;
  label: string;
  icon: string;
}

@Component({
  selector: 'app-admin-layout',
  standalone: true,
  imports: [RouterOutlet, RouterLink, RouterLinkActive],
  templateUrl: './admin-layout.component.html',
  styleUrl: './admin-layout.component.scss',
})
export class AdminLayoutComponent {
  auth = inject(AuthService);
  sidebarOpen = signal(false);

  readonly navItems: NavItem[] = [
    { path: '/admin/dashboard',     label: 'Admin Control Center', icon: '⚡' },
    { path: '/admin/users',         label: 'User Directory',       icon: '👥' },
    { path: '/admin/roles',         label: 'Roles & Matrix',       icon: '🔐' },
    { path: '/admin/settings',      label: 'System Config',        icon: '⚙️' },
    { path: '/admin/templates',     label: 'Email Templates',      icon: '✉️' },
    { path: '/admin/audit',         label: 'Audit Logs',           icon: '📜' },
    { path: '/admin/notifications', label: 'Broadcast Center',     icon: '🔔' },
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
