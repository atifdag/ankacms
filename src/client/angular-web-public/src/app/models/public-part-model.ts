import { IdCodeName } from '../value-objects/id-code-name';
import { PublicContentModel } from './public-content-model';

export class PublicPartModel {
    code: string;
    name: string;
    description: string;
    keywords: string;
    lastModificationTime: Date;
    partId: string;
    language: IdCodeName;
    contents: PublicContentModel[];
    constructor() {
        this.language = new IdCodeName();
        this.contents = [];
    }
}
