import { CommonModule } from '@angular/common';
import { Component, inject, OnInit } from '@angular/core';
import { AdminApiService, AnalyticsDashboard } from '../../../core/services/admin-api.service';

@Component({
  selector: 'app-admin-dashboard',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './admin-dashboard.component.html',
  styleUrl: './admin-dashboard.component.scss'
})
export class AdminDashboardComponent implements OnInit {
  private readonly api = inject(AdminApiService);
  data: AnalyticsDashboard | null = null;
  err = '';
  csvBusy = false;

  ngOnInit(): void {
    this.load();
  }

  load(): void {
    this.err = '';
    this.api.getDashboard(10).subscribe({
      next: (d) => (this.data = d),
      error: () => (this.err = 'Could not load analytics (check login and API).')
    });
  }

  exportCsv(): void {
    this.csvBusy = true;
    this.api.downloadAnalyticsCsv(10).subscribe({
      next: (blob) => {
        this.csvBusy = false;
        const a = document.createElement('a');
        a.href = URL.createObjectURL(blob);
        a.download = 'analytics-dashboard.csv';
        a.click();
        URL.revokeObjectURL(a.href);
      },
      error: () => {
        this.csvBusy = false;
        this.err = 'CSV export failed.';
      }
    });
  }
}
