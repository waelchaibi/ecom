import { BreakpointObserver } from '@angular/cdk/layout';
import { CommonModule } from '@angular/common';
import { Component, OnDestroy, ViewChild, ViewEncapsulation } from '@angular/core';
import { MatSidenav } from '@angular/material/sidenav';
import { RouterModule } from '@angular/router';
import { TablerIconsModule } from 'angular-tabler-icons';
import { Subscription } from 'rxjs';
import { ChatWidgetComponent } from '../../../core/components/chat-widget/chat-widget.component';
import { MaterialModule } from '../../../shared/material.module';
import { AdminHeaderComponent } from './header/header.component';
import { AdminNavItemComponent } from './sidebar/nav-item.component';
import { AdminSidebarComponent } from './sidebar/sidebar.component';
import { adminNavItems } from './sidebar/sidebar-data';

const MOBILE_VIEW = 'screen and (max-width: 768px)';

@Component({
  selector: 'app-admin-layout',
  standalone: true,
  imports: [
    RouterModule,
    AdminNavItemComponent,
    MaterialModule,
    CommonModule,
    AdminSidebarComponent,
    TablerIconsModule,
    AdminHeaderComponent,
    ChatWidgetComponent
  ],
  templateUrl: './admin-layout.component.html',
  styleUrl: './admin-layout.component.scss',
  encapsulation: ViewEncapsulation.None
})
export class AdminLayoutComponent implements OnDestroy {
  navItems = adminNavItems;

  @ViewChild('leftsidenav')
  public sidenav!: MatSidenav;

  private layoutChangesSubscription = Subscription.EMPTY;
  private isMobileScreen = false;

  get isOver(): boolean {
    return this.isMobileScreen;
  }

  constructor(private breakpointObserver: BreakpointObserver) {
    document.querySelector('html')?.classList.add('light-theme');
    this.layoutChangesSubscription = this.breakpointObserver.observe([MOBILE_VIEW]).subscribe((state) => {
      this.isMobileScreen = state.breakpoints[MOBILE_VIEW];
    });
  }

  ngOnDestroy(): void {
    this.layoutChangesSubscription.unsubscribe();
  }

  /** Close overlay nav after a link click — desktop side mode must stay open. */
  onNavNotify(): void {
    if (this.isOver && this.sidenav?.opened) {
      void this.sidenav.close();
    }
  }

  onSidenavClosedStart(): void {}

  onSidenavOpenedChange(_isOpened: boolean): void {}
}
