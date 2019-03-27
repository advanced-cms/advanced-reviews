import { CSSProperties } from "react";
import { action, computed, observable } from 'mobx';
import moment from "moment";

import screenshots from "./screenshots/screenshots.json";

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
        return moment(this.date).format("MMM Do YYYY");
    }

    @computed get userFriendlyDate() {
        if (!this.date) {
            return "";
        }
        return moment(this.date).fromNow();
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

    saveDialog(): void;

    load(): void;

    getUserAvatarUrl(userName: string): string;
}

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
                firstComment: Comment.create("Alfred", "Rephrase it. ", new Date("2019-01-01")),
                comments: [
                    Comment.create("Lina", "Could you describe it better?", new Date("2019-01-02"), screenshots.idylla),
                    Comment.create("Alfred", "Remove last sentence and include more information in first paragraph.", new Date("2019-01-03")),
                    Comment.create("Lina", "Ok, done.", new Date("2019-01-04"), screenshots.idylla),
                    Comment.create("Alfred", "I still see old text", new Date("2019-03-18"), screenshots.idylla),
                    Comment.create("Lina", "Probably something with the CMS. Now it should be ok", new Date("2019-03-19")),
                    Comment.create("Alfred", "Looks ok.", new Date("2019-03-19")),
                    Comment.create("Lina", "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Aenean sed nisi in erat posuere luctus.", new Date("2019-03-20")),
                    Comment.create("Alfred", "Vivamus sem est, aliquet eget nunc quis, imperdiet cursus sapien. Mauris ullamcorper dui ut nisl vulputate vestibulum.", new Date("2019-03-21")),
                    Comment.create("Lina", "Sed non nisi in odio facilisis aliquam eget volutpat augue. Phasellus vitae auctor risus, non luctus dolor.", new Date("2019-03-22")),
                    Comment.create("Alfred", "Integer sed libero at odio mattis sodales. Ut dapibus erat cursus porttitor malesuada.", new Date("2019-03-23")),
                ]
            }),
            new ReviewLocation(this, {
                id: "2",
                positionX: 100,
                positionY: 150,
                propertyName: "Page body",
                isDone: false,
                firstComment: Comment.create("John", "Remove the above text. It's already included in another article.", new Date("2019-01-01")),
                comments: [
                    Comment.create("Lina", "Etiam viverra ante mauris, eget pretium quam ultrices vel.", new Date("2019-01-02")),
                    Comment.create("Alfred", "Maecenas non lorem et lectus ultrices consequat vel eget magna.", new Date("2019-01-03")),
                    Comment.create("Lina", "Aenean malesuada nibh a ante scelerisque consequat.", new Date("2019-01-04")),
                    Comment.create("Alfred", "Phasellus eu nulla ac tellus semper imperdiet nec eu nulla.", new Date("2019-03-18")),
                    Comment.create("Lina", "Etiam vel tortor gravida, venenatis enim at, finibus dolor.", new Date("2019-03-19")),
                    Comment.create("Alfred", "Nunc ultricies tortor semper leo efficitur, vitae viverra ligula semper.", new Date("2019-03-19")),
                    Comment.create("Lina", "Nunc ultricies tortor semper leo efficitur, vitae viverra ligula semper.", new Date("2019-03-20")),
                    Comment.create("Alfred", "Ut viverra odio ligula, vitae gravida arcu aliquam id.", new Date("2019-03-21")),
                    Comment.create("Lina", "Pellentesque elementum sem quis eleifend gravida.", new Date("2019-03-22")),
                    Comment.create("Alfred", "Quisque tincidunt mi a pretium rutrum.", new Date("2019-03-23")),
                ]
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
    saveDialog(): void {
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
    }

    @computed get currentItemIndex(): number {
        return this.reviewLocations.indexOf(this.dialog.currentEditLocation);
    }

    @action getUserAvatarUrl(userName: string): string {
        return `reviewavatars/${userName}.jpg`;
    }
}

export const stores = {
    reviewStore: new ReviewComponentStore()
}
