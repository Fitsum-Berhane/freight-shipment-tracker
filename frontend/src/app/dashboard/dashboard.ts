import { Component, OnInit, inject, signal } from '@angular/core';
import { DatePipe, DecimalPipe } from '@angular/common';
import { RouterLink } from '@angular/router';
import { ShipmentService } from '../services/shipment.service';
import { Shipment } from '../models/shipment';

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
}
