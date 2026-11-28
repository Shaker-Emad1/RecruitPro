import { Component, inject } from '@angular/core';
import { RouterLink, RouterOutlet, RouterLinkActive } from '@angular/router';
import { AuthService } from '../../core/services/auth.service';
import { IconComponent } from '../../shared/components/icon/icon.component';

@Component({
  selector: 'app-candidate-layout',
  standalone: true,
  imports: [RouterOutlet, RouterLink, RouterLinkActive, IconComponent],
  templateUrl: './candidate-layout.component.html',
  styleUrl: './candidate-layout.component.scss'
})
export class CandidateLayoutComponent {
  auth = inject(AuthService);

  logout(): void {
    this.auth.logout();
  }
}
