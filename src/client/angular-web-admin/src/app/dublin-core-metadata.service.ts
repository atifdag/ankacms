import { Injectable } from '@angular/core';
import { DublinCore } from './value-objects/dublin-core';
import { Meta } from '@angular/platform-browser';

@Injectable({
  providedIn: 'root'
})
export class DublinCoreMetadataService {

  constructor(
    private meta: Meta
  ) { }

  set(dublinCore: DublinCore) {

    if (dublinCore.contributor !== undefined) {
      this.meta.addTag({ name: 'DC.contributor', content: dublinCore.contributor });
    }

    if (dublinCore.coverage !== undefined) {
      this.meta.addTag({ name: 'DC.coverage', content: dublinCore.coverage });
    }

    if (dublinCore.creator !== undefined) {
      this.meta.addTag({ name: 'DC.creator', content: dublinCore.creator });
    }

    if (dublinCore.date !== undefined) {
      this.meta.addTag({ name: 'DC.date', content: dublinCore.date.toString() });
    }

    if (dublinCore.description !== undefined) {
      this.meta.addTag({ name: 'DC.description', content: dublinCore.description });
    }

    if (dublinCore.format !== undefined) {
      this.meta.addTag({ name: 'DC.format', content: dublinCore.format });
    }

    if (dublinCore.identifier !== undefined) {
      this.meta.addTag({ name: 'DC.identifier', content: dublinCore.identifier });
    }

    if (dublinCore.language !== undefined) {
      this.meta.addTag({ name: 'DC.language', content: dublinCore.language });
    }

    if (dublinCore.publisher !== undefined) {
      this.meta.addTag({ name: 'DC.publisher', content: dublinCore.publisher });
    }

    if (dublinCore.relation !== undefined) {
      this.meta.addTag({ name: 'DC.relation', content: dublinCore.relation });
    }
    if (dublinCore.rights !== undefined) {
      this.meta.addTag({ name: 'DC.rights', content: dublinCore.rights });
    }

    if (dublinCore.source !== undefined) {
      this.meta.addTag({ name: 'DC.source', content: dublinCore.source });
    }

    if (dublinCore.subject !== undefined) {
      this.meta.addTag({ name: 'DC.subject', content: dublinCore.subject });
    }

    if (dublinCore.title !== undefined) {
      this.meta.addTag({ name: 'DC.title', content: dublinCore.title });
    }

    if (dublinCore.type !== undefined) {
      this.meta.addTag({ name: 'DC.type', content: dublinCore.type });
    }

  }

}
