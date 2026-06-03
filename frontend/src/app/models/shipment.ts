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

export interface StatusHistoryEntry {
  status: ShipmentStatus;
  changedAt: string;
}

export const ALLOWED_TRANSITIONS: Record<ShipmentStatus, ShipmentStatus[]> = {
  [ShipmentStatus.Pending]: [ShipmentStatus.InTransit, ShipmentStatus.Cancelled],
  [ShipmentStatus.InTransit]: [ShipmentStatus.Delivered, ShipmentStatus.Cancelled],
  [ShipmentStatus.Delivered]: [],
  [ShipmentStatus.Cancelled]: [],
};

export const STATUS_LABELS: Record<ShipmentStatus, string> = {
  [ShipmentStatus.Pending]: 'Pending',
  [ShipmentStatus.InTransit]: 'In Transit',
  [ShipmentStatus.Delivered]: 'Delivered',
  [ShipmentStatus.Cancelled]: 'Cancelled',
};
