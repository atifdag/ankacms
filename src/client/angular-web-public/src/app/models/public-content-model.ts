import { IdCodeName } from '../value-objects/id-code-name';

export class PublicContentModel {
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
    lastModificationTime: Date;
    category: IdCodeName;
    contentId: string;
    language: IdCodeName;
    constructor() {
        this.language = new IdCodeName();
        this.category = new IdCodeName();
    }
}
