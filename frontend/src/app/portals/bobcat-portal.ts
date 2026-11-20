import { BobcatHomeComponent } from './bobcat/bobcat-home';
import { BobcatMachinesComponent } from './bobcat/bobcat-machines';
import { PortalAppBase, PortalRoute } from './portal-app-base';

// REF: 181a0362-a14d-477d-ab15-425e2a64a525
export class BobcatPortal extends PortalAppBase {
  readonly MenuColor = '#E87722';
  readonly PageDisplayText = 'BobcatPortal';
  readonly Routes: readonly PortalRoute[] = [
    { Path: 'home', MenuText: 'Home', Component: BobcatHomeComponent },
    { Path: 'machines', MenuText: 'Machines', Component: BobcatMachinesComponent },
  ];
}
