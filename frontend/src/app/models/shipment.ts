export enum ShipmentStatus {
  Pending = 'Pending',
  InTransit = 'InTransit',
  Delivered = 'Delivered',
  Cancelled = 'Cancelled',
}

export interface Shipment {
  id: number;
  trackingNumber: string;
  origin: string;
  destination: string;
  carrier: string;
  weightKg: number;
  status: ShipmentStatus;
  estimatedDelivery: string | null;
  createdAt: string;
}

export interface CreateShipmentRequest {
  origin: string;
  destination: string;
  carrier: string;
  weightKg: number;
  estimatedDelivery?: string | null;
}

export interface UpdateShipmentRequest {
  origin: string;
  destination: string;
  carrier: string;
  weightKg: number;
  estimatedDelivery?: string | null;
}

export interface UpdateStatusRequest {
  status: ShipmentStatus;
}
