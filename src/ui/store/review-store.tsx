import { action, computed, observable } from "mobx";
import { distanceInWordsToNow, format } from "date-fns";

const locales = {
    da: require("date-fns/locale/da"),
    de: require("date-fns/locale/de"),
    en: require("date-fns/locale/en"),
    es: require("date-fns/locale/es"),
    fi: require("date-fns/locale/fi"),
    fr: require("date-fns/locale/fr"),
    it: require("date-fns/locale/it"),
    ja: require("date-fns/locale/ja"),
    no: require("date-fns/locale/nb"), // date-fns uses bokmål as the default norwegian culture, not nynorsk as in epi
    nl: require("date-fns/locale/nl"),
    sv: require("date-fns/locale/sv"),
    zh_cn: require("date-fns/locale/zh_cn")
};

/**
 * Represents a comment added by user
 */
export class Comment {
    author: string;
    text: string;
    date: Date;
    screenshot: string;

    store: IReviewComponentStore;

    constructor(store: IReviewComponentStore) {
        this.store = store;
    }

    @computed get formattedDate() {
        if (!this.date) {
            return "";
        }
        return format(this.date, "MMM Do YYYY");
    }

    @computed get userFriendlyDate() {
        if (!this.date) {
            return "";
        }

        const options: any = { addSuffix: true };
        if (this.store && this.store.currentLocale) {
            options.locale = locales[this.store.currentLocale.toLowerCase()];
        }

        return distanceInWordsToNow(this.date, options);
    }

    static create(
        author: string,
        text: string,
        store: IReviewComponentStore,
        date?: Date,
        screenshot?: string
    ): Comment {
        const instance = new Comment(store);
        instance.author = author;
        instance.text = text;
        instance.date = date || new Date();
        instance.screenshot = screenshot;
        return instance;
    }
}

interface UsersLastReadHashmap {
    [userName: string]: Date;
}

export interface NewPinDto {
    currentCommentText: string;
    currentPriority: Priority;
    currentScreenshot: string;
    screenshotMode: boolean;
}

export interface Dimensions {
    x: number;
    y: number;
}

export interface PinPositioningDetails {
    documentRelativePosition: Dimensions;
    documentSize: Dimensions;
    propertyName?: string;
    propertyPosition?: Dimensions;
    propertySize?: Dimensions;
}

export class PinLocation implements PinPositioningDetails {
    id: string;
    propertyName: string;
    @observable isDone: boolean;
    documentRelativePosition: Dimensions;
    documentSize: Dimensions;
    propertyPosition?: Dimensions;
    propertySize?: Dimensions;
    @observable priority: Priority = Priority.Normal;
    @observable comments: Comment[] = [];
    /**
     * FirstComment is a main comment added when saving review location for the first time
     */
    @observable firstComment: Comment;

    @computed get formattedFirstComment(): string {
        if (!this.firstComment.date) {
            return "";
        }
        const comment = this.firstComment;

        let result = "";
        if (comment.author && comment.author.trim()) {
            result += comment.author + ":";
        }

        if (comment.text && comment.text.trim()) {
            result += comment.text + ",";
        }

        result += comment.userFriendlyDate;

        return result;
    }

    @computed get displayName(): string {
        return this.propertyName || this.firstComment.text;
    }

    /**
     * List of users and date when they last saw the review.
     */
    usersLastRead: UsersLastReadHashmap = {};

    private _rootStore: IReviewComponentStore;

    constructor(rootStore: IReviewComponentStore, point: any) {
        this._rootStore = rootStore;
        this.firstComment = new Comment(this._rootStore);
        Object.keys(point).forEach(key => (this[key] = point[key]));
    }

    @action updateCurrentUserLastRead(): void {
        if (!this._rootStore) {
            return;
        }
        this.usersLastRead[this._rootStore.currentUser] = new Date();
    }

    @action clearLastUsersRead(): void {
        this.usersLastRead = {};
    }

    @computed get isUpdatedReview() {
        if (!this._rootStore) {
            return false;
        }

        const currentUser = this._rootStore.currentUser;

        let allComments = this.comments.slice();
        if (this.firstComment.date) {
            allComments.push(this.firstComment);
        }
        if (allComments.length === 0) {
            return false;
        }

        let otherUserComments = allComments
            .filter(x => x.author !== currentUser)
            .map(x => x.date)
            .sort();
        const lastOtherUserComment = otherUserComments.length > 0 ? otherUserComments.pop() : null;
        if (!lastOtherUserComment) {
            // there are no comments added by other users
            return false;
        }

        let currentUserComments = allComments
            .filter(x => x.author === currentUser)
            .map(x => x.date)
            .sort();
        const lastCurrentUserComment = currentUserComments.length > 0 ? currentUserComments.pop() : null;
        if (lastCurrentUserComment !== null && lastCurrentUserComment > lastOtherUserComment) {
            // current user has the last comment
            return false;
        }

        const lastCurrentUserReadDate = this.usersLastRead[this._rootStore.currentUser];
        if (!lastCurrentUserReadDate) {
            // current user hsa no comments
            return true;
        }
        if (lastCurrentUserReadDate > lastOtherUserComment) {
            return false;
        }

        return true;
    }
}

export enum Priority {
    Important = "Important",
    Normal = "Normal",
    Trivial = "Trivial"
}

export interface IReviewComponentStore {
    reviewLocations: PinLocation[];

    /**
     * Currently logged user.
     * Field is used when saving comment author
     */
    currentUser: string;

    currentLocale: string;

    editedPinLocation: PinLocation;

    selectedPinLocation: PinLocation;

    selectedPinLocationIndex: number;

    toggleResolve(): Promise<PinLocation>;

    addComment(commentText: string, screenshot?: string): Promise<PinLocation>;

    save(state: NewPinDto, reviewLocation: PinLocation): Promise<PinLocation>;

    load(): void;

    getUserAvatarUrl(userName: string): string;

    filteredReviewLocations: PinLocation[];

    filter: ReviewCollectionFilter;
}

class ReviewCollectionFilter {
    @observable reviewMode: boolean = true;
    @observable showUnread: boolean = true;
    @observable showActive: boolean = true;
    @observable showResolved: boolean = false;
}

class ReviewComponentStore implements IReviewComponentStore {
    @observable reviewLocations: PinLocation[] = [];
    @observable editedPinLocation: PinLocation;
    @observable selectedPinLocation: PinLocation;

    filter: ReviewCollectionFilter = new ReviewCollectionFilter();

    currentUser = "";

    currentLocale = "en";

    _advancedReviewService: any;

    constructor(advancedReviewService: AdvancedReviewService) {
        this._advancedReviewService = advancedReviewService;
    }

    private parseComment(json: any): Comment {
        return json
            ? Comment.create(json.author, json.text, this, json.date, json.screenshot)
            : Comment.create("", "", this);
    }

    @action.bound
    load(): void {
        this._advancedReviewService.load().then(reviewLocations => {
            this.reviewLocations = reviewLocations.map((x: any) => {
                return new PinLocation(this, {
                    id: x.id,
                    documentRelativePosition: x.data.documentRelativePosition,
                    documentSize: x.data.documentSize,
                    propertyPosition: x.data.propertyPosition,
                    propertySize: x.data.propertySize,
                    propertyName: x.data.propertyName,
                    isDone: x.data.isDone,
                    firstComment: this.parseComment(x.data.firstComment),
                    comments: (x.data.comments || []).map((x: any) => this.parseComment(x))
                });
            });
        });
    }

    @computed get selectedPinLocationIndex(): number {
        if (!this.selectedPinLocation) {
            return -1;
        }

        return this.reviewLocations.indexOf(this.selectedPinLocation);
    }

    @computed get filteredReviewLocations(): PinLocation[] {
        return this.reviewLocations.filter(location => {
            return (
                (this.filter.showResolved && location.isDone) ||
                (this.filter.showActive && !location.isDone && !location.isUpdatedReview) ||
                (this.filter.showUnread && location.isUpdatedReview)
            );
        });
    }

    @action.bound
    toggleResolve(): Promise<PinLocation> {
        this.editedPinLocation.isDone = !this.editedPinLocation.isDone;
        return this.saveLocation(this.editedPinLocation);
    }

    @action.bound
    addComment(commentText: string, screenshot?: string): Promise<PinLocation> {
        const comment = Comment.create(this.currentUser, commentText, this, null, screenshot);
        this.editedPinLocation.comments.push(comment);
        this.editedPinLocation.clearLastUsersRead();
        return this.saveLocation(this.editedPinLocation);
    }

    //TODO: convert to async method
    @action.bound
    save(item: NewPinDto, editedReview: PinLocation): Promise<PinLocation> {
        editedReview.priority = item.currentPriority;
        editedReview.firstComment = Comment.create(
            this.currentUser,
            item.currentCommentText,
            this,
            null,
            item.currentScreenshot
        );
        return this.saveLocation(editedReview);
    }

    @action getUserAvatarUrl(userName: string): string {
        return `/review-avatars/${userName}.jpg`;
    }

    private saveLocation(reviewLocation: PinLocation): Promise<PinLocation> {
        return new Promise((resolve, reject) => {
            const data = {
                propertyName: reviewLocation.propertyName,
                isDone: reviewLocation.isDone,
                documentRelativePosition: reviewLocation.documentRelativePosition,
                documentSize: reviewLocation.documentSize,
                propertyPosition: reviewLocation.propertyPosition,
                propertySize: reviewLocation.propertySize,
                priority: reviewLocation.priority,
                comments: reviewLocation.comments.map((x: any) => {
                    return {
                        author: x.author,
                        date: x.date,
                        screenshot: x.screenshot,
                        text: x.text
                    };
                }),
                firstComment: {
                    author: reviewLocation.firstComment.author,
                    date: reviewLocation.firstComment.date,
                    screenshot: reviewLocation.firstComment.screenshot,
                    text: reviewLocation.firstComment.text
                }
            };
            this._advancedReviewService
                .add(reviewLocation.id, data)
                .then(result => {
                    if (reviewLocation.id !== result.id) {
                        reviewLocation.id = result.id;
                        this.reviewLocations.push(reviewLocation);
                    }
                    resolve(reviewLocation);
                })
                .otherwise(e => {
                    reject(e);
                });
        });
    }
}

export const createStores = (advancedReviewService: AdvancedReviewService, res: ReviewResources): any => {
    return {
        reviewStore: new ReviewComponentStore(advancedReviewService),
        resources: res
    };
};
