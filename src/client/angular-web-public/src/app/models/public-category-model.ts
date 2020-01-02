import { IdCodeName } from '../value-objects/id-code-name';
import { PublicContentModel } from './public-content-model';

export class PublicCategoryModel {
    code: string;
    name: string;
    description: string;
    keywords: string;
    lastModificationTime: Date;
    categoryId: string;
    language: IdCodeName;
    contents: PublicContentModel[];
    constructor() {
        this.language = new IdCodeName();
        this.contents = [];
    }
}
