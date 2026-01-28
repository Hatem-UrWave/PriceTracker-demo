import { Component, OnInit, inject, signal } from '@angular/core';
import { Router, RouterLink, ActivatedRoute } from '@angular/router';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { AlertsService } from '../../../core/services/alerts.service';

@Component({
  selector: 'app-alert-create',
  imports: [RouterLink, ReactiveFormsModule],
  template: `
    <div class="max-w-2xl mx-auto space-y-6">
      <a routerLink="/alerts" class="inline-flex items-center text-sm text-gray-500 hover:text-gray-700">← Back to Alerts</a>

      <div class="card">
        <div class="p-6">
          <h1 class="text-xl font-bold text-gray-900 mb-6">Create Price Alert</h1>

          <form [formGroup]="form" (ngSubmit)="onSubmit()" class="space-y-6">
            <div>
              <label class="block text-sm font-medium text-gray-700 mb-2">Asset Type</label>
              <select formControlName="assetType" class="input">
                <option value="">Select type...</option>
                <option value="crypto">Cryptocurrency</option>
                <option value="stock">Stock</option>
                <option value="forex">Forex</option>
              </select>
            </div>

            <div>
              <label class="block text-sm font-medium text-gray-700 mb-2">Symbol</label>
              <input type="text" formControlName="symbol" class="input" placeholder="e.g., BTC, AAPL, EUR">
            </div>

            <div>
              <label class="block text-sm font-medium text-gray-700 mb-2">Condition</label>
              <div class="flex space-x-4">
                <label class="flex items-center">
                  <input type="radio" formControlName="condition" value="above" class="h-4 w-4">
                  <span class="ml-2 text-sm">Price goes above</span>
                </label>
                <label class="flex items-center">
                  <input type="radio" formControlName="condition" value="below" class="h-4 w-4">
                  <span class="ml-2 text-sm">Price goes below</span>
                </label>
              </div>
            </div>

            <div>
              <label class="block text-sm font-medium text-gray-700 mb-2">Target Price (USD)</label>
              <input type="number" formControlName="targetPrice" class="input" placeholder="0.00" step="0.01">
            </div>

            <div>
              <label class="block text-sm font-medium text-gray-700 mb-2">Webhook URL (optional)</label>
              <input type="url" formControlName="webhookUrl" class="input" placeholder="https://...">
            </div>

            <div class="flex items-center justify-end space-x-3 pt-4 border-t">
              <a routerLink="/alerts" class="btn-secondary">Cancel</a>
              <button type="submit" [disabled]="!form.valid || submitting()" class="btn-primary">
                @if (submitting()) { Creating... } @else { Create Alert }
              </button>
            </div>

            @if (error()) {
              <div class="p-3 bg-red-50 text-red-700 rounded-md text-sm">{{ error() }}</div>
            }
          </form>
        </div>
      </div>
    </div>
  `
})
export class AlertCreateComponent implements OnInit {
  private readonly fb = inject(FormBuilder);
  private readonly alertsService = inject(AlertsService);
  private readonly router = inject(Router);
  private readonly route = inject(ActivatedRoute);

  submitting = signal(false);
  error = signal<string | null>(null);

  form: FormGroup = this.fb.group({
    assetType: ['', Validators.required],
    symbol: ['', [Validators.required, Validators.maxLength(10)]],
    condition: ['above', Validators.required],
    targetPrice: [null, [Validators.required, Validators.min(0)]],
    webhookUrl: ['']
  });

  ngOnInit() {
    this.route.queryParams.subscribe(params => {
      if (params['type']) this.form.patchValue({ assetType: params['type'] });
      if (params['symbol']) this.form.patchValue({ symbol: params['symbol'] });
    });
  }

  onSubmit() {
    if (!this.form.valid) return;

    this.submitting.set(true);
    this.error.set(null);

    this.alertsService.create(this.form.value).subscribe({
      next: () => this.router.navigate(['/alerts']),
      error: (err) => { this.error.set(err.message); this.submitting.set(false); }
    });
  }
}
