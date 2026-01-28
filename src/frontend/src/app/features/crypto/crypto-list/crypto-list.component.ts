import { Component, OnInit, inject, signal } from '@angular/core';
import { CryptoService } from '../../../core/services/crypto.service';
import { CryptoPrice } from '../../../core/models/crypto-price.model';
import { PriceCardComponent } from '../../../shared/components/price-card/price-card.component';
import { LoadingSpinnerComponent } from '../../../shared/components/loading-spinner/loading-spinner.component';

@Component({
  selector: 'app-crypto-list',
  imports: [PriceCardComponent, LoadingSpinnerComponent],
  template: `
    <div class="space-y-6">
      <div>
        <h1 class="text-2xl font-bold text-gray-900">Cryptocurrencies</h1>
        <p class="mt-1 text-sm text-gray-500">Track real-time cryptocurrency prices</p>
      </div>

      @if (loading()) {
        <app-loading-spinner />
      } @else if (error()) {
        <div class="text-center py-8 text-red-600">
          {{ error() }}
        </div>
      } @else {
        <div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-4">
          @for (crypto of cryptos(); track crypto.symbol) {
            <app-price-card
              [symbol]="crypto.symbol"
              [name]="crypto.name"
              [price]="crypto.priceUsd"
              [changePercent]="crypto.changePercent24h / 100"
              [link]="'/crypto/' + crypto.symbol"
            />
          }
        </div>

        @if (cryptos().length === 0) {
          <div class="text-center py-12 text-gray-500">
            No cryptocurrency data available
          </div>
        }
      }
    </div>
  `
})
export class CryptoListComponent implements OnInit {
  private readonly cryptoService = inject(CryptoService);

  cryptos = signal<CryptoPrice[]>([]);
  loading = signal(true);
  error = signal<string | null>(null);

  ngOnInit() {
    this.cryptoService.getAll().subscribe({
      next: (data) => {
        this.cryptos.set(data);
        this.loading.set(false);
      },
      error: (err) => {
        this.error.set(err.message);
        this.loading.set(false);
      }
    });
  }
}
