import { Component, inject, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { PageHeaderComponent } from '../../../shared/components/page-header/page-header.component';
import { BadgeComponent } from '../../../shared/components/badge/badge.component';
import { ToastService } from '../../../core/services/toast.service';
import { DatePipe } from '@angular/common';
import { IconComponent } from '../../../shared/components/icon/icon.component';

export interface SystemNotificationItem {
  id: string;
  title: string;
  message: string;
  targetRole: string;
  createdAt: string;
}

@Component({
  selector: 'app-admin-notifications',
  standalone: true,
  imports: [ReactiveFormsModule, PageHeaderComponent, BadgeComponent, DatePipe, IconComponent ],
  templateUrl: './admin-notifications.component.html',
  styleUrl: './admin-notifications.component.scss'
})
export class AdminNotificationsComponent {
  private fb = inject(FormBuilder);
  private toast = inject(ToastService);

  notifications = signal<SystemNotificationItem[]>([
    { id: '1', title: 'System Maintenance Notice', message: 'Scheduled maintenance will occur on Saturday at 2:00 AM UTC.', targetRole: 'All Roles', createdAt: new Date().toISOString() },
    { id: '2', title: 'New Candidate Evaluation Guidelines', message: 'HR Managers please review the updated 2026 technical assessment rubrics.', targetRole: 'HRManager', createdAt: new Date(Date.now() - 86400000).toISOString() }
  ]);

  showBroadcastModal = signal(false);
  submitting = signal(false);

  form = this.fb.group({
    title: ['', [Validators.required]],
    targetRole: ['All Roles', [Validators.required]],
    message: ['', [Validators.required]]
  });

  broadcast(): void {
    if (this.form.invalid || this.submitting()) return;
    this.submitting.set(true);

    setTimeout(() => {
      const val = this.form.getRawValue();
      const newNotif: SystemNotificationItem = {
        id: (this.notifications().length + 1).toString(),
        title: val.title!,
        message: val.message!,
        targetRole: val.targetRole!,
        createdAt: new Date().toISOString()
      };

      this.notifications.update(list => [newNotif, ...list]);
      this.toast.success('System broadcast notification sent successfully!');
      this.showBroadcastModal.set(false);
      this.form.reset();
      this.submitting.set(false);
    }, 500);
  }

  deleteNotification(id: string): void {
    this.notifications.update(list => list.filter(n => n.id !== id));
    this.toast.info('Notification removed.');
  }
}
