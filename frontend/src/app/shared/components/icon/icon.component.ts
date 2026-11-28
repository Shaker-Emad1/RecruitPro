import { Component, Input } from '@angular/core';

/**
 * Inline SVG icon set (stroke-based, 24x24 viewBox).
 * Replaces emoji/icon-font glyphs so icons render identically everywhere.
 */
@Component({
  selector: 'app-icon',
  standalone: true,
  template: `
    <svg
      [attr.width]="size"
      [attr.height]="size"
      viewBox="0 0 24 24"
      fill="none"
      stroke="currentColor"
      [attr.stroke-width]="stroke"
      stroke-linecap="round"
      stroke-linejoin="round"
      aria-hidden="true"
    >
      @switch (name) {
        @case ('grid') {<rect x="3" y="3" width="7" height="7" rx="1"/><rect x="14" y="3" width="7" height="7" rx="1"/><rect x="3" y="14" width="7" height="7" rx="1"/><rect x="14" y="14" width="7" height="7" rx="1"/>}
        @case ('user') {<circle cx="12" cy="8" r="4"/><path d="M4 21c0-4 3.6-6.5 8-6.5s8 2.5 8 6.5"/>}
        @case ('users') {<circle cx="9" cy="8" r="3.5"/><path d="M2.5 20c0-3.5 3-5.5 6.5-5.5s6.5 2 6.5 5.5"/><path d="M16 4.7a3.5 3.5 0 0 1 0 6.6"/><path d="M17.5 14.9c2.3.7 4 2.3 4 5.1"/>}
        @case ('briefcase') {<rect x="3" y="7" width="18" height="13" rx="2"/><path d="M8 7V5a2 2 0 0 1 2-2h4a2 2 0 0 1 2 2v2"/><path d="M3 13h18"/>}
        @case ('file-text') {<path d="M14 2H6a2 2 0 0 0-2 2v16a2 2 0 0 0 2 2h12a2 2 0 0 0 2-2V8z"/><path d="M14 2v6h6"/><line x1="8" y1="13" x2="16" y2="13"/><line x1="8" y1="17" x2="13" y2="17"/>}
        @case ('calendar') {<rect x="3" y="5" width="18" height="16" rx="2"/><line x1="16" y1="3" x2="16" y2="7"/><line x1="8" y1="3" x2="8" y2="7"/><line x1="3" y1="11" x2="21" y2="11"/>}
        @case ('bar-chart') {<line x1="4" y1="20" x2="20" y2="20"/><line x1="7" y1="20" x2="7" y2="12"/><line x1="12" y1="20" x2="12" y2="6"/><line x1="17" y1="20" x2="17" y2="15"/>}
        @case ('trending-up') {<polyline points="3 17 9 11 13 15 21 7"/><polyline points="15 7 21 7 21 13"/>}
        @case ('zap') {<polygon points="13 2 4 14 11 14 10 22 20 9 13 9 13 2"/>}
        @case ('lock') {<rect x="4" y="11" width="16" height="10" rx="2"/><path d="M8 11V7a4 4 0 0 1 8 0v4"/>}
        @case ('settings') {<circle cx="12" cy="12" r="3"/><path d="M12 2v3M12 19v3M4.9 4.9l2.1 2.1M17 17l2.1 2.1M2 12h3M19 12h3M4.9 19.1 7 17M17 7l2.1-2.1"/>}
        @case ('mail') {<rect x="3" y="5" width="18" height="14" rx="2"/><polyline points="3 7 12 13 21 7"/>}
        @case ('bell') {<path d="M18 8a6 6 0 1 0-12 0c0 7-3 8-3 8h18s-3-1-3-8"/><path d="M10.3 21a2 2 0 0 0 3.4 0"/>}
        @case ('search') {<circle cx="11" cy="11" r="7"/><line x1="21" y1="21" x2="16.5" y2="16.5"/>}
        @case ('map-pin') {<path d="M20 10c0 6-8 12-8 12s-8-6-8-12a8 8 0 0 1 16 0z"/><circle cx="12" cy="10" r="3"/>}
        @case ('clock') {<circle cx="12" cy="12" r="9"/><polyline points="12 7 12 12 15.5 14"/>}
        @case ('dollar') {<line x1="12" y1="2" x2="12" y2="22"/><path d="M17 5.5H9.5a3.25 3.25 0 0 0 0 6.5h5a3.25 3.25 0 0 1 0 6.5H6"/>}
        @case ('arrow-right') {<line x1="4" y1="12" x2="20" y2="12"/><polyline points="13 5 20 12 13 19"/>}
        @case ('arrow-left') {<line x1="20" y1="12" x2="4" y2="12"/><polyline points="11 5 4 12 11 19"/>}
        @case ('logout') {<path d="M9 21H5a2 2 0 0 1-2-2V5a2 2 0 0 1 2-2h4"/><polyline points="16 17 21 12 16 7"/><line x1="21" y1="12" x2="9" y2="12"/>}
        @case ('check') {<polyline points="4 12.5 9.5 18 20 6.5"/>}
        @case ('x') {<line x1="6" y1="6" x2="18" y2="18"/><line x1="18" y1="6" x2="6" y2="18"/>}
        @case ('plus') {<line x1="12" y1="5" x2="12" y2="19"/><line x1="5" y1="12" x2="19" y2="12"/>}
        @case ('filter') {<polygon points="22 4 2 4 10 12.5 10 19 14 21 14 12.5 22 4"/>}
        @case ('star') {<polygon points="12 2.5 14.9 8.6 21.5 9.5 16.7 14.1 17.9 20.7 12 17.5 6.1 20.7 7.3 14.1 2.5 9.5 9.1 8.6 12 2.5"/>}
        @case ('menu') {<line x1="3" y1="6" x2="21" y2="6"/><line x1="3" y1="12" x2="21" y2="12"/><line x1="3" y1="18" x2="21" y2="18"/>}
        @case ('send') {<line x1="22" y1="2" x2="11" y2="13"/><polygon points="22 2 15 22 11 13 2 9 22 2"/>}
        @case ('download') {<polyline points="12 3 12 15"/><polyline points="6 9 12 15 18 9"/><path d="M4 17v2a2 2 0 0 0 2 2h12a2 2 0 0 0 2-2v-2"/>}
        @case ('upload') {<polyline points="12 15 12 3"/><polyline points="6 9 12 3 18 9"/><path d="M4 17v2a2 2 0 0 0 2 2h12a2 2 0 0 0 2-2v-2"/>}
        @case ('eye') {<path d="M2 12s3.5-7 10-7 10 7 10 7-3.5 7-10 7-10-7-10-7z"/><circle cx="12" cy="12" r="3"/>}
        @case ('edit') {<path d="M12 20h9"/><path d="M16.5 3.5a2.1 2.1 0 0 1 3 3L7 19l-4 1 1-4z"/>}
        @case ('trash') {<polyline points="3 6 5 6 21 6"/><path d="M19 6v14a2 2 0 0 1-2 2H7a2 2 0 0 1-2-2V6m3 0V4a2 2 0 0 1 2-2h4a2 2 0 0 1 2 2v2"/>}
        @case ('alert') {<path d="M10.3 3.9 1.8 18a2 2 0 0 0 1.7 3h17a2 2 0 0 0 1.7-3L13.7 3.9a2 2 0 0 0-3.4 0z"/><line x1="12" y1="9" x2="12" y2="13"/><line x1="12" y1="17" x2="12.01" y2="17"/>}
        @case ('info') {<circle cx="12" cy="12" r="9"/><line x1="12" y1="16" x2="12" y2="12"/><line x1="12" y1="8" x2="12.01" y2="8"/>}
        @case ('building') {<rect x="4" y="3" width="16" height="18" rx="1"/><line x1="9" y1="21" x2="9" y2="17"/><line x1="15" y1="21" x2="15" y2="17"/><line x1="8" y1="7" x2="10" y2="7"/><line x1="14" y1="7" x2="16" y2="7"/><line x1="8" y1="11" x2="10" y2="11"/><line x1="14" y1="11" x2="16" y2="11"/>}
        @case ('globe') {<circle cx="12" cy="12" r="9"/><line x1="3" y1="12" x2="21" y2="12"/><path d="M12 3a13.5 13.5 0 0 1 0 18 13.5 13.5 0 0 1 0-18z"/>}
        @case ('chevron-right') {<polyline points="9 5 16 12 9 19"/>}
        @case ('chevron-left') {<polyline points="15 5 8 12 15 19"/>}
        @case ('chevron-down') {<polyline points="5 9 12 16 19 9"/>}
        @case ('external-link') {<path d="M18 13v6a2 2 0 0 1-2 2H5a2 2 0 0 1-2-2V8a2 2 0 0 1 2-2h6"/><polyline points="15 3 21 3 21 9"/><line x1="10" y1="14" x2="21" y2="3"/>}
        @case ('phone') {<path d="M22 16.9v3a2 2 0 0 1-2.2 2 19.8 19.8 0 0 1-8.6-3.1 19.5 19.5 0 0 1-6-6A19.8 19.8 0 0 1 2.1 4.2 2 2 0 0 1 4.1 2h3a2 2 0 0 1 2 1.7c.13.96.36 1.9.7 2.8a2 2 0 0 1-.45 2.1L8.1 9.9a16 16 0 0 0 6 6l1.3-1.25a2 2 0 0 1 2.1-.45c.9.34 1.84.57 2.8.7a2 2 0 0 1 1.7 2z"/>}
        @case ('target') {<circle cx="12" cy="12" r="9"/><circle cx="12" cy="12" r="5"/><circle cx="12" cy="12" r="1.5" fill="currentColor"/>}
        @case ('radar') {<circle cx="12" cy="12" r="9"/><circle cx="12" cy="12" r="5"/><circle cx="12" cy="12" r="1.5" fill="currentColor"/><line x1="12" y1="12" x2="19" y2="5"/>}
        @case ('award') {<circle cx="12" cy="9" r="6"/><polyline points="8.5 14 7 22 12 19 17 22 15.5 14"/>}
        @case ('message') {<path d="M21 15a2 2 0 0 1-2 2H7l-4 4V5a2 2 0 0 1 2-2h14a2 2 0 0 1 2 2z"/>}
        @case ('shield') {<path d="M12 22s8-3.5 8-10V5l-8-3-8 3v7c0 6.5 8 10 8 10z"/>}
        @case ('play') {<polygon points="6 4 20 12 6 20 6 4"/>}
        @case ('activity') {<polyline points="22 12 18 12 15 21 9 3 6 12 2 12"/>}
        @case ('layers') {<polygon points="12 2 2 7 12 12 22 7 12 2"/><polyline points="2 12 12 17 22 12"/><polyline points="2 17 12 22 22 17"/>}
        @case ('key') {<circle cx="7.5" cy="15.5" r="4.5"/><path d="M11 12 21 2"/><path d="M16 7l3 3"/>}
        @case ('sparkle') {<path d="M12 3l1.9 5.6L19.5 10l-5.6 1.9L12 17.5l-1.9-5.6L4.5 10l5.6-1.4z"/>}
        @case ('sun') {<circle cx="12" cy="12" r="4"/><line x1="12" y1="2" x2="12" y2="5"/><line x1="12" y1="19" x2="12" y2="22"/><line x1="2" y1="12" x2="5" y2="12"/><line x1="19" y1="12" x2="22" y2="12"/><line x1="4.5" y1="4.5" x2="6.5" y2="6.5"/><line x1="17.5" y1="17.5" x2="19.5" y2="19.5"/><line x1="4.5" y1="19.5" x2="6.5" y2="17.5"/><line x1="17.5" y1="6.5" x2="19.5" y2="4.5"/>}
        @case ('moon') {<path d="M21 12.8A9 9 0 1 1 11.2 3a7 7 0 0 0 9.8 9.8z"/>}
        @default {<circle cx="12" cy="12" r="9"/>}
      }
    </svg>
  `,
  styles: [`
    :host { display: inline-flex; align-items: center; justify-content: center; line-height: 0; }
  `]
})
export class IconComponent {
  @Input() name = '';
  @Input() size = 18;
  @Input() stroke = 1.8;
}
