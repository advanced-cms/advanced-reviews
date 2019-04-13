import { action, computed, observable } from 'mobx';
import { distanceInWordsToNow, format } from "date-fns";
import { ReviewDialogState } from "./dialog/reviewEditorDialog";

/**
 * Represents a comment added by user
 */
export class Comment {
    author: string;
    text: string;
    date: Date;
    screenshot: string;

    store: IReviewComponentStore;

    constructor(store?: IReviewComponentStore) {
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
            options.locale = require(`date-fns/locale/${this.store.currentLocale}/index.js`)
        }

        return distanceInWordsToNow(this.date, options);
    }

    static create(author: string, text: string, date?: Date, screenshot?: string): Comment {
        const instance = new Comment();
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

export class ReviewLocation {
    id: string;
    propertyName: string;
    @observable isDone: boolean;
    positionX: number;
    positionY: number;
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

        return `${comment.author}: ${comment.text}, ${comment.userFriendlyDate}`;
    }

    @computed get displayName(): string {
        if (this.propertyName) {
            return this.propertyName;
        }
        if (this.firstComment.date) {
            this.firstComment.text;
        }
        //TODO: resources
        return '[Unsaved review]';
    }

    /**
     * List of users and date when they last saw the review.
     */
    usersLastRead: UsersLastReadHashmap = {};

    private _rootStore: IReviewComponentStore;

    constructor(rootStore: IReviewComponentStore, point: any) {
        this._rootStore = rootStore;
        this.firstComment = new Comment(this._rootStore);
        Object.keys(point).forEach((key) => this[key] = point[key]);
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

        let otherUserComments = allComments.filter(x => x.author !== currentUser).map(x => x.date).sort();
        const lastOtherUserComment = otherUserComments.length > 0 ? otherUserComments.pop() : null;
        if (!lastOtherUserComment) {
            // there are no comments added by other users
            return false;
        }

        let currentUserComments = allComments.filter(x => x.author === currentUser).map(x => x.date).sort();
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
    reviewLocations: ReviewLocation[];

    /**
     * Currently logged user.
     * Field is used when saving comment author
     */
    currentUser: string;

    currentLocale: any;

    save(state: ReviewDialogState, reviewLocation: ReviewLocation): Promise<ReviewLocation>;

    load(): void;

    getUserAvatarUrl(userName: string): string;

    filteredReviewLocations: ReviewLocation[];

    filter: ReviewCollectionFilter;
}

class ReviewCollectionFilter {
    @observable showPoints: boolean = true;
    @observable showOnlyOpenTasks: boolean = true;
    @observable showOnlyNewTasks: boolean = true;
}

class ReviewComponentStore implements IReviewComponentStore {
    @observable reviewLocations: ReviewLocation[] = [];

    filter: ReviewCollectionFilter = new ReviewCollectionFilter();

    currentUser = "";

    currentLocale = "en";

    _advancedReviewService: any;

    constructor(advancedReviewService: AdvancedReviewService) {
        this._advancedReviewService = advancedReviewService;
    }

    @action.bound
    load(): void {
        function parseComment(json: any): Comment {
            const comment = json ? Comment.create(json.author, json.text, json.date, json.screenshot) : Comment.create("", "");
            comment.store = this;
            return comment;
        }

        this._advancedReviewService.load()
            .then(reviewLocations => {
                this.reviewLocations = reviewLocations
                    .map((x: any) => {
                        return new ReviewLocation(this, {
                            id: x.id,
                            positionX: x.data.positionX,
                            positionY: x.data.positionY,
                            propertyName: x.data.propertyName,
                            isDone: x.data.isDone,
                            firstComment: parseComment(x.data.firstComment),
                            comments: (x.data.comments || []).map((x: any) => parseComment(x))
                        })
                    });
            });
    }

    @computed get filteredReviewLocations(): ReviewLocation[] {
        let result = this.reviewLocations;
        if (this.filter.showOnlyNewTasks) {
            result = result.filter(x => x.isUpdatedReview);
        }
        if (this.filter.showOnlyOpenTasks) {
            result = result.filter(x => !x.isDone);
        }
        return result;
    }

    //TODO: convert to async method
    @action.bound
    save(item: ReviewDialogState, editedReview: ReviewLocation): Promise<ReviewLocation> {
        editedReview.isDone = item.currentIsDone;
        editedReview.priority = item.currentPriority;
        editedReview.clearLastUsersRead();
        const comment = Comment.create(this.currentUser, item.currentCommentText, null, item.currentScreenshot);
        if (editedReview.firstComment.date) {
            editedReview.comments.push(comment);
        } else {
            editedReview.firstComment = comment;
        }
        return this.saveLocation(editedReview);
    }

    @action getUserAvatarUrl(userName: string): string {
        return `reviewavatars/${userName}.jpg`;
    }

    private saveLocation(reviewLocation: ReviewLocation): Promise<ReviewLocation> {
        return new Promise((resolve, reject) => {
            const data = {
                propertyName: reviewLocation.propertyName,
                isDone: reviewLocation.isDone,
                positionX: reviewLocation.positionX,
                positionY: reviewLocation.positionY,
                priority: reviewLocation.priority,
                comments: reviewLocation.comments.map((x: any) => { 
                    return {
                        author: x.author,
                        date: x.date,
                        screenshot: x.screenshot,
                        text: x.text
                }}),
                firstComment: {
                    author: reviewLocation.firstComment.author,
                    date: reviewLocation.firstComment.date,
                    screenshot: reviewLocation.firstComment.screenshot,
                    text: reviewLocation.firstComment.text
                }
            };
            this._advancedReviewService.add(reviewLocation.id, data).then((result) => {
                if (reviewLocation.id !== result.id) {
                    this.reviewLocations.push(reviewLocation);
                }
                resolve(reviewLocation);
            }).otherwise((e) => {
                reject(e);
            });
        });
    }
}

export const createStores = (advancedReviewService: AdvancedReviewService, res: ReviewResorces): any => {
    return {
        reviewStore: new ReviewComponentStore(advancedReviewService),
        resources: res
    }
}
