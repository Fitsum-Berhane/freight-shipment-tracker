import { Component, OnInit, inject, signal } from '@angular/core';
import { DatePipe, DecimalPipe } from '@angular/common';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { ShipmentService } from '../services/shipment.service';
import { Shipment, StatusHistoryEntry } from '../models/shipment';

@Component({
  selector: 'app-shipment-detail',
  imports: [DatePipe, DecimalPipe, RouterLink],
  templateUrl: './shipment-detail.html',
  styleUrl: './shipment-detail.css',
})
export class ShipmentDetail implements OnInit {
  private readonly route = inject(ActivatedRoute);
  private readonly shipmentService = inject(ShipmentService);

  readonly shipment = signal<Shipment | null>(null);
  readonly history = signal<StatusHistoryEntry[]>([]);
  readonly loading = signal(true);
  readonly error = signal<string | null>(null);
  readonly historyError = signal(false);

  ngOnInit(): void {
    const id = Number(this.route.snapshot.paramMap.get('id'));

    this.shipmentService.getById(id).subscribe({
      next: (shipment) => {
        this.shipment.set(shipment);
        this.loading.set(false);
      },
      error: () => {
        this.error.set('Could not load this shipment.');
        this.loading.set(false);
      },
    });

    this.shipmentService.getStatusHistory(id).subscribe({
      next: (history) => this.history.set(history),
      error: () => this.historyError.set(true),
    });
  }

  badgeClass(status: string): string {
    return `badge badge-${status.toLowerCase()}`;
  }
}
