import { BobcatHomeComponent } from './bobcat/bobcat-home';
import { EdisonHomeComponent } from './edison/edison-home';
import { HomeComponentBase } from './home-component-base';
import { IxomHomeComponent } from './ixom/ixom-home';

describe('HomeComponentBase', () => {
  const cases: ReadonlyArray<{
    type: new () => HomeComponentBase;
    clientName: string;
  }> = [
    { type: EdisonHomeComponent, clientName: 'Edison' },
    { type: BobcatHomeComponent, clientName: 'Bobcat' },
    { type: IxomHomeComponent, clientName: 'Ixom' },
  ];

  for (const item of cases) {
    it(`${item.type.name} shares the home page text`, () => {
      const home = new item.type();

      expect(home).toBeInstanceOf(HomeComponentBase);
      expect(home.ClientName).toBe(item.clientName);
      expect(home.PageHeading).toBe(`${item.clientName} Home`);
      expect(home.PageSummary).toBe(`Home page for the ${item.clientName} portal.`);
    });
  }
});
