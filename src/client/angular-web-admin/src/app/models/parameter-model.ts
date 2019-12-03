import { IdCodeName } from '../value-objects/id-code-name';

export class ParameterModel {
    id: string;
    displayOrder: number;
    isApproved: boolean;
    version: number;
    creationTime: Date;
    creator: IdCodeName;
    lastModificationTime: Date;
    lastModifier: IdCodeName;
    key: string;
    value: string;
    erasable: boolean;
    description: string;
    parameterGroup: IdCodeName;
    constructor() {
        this.creator = new IdCodeName();
        this.lastModifier = new IdCodeName();
        this.parameterGroup = new IdCodeName();
    }
}
