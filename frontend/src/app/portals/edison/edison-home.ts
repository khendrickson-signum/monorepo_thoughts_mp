import { Component } from '@angular/core';
import { HomeComponentBase } from '../home-component-base';

@Component({
  selector: 'app-edison-home',
  templateUrl: '../home-page.html',
  styleUrl: '../portal-page.scss',
})
export class EdisonHomeComponent extends HomeComponentBase {
  readonly ClientName = 'Edison';
}
