import { IdCodeName } from '../value-objects/id-code-name';
import { IdCodeNameSelected } from '../value-objects/id-code-name-selected';

export class UserModel {
    id: string;
    displayOrder: number;
    isApproved: boolean;
    version: number;
    creationTime: Date;
    creator: IdCodeName;
    lastModificationTime: Date;
    lastModifier: IdCodeName;
    username: string;
    password: string;
    email: string;
    birthDate: Date;
    identityCode: string;
    firstName: string;
    lastName: string;
    biography: string;
    displayName: string;
    roles: IdCodeNameSelected[];
    constructor() {
        this.creator = new IdCodeName();
        this.lastModifier = new IdCodeName();
        this.roles = [];
    }
}
