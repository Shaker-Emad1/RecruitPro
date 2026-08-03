import { Component } from '@angular/core';
import { RouterLink } from '@angular/router';

@Component({
  selector: 'app-careers',
  standalone: true,
  imports: [RouterLink],
  templateUrl: './careers.component.html',
  styleUrl: './careers.component.scss'
})
export class CareersComponent {
  departments = [
    { name: 'Engineering', count: '12 open positions', desc: 'Build scalable .NET 10 APIs, Angular 20 interfaces, and distributed systems.' },
    { name: 'Product & Design', count: '5 open positions', desc: 'Craft high-converting candidate portals and intuitive HR workflows.' },
    { name: 'Data Science & AI', count: '3 open positions', desc: 'Train predictive scoring models and matching engines.' },
    { name: 'DevOps & Security', count: '4 open positions', desc: 'Maintain zero-trust infrastructure, Docker orchestration, and CI/CD pipelines.' },
  ];

  benefits = [
    { title: 'Remote & Hybrid Flexibility', desc: 'Work from where you perform best with flexible core hours.' },
    { title: 'Continuous Growth Budget', desc: '$3,000 annual stipend for conferences, courses, and technical certifications.' },
    { title: 'Comprehensive Health & Wellness', desc: 'Top-tier medical, dental, vision, and mental health coverage.' },
    { title: 'Competitive Compensation', desc: 'Industry-leading base salary with performance bonuses and equity option grants.' }
  ];
}
