import { Component, inject, signal } from '@angular/core';
import { RouterOutlet, RouterLink, RouterLinkActive } from '@angular/router';
import { AuthService } from '../../core/services/auth.service';
import { IconComponent } from '../../shared/components/icon/icon.component';
import { TranslatePipe } from '../../core/i18n/t.pipe';

interface NavItem {
  path: string;
  label: string;
  icon: string;
  adminOnly?: boolean;
}

@Component({
  selector: 'app-main-layout',
  standalone: true,
  imports: [RouterOutlet, RouterLink, RouterLinkActive, IconComponent, TranslatePipe],
  templateUrl: './main-layout.component.html',
  styleUrl: './main-layout.component.scss',
})
export class MainLayoutComponent {
  auth = inject(AuthService);
  sidebarOpen = signal(false);

  readonly navItems: NavItem[] = [
    { path: '/dashboard',           label: 'side.overview',           icon: 'grid' },
    { path: '/candidates',          label: 'side.candidates',         icon: 'user' },
    { path: '/portal-jobs',         label: 'side.jobs',               icon: 'briefcase' },
    { path: '/applications',        label: 'side.applications',       icon: 'file-text' },
    { path: '/interviews',          label: 'side.interviews',         icon: 'calendar' },
    { path: '/analytics',           label: 'side.analytics',          icon: 'bar-chart' },
    { path: '/admin/users',         label: 'side.admin.users',    icon: 'users', adminOnly: true },
    { path: '/admin/roles',         label: 'side.admin.roles',     icon: 'lock', adminOnly: true },
    { path: '/admin/settings',      label: 'side.admin.settings',    icon: 'settings', adminOnly: true },
    { path: '/admin/templates',     label: 'side.admin.templates',    icon: 'mail', adminOnly: true },
    { path: '/admin/audit',         label: 'side.admin.audit',         icon: 'file-text', adminOnly: true },
    { path: '/admin/notifications', label: 'side.admin.notifs',   icon: 'bell', adminOnly: true },
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
