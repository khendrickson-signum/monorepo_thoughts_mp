import { BobcatHomeComponent } from './bobcat/bobcat-home';
import { BobcatMachinesComponent } from './bobcat/bobcat-machines';
import { BobcatPortal } from './bobcat-portal';
import { EdisonCircuitsComponent } from './edison/edison-circuits';
import { EdisonHomeComponent } from './edison/edison-home';
import { EdisonPortal } from './edison-portal';
import { IxomHomeComponent } from './ixom/ixom-home';
import { IxomWaterCareComponent } from './ixom/ixom-water-care';
import { IxomPortal } from './ixom-portal';
import { PortalAppBase, routesForPortal } from './portal-app-base';
import { resolvePortal } from './portal-resolver';

describe('resolvePortal', () => {
  const cases: ReadonlyArray<{
    setting: string;
    type: new () => PortalAppBase;
    clientCode: string;
    pageDisplayText: string;
    menuColor: string;
    routes: ReadonlyArray<{ path: string; menuText: string; component: new () => unknown }>;
  }> = [
    {
      setting: 'Edison',
      type: EdisonPortal,
      clientCode: 'Edison',
      pageDisplayText: 'EdisonPortal',
      menuColor: '#C9A227',
      routes: [
        { path: 'home', menuText: 'Home', component: EdisonHomeComponent },
        { path: 'circuits', menuText: 'Circuits', component: EdisonCircuitsComponent },
      ],
    },
    {
      setting: 'bobcat',
      type: BobcatPortal,
      clientCode: 'Bobcat',
      pageDisplayText: 'BobcatPortal',
      menuColor: '#E87722',
      routes: [
        { path: 'home', menuText: 'Home', component: BobcatHomeComponent },
        { path: 'machines', menuText: 'Machines', component: BobcatMachinesComponent },
      ],
    },
    {
      setting: 'IXOM',
      type: IxomPortal,
      clientCode: 'Ixom',
      pageDisplayText: 'IxomPortal',
      menuColor: '#007A8A',
      routes: [
        { path: 'home', menuText: 'Home', component: IxomHomeComponent },
        { path: 'water-care', menuText: 'Water Care', component: IxomWaterCareComponent },
      ],
    },
  ];

  for (const item of cases) {
    it(`resolves ${item.setting} to ${item.type.name}`, () => {
      const resolved = resolvePortal(item.setting);

      expect(resolved.clientCode).toBe(item.clientCode);
      expect(resolved.portal).toBeInstanceOf(PortalAppBase);
      expect(resolved.portal).toBeInstanceOf(item.type);
      expect(resolved.portal.PageDisplayText).toBe(item.pageDisplayText);
      expect(resolved.portal.MenuColor).toBe(item.menuColor);
      expect(resolved.portal.Routes.map((route) => route.Path)).toEqual(
        item.routes.map((route) => route.path),
      );
      expect(resolved.portal.Routes.map((route) => route.MenuText)).toEqual(
        item.routes.map((route) => route.menuText),
      );
      expect(resolved.portal.Routes.map((route) => route.Component)).toEqual(
        item.routes.map((route) => route.component),
      );
      expect(routesForPortal(resolved.portal).map((route) => route.path)).toEqual([
        ...item.routes.map((route) => route.path),
        '',
        '**',
      ]);
    });
  }

  it('throws when ClientCode is missing', () => {
    expect(() => resolvePortal('  ')).toThrow(/ClientCode/);
  });

  it('throws when ClientCode is unknown', () => {
    expect(() => resolvePortal('NotAClient')).toThrow(/NotAClient/);
  });
});
