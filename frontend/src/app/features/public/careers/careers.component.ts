import { Component } from '@angular/core';
import { RouterLink } from '@angular/router';
import { IconComponent } from '../../../shared/components/icon/icon.component';
import { TranslatePipe } from '../../../core/i18n/t.pipe';

@Component({
  selector: 'app-careers',
  standalone: true,
  imports: [RouterLink, IconComponent, TranslatePipe ],
  templateUrl: './careers.component.html',
  styleUrl: './careers.component.scss'
})
export class CareersComponent {
  departments = [
    { name: 'careers.d1.name', count: 'careers.d1.count', desc: 'careers.d1.desc' },
    { name: 'careers.d2.name', count: 'careers.d2.count', desc: 'careers.d2.desc' },
    { name: 'careers.d3.name', count: 'careers.d3.count', desc: 'careers.d3.desc' },
    { name: 'careers.d4.name', count: 'careers.d4.count', desc: 'careers.d4.desc' },
  ];

  benefits = [
    { title: 'careers.b1.t', desc: 'careers.b1.d' },
    { title: 'careers.b2.t', desc: 'careers.b2.d' },
    { title: 'careers.b3.t', desc: 'careers.b3.d' },
    { title: 'careers.b4.t', desc: 'careers.b4.d' },
  ];
}
