import { CommonModule } from '@angular/common';
import { Component, EventEmitter, Input, Output, inject } from '@angular/core';
import { Router } from '@angular/router';
import { TablerIconsModule } from 'angular-tabler-icons';
import { NavItem } from './nav-item';

@Component({
  selector: 'ecom-admin-nav-item',
  standalone: true,
  imports: [CommonModule, TablerIconsModule],
  template: `
    @if (item.navCap) {
      <div class="nav-caption">{{ item.navCap }}</div>
    }
    @if (!item.navCap && !item.external && !item.twoLines) {
      <button
        type="button"
        class="menu-list-item"
        [class.activeMenu]="isActive()"
        (click)="onItemSelected()"
      >
        <i-tabler class="routeIcon" name="{{ item.iconName }}"></i-tabler>
        <span class="hide-menu">{{ item.displayName }}</span>
      </button>
    }
  `,
  styles: [
    `
      :host {
        display: block;
      }

      .nav-caption {
        margin: 24px 0 12px;
        text-transform: uppercase;
        font-size: 0.75rem;
        font-weight: 700;
        color: #7c8fac;
        padding: 0 10px;
      }

      .menu-list-item {
        display: flex;
        align-items: center;
        gap: 14px;
        width: 100%;
        border: 0;
        border-radius: 7px;
        height: 45px;
        padding: 8px 10px;
        margin-bottom: 2px;
        background: transparent;
        color: #2a3547;
        cursor: pointer;
        text-align: left;
        font: inherit;
      }

      .menu-list-item:hover {
        background: #ecf2ff;
        color: #5d87ff;
      }

      .menu-list-item.activeMenu {
        background: #5d87ff;
        color: #fff;
      }

      .routeIcon {
        width: 20px;
        height: 20px;
        flex-shrink: 0;
        display: inline-flex;
      }

      .hide-menu {
        font-size: 0.875rem;
        font-weight: 500;
      }
    `
  ]
})
export class AdminNavItemComponent {
  @Output() notify = new EventEmitter<boolean>();
  @Input() item: NavItem | any;

  private readonly router = inject(Router);

  isActive(): boolean {
    const route = this.item?.route as string | undefined;
    if (!route) return false;
    return this.router.isActive(route, {
      paths: 'subset',
      queryParams: 'ignored',
      fragment: 'ignored',
      matrixParams: 'ignored'
    });
  }

  onItemSelected(): void {
    const route = this.item?.route as string | undefined;
    if (!route) return;
    void this.router.navigateByUrl(route).then((ok) => {
      if (ok !== false) {
        window.scroll({ top: 0, left: 0, behavior: 'smooth' });
        this.notify.emit(true);
      }
    });
  }
}
