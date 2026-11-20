import { EdisonCircuitsComponent } from './edison/edison-circuits';
import { EdisonHomeComponent } from './edison/edison-home';
import { PortalAppBase, PortalRoute } from './portal-app-base';

export class EdisonPortal extends PortalAppBase {
  readonly MenuColor = '#C9A227';
  readonly PageDisplayText = 'EdisonPortal';
  readonly Routes: readonly PortalRoute[] = [
    { Path: 'home', MenuText: 'Home', Component: EdisonHomeComponent },
    { Path: 'circuits', MenuText: 'Circuits', Component: EdisonCircuitsComponent },
  ];
}
