import { Routes } from '@angular/router';
import { authGuard } from './core/guards/auth.guard';
import { guestGuard } from './core/guards/guest.guard';
import { roleGuard } from './core/guards/role.guard';

export const routes: Routes = [
  // ── Public Website Routes (PublicLayoutComponent) ─────────────────────────
  {
    path: '',
    loadComponent: () =>
      import('./layouts/public-layout/public-layout.component').then((m) => m.PublicLayoutComponent),
    children: [
      {
        path: '',
        loadComponent: () =>
          import('./features/public/home/home.component').then((m) => m.HomeComponent),
      },
      {
        path: 'about',
        loadComponent: () =>
          import('./features/public/about/about.component').then((m) => m.AboutComponent),
      },
      {
        path: 'careers',
        loadComponent: () =>
          import('./features/public/careers/careers.component').then((m) => m.CareersComponent),
      },
      {
        path: 'jobs',
        loadComponent: () =>
          import('./features/public/public-jobs/public-jobs.component').then((m) => m.PublicJobsComponent),
      },
      {
        path: 'jobs/:id',
        loadComponent: () =>
          import('./features/public/public-job-detail/public-job-detail.component').then((m) => m.PublicJobDetailComponent),
      },
      {
        path: 'contact',
        loadComponent: () =>
          import('./features/public/contact/contact.component').then((m) => m.ContactComponent),
      },
      {
        path: 'login',
        canActivate: [guestGuard],
        loadComponent: () =>
          import('./features/auth/login/login.component').then((m) => m.LoginComponent),
      },
      {
        path: 'register',
        canActivate: [guestGuard],
        loadComponent: () =>
          import('./features/public/register/register.component').then((m) => m.RegisterComponent),
      },
      {
        path: '403',
        loadComponent: () =>
          import('./features/error/forbidden/forbidden.component').then((m) => m.ForbiddenComponent),
      },
    ],
  },

  // ── Candidate Portal Routes (CandidateLayoutComponent) ─────────────────────
  {
    path: 'candidate',
    canActivate: [authGuard, roleGuard(['Candidate', 'Admin'])],
    loadComponent: () =>
      import('./layouts/candidate-layout/candidate-layout.component').then((m) => m.CandidateLayoutComponent),
    children: [
      {
        path: '',
        redirectTo: 'dashboard',
        pathMatch: 'full',
      },
      {
        path: 'dashboard',
        loadComponent: () =>
          import('./features/candidate/candidate-dashboard/candidate-dashboard.component').then((m) => m.CandidateDashboardComponent),
      },
      {
        path: 'profile',
        loadComponent: () =>
          import('./features/candidate/candidate-profile/candidate-profile.component').then((m) => m.CandidateProfileComponent),
      },
      {
        path: 'applications',
        loadComponent: () =>
          import('./features/candidate/candidate-applications/candidate-applications.component').then((m) => m.CandidateApplicationsComponent),
      },
      {
        path: 'interviews',
        loadComponent: () =>
          import('./features/candidate/candidate-interviews/candidate-interviews.component').then((m) => m.CandidateInterviewsComponent),
      },
      {
        path: 'settings',
        loadComponent: () =>
          import('./features/candidate/candidate-settings/candidate-settings.component').then((m) => m.CandidateSettingsComponent),
      },
    ],
  },

  // ── HR Workspace Routes (HrLayoutComponent) ───────────────────────────────
  {
    path: 'hr',
    canActivate: [authGuard, roleGuard(['HRManager', 'Recruiter', 'Admin'])],
    loadComponent: () =>
      import('./layouts/hr-layout/hr-layout.component').then((m) => m.HrLayoutComponent),
    children: [
      {
        path: '',
        redirectTo: 'dashboard',
        pathMatch: 'full',
      },
      {
        path: 'dashboard',
        loadComponent: () =>
          import('./features/dashboard/dashboard.component').then((m) => m.DashboardComponent),
      },
      {
        path: 'candidates',
        loadComponent: () =>
          import('./features/candidates/candidates-list/candidates-list.component').then((m) => m.CandidatesListComponent),
      },
      {
        path: 'candidates/:id',
        loadComponent: () =>
          import('./features/candidates/candidate-detail/candidate-detail.component').then((m) => m.CandidateDetailComponent),
      },
      {
        path: 'jobs',
        loadComponent: () =>
          import('./features/jobs/jobs-list/jobs-list.component').then((m) => m.JobsListComponent),
      },
      {
        path: 'jobs/:id',
        loadComponent: () =>
          import('./features/jobs/job-detail/job-detail.component').then((m) => m.JobDetailComponent),
      },
      {
        path: 'applications',
        loadComponent: () =>
          import('./features/applications/applications-list/applications-list.component').then((m) => m.ApplicationsListComponent),
      },
      {
        path: 'interviews',
        loadComponent: () =>
          import('./features/interviews/interviews-list/interviews-list.component').then((m) => m.InterviewsListComponent),
      },
      {
        path: 'analytics',
        loadComponent: () =>
          import('./features/analytics/analytics.component').then((m) => m.AnalyticsComponent),
      },
    ],
  },

  // ── Backward-compatible redirects for legacy HR paths ──────────────────────
  { path: 'dashboard', redirectTo: 'hr/dashboard', pathMatch: 'full' },
  { path: 'candidates', redirectTo: 'hr/candidates', pathMatch: 'full' },
  { path: 'portal-jobs', redirectTo: 'hr/jobs', pathMatch: 'full' },
  { path: 'applications', redirectTo: 'hr/applications', pathMatch: 'full' },
  { path: 'interviews', redirectTo: 'hr/interviews', pathMatch: 'full' },
  { path: 'analytics', redirectTo: 'hr/analytics', pathMatch: 'full' },

  // ── Admin Workspace Routes (AdminLayoutComponent) ──────────────────────────
  {
    path: 'admin',
    canActivate: [authGuard, roleGuard(['Admin'])],
    loadComponent: () =>
      import('./layouts/admin-layout/admin-layout.component').then((m) => m.AdminLayoutComponent),
    children: [
      {
        path: '',
        redirectTo: 'dashboard',
        pathMatch: 'full',
      },
      {
        path: 'dashboard',
        loadComponent: () =>
          import('./features/dashboard/dashboard.component').then((m) => m.DashboardComponent),
      },
      {
        path: 'users',
        loadComponent: () =>
          import('./features/admin/admin-users/admin-users.component').then((m) => m.AdminUsersComponent),
      },
      {
        path: 'roles',
        loadComponent: () =>
          import('./features/admin/admin-roles/admin-roles.component').then((m) => m.AdminRolesComponent),
      },
      {
        path: 'settings',
        loadComponent: () =>
          import('./features/admin/admin-settings/admin-settings.component').then((m) => m.AdminSettingsComponent),
      },
      {
        path: 'templates',
        loadComponent: () =>
          import('./features/admin/admin-templates/admin-templates.component').then((m) => m.AdminTemplatesComponent),
      },
      {
        path: 'audit',
        loadComponent: () =>
          import('./features/admin/admin-audit/admin-audit.component').then((m) => m.AdminAuditComponent),
      },
      {
        path: 'notifications',
        loadComponent: () =>
          import('./features/admin/admin-notifications/admin-notifications.component').then((m) => m.AdminNotificationsComponent),
      },
    ],
  },

  // ── Wildcard 404 Route ────────────────────────────────────────────────────
  {
    path: '**',
    loadComponent: () =>
      import('./features/error/not-found/not-found.component').then((m) => m.NotFoundComponent),
  },
];
