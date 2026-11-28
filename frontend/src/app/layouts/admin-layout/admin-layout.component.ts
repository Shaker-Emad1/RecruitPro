import { Component, inject, signal } from '@angular/core';
import { RouterOutlet, RouterLink, RouterLinkActive } from '@angular/router';
import { AuthService } from '../../core/services/auth.service';
import { IconComponent } from '../../shared/components/icon/icon.component';
import { TranslatePipe } from '../../core/i18n/t.pipe';

interface NavItem {
  path: string;
  label: string;
  icon: string;
}

@Component({
  selector: 'app-admin-layout',
  standalone: true,
  imports: [RouterOutlet, RouterLink, RouterLinkActive, IconComponent, TranslatePipe],
  templateUrl: './admin-layout.component.html',
  styleUrl: './admin-layout.component.scss',
})
export class AdminLayoutComponent {
  auth = inject(AuthService);
  sidebarOpen = signal(false);

  readonly navItems: NavItem[] = [
    { path: '/admin/dashboard',     label: 'side.admin.center', icon: 'zap' },
    { path: '/admin/users',         label: 'side.admin.users',       icon: 'users' },
    { path: '/admin/roles',         label: 'side.admin.roles',       icon: 'lock' },
    { path: '/admin/settings',      label: 'side.admin.settings',        icon: 'settings' },
    { path: '/admin/templates',     label: 'side.admin.templates',      icon: 'mail' },
    { path: '/admin/audit',         label: 'side.admin.audit',           icon: 'file-text' },
    { path: '/admin/notifications', label: 'side.admin.notifs',     icon: 'bell' },
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
