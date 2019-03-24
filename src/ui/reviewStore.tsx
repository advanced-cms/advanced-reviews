import { CSSProperties } from "react";
import { action, computed, observable } from 'mobx';
import moment from "moment";

import screenshots from "./screenshots.json";

/**
 * Represents a comment added by user
 */
export class Comment {
    author: string;
    authorAvatarUrl: string;
    text: string;
    date: Date;
    screenshot: string;

    @computed get formattedDate() {
        if (!this.date) {
            return "";
        }
        return moment(this.date).format("MMM Do YYYY");
    }

    @computed get userFriendlyDate() {
        if (!this.date) {
            return "";
        }
        return moment(this.date).fromNow();
    }

    static create(author: string, avatarUrl: string, text: string, date?: Date, screenshot?: string): Comment {
        const instance = new Comment();
        instance.author = author;
        instance.authorAvatarUrl = avatarUrl;
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

    @computed get formattedFirstComment() {
        if (!this.firstComment.date) {
            return "";
        }
        const comment = this.firstComment;

        return `${comment.author}: ${comment.text}, ${comment.userFriendlyDate}`;
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

    @computed
    get style(): CSSProperties {
        return {
            top: this.positionY + "px",
            left: this.positionX + "px"
        }
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
    isDialogOpen: boolean;
    isScreenshotMode: boolean;

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
    @observable isDialogOpen = false;
    @observable isScreenshotMode = false;
    @observable currentEditLocation?= new ReviewLocation(null, {});
    @observable currentScreenshot = null;
    @observable currentCommentText = "";
    @observable currentIsDone = false;
    @observable currentPriority = Priority.Normal;

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
        this.currentCommentText = "";
        this.currentScreenshot = null;
        this.currentIsDone = location.isDone;
        this.currentPriority = location.priority;
        this.initialDoneChecked = location.isDone;
        this.initialPriority = location.priority;

        location.updateCurrentUserLastRead();

        this.currentEditLocation = location;
        this.isDialogOpen = true;
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

    closeDialog(action: string): void;

    load(): void;
}


//TODO: remove
const defaultAvatarUrl = "sample_avatar.png";

class ReviewComponentStore implements IReviewComponentStore {
    @observable reviewLocations = [];
    @observable dialog = new DialogState();

    //TODO: read user from identity
    currentUser = "Lina";

    @action.bound
    load(): void {
        //TODO: load from episerver store

        this.reviewLocations = [
            new ReviewLocation(this, {
                id: "1",
                positionX: 10,
                positionY: 80,
                propertyName: "Page name",
                isDone: false,
                firstComment: Comment.create("Alfred", defaultAvatarUrl, "Rephrase it. ", new Date("2019-01-01")),
                comments: [
                    Comment.create("Lina", defaultAvatarUrl, "Could you describe it better?", new Date("2019-01-02"), screenshots.idylla),
                    Comment.create("Alfred", defaultAvatarUrl, "Remove last sentence and include more information in first paragraph.", new Date("2019-01-03")),
                    Comment.create("Lina", defaultAvatarUrl, "Ok, done.", new Date("2019-01-04"), screenshots.idylla),
                    Comment.create("Alfred", defaultAvatarUrl, "I still see old text", new Date("2019-03-18"), screenshots.idylla),
                    Comment.create("Lina", defaultAvatarUrl, "Probably something with the CMS. Now it should be ok", new Date("2019-03-19")),
                    Comment.create("Alfred", defaultAvatarUrl, "Looks ok.", new Date("2019-03-19")),
                ]
            }),
            new ReviewLocation(this, {
                id: "2",
                positionX: 100,
                positionY: 150,
                propertyName: "Page body",
                isDone: false
            }),
            new ReviewLocation(this, {
                id: "3",
                positionX: 250,
                positionY: 200,
                propertyName: "Main ContentArea",
                isDone: false
            }),
            new ReviewLocation(this, {
                id: "4",
                positionX: 125,
                positionY: 330,
                propertyName: "Description",
                isDone: false
            })
        ];
    }

    @action.bound
    closeDialog(action: string): void {
        if (action !== "save") {
            this.dialog.isDialogOpen = false;
            return;
        }

        const editedReview = this.dialog.currentEditLocation;

        this.dialog.isDialogOpen = false;
        editedReview.isDone = this.dialog.currentIsDone;
        editedReview.priority = this.dialog.currentPriority;
        editedReview.clearLastUsersRead();
        //TODO: avatar resolver
        const comment = Comment.create(this.currentUser, defaultAvatarUrl, this.dialog.currentCommentText, null, this.dialog.currentScreenshot);
        if (editedReview.firstComment.date) {
            editedReview.comments.push(comment);
        } else {
            editedReview.firstComment = comment;
        }
        this.dialog.currentEditLocation = new ReviewLocation(this, {});
    }

    @computed get currentItemIndex(): number {
        return this.reviewLocations.indexOf(this.dialog.currentEditLocation);
    }
}

export const stores = {
    reviewStore: new ReviewComponentStore()
}
