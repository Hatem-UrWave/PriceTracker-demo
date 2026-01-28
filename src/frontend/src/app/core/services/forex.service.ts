import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { ApiService } from './api.service';
import { ForexRate } from '../models/forex-rate.model';

@Injectable({
  providedIn: 'root'
})
export class ForexService {
  private readonly api = inject(ApiService);

  getAll(): Observable<ForexRate[]> {
    return this.api.get<ForexRate[]>('/forex');
  }

  getRate(base: string, target: string): Observable<ForexRate> {
    return this.api.get<ForexRate>(`/forex/${base}/${target}`);
  }
}
