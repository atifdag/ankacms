import { IdCodeName } from '../value-objects/id-code-name';

export class MenuModel {
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
    address: string;
    icon: string;
    parentMenu: IdCodeName;
    constructor() {
        this.creator = new IdCodeName();
        this.lastModifier = new IdCodeName();
        this.parentMenu = new IdCodeName();
    }
}
