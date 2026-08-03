import { Component } from '@angular/core';
import { RouterLink } from '@angular/router';

@Component({
  selector: 'app-not-found',
  standalone: true,
  imports: [RouterLink],
  template: `
    <div class="error-container">
      <div class="card-ui error-card text-center">
        <div class="error-code">404</div>
        <h1 class="error-title">Page Not Found</h1>
        <p class="error-message">The requested page or route does not exist or may have been moved.</p>
        <div class="d-flex justify-content-center gap-3 mt-4">
          <a routerLink="/" class="btn-ui-primary">Back to Homepage</a>
        </div>
      </div>
    </div>
  `,
  styles: [`
    .error-container {
      min-height: 80vh;
      display: flex;
      align-items: center;
      justify-content: center;
      padding: 24px;
    }
    .error-card {
      max-width: 500px;
      padding: 48px 32px;
    }
    .error-code {
      font-size: 5rem;
      font-weight: 900;
      color: var(--primary);
      line-height: 1;
      margin-bottom: 12px;
    }
    .error-title { font-size: 1.5rem; font-weight: 800; margin-bottom: 8px; }
    .error-message { font-size: 0.9rem; color: var(--muted); margin: 0; }
  `]
})
export class NotFoundComponent {}
