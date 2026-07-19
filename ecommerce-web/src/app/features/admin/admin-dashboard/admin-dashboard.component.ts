import { CommonModule } from '@angular/common';
import { Component, OnInit, ViewChild, inject } from '@angular/core';
import {
  ApexAxisChartSeries,
  ApexChart,
  ApexDataLabels,
  ApexFill,
  ApexGrid,
  ApexLegend,
  ApexNonAxisChartSeries,
  ApexPlotOptions,
  ApexResponsive,
  ApexStroke,
  ApexTooltip,
  ApexXAxis,
  ApexYAxis,
  ChartComponent,
  NgApexchartsModule
} from 'ng-apexcharts';
import { TablerIconsModule } from 'angular-tabler-icons';
import { AdminApiService, AnalyticsDashboard } from '../../../core/services/admin-api.service';
import { MaterialModule } from '../../../shared/material.module';

export type BarChartOptions = {
  series: ApexAxisChartSeries;
  chart: ApexChart;
  dataLabels: ApexDataLabels;
  plotOptions: ApexPlotOptions;
  xaxis: ApexXAxis;
  yaxis: ApexYAxis;
  fill: ApexFill;
  tooltip: ApexTooltip;
  stroke: ApexStroke;
  legend: ApexLegend;
  grid: ApexGrid;
};

export type DonutChartOptions = {
  series: ApexNonAxisChartSeries;
  chart: ApexChart;
  labels: string[];
  colors: string[];
  legend: ApexLegend;
  dataLabels: ApexDataLabels;
  plotOptions: ApexPlotOptions;
  stroke: ApexStroke;
  tooltip: ApexTooltip;
  responsive: ApexResponsive[];
};

@Component({
  selector: 'app-admin-dashboard',
  standalone: true,
  imports: [CommonModule, MaterialModule, NgApexchartsModule, TablerIconsModule],
  templateUrl: './admin-dashboard.component.html',
  styleUrl: './admin-dashboard.component.scss'
})
export class AdminDashboardComponent implements OnInit {
  private readonly api = inject(AdminApiService);

  @ViewChild('bestSellingChart') bestSellingChart: ChartComponent = Object.create(null);

  data: AnalyticsDashboard | null = null;
  err = '';
  csvBusy = false;
  xlsxBusy = false;

  bestSellingChartOptions: Partial<BarChartOptions> | any = null;
  customersChartOptions: Partial<BarChartOptions> | any = null;
  giftsChartOptions: Partial<DonutChartOptions> | any = null;

  ngOnInit(): void {
    this.load();
  }

  load(): void {
    this.err = '';
    this.api.getDashboard(10).subscribe({
      next: (d) => {
        this.data = d;
        this.buildCharts(d);
      },
      error: () => (this.err = 'Could not load analytics (check login and API).')
    });
  }

  private buildCharts(d: AnalyticsDashboard): void {
    const products = d.bestSellingProducts ?? [];
    this.bestSellingChartOptions = {
      series: [
        {
          name: 'Qty sold',
          data: products.map((p) => p.quantitySold),
          color: '#5D87FF'
        }
      ],
      chart: {
        type: 'bar',
        height: 340,
        toolbar: { show: false },
        fontFamily: 'inherit',
        foreColor: '#adb0bb'
      },
      plotOptions: {
        bar: { horizontal: false, columnWidth: '40%', borderRadius: 4 }
      },
      dataLabels: { enabled: false },
      legend: { show: false },
      grid: {
        borderColor: 'rgba(0,0,0,0.1)',
        strokeDashArray: 3
      },
      xaxis: {
        categories: products.map((p) => p.productName),
        labels: { style: { cssClass: 'grey--text lighten-2--text fill-color' } }
      },
      yaxis: {
        labels: { style: { cssClass: 'grey--text lighten-2--text fill-color' } }
      },
      stroke: { show: true, width: 3, colors: ['transparent'] },
      tooltip: { theme: 'light' },
      fill: { opacity: 1 }
    };

    const customers = d.topCustomersByRevenue ?? [];
    this.customersChartOptions = {
      series: [
        {
          name: 'Revenue',
          data: customers.map((c) => c.totalPurchaseValue),
          color: '#49BEFF'
        }
      ],
      chart: {
        type: 'bar',
        height: 300,
        toolbar: { show: false },
        fontFamily: 'inherit',
        foreColor: '#adb0bb'
      },
      plotOptions: {
        bar: { horizontal: true, barHeight: '55%', borderRadius: 4 }
      },
      dataLabels: { enabled: false },
      legend: { show: false },
      grid: { borderColor: 'rgba(0,0,0,0.1)', strokeDashArray: 3 },
      xaxis: {
        categories: customers.map((c) => c.customerName)
      },
      yaxis: {},
      stroke: { show: true, width: 2, colors: ['transparent'] },
      tooltip: { theme: 'light', y: { formatter: (v: number) => v.toFixed(2) } },
      fill: { opacity: 1 }
    };

    const gifts = d.mostAssignedGifts ?? [];
    this.giftsChartOptions = {
      series: gifts.map((g) => g.timesAssigned),
      labels: gifts.map((g) => g.giftName),
      chart: {
        type: 'donut',
        height: 250,
        fontFamily: 'inherit',
        foreColor: '#adb0bb'
      },
      colors: ['#5D87FF', '#49BEFF', '#13DEB9', '#FFAE1F', '#FA896B'],
      plotOptions: {
        pie: { donut: { size: '72%' } }
      },
      stroke: { show: false },
      dataLabels: { enabled: false },
      legend: { show: true, position: 'bottom' },
      tooltip: { theme: 'dark', fillSeriesColor: false },
      responsive: [{ breakpoint: 991, options: { chart: { height: 220 } } }]
    };
  }

  exportCsv(): void {
    this.csvBusy = true;
    this.api.downloadAnalyticsCsv(10).subscribe({
      next: (blob) => {
        this.csvBusy = false;
        this.downloadBlob(blob, 'analytics-dashboard.csv');
      },
      error: () => {
        this.csvBusy = false;
        this.err = 'CSV export failed.';
      }
    });
  }

  exportExcel(): void {
    this.xlsxBusy = true;
    this.api.downloadAnalyticsExcel(10).subscribe({
      next: (blob) => {
        this.xlsxBusy = false;
        this.downloadBlob(blob, 'analytics-dashboard.xlsx');
      },
      error: () => {
        this.xlsxBusy = false;
        this.err = 'Excel export failed.';
      }
    });
  }

  private downloadBlob(blob: Blob, filename: string): void {
    const a = document.createElement('a');
    a.href = URL.createObjectURL(blob);
    a.download = filename;
    a.click();
    URL.revokeObjectURL(a.href);
  }
}
