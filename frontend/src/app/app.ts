import { HttpClient } from '@angular/common/http';
import { Component, inject, OnInit, signal } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatProgressBarModule } from '@angular/material/progress-bar';
import { MatToolbarModule } from '@angular/material/toolbar';
import { Title } from '@angular/platform-browser';
import { PortalAppBase } from './portals/portal-app-base';
import { CLIENT_CODE_FILE, resolvePortal } from './portals/portal-resolver';

interface ClientCodeFile {
  ClientCode?: string;
}

@Component({
  selector: 'app-root',
  imports: [MatButtonModule, MatCardModule, MatProgressBarModule, MatToolbarModule],
  templateUrl: './app.html',
  styleUrl: './app.scss',
})
export class App implements OnInit {
  private readonly http = inject(HttpClient);
  private readonly title = inject(Title);

  protected readonly portal = signal<PortalAppBase | null>(null);
  protected readonly clientCode = signal<string | null>(null);
  protected readonly portalLoading = signal(true);
  protected readonly portalError = signal<string | null>(null);
  protected readonly message = signal('');
  protected readonly loading = signal(true);
  protected readonly error = signal<string | null>(null);

  ngOnInit(): void {
    this.loadPortal();
    this.loadHello();
  }

  protected menuTextColor(): string | null {
    const menuColor = this.portal()?.MenuColor;
    if (!menuColor) {
      return null;
    }

    return textColorForMenu(menuColor);
  }

  private loadPortal(): void {
    this.portalLoading.set(true);
    this.portalError.set(null);

    this.http.get<ClientCodeFile>(`/${CLIENT_CODE_FILE}`).subscribe({
      next: (file) => {
        try {
          const resolved = resolvePortal(file.ClientCode);
          this.portal.set(resolved.portal);
          this.clientCode.set(resolved.clientCode);
          this.title.setTitle(resolved.portal.PageDisplayText);
          this.portalLoading.set(false);
        } catch (caught) {
          this.portal.set(null);
          this.clientCode.set(null);
          this.portalError.set(
            caught instanceof Error ? caught.message : 'Could not load the portal.',
          );
          this.portalLoading.set(false);
        }
      },
      error: () => {
        this.portal.set(null);
        this.clientCode.set(null);
        this.portalError.set('Could not read the client code.');
        this.portalLoading.set(false);
      },
    });
  }

  protected loadHello(): void {
    this.loading.set(true);
    this.error.set(null);

    this.http.get('/api/hello', { responseType: 'text' }).subscribe({
      next: (body: string) => {
        this.message.set(body);
        this.loading.set(false);
      },
      error: () => {
        this.message.set('');
        this.error.set('Could not reach the API.');
        this.loading.set(false);
      },
    });
  }
}

function textColorForMenu(menuColor: string): string {
  const match = /^#([0-9a-f]{6})$/i.exec(menuColor.trim());
  if (!match) {
    return '#ffffff';
  }

  const hex = match[1];
  const red = Number.parseInt(hex.slice(0, 2), 16);
  const green = Number.parseInt(hex.slice(2, 4), 16);
  const blue = Number.parseInt(hex.slice(4, 6), 16);
  const luminance = (0.299 * red + 0.587 * green + 0.114 * blue) / 255;
  return luminance > 0.62 ? '#1b1b1b' : '#ffffff';
}
