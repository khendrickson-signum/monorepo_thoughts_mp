import { Component } from '@angular/core';
import { HomeComponentBase } from '../home-component-base';

@Component({
  selector: 'app-ixom-home',
  templateUrl: '../home-page.html',
  styleUrl: '../portal-page.scss',
})
export class IxomHomeComponent extends HomeComponentBase {
  readonly ClientName = 'Ixom';
}
