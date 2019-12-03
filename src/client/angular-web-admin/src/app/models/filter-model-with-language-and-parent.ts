import { IdCodeName } from '../value-objects/id-code-name';

export class FilterModelWithLanguageAndParent {
    language: IdCodeName;
    startDate: Date;
    endDate: Date;
    pageNumber: number;
    pageSize: number;
    status: number;
    searched: string;
    parent: IdCodeName;
    constructor() {
        this.parent = new IdCodeName();
        this.language = new IdCodeName();
    }
}
