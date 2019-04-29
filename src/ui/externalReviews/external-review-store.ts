import { computed, observable } from 'mobx';

export class ReviewLink {
    @observable token: string;
    @observable linkUrl: string;
    @observable validTo: Date;
    @observable isEditable: boolean;

    @computed get isActive(): boolean {
        return this.validTo > new Date();
    }

    constructor(token: string, linkUrl: string, validToStr: string, isEditable: boolean) {
        this.token = token;
        this.linkUrl = linkUrl;
        this.isEditable = isEditable;
        try {
            this.validTo = new Date(validToStr);
        } catch(error) {
            console.error(error);            
        }
    }
}

export interface IExternalReviewStore {
    links: ReviewLink[];

    addLink(isEditable: boolean): void;

    delete(item: ReviewLink): void;
}

export class ExternalReviewStore implements IExternalReviewStore {
    _externalReviewService: ExternalReviewService;

    @observable links: ReviewLink[] = [];

    constructor(externalReviewService: ExternalReviewService) {
        this._externalReviewService = externalReviewService;
    }

    addLink(isEditable: boolean): void {
        this._externalReviewService.add(isEditable).then((item) => {
            this.links.push(new ReviewLink(item.token, item.linkUrl, item.validTo, item.isEditable));
        });
    }

    load() {
        this.links = [];
        this._externalReviewService.load().then((items) => {
            this.links = items.map((x) => new ReviewLink(x.token, x.linkUrl, x.validTo, x.isEditable));
        })
    }

    delete(item: ReviewLink): void {
        var itemIndex = this.links.indexOf(item);
        if (itemIndex === -1) {
            return;
        }
        this.links.splice(itemIndex, 1);
        this._externalReviewService.delete(item.token);
    }
}