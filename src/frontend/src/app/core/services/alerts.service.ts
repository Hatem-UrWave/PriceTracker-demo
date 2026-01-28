import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { ApiService } from './api.service';
import { Alert, CreateAlertRequest } from '../models/alert.model';

@Injectable({
  providedIn: 'root'
})
export class AlertsService {
  private readonly api = inject(ApiService);

  getAll(): Observable<Alert[]> {
    return this.api.get<Alert[]>('/alerts');
  }

  getById(id: number): Observable<Alert> {
    return this.api.get<Alert>(`/alerts/${id}`);
  }

  create(request: CreateAlertRequest): Observable<Alert> {
    return this.api.post<Alert>('/alerts', request);
  }

  delete(id: number): Observable<void> {
    return this.api.delete<void>(`/alerts/${id}`);
  }
}
