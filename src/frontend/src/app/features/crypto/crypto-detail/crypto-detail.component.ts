import { Component, OnInit, Input, inject, signal } from '@angular/core';
import { DecimalPipe, DatePipe } from '@angular/common';
import { RouterLink } from '@angular/router';
import { CryptoService } from '../../../core/services/crypto.service';
import { CryptoPrice } from '../../../core/models/crypto-price.model';
import { LoadingSpinnerComponent } from '../../../shared/components/loading-spinner/loading-spinner.component';

@Component({
  selector: 'app-crypto-detail',
  imports: [RouterLink, LoadingSpinnerComponent, DecimalPipe, DatePipe],
  template: `
    <div class="space-y-6">
      <a routerLink="/crypto" class="inline-flex items-center text-sm text-gray-500 hover:text-gray-700">
        ← Back to Cryptocurrencies
      </a>

      @if (loading()) {
        <app-loading-spinner />
      } @else if (error()) {
        <div class="text-center py-8 text-red-600">
          {{ error() }}
        </div>
      } @else if (crypto()) {
        <div class="card">
          <div class="p-6">
            <div class="flex items-center justify-between mb-6">
              <div class="flex items-center">
                <div class="w-16 h-16 rounded-full bg-gray-100 flex items-center justify-center">
                  <span class="text-2xl font-bold text-gray-600">{{ crypto()!.symbol.charAt(0) }}</span>
                </div>
                <div class="ml-4">
                  <h1 class="text-2xl font-bold text-gray-900">{{ crypto()!.name }}</h1>
                  <p class="text-gray-500">{{ crypto()!.symbol }}</p>
                </div>
              </div>
              <div class="text-right">
                <p class="text-3xl font-bold text-gray-900">
                  \${{ crypto()!.priceUsd | number:'1.2-2' }}
                </p>
                <p [class]="crypto()!.changePercent24h >= 0 ? 'price-up' : 'price-down'"
                   class="text-lg font-medium">
                  {{ crypto()!.changePercent24h >= 0 ? '+' : '' }}{{ crypto()!.changePercent24h | number:'1.2-2' }}%
                </p>
              </div>
            </div>

            <div class="grid grid-cols-2 md:grid-cols-4 gap-6">
              <div>
                <p class="text-sm text-gray-500">Market Cap</p>
                <p class="text-lg font-semibold text-gray-900">
                  \${{ crypto()!.marketCapUsd | number:'1.0-0' }}
                </p>
              </div>
              <div>
                <p class="text-sm text-gray-500">24h Volume</p>
                <p class="text-lg font-semibold text-gray-900">
                  \${{ crypto()!.volume24hUsd | number:'1.0-0' }}
                </p>
              </div>
              <div>
                <p class="text-sm text-gray-500">Price (EUR)</p>
                <p class="text-lg font-semibold text-gray-900">
                  €{{ crypto()!.priceEur | number:'1.2-2' }}
                </p>
              </div>
              <div>
                <p class="text-sm text-gray-500">Last Updated</p>
                <p class="text-lg font-semibold text-gray-900">
                  {{ crypto()!.lastUpdated | date:'short' }}
                </p>
              </div>
            </div>

            <div class="mt-6 pt-6 border-t border-gray-200">
              <a [routerLink]="['/alerts/create']"
                 [queryParams]="{type: 'crypto', symbol: crypto()!.symbol}"
                 class="btn-primary">
                Create Price Alert
              </a>
            </div>
          </div>
        </div>
      }
    </div>
  `
})
export class CryptoDetailComponent implements OnInit {
  @Input() symbol!: string;

  private readonly cryptoService = inject(CryptoService);

  crypto = signal<CryptoPrice | null>(null);
  loading = signal(true);
  error = signal<string | null>(null);

  ngOnInit() {
    this.cryptoService.getBySymbol(this.symbol).subscribe({
      next: (data) => {
        this.crypto.set(data);
        this.loading.set(false);
      },
      error: (err) => {
        this.error.set(err.message);
        this.loading.set(false);
      }
    });
  }
}
