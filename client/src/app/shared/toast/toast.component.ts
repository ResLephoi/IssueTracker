import { Component, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ToastService, Toast } from '../../core/services/toast.service';
import { Subscription } from 'rxjs';

@Component({
  selector: 'app-toast',
  standalone: true,
  imports: [CommonModule],
  template: `
    <div class="toast-container">
      <div 
        *ngFor="let toast of toasts" 
        class="toast" 
        [ngClass]="'toast-' + toast.type"
        (click)="removeToast(toast.id)"
      >
        <div class="toast-content">
          <span class="toast-icon">{{ getIcon(toast.type) }}</span>
          <span class="toast-message">{{ toast.message }}</span>
        </div>
        <button class="toast-close" (click)="removeToast(toast.id)" aria-label="Close">×</button>
      </div>
    </div>
  `,
  styleUrls: ['./toast.component.less']
})
export class ToastComponent implements OnDestroy {
  toasts: Toast[] = [];
  private subscription?: Subscription;

  constructor(private toastService: ToastService) {
    this.subscription = this.toastService.toasts$.subscribe(toasts => {
      this.toasts = toasts;
    });
  }

  ngOnDestroy() {
    if (this.subscription) {
      this.subscription.unsubscribe();
    }
  }

  removeToast(id: string) {
    this.toastService.remove(id);
  }

  getIcon(type: Toast['type']): string {
    switch (type) {
      case 'success': return '✓';
      case 'error': return '✗';
      case 'warning': return '⚠';
      case 'info': 
      default: return 'ℹ';
    }
  }
}
