import { Directive } from '@angular/core';

@Directive()
export abstract class HomeComponentBase {
  abstract readonly ClientName: string;

  get PageHeading(): string {
    return `${this.ClientName} Home`;
  }

  get PageSummary(): string {
    return `Home page for the ${this.ClientName} portal.`;
  }
}
