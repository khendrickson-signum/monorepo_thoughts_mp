import { HttpClient } from '@angular/common/http';
import { Component, inject, OnInit, signal } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatProgressBarModule } from '@angular/material/progress-bar';
import { MatToolbarModule } from '@angular/material/toolbar';

@Component({
  selector: 'app-root',
  imports: [MatButtonModule, MatCardModule, MatProgressBarModule, MatToolbarModule],
  templateUrl: './app.html',
  styleUrl: './app.scss',
})
export class App implements OnInit {
  private readonly http = inject(HttpClient);

  protected readonly message = signal('');
  protected readonly loading = signal(true);
  protected readonly error = signal<string | null>(null);

  ngOnInit(): void {
    this.loadHello();
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
