import { IdCodeName } from '../value-objects/id-code-name';

export class FilterModelWithLanguage {
    language: IdCodeName;
    startDate: Date;
    endDate: Date;
    pageNumber: number;
    pageSize: number;
    status: number;
    searched: string;
    constructor() {
        this.language = new IdCodeName();
    }
}
