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
  selector: 'app-hr-layout',
  standalone: true,
  imports: [RouterOutlet, RouterLink, RouterLinkActive, IconComponent, TranslatePipe],
  templateUrl: './hr-layout.component.html',
  styleUrl: './hr-layout.component.scss',
})
export class HrLayoutComponent {
  auth = inject(AuthService);
  sidebarOpen = signal(false);

  readonly navItems: NavItem[] = [
    { path: '/hr/dashboard',      label: 'side.overview',      icon: 'grid' },
    { path: '/hr/candidates',     label: 'side.candidates',    icon: 'user' },
    { path: '/hr/jobs',           label: 'side.jobs',  icon: 'briefcase' },
    { path: '/hr/applications',   label: 'side.applications',  icon: 'file-text' },
    { path: '/hr/interviews',     label: 'side.interviews',    icon: 'calendar' },
    { path: '/hr/analytics',      label: 'side.analytics',     icon: 'bar-chart' },
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
