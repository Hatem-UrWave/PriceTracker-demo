import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { ApiService } from './api.service';
import { CryptoPrice } from '../models/crypto-price.model';

@Injectable({
  providedIn: 'root'
})
export class CryptoService {
  private readonly api = inject(ApiService);

  getAll(): Observable<CryptoPrice[]> {
    return this.api.get<CryptoPrice[]>('/crypto');
  }

  getBySymbol(symbol: string): Observable<CryptoPrice> {
    return this.api.get<CryptoPrice>(`/crypto/${symbol}`);
  }

  getTop(count: number = 10): Observable<CryptoPrice[]> {
    return this.api.get<CryptoPrice[]>(`/crypto/top/${count}`);
  }
}
