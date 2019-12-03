import { IdCodeName } from '../value-objects/id-code-name';

export class UpdateMyInformationModel {
    id: string;
    username: string;
    email: string;
    firstName: string;
    lastName: string;
    biography: string;
    creationTime: Date;
    creator: IdCodeName;
    lastModificationTime: Date;
    lastModifier: IdCodeName;
    constructor() {
        this.creator = new IdCodeName();
        this.lastModifier = new IdCodeName();
    }
}
