import { CommonModule } from '@angular/common';
import { Component, OnInit, inject } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { AdminApiService, PagedAuditLogs } from '../../../core/services/admin-api.service';
import { MaterialModule } from '../../../shared/material.module';

@Component({
  selector: 'app-admin-audit-logs',
  standalone: true,
  imports: [CommonModule, FormsModule, MaterialModule],
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
  /** YYYY-MM-DD */
  fromLocal = '';
  /** YYYY-MM-DD */
  toLocal = '';
  data: PagedAuditLogs | null = null;
  err = '';

  ngOnInit(): void {
    this.route.queryParamMap.subscribe((params) => {
      this.page = Math.max(1, +(params.get('page') ?? 1));
      this.action = params.get('action') ?? '';
      this.entityType = params.get('entityType') ?? '';
      this.fromLocal = this.isoToDateInput(params.get('fromUtc'));
      this.toLocal = this.isoToDateInput(params.get('toUtc'));
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
    const from = this.dateStartToIso(this.fromLocal);
    const to = this.dateEndToIso(this.toLocal);
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
    const from = this.dateStartToIso(this.fromLocal);
    const to = this.dateEndToIso(this.toLocal);
    if (from) f.fromUtc = from;
    if (to) f.toUtc = to;
    return f;
  }

  private parseDateOnly(value: string): Date | null {
    const m = /^(\d{4})-(\d{2})-(\d{2})$/.exec(value.trim());
    if (!m) return null;
    const y = +m[1];
    const mo = +m[2] - 1;
    const day = +m[3];
    const d = new Date(y, mo, day);
    if (d.getFullYear() !== y || d.getMonth() !== mo || d.getDate() !== day) return null;
    return d;
  }

  private dateStartToIso(value: string): string | undefined {
    const d = this.parseDateOnly(value);
    if (!d) return undefined;
    d.setHours(0, 0, 0, 0);
    return d.toISOString();
  }

  private dateEndToIso(value: string): string | undefined {
    const d = this.parseDateOnly(value);
    if (!d) return undefined;
    d.setHours(23, 59, 59, 999);
    return d.toISOString();
  }

  private isoToDateInput(iso: string | null): string {
    if (!iso) return '';
    const d = new Date(iso);
    if (Number.isNaN(d.getTime())) return '';
    const pad = (n: number) => String(n).padStart(2, '0');
    return `${d.getFullYear()}-${pad(d.getMonth() + 1)}-${pad(d.getDate())}`;
  }
}
