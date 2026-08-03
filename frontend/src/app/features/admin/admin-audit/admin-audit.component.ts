import { Component, signal } from '@angular/core';
import { PageHeaderComponent } from '../../../shared/components/page-header/page-header.component';
import { BadgeComponent } from '../../../shared/components/badge/badge.component';
import { DatePipe } from '@angular/common';
import { FormsModule } from '@angular/forms';

export interface AuditLogItem {
  id: string;
  userEmail: string;
  action: string;
  module: string;
  ipAddress: string;
  timestamp: string;
  description: string;
}

@Component({
  selector: 'app-admin-audit',
  standalone: true,
  imports: [PageHeaderComponent, BadgeComponent, DatePipe, FormsModule],
  templateUrl: './admin-audit.component.html',
  styleUrl: './admin-audit.component.scss'
})
export class AdminAuditComponent {
  searchQuery = '';
  selectedModule = '';

  logs = signal<AuditLogItem[]>([
    { id: '1', userEmail: 'admin@recruit.com', action: 'User Created', module: 'Users', ipAddress: '192.168.1.10', timestamp: new Date().toISOString(), description: 'Created system user sarah@recruit.com' },
    { id: '2', userEmail: 'sarah@recruit.com', action: 'Job Published', module: 'Jobs', ipAddress: '192.168.1.15', timestamp: new Date(Date.now() - 3600000).toISOString(), description: 'Published job position Senior .NET Developer' },
    { id: '3', userEmail: 'michael@recruit.com', action: 'Interview Scheduled', module: 'Interviews', ipAddress: '192.168.1.22', timestamp: new Date(Date.now() - 7200000).toISOString(), description: 'Scheduled Technical Interview for candidate Alex Morgan' },
    { id: '4', userEmail: 'admin@recruit.com', action: 'Settings Updated', module: 'Settings', ipAddress: '192.168.1.10', timestamp: new Date(Date.now() - 86400000).toISOString(), description: 'Updated max resume upload size limit to 5MB' }
  ]);

  getFilteredLogs(): AuditLogItem[] {
    return this.logs().filter(l => {
      const matchSearch = !this.searchQuery || l.action.toLowerCase().includes(this.searchQuery.toLowerCase()) || l.userEmail.toLowerCase().includes(this.searchQuery.toLowerCase());
      const matchModule = !this.selectedModule || l.module.toLowerCase() === this.selectedModule.toLowerCase();
      return matchSearch && matchModule;
    });
  }
}
