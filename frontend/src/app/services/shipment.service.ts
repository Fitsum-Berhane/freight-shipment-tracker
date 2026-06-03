import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import {
  Shipment,
  ShipmentStatus,
  CreateShipmentRequest,
  UpdateShipmentRequest,
  UpdateStatusRequest,
} from '../models/shipment';

export interface ShipmentFilters {
  status?: ShipmentStatus;
  carrier?: string;
  search?: string;
}

@Injectable({ providedIn: 'root' })
export class ShipmentService {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = 'http://localhost:5243/api/shipments';

  getAll(filters?: ShipmentFilters): Observable<Shipment[]> {
    let params = new HttpParams();
    if (filters?.status) {
      params = params.set('status', filters.status);
    }
    if (filters?.carrier) {
      params = params.set('carrier', filters.carrier);
    }
    if (filters?.search) {
      params = params.set('search', filters.search);
    }
    return this.http.get<Shipment[]>(this.baseUrl, { params });
  }

  getById(id: number): Observable<Shipment> {
    return this.http.get<Shipment>(`${this.baseUrl}/${id}`);
  }

  create(request: CreateShipmentRequest): Observable<Shipment> {
    return this.http.post<Shipment>(this.baseUrl, request);
  }

  update(id: number, request: UpdateShipmentRequest): Observable<Shipment> {
    return this.http.put<Shipment>(`${this.baseUrl}/${id}`, request);
  }

  updateStatus(id: number, request: UpdateStatusRequest): Observable<Shipment> {
    return this.http.patch<Shipment>(`${this.baseUrl}/${id}/status`, request);
  }

  delete(id: number): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}/${id}`);
  }
}
