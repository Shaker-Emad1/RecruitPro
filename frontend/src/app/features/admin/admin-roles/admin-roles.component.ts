import { Component, inject, signal } from '@angular/core';
import { PageHeaderComponent } from '../../../shared/components/page-header/page-header.component';
import { BadgeComponent } from '../../../shared/components/badge/badge.component';
import { ToastService } from '../../../core/services/toast.service';
import { FormsModule } from '@angular/forms';

export interface PermissionGroup {
  module: string;
  create: boolean;
  read: boolean;
  update: boolean;
  delete: boolean;
}

@Component({
  selector: 'app-admin-roles',
  standalone: true,
  imports: [PageHeaderComponent, BadgeComponent, FormsModule],
  templateUrl: './admin-roles.component.html',
  styleUrl: './admin-roles.component.scss'
})
export class AdminRolesComponent {
  private toast = inject(ToastService);

  selectedRole = signal<string>('HRManager');

  permissions = signal<PermissionGroup[]>([
    { module: 'Users', create: false, read: true, update: false, delete: false },
    { module: 'Jobs', create: true, read: true, update: true, delete: true },
    { module: 'Applications', create: true, read: true, update: true, delete: false },
    { module: 'Candidates', create: true, read: true, update: true, delete: false },
    { module: 'Interviews', create: true, read: true, update: true, delete: true },
    { module: 'Notifications', create: true, read: true, update: true, delete: false },
    { module: 'Settings', create: false, read: true, update: false, delete: false },
    { module: 'Audit Logs', create: false, read: false, update: false, delete: false }
  ]);

  savePermissions(): void {
    this.toast.success(`Permissions for role "${this.selectedRole()}" updated successfully.`);
  }
}
