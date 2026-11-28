import { Injectable, signal } from '@angular/core';
import { AR } from './ar';
import { EN } from './en';

export type Lang = 'ar' | 'en';

const DICTS: Record<Lang, Record<string, string>> = { ar: AR, en: EN };

@Injectable({ providedIn: 'root' })
export class LangService {
  readonly lang = signal<Lang>(this.initial());

  private initial(): Lang {
    const saved = localStorage.getItem('recruit_lang');
    return saved === 'en' ? 'en' : 'ar'; // Arabic is the default
  }

  constructor() {
    this.apply(this.lang());
  }

  set(lang: Lang): void {
    this.lang.set(lang);
    localStorage.setItem('recruit_lang', lang);
    this.apply(lang);
  }

  toggle(): void {
    this.set(this.lang() === 'ar' ? 'en' : 'ar');
  }

  t(key: string): string {
    return DICTS[this.lang()][key] ?? DICTS.en[key] ?? key;
  }

  dir(): 'rtl' | 'ltr' {
    return this.lang() === 'ar' ? 'rtl' : 'ltr';
  }

  private apply(lang: Lang): void {
    const dir = lang === 'ar' ? 'rtl' : 'ltr';
    document.documentElement.lang = lang;
    document.documentElement.dir = dir;
    document.body.classList.toggle('font-ar', lang === 'ar');
  }
}
