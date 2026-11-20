import { IxomHomeComponent } from './ixom/ixom-home';
import { IxomWaterCareComponent } from './ixom/ixom-water-care';
import { PortalAppBase, PortalRoute } from './portal-app-base';

export class IxomPortal extends PortalAppBase {
  readonly MenuColor = '#007A8A';
  readonly PageDisplayText = 'IxomPortal';
  readonly Routes: readonly PortalRoute[] = [
    { Path: 'home', MenuText: 'Home', Component: IxomHomeComponent },
    { Path: 'water-care', MenuText: 'Water Care', Component: IxomWaterCareComponent },
  ];
}
