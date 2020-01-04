import { IdCodeName } from '../value-objects/id-code-name';
import { IdCodeNameSelected } from '../value-objects/id-code-name-selected';

export class PartModel {
    id: string;
    displayOrder: number;
    isApproved: boolean;
    version: number;
    creationTime: Date;
    creator: IdCodeName;
    lastModificationTime: Date;
    lastModifier: IdCodeName;
    code: string;
    name: string;
    description: string;
    keywords: string;
    language: IdCodeName;
    partId: string;
    maxItemCount: number;
    contents: IdCodeNameSelected[];
    constructor() {
        this.creator = new IdCodeName();
        this.lastModifier = new IdCodeName();
        this.language = new IdCodeName();
        this.contents = [];
    }
}
