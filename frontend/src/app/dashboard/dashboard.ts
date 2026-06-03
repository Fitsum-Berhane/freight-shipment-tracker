import { Component, OnInit, inject, signal } from '@angular/core';
import { DatePipe, DecimalPipe } from '@angular/common';
import { RouterLink } from '@angular/router';
import { ShipmentService } from '../services/shipment.service';
import { Shipment, ShipmentStatus, ALLOWED_TRANSITIONS } from '../models/shipment';

@Component({
  selector: 'app-dashboard',
  imports: [DatePipe, DecimalPipe, RouterLink],
  templateUrl: './dashboard.html',
  styleUrl: './dashboard.css',
})
export class Dashboard implements OnInit {
  private readonly shipmentService = inject(ShipmentService);

  readonly shipments = signal<Shipment[]>([]);

  ngOnInit(): void {
    this.shipmentService.getAll().subscribe((data) => this.shipments.set(data));
  }

  badgeClass(status: string): string {
    return `badge badge-${status.toLowerCase()}`;
  }

  allowedNext(status: ShipmentStatus): ShipmentStatus[] {
    return ALLOWED_TRANSITIONS[status];
  }

  changeStatus(shipment: Shipment, newStatus: string): void {
    if (!newStatus) {
      return;
    }

    this.shipmentService
      .updateStatus(shipment.id, { status: newStatus as ShipmentStatus })
      .subscribe((updated) => {
        this.shipments.update((list) =>
          list.map((s) => (s.id === updated.id ? updated : s)),
        );
      });
  }
}

