import { Injectable } from '@angular/core';
import { Meta, Title } from '@angular/platform-browser';

@Injectable({
  providedIn: 'root'
})
export class TitleAndMetaService {

  constructor(
    private title: Title,
    private meta: Meta
  ) { }

  updateTitle(title: string) {
    this.title.setTitle(title);
  }

  updateMeta(name: string, content: string) {
    this.meta.removeTag(name);
    this.meta.addTag({name, content});
  }

}
