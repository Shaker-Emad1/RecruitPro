import { Component, inject, OnInit, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { PageHeaderComponent } from '../../../shared/components/page-header/page-header.component';
import { BadgeComponent } from '../../../shared/components/badge/badge.component';
import { ToastService } from '../../../core/services/toast.service';
import { DatePipe } from '@angular/common';

export interface AdminUser {
  id: string;
  firstName: string;
  lastName: string;
  email: string;
  role: 'Admin' | 'HRManager' | 'Recruiter' | 'Candidate';
  isActive: boolean;
  createdAt: string;
}

@Component({
  selector: 'app-admin-users',
  standalone: true,
  imports: [ReactiveFormsModule, PageHeaderComponent, BadgeComponent, DatePipe],
  templateUrl: './admin-users.component.html',
  styleUrl: './admin-users.component.scss'
})
export class AdminUsersComponent implements OnInit {
  private fb = inject(FormBuilder);
  private toast = inject(ToastService);

  users = signal<AdminUser[]>([
    { id: '1', firstName: 'Admin', lastName: 'User', email: 'admin@recruit.com', role: 'Admin', isActive: true, createdAt: '2026-01-10' },
    { id: '2', firstName: 'Sarah', lastName: 'Manager', email: 'sarah@recruit.com', role: 'HRManager', isActive: true, createdAt: '2026-01-15' },
    { id: '3', firstName: 'Michael', lastName: 'Recruiter', email: 'michael@recruit.com', role: 'Recruiter', isActive: true, createdAt: '2026-01-20' },
    { id: '4', firstName: 'Alex', lastName: 'Candidate', email: 'alex@candidate.com', role: 'Candidate', isActive: true, createdAt: '2026-02-01' }
  ]);

  showCreateModal = signal(false);
  submitting = signal(false);

  userForm = this.fb.group({
    firstName: ['', [Validators.required]],
    lastName: ['', [Validators.required]],
    email: ['', [Validators.required, Validators.email]],
    role: ['Recruiter', [Validators.required]],
    password: ['DefaultPassword123!', [Validators.required]]
  });

  ngOnInit(): void {}

  createUser(): void {
    if (this.userForm.invalid || this.submitting()) return;
    this.submitting.set(true);

    setTimeout(() => {
      const val = this.userForm.getRawValue();
      const newUser: AdminUser = {
        id: (this.users().length + 1).toString(),
        firstName: val.firstName!,
        lastName: val.lastName!,
        email: val.email!,
        role: val.role as any,
        isActive: true,
        createdAt: new Date().toISOString()
      };

      this.users.update(list => [newUser, ...list]);
      this.toast.success(`User ${newUser.email} created successfully.`);
      this.showCreateModal.set(false);
      this.userForm.reset();
      this.submitting.set(false);
    }, 500);
  }

  toggleActive(user: AdminUser): void {
    user.isActive = !user.isActive;
    this.toast.info(`User ${user.email} is now ${user.isActive ? 'Active' : 'Deactivated'}.`);
  }

  resetPassword(user: AdminUser): void {
    this.toast.success(`Password reset email sent to ${user.email}.`);
  }

  deleteUser(id: string): void {
    if (confirm('Are you sure you want to delete this system user?')) {
      this.users.update(list => list.filter(u => u.id !== id));
      this.toast.info('User account deleted.');
    }
  }
}
