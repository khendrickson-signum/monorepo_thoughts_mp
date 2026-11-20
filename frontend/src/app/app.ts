import { HttpClient } from '@angular/common/http';
import { Component, inject, OnInit, signal } from '@angular/core';
import { MatProgressBarModule } from '@angular/material/progress-bar';
import { MatToolbarModule } from '@angular/material/toolbar';
import { Title } from '@angular/platform-browser';
import { Router, RouterLink, RouterLinkActive, RouterOutlet } from '@angular/router';
import { PortalAppBase, routesForPortal } from './portals/portal-app-base';
import { CLIENT_CODE_FILE, resolvePortal } from './portals/portal-resolver';

interface ClientCodeFile {
  ClientCode?: string;
}

@Component({
  selector: 'app-root',
  imports: [
    MatProgressBarModule,
    MatToolbarModule,
    RouterLink,
    RouterLinkActive,
    RouterOutlet,
  ],
  templateUrl: './app.html',
  styleUrl: './app.scss',
})
export class App implements OnInit {
  private readonly http = inject(HttpClient);
  private readonly router = inject(Router);
  private readonly title = inject(Title);

  protected readonly portal = signal<PortalAppBase | null>(null);
  protected readonly clientCode = signal<string | null>(null);
  protected readonly portalLoading = signal(true);
  protected readonly portalError = signal<string | null>(null);

  ngOnInit(): void {
    this.loadPortal();
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
          this.showPortalRoutes(resolved.portal);
        } catch (caught) {
          this.portal.set(null);
          this.clientCode.set(null);
          this.clearPortalRoutes();
          this.portalError.set(
            caught instanceof Error ? caught.message : 'Could not load the portal.',
          );
          this.portalLoading.set(false);
        }
      },
      error: () => {
        this.portal.set(null);
        this.clientCode.set(null);
        this.clearPortalRoutes();
        this.portalError.set('Could not read the client code.');
        this.portalLoading.set(false);
      },
    });
  }

  private showPortalRoutes(portal: PortalAppBase): void {
    this.router.resetConfig(routesForPortal(portal));
    if (portal.Routes.length === 0) {
      return;
    }

    void this.router.navigateByUrl(this.landingPath(portal));
  }

  private clearPortalRoutes(): void {
    this.router.resetConfig([]);
  }

  private landingPath(portal: PortalAppBase): string {
    const current = this.router.url.split(/[?#]/)[0].replace(/^\//, '');
    const match = portal.Routes.find((route) => route.Path === current);
    const path = match?.Path ?? portal.Routes[0]?.Path ?? '';
    return path ? `/${path}` : '/';
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
