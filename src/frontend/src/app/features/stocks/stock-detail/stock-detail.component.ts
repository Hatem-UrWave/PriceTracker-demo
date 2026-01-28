import { Component, OnInit, Input, inject, signal } from '@angular/core';
import { DecimalPipe, DatePipe } from '@angular/common';
import { RouterLink } from '@angular/router';
import { StocksService } from '../../../core/services/stocks.service';
import { StockPrice } from '../../../core/models/stock-price.model';
import { LoadingSpinnerComponent } from '../../../shared/components/loading-spinner/loading-spinner.component';

@Component({
  selector: 'app-stock-detail',
  imports: [RouterLink, LoadingSpinnerComponent, DecimalPipe, DatePipe],
  template: `
    <div class="space-y-6">
      <a routerLink="/stocks" class="inline-flex items-center text-sm text-gray-500 hover:text-gray-700">← Back to Stocks</a>
      @if (loading()) {
        <app-loading-spinner />
      } @else if (error()) {
        <div class="text-center py-8 text-red-600">{{ error() }}</div>
      } @else if (stock()) {
        <div class="card p-6">
          <h1 class="text-2xl font-bold">{{ stock()!.name }} ({{ stock()!.symbol }})</h1>
          <p class="text-3xl font-bold mt-4">\${{ stock()!.price | number:'1.2-2' }}</p>
          <p [class]="stock()!.changePercent >= 0 ? 'price-up' : 'price-down'" class="text-lg">
            {{ stock()!.changePercent >= 0 ? '+' : '' }}{{ stock()!.changePercent | number:'1.2-2' }}%
          </p>
        </div>
      }
    </div>
  `
})
export class StockDetailComponent implements OnInit {
  @Input() symbol!: string;
  private readonly stocksService = inject(StocksService);
  stock = signal<StockPrice | null>(null);
  loading = signal(true);
  error = signal<string | null>(null);

  ngOnInit() {
    this.stocksService.getBySymbol(this.symbol).subscribe({
      next: (data) => { this.stock.set(data); this.loading.set(false); },
      error: (err) => { this.error.set(err.message); this.loading.set(false); }
    });
  }
}
