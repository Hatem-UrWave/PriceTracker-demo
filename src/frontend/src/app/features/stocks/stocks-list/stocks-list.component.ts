import { Component, OnInit, inject, signal } from '@angular/core';
import { StocksService } from '../../../core/services/stocks.service';
import { StockPrice } from '../../../core/models/stock-price.model';
import { PriceCardComponent } from '../../../shared/components/price-card/price-card.component';
import { LoadingSpinnerComponent } from '../../../shared/components/loading-spinner/loading-spinner.component';

@Component({
  selector: 'app-stocks-list',
  imports: [PriceCardComponent, LoadingSpinnerComponent],
  template: `
    <div class="space-y-6">
      <div>
        <h1 class="text-2xl font-bold text-gray-900">Stocks</h1>
        <p class="mt-1 text-sm text-gray-500">Track real-time stock prices</p>
      </div>

      @if (loading()) {
        <app-loading-spinner />
      } @else if (error()) {
        <div class="text-center py-8 text-red-600">{{ error() }}</div>
      } @else {
        <div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-4">
          @for (stock of stocks(); track stock.symbol) {
            <app-price-card
              [symbol]="stock.symbol"
              [name]="stock.name"
              [price]="stock.price"
              [changePercent]="stock.changePercent / 100"
              [link]="'/stocks/' + stock.symbol"
            />
          }
        </div>
      }
    </div>
  `
})
export class StocksListComponent implements OnInit {
  private readonly stocksService = inject(StocksService);
  stocks = signal<StockPrice[]>([]);
  loading = signal(true);
  error = signal<string | null>(null);

  ngOnInit() {
    this.stocksService.getAll().subscribe({
      next: (data) => { this.stocks.set(data); this.loading.set(false); },
      error: (err) => { this.error.set(err.message); this.loading.set(false); }
    });
  }
}
