import { IdCodeName } from '../value-objects/id-code-name';
import { IdCodeNameSelected } from '../value-objects/id-code-name-selected';

export class RoleModel {
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
    level: number;
    permissions: IdCodeNameSelected[];
    constructor() {
        this.creator = new IdCodeName();
        this.lastModifier = new IdCodeName();
        this.permissions = [];
    }
}
