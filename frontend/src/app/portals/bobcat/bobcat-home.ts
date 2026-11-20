import { Component } from '@angular/core';
import { HomeComponentBase } from '../home-component-base';

@Component({
  selector: 'app-bobcat-home',
  templateUrl: '../home-page.html',
  styleUrl: '../portal-page.scss',
})
export class BobcatHomeComponent extends HomeComponentBase {
  readonly ClientName = 'Bobcat';
}
