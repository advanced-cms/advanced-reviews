import { action, computed, observable } from "mobx";

export class ReviewLink {
    @observable token: string;
    @observable displayName: string;
    @observable linkUrl: string;
    @observable validTo: Date;
    @observable isEditable: boolean;
    @observable pinCode: string;
    @observable projectId: number;
    projectName: string;

    @computed get isActive(): boolean {
        return this.validTo > new Date();
    }

    @computed get isPersisted(): boolean {
        return !!this.token;
    }

    @action.bound setValidDateFromStr(validToStr: string): void {
        try {
            this.validTo = new Date(validToStr);
        } catch (error) {
            console.error(error);
        }
    }

    constructor(
        token: string,
        displayName: string,
        linkUrl: string,
        validToStr: string,
        isEditable: boolean,
        projectId?: number,
        pinCode?: string,
        projectName?: string
    ) {
        this.token = token;
        this.displayName = displayName;
        this.linkUrl = linkUrl;
        this.isEditable = isEditable;
        this.projectId = projectId;
        this.pinCode = pinCode;
        this.projectName = projectName;
        this.setValidDateFromStr(validToStr);
    }
}

export interface IExternalReviewStore {
    links: ReviewLink[];

    initialMailSubject: string;

    initialViewMailMessage: string;

    initialEditMailMessage: string;

    addLink(isEditable: boolean): Promise<ReviewLink>;

    delete(item: ReviewLink): void;

    share(item: ReviewLink, email: string, subject: string, message: string): void;

    edit(item: ReviewLink, validTo: Date, pinCode: string, displayName: string): void;
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

    addLink(isEditable: boolean): Promise<ReviewLink> {
        return this._externalReviewService.add(isEditable).then(item => {
            const reviewLink = new ReviewLink(
                item.token,
                item.displayName,
                item.linkUrl,
                item.validTo,
                item.isEditable,
                item.projectId,
                null,
                item.projectName
            );
            this.links.push(reviewLink);
            return reviewLink;
        });
    }

    load() {
        this.links = [];
        this._externalReviewService.load().then(items => {
            this.links = items.map(
                x =>
                    new ReviewLink(
                        x.token,
                        x.displayName,
                        x.linkUrl,
                        x.validTo,
                        x.isEditable,
                        x.projectId,
                        x.pinCode,
                        x.projectName
                    )
            );
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

    edit(item: ReviewLink, validTo: Date, pinCode: string, displayName: string): void {
        this._externalReviewService.edit(item.token, validTo, pinCode, displayName).then(result => {
            if (result) {
                item.setValidDateFromStr(result.validTo);
                item.pinCode = result.pinCode;
                item.displayName = result.displayName;
            }
        });
    }
}
