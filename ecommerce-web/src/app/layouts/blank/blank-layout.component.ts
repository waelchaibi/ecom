import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { MaterialModule } from '../../shared/material.module';

@Component({
  selector: 'ecom-blank-layout',
  standalone: true,
  imports: [RouterOutlet, MaterialModule, CommonModule],
  template: `
    <mat-sidenav-container class="blue_theme light-theme" autosize autoFocus dir="ltr">
      <router-outlet />
    </mat-sidenav-container>
  `
})
export class BlankLayoutComponent {}
