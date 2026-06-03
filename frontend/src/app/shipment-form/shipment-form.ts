import { Component, OnInit, inject, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { ShipmentService } from '../services/shipment.service';

@Component({
  selector: 'app-shipment-form',
  imports: [ReactiveFormsModule, RouterLink],
  templateUrl: './shipment-form.html',
  styleUrl: './shipment-form.css',
})
export class ShipmentForm implements OnInit {
  private readonly fb = inject(FormBuilder);
  private readonly route = inject(ActivatedRoute);
  private readonly router = inject(Router);
  private readonly shipmentService = inject(ShipmentService);

  readonly editingId = signal<number | null>(null);
  readonly submitting = signal(false);

  readonly form = this.fb.group({
    origin: ['', [Validators.required, Validators.maxLength(100)]],
    destination: ['', [Validators.required, Validators.maxLength(100)]],
    carrier: ['', [Validators.required, Validators.maxLength(100)]],
    weightKg: [null as number | null, [Validators.required, Validators.min(0.01)]],
    estimatedDelivery: [''],
  });

  get isEditMode(): boolean {
    return this.editingId() !== null;
  }

  ngOnInit(): void {
    const idParam = this.route.snapshot.paramMap.get('id');
    if (!idParam) {
      return;
    }

    const id = Number(idParam);
    this.editingId.set(id);
    this.shipmentService.getById(id).subscribe((shipment) => {
      this.form.patchValue({
        origin: shipment.origin,
        destination: shipment.destination,
        carrier: shipment.carrier,
        weightKg: shipment.weightKg,
        estimatedDelivery: shipment.estimatedDelivery
          ? shipment.estimatedDelivery.substring(0, 10)
          : '',
      });
    });
  }

  submit(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    const value = this.form.getRawValue();
    const payload = {
      origin: value.origin!,
      destination: value.destination!,
      carrier: value.carrier!,
      weightKg: value.weightKg!,
      estimatedDelivery: value.estimatedDelivery ? value.estimatedDelivery : null,
    };

    this.submitting.set(true);
    const request$ = this.isEditMode
      ? this.shipmentService.update(this.editingId()!, payload)
      : this.shipmentService.create(payload);

    request$.subscribe({
      next: () => this.router.navigate(['/']),
      error: () => this.submitting.set(false),
    });
  }
}
