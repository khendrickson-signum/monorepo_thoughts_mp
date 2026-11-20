import { Type } from '@angular/core';
import { Routes } from '@angular/router';

export interface PortalRoute {
  readonly Path: string;
  readonly MenuText: string;
  readonly Component: Type<unknown>;
}

export abstract class PortalAppBase {
  abstract readonly MenuColor: string;
  abstract readonly PageDisplayText: string;
  abstract readonly Routes: readonly PortalRoute[];
}

export function routesForPortal(portal: PortalAppBase): Routes {
  const declared = portal.Routes.map((route) => ({
    path: route.Path,
    component: route.Component,
  }));
  const landing = declared[0]?.path;
  if (!landing) {
    return [];
  }

  return [
    ...declared,
    { path: '', pathMatch: 'full', redirectTo: landing },
    { path: '**', redirectTo: landing },
  ];
}
