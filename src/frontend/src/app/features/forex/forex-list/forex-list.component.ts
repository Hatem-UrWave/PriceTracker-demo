import { Component, OnInit, inject, signal } from '@angular/core';
import { DecimalPipe } from '@angular/common';
import { ForexService } from '../../../core/services/forex.service';
import { ForexRate } from '../../../core/models/forex-rate.model';
import { LoadingSpinnerComponent } from '../../../shared/components/loading-spinner/loading-spinner.component';

@Component({
  selector: 'app-forex-list',
  imports: [LoadingSpinnerComponent, DecimalPipe],
  template: `
    <div class="space-y-6">
      <div>
        <h1 class="text-2xl font-bold text-gray-900">Forex Rates</h1>
        <p class="mt-1 text-sm text-gray-500">Real-time foreign exchange rates</p>
      </div>

      @if (loading()) {
        <app-loading-spinner />
      } @else if (error()) {
        <div class="text-center py-8 text-red-600">{{ error() }}</div>
      } @else {
        <div class="card overflow-hidden">
          <table class="min-w-full divide-y divide-gray-200">
            <thead class="bg-gray-50">
              <tr>
                <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase">Currency Pair</th>
                <th class="px-6 py-3 text-right text-xs font-medium text-gray-500 uppercase">Rate</th>
              </tr>
            </thead>
            <tbody class="bg-white divide-y divide-gray-200">
              @for (rate of rates(); track rate.targetCurrency) {
                <tr class="hover:bg-gray-50">
                  <td class="px-6 py-4 whitespace-nowrap text-sm font-medium text-gray-900">
                    {{ rate.baseCurrency }} / {{ rate.targetCurrency }}
                  </td>
                  <td class="px-6 py-4 whitespace-nowrap text-sm text-gray-900 text-right">
                    {{ rate.rate | number:'1.4-4' }}
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
export class ForexListComponent implements OnInit {
  private readonly forexService = inject(ForexService);
  rates = signal<ForexRate[]>([]);
  loading = signal(true);
  error = signal<string | null>(null);

  ngOnInit() {
    this.forexService.getAll().subscribe({
      next: (data) => { this.rates.set(data); this.loading.set(false); },
      error: (err) => { this.error.set(err.message); this.loading.set(false); }
    });
  }
}
