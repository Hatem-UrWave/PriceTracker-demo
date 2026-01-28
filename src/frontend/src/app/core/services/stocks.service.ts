import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { ApiService } from './api.service';
import { StockPrice } from '../models/stock-price.model';

@Injectable({
  providedIn: 'root'
})
export class StocksService {
  private readonly api = inject(ApiService);

  getAll(): Observable<StockPrice[]> {
    return this.api.get<StockPrice[]>('/stocks');
  }

  getBySymbol(symbol: string): Observable<StockPrice> {
    return this.api.get<StockPrice>(`/stocks/${symbol}`);
  }
}
