import { Component, OnInit, inject, signal } from '@angular/core';
import { DecimalPipe } from '@angular/common';
import { RouterLink } from '@angular/router';
import { CryptoService } from '../../core/services/crypto.service';
import { ForexService } from '../../core/services/forex.service';
import { CryptoPrice } from '../../core/models/crypto-price.model';
import { ForexRate } from '../../core/models/forex-rate.model';
import { PriceCardComponent } from '../../shared/components/price-card/price-card.component';
import { LoadingSpinnerComponent } from '../../shared/components/loading-spinner/loading-spinner.component';

@Component({
  selector: 'app-dashboard',
  imports: [RouterLink, PriceCardComponent, LoadingSpinnerComponent, DecimalPipe],
  template: `
    <div class="space-y-8">
      <div>
        <h1 class="text-2xl font-bold text-gray-900">Dashboard</h1>
        <p class="mt-1 text-sm text-gray-500">Real-time price tracking for crypto, stocks, and forex</p>
      </div>

      <section>
        <div class="flex items-center justify-between mb-4">
          <h2 class="text-lg font-semibold text-gray-900">Top Cryptocurrencies</h2>
          <a routerLink="/crypto" class="text-sm text-primary-600 hover:text-primary-700">
            View all →
          </a>
        </div>

        @if (loadingCrypto()) {
          <app-loading-spinner />
        } @else if (cryptoError()) {
          <div class="text-center py-8 text-red-600">
            {{ cryptoError() }}
          </div>
        } @else {
          <div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-4">
            @for (crypto of topCrypto(); track crypto.symbol) {
              <app-price-card
                [symbol]="crypto.symbol"
                [name]="crypto.name"
                [price]="crypto.priceUsd"
                [changePercent]="crypto.changePercent24h / 100"
                [link]="'/crypto/' + crypto.symbol"
              />
            }
          </div>
        }
      </section>

      <section>
        <div class="flex items-center justify-between mb-4">
          <h2 class="text-lg font-semibold text-gray-900">Forex Rates (USD)</h2>
          <a routerLink="/forex" class="text-sm text-primary-600 hover:text-primary-700">
            View all →
          </a>
        </div>

        @if (loadingForex()) {
          <app-loading-spinner />
        } @else if (forexError()) {
          <div class="text-center py-8 text-red-600">
            {{ forexError() }}
          </div>
        } @else {
          <div class="card overflow-hidden">
            <table class="min-w-full divide-y divide-gray-200">
              <thead class="bg-gray-50">
                <tr>
                  <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                    Currency
                  </th>
                  <th class="px-6 py-3 text-right text-xs font-medium text-gray-500 uppercase tracking-wider">
                    Rate
                  </th>
                </tr>
              </thead>
              <tbody class="bg-white divide-y divide-gray-200">
                @for (rate of forexRates(); track rate.targetCurrency) {
                  <tr class="hover:bg-gray-50">
                    <td class="px-6 py-4 whitespace-nowrap text-sm font-medium text-gray-900">
                      USD / {{ rate.targetCurrency }}
                    </td>
                    <td class="px-6 py-4 whitespace-nowrap text-sm text-gray-500 text-right">
                      {{ rate.rate | number:'1.4-4' }}
                    </td>
                  </tr>
                }
              </tbody>
            </table>
          </div>
        }
      </section>

      <section>
        <h2 class="text-lg font-semibold text-gray-900 mb-4">Quick Actions</h2>
        <div class="grid grid-cols-1 md:grid-cols-3 gap-4">
          <a routerLink="/alerts/create"
             class="card p-6 hover:shadow-md transition-shadow duration-200 text-center">
            <div class="text-3xl mb-2">🔔</div>
            <h3 class="font-medium text-gray-900">Create Alert</h3>
            <p class="text-sm text-gray-500 mt-1">Set price notifications</p>
          </a>
          <a routerLink="/alerts"
             class="card p-6 hover:shadow-md transition-shadow duration-200 text-center">
            <div class="text-3xl mb-2">📋</div>
            <h3 class="font-medium text-gray-900">My Alerts</h3>
            <p class="text-sm text-gray-500 mt-1">View active alerts</p>
          </a>
          <a href="/swagger" target="_blank"
             class="card p-6 hover:shadow-md transition-shadow duration-200 text-center">
            <div class="text-3xl mb-2">📚</div>
            <h3 class="font-medium text-gray-900">API Docs</h3>
            <p class="text-sm text-gray-500 mt-1">Explore the API</p>
          </a>
        </div>
      </section>
    </div>
  `
})
export class DashboardComponent implements OnInit {
  private readonly cryptoService = inject(CryptoService);
  private readonly forexService = inject(ForexService);

  topCrypto = signal<CryptoPrice[]>([]);
  forexRates = signal<ForexRate[]>([]);

  loadingCrypto = signal(true);
  loadingForex = signal(true);

  cryptoError = signal<string | null>(null);
  forexError = signal<string | null>(null);

  ngOnInit() {
    this.loadCrypto();
    this.loadForex();
  }

  private loadCrypto() {
    this.cryptoService.getTop(6).subscribe({
      next: (data) => {
        this.topCrypto.set(data);
        this.loadingCrypto.set(false);
      },
      error: (err) => {
        this.cryptoError.set(err.message);
        this.loadingCrypto.set(false);
      }
    });
  }

  private loadForex() {
    this.forexService.getAll().subscribe({
      next: (data) => {
        this.forexRates.set(data.slice(0, 6));
        this.loadingForex.set(false);
      },
      error: (err) => {
        this.forexError.set(err.message);
        this.loadingForex.set(false);
      }
    });
  }
}
