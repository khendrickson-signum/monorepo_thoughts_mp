import { provideHttpClient } from '@angular/common/http';
import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';
import { TestBed } from '@angular/core/testing';
import { provideRouter, withDisabledInitialNavigation } from '@angular/router';
import { App } from './app';

describe('App', () => {
  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [App],
      providers: [
        provideHttpClient(),
        provideHttpClientTesting(),
        provideRouter([], withDisabledInitialNavigation()),
      ],
    }).compileComponents();
  });

  it('should create the app', () => {
    const fixture = TestBed.createComponent(App);
    expect(fixture.componentInstance).toBeTruthy();
  });

  it('should show the portal selected by ClientCode', async () => {
    const fixture = TestBed.createComponent(App);
    const http = TestBed.inject(HttpTestingController);

    fixture.detectChanges();

    flushClientCode(http, 'Edison');

    await fixture.whenStable();

    const compiled = fixture.nativeElement as HTMLElement;
    expect(compiled.querySelector('.portal-title')?.textContent).toContain('EdisonPortal');
    expect(compiled.querySelector('.client-code')?.textContent).toContain('ClientCode Edison');
    expect(menuLinkText(compiled)).toEqual(['Home', 'Circuits']);
    expect(compiled.querySelector('.route-page')?.textContent).toContain('Edison Home');
    expect(menuBackground(compiled)).toBe('rgb(201, 162, 39)');

    const circuits = menuLink(compiled, 'Circuits');
    circuits.click();
    await fixture.whenStable();
    fixture.detectChanges();

    expect(compiled.querySelector('.route-page')?.textContent).toContain(
      'this component is unique to this client',
    );
    expect(circuits.classList.contains('menu-link-active')).toBe(true);
    http.verify();
  });

  it('should render the portal selected by ClientCode', async () => {
    const fixture = TestBed.createComponent(App);
    const http = TestBed.inject(HttpTestingController);

    fixture.detectChanges();

    flushClientCode(http, 'Bobcat');

    await fixture.whenStable();

    const compiled = fixture.nativeElement as HTMLElement;
    expect(compiled.querySelector('.portal-title')?.textContent).toContain('BobcatPortal');
    expect(compiled.querySelector('.menu')?.textContent).toContain('BobcatPortal');
    expect(menuLinkText(compiled)).toEqual(['Home', 'Machines']);
    expect(compiled.querySelector('.route-page')?.textContent).toContain('Bobcat Home');
    expect(menuBackground(compiled)).toBe('rgb(232, 119, 34)');

    const machines = menuLink(compiled, 'Machines');
    machines.click();
    await fixture.whenStable();
    fixture.detectChanges();

    expect(compiled.querySelector('.route-page')?.textContent).toContain(
      'this component is unique to this client',
    );
    expect(machines.classList.contains('menu-link-active')).toBe(true);
    http.verify();
  });

  it('should open the Ixom Water Care page from its menu link', async () => {
    const fixture = TestBed.createComponent(App);
    const http = TestBed.inject(HttpTestingController);

    fixture.detectChanges();

    flushClientCode(http, 'Ixom');

    await fixture.whenStable();

    const compiled = fixture.nativeElement as HTMLElement;
    expect(menuLinkText(compiled)).toEqual(['Home', 'Water Care']);

    const waterCare = menuLink(compiled, 'Water Care');
    waterCare.click();
    await fixture.whenStable();
    fixture.detectChanges();

    expect(compiled.querySelector('.route-page')?.textContent).toContain(
      'this component is unique to this client',
    );
    expect(waterCare.classList.contains('menu-link-active')).toBe(true);
    http.verify();
  });

  it('should show an error when ClientCode is unknown', async () => {
    const fixture = TestBed.createComponent(App);
    const http = TestBed.inject(HttpTestingController);

    fixture.detectChanges();

    flushClientCode(http, 'NotAClient');

    await fixture.whenStable();

    const compiled = fixture.nativeElement as HTMLElement;
    expect(compiled.querySelector('.portal-error')?.textContent).toContain('NotAClient');
    expect(compiled.querySelector('.portal-title')).toBeNull();
    http.verify();
  });
});

function flushClientCode(http: HttpTestingController, clientCode: string): void {
  const request = http.expectOne('/client-config.json');
  expect(request.request.method).toBe('GET');
  request.flush({ ClientCode: clientCode });
}

function menuBackground(root: HTMLElement): string {
  const menu = root.querySelector('.menu') as HTMLElement | null;
  return (menu?.style.background ?? '').toLowerCase();
}

function menuLinkText(root: HTMLElement): string[] {
  return [...root.querySelectorAll('.menu-link')].map((link) => link.textContent?.trim() ?? '');
}

function menuLink(root: HTMLElement, text: string): HTMLElement {
  const link = [...root.querySelectorAll('.menu-link')].find((item) =>
    item.textContent?.includes(text),
  );
  if (!(link instanceof HTMLElement)) {
    throw new Error(`Missing menu link ${text}`);
  }

  return link;
}
