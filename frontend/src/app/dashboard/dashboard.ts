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

    const isTerminal =
      newStatus === ShipmentStatus.Delivered || newStatus === ShipmentStatus.Cancelled;
    const message = isTerminal
      ? `Mark ${shipment.trackingNumber} as ${newStatus}? This is final and can't be changed afterwards.`
      : `Change ${shipment.trackingNumber} status to ${newStatus}?`;

    if (!confirm(message)) {
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

  deleteShipment(shipment: Shipment): void {
    const confirmed = confirm(
      `Delete shipment ${shipment.trackingNumber}? This cannot be undone.`,
    );
    if (!confirmed) {
      return;
    }

    this.shipmentService.delete(shipment.id).subscribe(() => {
      this.shipments.update((list) => list.filter((s) => s.id !== shipment.id));
    });
  }
}

