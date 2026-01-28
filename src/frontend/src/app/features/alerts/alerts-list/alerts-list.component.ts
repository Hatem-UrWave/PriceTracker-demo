import { Component, OnInit, inject, signal } from '@angular/core';
import { DatePipe, DecimalPipe } from '@angular/common';
import { RouterLink } from '@angular/router';
import { AlertsService } from '../../../core/services/alerts.service';
import { Alert } from '../../../core/models/alert.model';
import { LoadingSpinnerComponent } from '../../../shared/components/loading-spinner/loading-spinner.component';

@Component({
  selector: 'app-alerts-list',
  imports: [RouterLink, LoadingSpinnerComponent, DatePipe, DecimalPipe],
  template: `
    <div class="space-y-6">
      <div class="flex items-center justify-between">
        <div>
          <h1 class="text-2xl font-bold text-gray-900">Price Alerts</h1>
          <p class="mt-1 text-sm text-gray-500">Get notified when prices hit your targets</p>
        </div>
        <a routerLink="/alerts/create" class="btn-primary">+ Create Alert</a>
      </div>

      @if (loading()) {
        <app-loading-spinner />
      } @else if (error()) {
        <div class="text-center py-8 text-red-600">{{ error() }}</div>
      } @else if (alerts().length === 0) {
        <div class="card p-12 text-center">
          <div class="text-4xl mb-4">🔔</div>
          <h3 class="text-lg font-medium text-gray-900">No alerts yet</h3>
          <p class="text-gray-500 mt-2">Create your first price alert to get started</p>
          <a routerLink="/alerts/create" class="btn-primary mt-4 inline-block">Create Alert</a>
        </div>
      } @else {
        <div class="card overflow-hidden">
          <table class="min-w-full divide-y divide-gray-200">
            <thead class="bg-gray-50">
              <tr>
                <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase">Asset</th>
                <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase">Condition</th>
                <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase">Target</th>
                <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase">Status</th>
                <th class="px-6 py-3 text-right text-xs font-medium text-gray-500 uppercase">Actions</th>
              </tr>
            </thead>
            <tbody class="bg-white divide-y divide-gray-200">
              @for (alert of alerts(); track alert.id) {
                <tr class="hover:bg-gray-50">
                  <td class="px-6 py-4 whitespace-nowrap">
                    <span class="font-medium text-gray-900">{{ alert.symbol }}</span>
                    <span class="ml-2 text-xs text-gray-500">({{ alert.assetType }})</span>
                  </td>
                  <td class="px-6 py-4 whitespace-nowrap text-sm text-gray-500">{{ alert.condition }}</td>
                  <td class="px-6 py-4 whitespace-nowrap text-sm font-medium text-gray-900">
                    \${{ alert.targetPrice | number:'1.2-2' }}
                  </td>
                  <td class="px-6 py-4 whitespace-nowrap">
                    @if (alert.isTriggered) {
                      <span class="px-2 py-1 text-xs rounded-full bg-green-100 text-green-800">Triggered</span>
                    } @else if (alert.isActive) {
                      <span class="px-2 py-1 text-xs rounded-full bg-blue-100 text-blue-800">Active</span>
                    } @else {
                      <span class="px-2 py-1 text-xs rounded-full bg-gray-100 text-gray-800">Inactive</span>
                    }
                  </td>
                  <td class="px-6 py-4 whitespace-nowrap text-right text-sm">
                    <button (click)="deleteAlert(alert.id)" class="text-red-600 hover:text-red-900">Delete</button>
                  </td>
                </tr>
              }
            </tbody>
          </table>
        </div>
      }
    </div>
  `
})
export class AlertsListComponent implements OnInit {
  private readonly alertsService = inject(AlertsService);
  alerts = signal<Alert[]>([]);
  loading = signal(true);
  error = signal<string | null>(null);

  ngOnInit() {
    this.loadAlerts();
  }

  loadAlerts() {
    this.alertsService.getAll().subscribe({
      next: (data) => { this.alerts.set(data); this.loading.set(false); },
      error: (err) => { this.error.set(err.message); this.loading.set(false); }
    });
  }

  deleteAlert(id: number) {
    if (confirm('Delete this alert?')) {
      this.alertsService.delete(id).subscribe({
        next: () => this.alerts.update(alerts => alerts.filter(a => a.id !== id)),
        error: (err) => alert('Failed to delete alert: ' + err.message)
      });
    }
  }
}
