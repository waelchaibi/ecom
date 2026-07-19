import { CommonModule } from '@angular/common';
import { Component, inject, OnInit } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { AdminApiService, AdminOrderListItem, PagedOrders } from '../../../core/services/admin-api.service';
import { ConfirmDialogService } from '../../../core/services/confirm-dialog.service';
import { MaterialModule } from '../../../shared/material.module';

@Component({
  selector: 'app-admin-orders',
  standalone: true,
  imports: [CommonModule, RouterLink, FormsModule, MaterialModule],
  templateUrl: './admin-orders.component.html',
  styleUrl: './admin-orders.component.scss'
})
export class AdminOrdersComponent implements OnInit {
  private readonly api = inject(AdminApiService);
  private readonly confirmDialog = inject(ConfirmDialogService);
  private readonly route = inject(ActivatedRoute);
  private readonly router = inject(Router);

  page = 1;
  readonly pageSize = 20;
  status = '';
  customerSearch = '';
  customerId: number | null = null;
  fromLocal = '';
  toLocal = '';
  data: PagedOrders | null = null;
  err = '';
  busyId: number | null = null;

  ngOnInit(): void {
    this.route.queryParamMap.subscribe((params) => {
      this.page = Math.max(1, +(params.get('page') ?? 1));
      this.status = params.get('status') ?? '';
      this.customerSearch = params.get('customerSearch') ?? '';
      const cid = params.get('customerId');
      this.customerId = cid && !Number.isNaN(+cid) ? +cid : null;
      this.fromLocal = this.isoToLocalInput(params.get('fromUtc'));
      this.toLocal = this.isoToLocalInput(params.get('toUtc'));
      this.load();
    });
  }

  applyFilters(): void {
    this.router.navigate([], {
      relativeTo: this.route,
      queryParams: this.buildQueryParams(1),
      queryParamsHandling: ''
    });
  }

  clearFilters(): void {
    this.status = '';
    this.customerSearch = '';
    this.customerId = null;
    this.fromLocal = '';
    this.toLocal = '';
    this.router.navigate([], { relativeTo: this.route, queryParams: { page: 1 } });
  }

  load(): void {
    this.err = '';
    this.api.getOrders(this.page, this.pageSize, this.currentFilters()).subscribe({
      next: (d) => (this.data = d),
      error: () => (this.err = 'Failed to load orders.')
    });
  }

  async confirm(o: AdminOrderListItem): Promise<void> {
    const confirmed = await this.confirmDialog.open({
      title: 'Confirm payment',
      message: `Confirm payment for order #${o.id}? Gifts will be applied if eligible.`,
      confirmLabel: 'Confirm payment'
    });
    if (!confirmed) return;
    this.err = '';
    this.busyId = o.id;
    this.api.confirmPayment(o.id).subscribe({
      next: () => {
        this.busyId = null;
        this.load();
      },
      error: (e) => {
        this.busyId = null;
        this.err = e.error?.error ?? e.message ?? 'Confirm failed';
      }
    });
  }

  async cancel(o: AdminOrderListItem): Promise<void> {
    const confirmed = await this.confirmDialog.open({
      title: 'Cancel order',
      message: `Cancel order #${o.id}? Stock will be restored.`,
      confirmLabel: 'Cancel order',
      tone: 'danger'
    });
    if (!confirmed) return;
    this.err = '';
    this.busyId = o.id;
    this.api.cancelOrder(o.id).subscribe({
      next: () => {
        this.busyId = null;
        this.load();
      },
      error: (e) => {
        this.busyId = null;
        this.err = e.error?.error ?? e.message ?? 'Cancel failed';
      }
    });
  }

  prev(): void {
    if (this.page > 1) this.goPage(this.page - 1);
  }

  next(): void {
    if (this.data && this.page * this.pageSize < this.data.totalCount) this.goPage(this.page + 1);
  }

  private goPage(page: number): void {
    this.router.navigate([], {
      relativeTo: this.route,
      queryParams: this.buildQueryParams(page),
      queryParamsHandling: 'merge'
    });
  }

  private buildQueryParams(page: number): Record<string, string | number> {
    const q: Record<string, string | number> = { page };
    if (this.status) q['status'] = this.status;
    if (this.customerSearch.trim()) q['customerSearch'] = this.customerSearch.trim();
    if (this.customerId != null && this.customerId > 0) q['customerId'] = this.customerId;
    const from = this.localInputToIso(this.fromLocal);
    const to = this.localInputToIso(this.toLocal);
    if (from) q['fromUtc'] = from;
    if (to) q['toUtc'] = to;
    return q;
  }

  private currentFilters(): {
    status?: string;
    customerSearch?: string;
    customerId?: number;
    fromUtc?: string;
    toUtc?: string;
  } {
    const f: ReturnType<typeof this.currentFilters> = {};
    if (this.status) f.status = this.status;
    if (this.customerSearch.trim()) f.customerSearch = this.customerSearch.trim();
    if (this.customerId != null && this.customerId > 0) f.customerId = this.customerId;
    const from = this.localInputToIso(this.fromLocal);
    const to = this.localInputToIso(this.toLocal);
    if (from) f.fromUtc = from;
    if (to) f.toUtc = to;
    return f;
  }

  private localInputToIso(value: string): string | undefined {
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
