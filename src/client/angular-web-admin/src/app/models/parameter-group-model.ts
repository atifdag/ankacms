import { IdCodeName } from '../value-objects/id-code-name';

export class ParameterGroupModel {
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

    constructor() {
        this.creator = new IdCodeName();
        this.lastModifier = new IdCodeName();
    }
}
