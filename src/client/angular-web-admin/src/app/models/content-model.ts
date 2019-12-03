import { IdCodeName } from '../value-objects/id-code-name';

export class ContentModel {
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
    shortName: string;
    description: string;
    keywords: string;
    contentDetail: string;
    url: string;
    imageName: string;
    imagePath: string;
    imageFileType: string;
    imageFileLength: number;
    viewCount: number;
    category: IdCodeName;
    contentId: string;
    file: string|any;
    language: IdCodeName;
    constructor() {
        this.creator = new IdCodeName();
        this.lastModifier = new IdCodeName();
        this.language = new IdCodeName();
        this.category = new IdCodeName();
    }
}
