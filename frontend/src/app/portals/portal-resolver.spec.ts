import { BobcatPortal } from './bobcat-portal';
import { EdisonPortal } from './edison-portal';
import { IxomPortal } from './ixom-portal';
import { PortalAppBase } from './portal-app-base';
import { resolvePortal } from './portal-resolver';

describe('resolvePortal', () => {
  const cases: ReadonlyArray<{
    setting: string;
    type: new () => PortalAppBase;
    clientCode: string;
    pageDisplayText: string;
    menuColor: string;
  }> = [
    {
      setting: 'Edison',
      type: EdisonPortal,
      clientCode: 'Edison',
      pageDisplayText: 'EdisonPortal',
      menuColor: '#C9A227',
    },
    {
      setting: 'bobcat',
      type: BobcatPortal,
      clientCode: 'Bobcat',
      pageDisplayText: 'BobcatPortal',
      menuColor: '#E87722',
    },
    {
      setting: 'IXOM',
      type: IxomPortal,
      clientCode: 'Ixom',
      pageDisplayText: 'IxomPortal',
      menuColor: '#007A8A',
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
    });
  }

  it('throws when ClientCode is missing', () => {
    expect(() => resolvePortal('  ')).toThrow(/ClientCode/);
  });

  it('throws when ClientCode is unknown', () => {
    expect(() => resolvePortal('NotAClient')).toThrow(/NotAClient/);
  });
});
