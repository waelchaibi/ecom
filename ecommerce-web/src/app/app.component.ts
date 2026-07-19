import { Component, OnInit, inject } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { ConfirmDialogComponent } from './core/components/confirm-dialog/confirm-dialog.component';
import { AuthCoordinatorService } from './core/services/auth-coordinator.service';

@Component({
  selector: 'ecom-root',
  standalone: true,
  imports: [RouterOutlet, ConfirmDialogComponent],
  templateUrl: './app.component.html',
  styleUrl: './app.component.scss'
})
export class AppComponent implements OnInit {
  private readonly coordinator = inject(AuthCoordinatorService);

  ngOnInit(): void {
    this.coordinator.resolveConflictingSessions();
  }
}
