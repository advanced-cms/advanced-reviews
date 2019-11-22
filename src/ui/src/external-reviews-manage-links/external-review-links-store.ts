import { computed, observable } from "mobx";

export class ReviewLink {
    @observable token: string;
    @observable linkUrl: string;
    @observable validTo: Date;
    @observable isEditable: boolean;
    @observable pinCode: string;
    @observable projectId: number;

    @computed get isActive(): boolean {
        return this.validTo > new Date();
    }

    constructor(token: string, linkUrl: string, validToStr: string, isEditable: boolean, projectId?: number, pinCode?: string) {
        this.token = token;
        this.linkUrl = linkUrl;
        this.isEditable = isEditable;
        this.projectId = projectId;
        this.pinCode = pinCode;
        try {
            this.validTo = new Date(validToStr);
        } catch (error) {
            console.error(error);
        }
    }
}

export interface IExternalReviewStore {
    links: ReviewLink[];

    initialMailSubject: string;

    initialViewMailMessage: string;

    initialEditMailMessage: string;

    addLink(isEditable: boolean): void;

    delete(item: ReviewLink): void;

    share(item: ReviewLink, email: string, subject: string, message: string): void;

    edit(item: ReviewLink, validTo: Date, pinCode: string): void;
}

export class ExternalReviewStore implements IExternalReviewStore {
    _externalReviewService: ExternalReviewService;

    initialMailSubject: string;

    initialViewMailMessage: string;

    initialEditMailMessage: string;

    @observable links: ReviewLink[] = [];

    constructor(externalReviewService: ExternalReviewService) {
        this._externalReviewService = externalReviewService;
    }

    addLink(isEditable: boolean): void {
        this._externalReviewService.add(isEditable).then(item => {
            this.links.push(new ReviewLink(item.token, item.linkUrl, item.validTo, item.isEditable, item.projectId));
        });
    }

    load() {
        this.links = [];
        this._externalReviewService.load().then(items => {
            this.links = items.map(x => new ReviewLink(x.token, x.linkUrl, x.validTo, x.isEditable, x.projectId, x.pinCode));
        });
    }

    delete(item: ReviewLink): void {
        var itemIndex = this.links.indexOf(item);
        if (itemIndex === -1) {
            return;
        }
        this.links.splice(itemIndex, 1);
        this._externalReviewService.delete(item.token);
    }

    share(item: ReviewLink, email: string, subject: string, message: string): void {
        this._externalReviewService.share(item.token, email, subject, message);
    }

    edit(item: ReviewLink, validTo: Date, pinCode: string): void {
        this._externalReviewService.edit(item.token, validTo, pinCode).then(() => {
            item.validTo = validTo;
            item.pinCode = pinCode;
        });
    }
}
