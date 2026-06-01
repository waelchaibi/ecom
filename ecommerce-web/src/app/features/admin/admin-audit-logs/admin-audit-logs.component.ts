import { CommonModule } from '@angular/common';
import { Component, OnInit, inject } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { AdminApiService, PagedAuditLogs } from '../../../core/services/admin-api.service';

@Component({
  selector: 'app-admin-audit-logs',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './admin-audit-logs.component.html',
  styleUrl: './admin-audit-logs.component.scss'
})
export class AdminAuditLogsComponent implements OnInit {
  private readonly api = inject(AdminApiService);
  private readonly route = inject(ActivatedRoute);
  private readonly router = inject(Router);

  page = 1;
  readonly pageSize = 50;
  action = '';
  entityType = '';
  fromLocal = '';
  toLocal = '';
  data: PagedAuditLogs | null = null;
  err = '';

  ngOnInit(): void {
    this.route.queryParamMap.subscribe((params) => {
      this.page = Math.max(1, +(params.get('page') ?? 1));
      this.action = params.get('action') ?? '';
      this.entityType = params.get('entityType') ?? '';
      this.fromLocal = this.isoToLocalInput(params.get('fromUtc'));
      this.toLocal = this.isoToLocalInput(params.get('toUtc'));
      this.load();
    });
  }

  applyFilters(): void {
    this.router.navigate([], {
      relativeTo: this.route,
      queryParams: this.buildQuery(1),
      queryParamsHandling: ''
    });
  }

  clearFilters(): void {
    this.action = '';
    this.entityType = '';
    this.fromLocal = '';
    this.toLocal = '';
    this.router.navigate([], { relativeTo: this.route, queryParams: { page: 1 } });
  }

  load(): void {
    this.err = '';
    this.api.getAuditLogs(this.page, this.pageSize, this.filters()).subscribe({
      next: (d) => (this.data = d),
      error: () => (this.err = 'Failed to load audit logs.')
    });
  }

  prev(): void {
    if (this.page > 1) this.go(this.page - 1);
  }

  next(): void {
    if (this.data && this.page * this.pageSize < this.data.totalCount) this.go(this.page + 1);
  }

  private go(page: number): void {
    this.router.navigate([], {
      relativeTo: this.route,
      queryParams: this.buildQuery(page),
      queryParamsHandling: 'merge'
    });
  }

  private buildQuery(page: number): Record<string, string | number> {
    const q: Record<string, string | number> = { page };
    if (this.action.trim()) q['action'] = this.action.trim();
    if (this.entityType.trim()) q['entityType'] = this.entityType.trim();
    const from = this.localToIso(this.fromLocal);
    const to = this.localToIso(this.toLocal);
    if (from) q['fromUtc'] = from;
    if (to) q['toUtc'] = to;
    return q;
  }

  private filters(): {
    action?: string;
    entityType?: string;
    fromUtc?: string;
    toUtc?: string;
  } {
    const f: ReturnType<typeof this.filters> = {};
    if (this.action.trim()) f.action = this.action.trim();
    if (this.entityType.trim()) f.entityType = this.entityType.trim();
    const from = this.localToIso(this.fromLocal);
    const to = this.localToIso(this.toLocal);
    if (from) f.fromUtc = from;
    if (to) f.toUtc = to;
    return f;
  }

  private localToIso(value: string): string | undefined {
    if (!value?.trim()) return undefined;
    const d = new Date(value);
    return Number.isNaN(d.getTime()) ? undefined : d.toISOString();
  }

  private isoToLocalInput(iso: string | null): string {
    if (!iso) return '';
    const d = new Date(iso);
    if (Number.isNaN(d.getTime())) return '';
    const pad = (n: number) => String(n).padStart(2, '0');
    return `${d.getFullYear()}-${pad(d.getMonth() + 1)}-${pad(d.getDate())}T${pad(d.getHours())}:${pad(d.getMinutes())}`;
  }
}
