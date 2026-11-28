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
  selector: 'app-admin-layout',
  standalone: true,
  imports: [RouterOutlet, RouterLink, RouterLinkActive, IconComponent],
  templateUrl: './admin-layout.component.html',
  styleUrl: './admin-layout.component.scss',
})
export class AdminLayoutComponent {
  auth = inject(AuthService);
  sidebarOpen = signal(false);

  readonly navItems: NavItem[] = [
    { path: '/admin/dashboard',     label: 'Admin Control Center', icon: 'zap' },
    { path: '/admin/users',         label: 'User Directory',       icon: 'users' },
    { path: '/admin/roles',         label: 'Roles & Matrix',       icon: 'lock' },
    { path: '/admin/settings',      label: 'System Config',        icon: 'settings' },
    { path: '/admin/templates',     label: 'Email Templates',      icon: 'mail' },
    { path: '/admin/audit',         label: 'Audit Logs',           icon: 'file-text' },
    { path: '/admin/notifications', label: 'Broadcast Center',     icon: 'bell' },
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
