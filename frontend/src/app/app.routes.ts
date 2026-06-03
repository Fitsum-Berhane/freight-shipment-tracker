import { Routes } from '@angular/router';
import { Dashboard } from './dashboard/dashboard';
import { ShipmentForm } from './shipment-form/shipment-form';

export const routes: Routes = [
  { path: '', component: Dashboard },
  { path: 'new', component: ShipmentForm },
  { path: ':id/edit', component: ShipmentForm },
];
