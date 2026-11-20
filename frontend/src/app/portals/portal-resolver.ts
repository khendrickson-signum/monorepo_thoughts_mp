import { BobcatPortal } from './bobcat-portal';
import { EdisonPortal } from './edison-portal';
import { IxomPortal } from './ixom-portal';
import { PortalAppBase } from './portal-app-base';

export const CLIENT_CODE_KEY = 'ClientCode';
export const CLIENT_CODE_FILE = 'client-config.json';

export interface ResolvedPortal {
  clientCode: string;
  portal: PortalAppBase;
}

export function resolvePortal(clientCode: string | null | undefined): ResolvedPortal {
  const configured = clientCode?.trim() ?? '';
  if (!configured) {
    throw new Error(
      `The '${CLIENT_CODE_KEY}' setting is missing. Set it to one of: ${knownClients()}.`,
    );
  }

  switch (configured.toLowerCase()) {
    case 'edison':
      return { clientCode: 'Edison', portal: new EdisonPortal() };
    case 'bobcat':
      return { clientCode: 'Bobcat', portal: new BobcatPortal() };
    case 'ixom':
      return { clientCode: 'Ixom', portal: new IxomPortal() };
    default:
      throw new Error(
        `The '${CLIENT_CODE_KEY}' setting '${configured}' is not a known client. Expected one of: ${knownClients()}.`,
      );
  }
}

function knownClients(): string {
  return 'Edison, Bobcat, Ixom';
}
