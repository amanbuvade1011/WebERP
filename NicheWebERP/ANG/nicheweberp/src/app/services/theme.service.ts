import { Injectable, signal } from '@angular/core';

export type Theme = 'light' | 'dark';

const STORAGE_KEY = 'niche_erp_theme';

@Injectable({ providedIn: 'root' })
export class ThemeService {
  readonly theme = signal<Theme>(this.resolveInitialTheme());

  private readonly media = window.matchMedia('(prefers-color-scheme: dark)');

  constructor() {
    this.apply(this.theme());

    // Only follow live OS theme changes while the user hasn't made an
    // explicit choice in this app - once they toggle, that choice sticks.
    this.media.addEventListener('change', (e) => {
      if (!this.hasExplicitPreference()) {
        this.theme.set(e.matches ? 'dark' : 'light');
        this.apply(this.theme());
      }
    });
  }

  toggle(): void {
    this.set(this.theme() === 'dark' ? 'light' : 'dark');
  }

  set(theme: Theme): void {
    this.theme.set(theme);
    localStorage.setItem(STORAGE_KEY, theme);
    this.apply(theme);
  }

  isDark(): boolean {
    return this.theme() === 'dark';
  }

  private hasExplicitPreference(): boolean {
    const saved = localStorage.getItem(STORAGE_KEY);
    return saved === 'light' || saved === 'dark';
  }

  private resolveInitialTheme(): Theme {
    const saved = localStorage.getItem(STORAGE_KEY);
    if (saved === 'light' || saved === 'dark') {
      return saved;
    }
    return window.matchMedia('(prefers-color-scheme: dark)').matches ? 'dark' : 'light';
  }

  private apply(theme: Theme): void {
    document.documentElement.setAttribute('data-theme', theme);
  }
}
