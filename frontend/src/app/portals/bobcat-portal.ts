import { BobcatHomeComponent } from './bobcat/bobcat-home';
import { BobcatMachinesComponent } from './bobcat/bobcat-machines';
import { PortalAppBase, PortalRoute } from './portal-app-base';

export class BobcatPortal extends PortalAppBase {
  readonly MenuColor = '#E87722';
  readonly PageDisplayText = 'BobcatPortal';
  readonly Routes: readonly PortalRoute[] = [
    { Path: 'home', MenuText: 'Home', Component: BobcatHomeComponent },
    { Path: 'machines', MenuText: 'Machines', Component: BobcatMachinesComponent },
  ];
}
