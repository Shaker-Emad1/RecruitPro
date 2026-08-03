import { Component, inject } from '@angular/core';
import { NgClass } from '@angular/common';
import { ToastService } from '../../../core/services/toast.service';

@Component({
  selector: 'app-toast-container',
  imports: [NgClass],
  template: `
    <div class="toast-container">
      @for (toast of toastService.toasts(); track toast.id) {
        <div class="toast-item" [ngClass]="toast.type" (click)="toastService.dismiss(toast.id)">
          <span class="toast-icon">
            @switch (toast.type) {
              @case ('success') { ✓ }
              @case ('error') { ✕ }
              @case ('warning') { ⚠ }
              @default { ℹ }
            }
          </span>
          <span class="toast-msg">{{ toast.message }}</span>
        </div>
      }
    </div>
  `,
  styles: [`
    .toast-container {
      position: fixed;
      bottom: 24px;
      right: 24px;
      z-index: 9999;
      display: flex;
      flex-direction: column;
      gap: 10px;
      max-width: 380px;
    }
    .toast-item {
      display: flex;
      align-items: center;
      gap: 10px;
      padding: 14px 18px;
      border-radius: 14px;
      font-size: 0.875rem;
      font-weight: 500;
      cursor: pointer;
      animation: slideIn 0.25s ease;
      box-shadow: 0 8px 30px rgba(0,0,0,0.12);
    }
    .toast-item.success { background: #f0fdf4; color: #166534; border: 1px solid #bbf7d0; }
    .toast-item.error   { background: #fef2f2; color: #991b1b; border: 1px solid #fecaca; }
    .toast-item.warning { background: #fffbeb; color: #92400e; border: 1px solid #fde68a; }
    .toast-item.info    { background: #eff6ff; color: #1e40af; border: 1px solid #bfdbfe; }
    .toast-icon { font-size: 1rem; flex-shrink: 0; }
    @keyframes slideIn {
      from { opacity: 0; transform: translateY(8px); }
      to   { opacity: 1; transform: translateY(0); }
    }
  `],
})
export class ToastContainerComponent {
  toastService = inject(ToastService);
}
