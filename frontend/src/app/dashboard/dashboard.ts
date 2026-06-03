import { Component, OnInit, inject, signal } from '@angular/core';
import { DatePipe, DecimalPipe } from '@angular/common';
import { HttpErrorResponse } from '@angular/common/http';
import { RouterLink } from '@angular/router';
import { FormBuilder, ReactiveFormsModule } from '@angular/forms';
import { debounceTime, distinctUntilChanged } from 'rxjs';
import { ShipmentService, ShipmentFilters } from '../services/shipment.service';
import { Shipment, ShipmentStatus, ALLOWED_TRANSITIONS, STATUS_LABELS } from '../models/shipment';

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
  readonly statusLabels = STATUS_LABELS;

  readonly loading = signal(false);
  readonly error = signal<string | null>(null);
  readonly actionError = signal<string | null>(null);

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

  dismissActionError(): void {
    this.actionError.set(null);
  }

  private loadShipments(): void {
    const { search, status, carrier } = this.filterForm.value;
    const filters: ShipmentFilters = {
      search: search || undefined,
      status: status ? (status as ShipmentStatus) : undefined,
      carrier: carrier || undefined,
    };

    this.loading.set(true);
    this.error.set(null);
    this.shipmentService.getAll(filters).subscribe({
      next: (data) => {
        this.shipments.set(data);
        this.loading.set(false);
      },
      error: (err) => {
        this.error.set(this.extractError(err));
        this.loading.set(false);
      },
    });
  }

  private extractError(err: unknown): string {
    const httpError = err as HttpErrorResponse;
    if (httpError?.error?.message) {
      return httpError.error.message;
    }
    if (httpError?.status === 0) {
      return 'Cannot reach the server. Make sure the API is running.';
    }
    return 'Something went wrong. Please try again.';
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
      .subscribe({
        next: (updated) => {
          this.actionError.set(null);
          this.shipments.update((list) =>
            list.map((s) => (s.id === updated.id ? updated : s)),
          );
        },
        error: (err) => this.actionError.set(this.extractError(err)),
      });
  }

  deleteShipment(shipment: Shipment): void {
    const confirmed = confirm(
      `Delete shipment ${shipment.trackingNumber}? This cannot be undone.`,
    );
    if (!confirmed) {
      return;
    }

    this.shipmentService.delete(shipment.id).subscribe({
      next: () => {
        this.actionError.set(null);
        this.shipments.update((list) => list.filter((s) => s.id !== shipment.id));
      },
      error: (err) => this.actionError.set(this.extractError(err)),
    });
  }
}

