import { provideHttpClient } from '@angular/common/http';
import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';
import { TestBed } from '@angular/core/testing';
import { App } from './app';

describe('App', () => {
  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [App],
      providers: [provideHttpClient(), provideHttpClientTesting()],
    }).compileComponents();
  });

  it('should create the app', () => {
    const fixture = TestBed.createComponent(App);
    expect(fixture.componentInstance).toBeTruthy();
  });

  it('should show the message returned by the API', async () => {
    const fixture = TestBed.createComponent(App);
    const http = TestBed.inject(HttpTestingController);

    fixture.detectChanges();

    flushClientCode(http, 'Edison');
    const request = http.expectOne('/api/hello');
    expect(request.request.method).toBe('GET');
    request.flush('Hello World');

    await fixture.whenStable();

    const compiled = fixture.nativeElement as HTMLElement;
    expect(compiled.querySelector('.message')?.textContent).toContain('Hello World');
    expect(compiled.querySelector('.portal-title')?.textContent).toContain('EdisonPortal');
    expect(compiled.querySelector('.client-code')?.textContent).toContain('ClientCode Edison');
    expect(menuBackground(compiled)).toBe('rgb(201, 162, 39)');
    http.verify();
  });

  it('should render the portal selected by ClientCode', async () => {
    const fixture = TestBed.createComponent(App);
    const http = TestBed.inject(HttpTestingController);

    fixture.detectChanges();

    flushClientCode(http, 'Bobcat');
    http.expectOne('/api/hello').flush('Hello World');

    await fixture.whenStable();

    const compiled = fixture.nativeElement as HTMLElement;
    expect(compiled.querySelector('.portal-title')?.textContent).toContain('BobcatPortal');
    expect(compiled.querySelector('.menu')?.textContent).toContain('BobcatPortal');
    expect(menuBackground(compiled)).toBe('rgb(232, 119, 34)');
    http.verify();
  });

  it('should show an error when ClientCode is unknown', async () => {
    const fixture = TestBed.createComponent(App);
    const http = TestBed.inject(HttpTestingController);

    fixture.detectChanges();

    flushClientCode(http, 'NotAClient');
    http.expectOne('/api/hello').flush('Hello World');

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
