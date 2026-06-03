import { Component, OnInit, inject, signal } from '@angular/core';
import { DatePipe, DecimalPipe } from '@angular/common';
import { RouterLink } from '@angular/router';
import { FormBuilder, ReactiveFormsModule } from '@angular/forms';
import { debounceTime, distinctUntilChanged } from 'rxjs';
import { ShipmentService, ShipmentFilters } from '../services/shipment.service';
import { Shipment, ShipmentStatus, ALLOWED_TRANSITIONS } from '../models/shipment';

@Component({
  selector: 'app-dashboard',
  imports: [DatePipe, DecimalPipe, RouterLink, ReactiveFormsModule],
  templateUrl: './dashboard.html',
  styleUrl: './dashboard.css',
})
export class Dashboard implements OnInit {
  private readonly shipmentService = inject(ShipmentService);
  private readonly fb = inject(FormBuilder);

  readonly shipments = signal<Shipment[]>([]);
  readonly carriers = signal<string[]>([]);
  readonly statuses = Object.values(ShipmentStatus);

  readonly filterForm = this.fb.group({
    search: [''],
    status: [''],
    carrier: [''],
  });

  ngOnInit(): void {
    this.loadCarriers();
    this.loadShipments();

    this.filterForm.valueChanges
      .pipe(
        debounceTime(300),
        distinctUntilChanged((a, b) => JSON.stringify(a) === JSON.stringify(b)),
      )
      .subscribe(() => this.loadShipments());
  }

  clearFilters(): void {
    this.filterForm.reset({ search: '', status: '', carrier: '' });
  }

  private loadShipments(): void {
    const { search, status, carrier } = this.filterForm.value;
    const filters: ShipmentFilters = {
      search: search || undefined,
      status: status ? (status as ShipmentStatus) : undefined,
      carrier: carrier || undefined,
    };
    this.shipmentService.getAll(filters).subscribe((data) => this.shipments.set(data));
  }

  private loadCarriers(): void {
    this.shipmentService.getAll().subscribe((all) => {
      const unique = Array.from(new Set(all.map((s) => s.carrier))).sort();
      this.carriers.set(unique);
    });
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

