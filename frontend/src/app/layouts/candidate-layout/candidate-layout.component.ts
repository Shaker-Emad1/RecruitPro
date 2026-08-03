import { Component, inject, signal } from '@angular/core';
import { RouterLink, RouterOutlet, RouterLinkActive } from '@angular/router';
import { AuthService } from '../../core/services/auth.service';

@Component({
  selector: 'app-candidate-layout',
  standalone: true,
  imports: [RouterOutlet, RouterLink, RouterLinkActive],
  templateUrl: './candidate-layout.component.html',
  styleUrl: './candidate-layout.component.scss'
})
export class CandidateLayoutComponent {
  auth = inject(AuthService);
  isDarkMode = signal(false);

  toggleTheme(): void {
    this.isDarkMode.set(!this.isDarkMode());
    document.body.classList.toggle('dark-theme', this.isDarkMode());
  }

  logout(): void {
    this.auth.logout();
  }
}
