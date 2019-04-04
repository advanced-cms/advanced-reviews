import { action, computed, observable } from 'mobx';
import { distanceInWordsToNow, format } from "date-fns";

/**
 * Represents a comment added by user
 */
export class Comment {
    author: string;
    text: string;
    date: Date;
    screenshot: string;

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
        return distanceInWordsToNow(this.date, { addSuffix: true });
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
    @observable firstComment: Comment = new Comment();

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

/**
 * State of the edit review location dialog
 */
interface IDialogState {
    /**
     * New comment textarea value
     */
    currentCommentText: string;

    /**
     * Currently edited `isDone` state
     */
    currentIsDone: boolean;
    currentScreenshot: null;
    currentPriority: Priority;
    isScreenshotMode: boolean;
    // isDialogOpen: boolean;

    /**
     * Currently edited location
     */
    currentEditLocation?: ReviewLocation;

    /**
     * Check if the form is dirty
     */
    canSave: boolean;

    showDialog(location: ReviewLocation): void;
}

class DialogState implements IDialogState {
    @observable isScreenshotMode = false;
    @observable currentEditLocation?= new ReviewLocation(null, {});
    @observable currentScreenshot = null;
    @observable currentCommentText = "";
    @observable currentIsDone = false;
    @observable currentPriority = Priority.Normal;
    // @observable isDialogOpen = false;

    @observable private initialDoneChecked = false;
    @observable private initialPriority = Priority.Normal;

    @computed
    get canSave(): boolean {
        return this.currentCommentText.trim() !== "" ||
            this.currentIsDone !== this.initialDoneChecked ||
            this.currentPriority !== this.initialPriority;
    }

    @action.bound
    showDialog(location: ReviewLocation): void {
        // this.isDialogOpen = true;
        this.currentCommentText = "";
        this.currentScreenshot = null;
        this.currentIsDone = location.isDone;
        this.currentPriority = location.priority;
        this.initialDoneChecked = location.isDone;
        this.initialPriority = location.priority;

        location.updateCurrentUserLastRead();

        this.currentEditLocation = location;
    }
}

export interface IReviewComponentStore {
    reviewLocations: ReviewLocation[];

    readonly dialog: IDialogState;

    /**
     * Currently logged user.
     * Field is used when saving comment author
     */
    currentUser: string;

    currentItemIndex: number;

    saveDialog(): Promise<ReviewLocation>;

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
    @observable reviewLocations = [];
    @observable dialog = new DialogState();

    filter: ReviewCollectionFilter = new ReviewCollectionFilter();

    //TODO: read user from identity
    currentUser = "Lina";

    _advancedReviewService: any;

    constructor(advancedReviewService: AdvancedReviewService) {
        this._advancedReviewService = advancedReviewService;
    }

    @action.bound
    load(): void {
        function parseComment(json): Comment {
            if (!json) {
                return Comment.create("", "");
            }
            return Comment.create(json.author, json.text, json.date, json.screenshot);
        }

        this._advancedReviewService.load()
            .then(reviewLocations => {
                this.reviewLocations = reviewLocations
                    .map(x => {
                        let reviewLocation;
                        try {
                            reviewLocation = JSON.parse(x.data);
                        } catch (exception) {
                            reviewLocation = null;
                        }
                        return {
                            id: x.id,
                            data: reviewLocation
                        }
                    })
                    .filter(x => !!x.data)
                    .map(x => {

                        return new ReviewLocation(this, {
                            id: x.id,
                            positionX: x.data.positionX,
                            positionY: x.data.positionY,
                            propertyName: x.data.propertyName,
                            isDone: x.data.isDone,
                            firstComment: parseComment(x.data.firstComment),
                            comments: (x.data.comments || []).map(parseComment)
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
    saveDialog(): Promise<ReviewLocation> {
        const editedReview = this.dialog.currentEditLocation;

        editedReview.isDone = this.dialog.currentIsDone;
        editedReview.priority = this.dialog.currentPriority;
        editedReview.clearLastUsersRead();
        const comment = Comment.create(this.currentUser, this.dialog.currentCommentText, null, this.dialog.currentScreenshot);
        if (editedReview.firstComment.date) {
            editedReview.comments.push(comment);
        } else {
            editedReview.firstComment = comment;
        }
        this.dialog.currentEditLocation = new ReviewLocation(this, {});
        return this.saveLocation(editedReview);
    }

    @computed get currentItemIndex(): number {
        return this.reviewLocations.indexOf(this.dialog.currentEditLocation);
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
                comments: reviewLocation.comments.map(x => {
                    x.author,
                        x.date,
                        x.screenshot,
                        x.text
                }),
                firstComment: {
                    author: reviewLocation.firstComment.author,
                    date: reviewLocation.firstComment.date,
                    screenshot: reviewLocation.firstComment.screenshot,
                    text: reviewLocation.firstComment.text
                }
            }
            this._advancedReviewService.add(reviewLocation.id, data).then((result) => {
                reviewLocation.id = result.id;
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
