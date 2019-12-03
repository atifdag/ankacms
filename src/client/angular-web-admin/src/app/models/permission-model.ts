import { IdCodeName } from '../value-objects/id-code-name';
import { IdCodeNameSelected } from '../value-objects/id-code-name-selected';

export class PermissionModel {
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
    controllerName: string;
    actionName: string;
    menus: IdCodeNameSelected[];
    constructor() {
        this.creator = new IdCodeName();
        this.lastModifier = new IdCodeName();
        this.menus = [];
    }

}
