import { IxomHomeComponent } from './ixom/ixom-home';
import { IxomPlantsComponent } from './ixom/ixom-plants';
import { PortalAppBase, PortalRoute } from './portal-app-base';

export class IxomPortal extends PortalAppBase {
  readonly MenuColor = '#007A8A';
  readonly PageDisplayText = 'IxomPortal';
  readonly Routes: readonly PortalRoute[] = [
    { Path: 'home', MenuText: 'Home', Component: IxomHomeComponent },
    { Path: 'plants', MenuText: 'Plants', Component: IxomPlantsComponent },
  ];
}
