import { CSSProperties } from "react";
import { action, computed, observable } from 'mobx';

/**
 * Represents a comment added by user
 */
class Comment {
    author: string;
    text: string;
    date: Date;

    static create(author: string, text: string): Comment {
        const instance = new Comment();
        instance.author = author;
        instance.text = text;
        instance.date = new Date();
        return instance;
    }
}

class ReviewLocation {
    id: string;
    propertyName: string;
    @observable isDone: boolean;
    positionX: number;
    positionY: number;
    @observable priority: Priority = Priority.Minor;
    @observable comments: Comment[] = [];
    /**
     * FirstComment is a main comment added when saving review location for the first time
     */
    @observable firstComment: Comment = new Comment();

    constructor(point: any) {
        Object.keys(point).forEach((key) => this[key] = point[key]);
    }

    @computed
    get style(): CSSProperties {
        return {
            top: this.positionY + "px",
            left: this.positionX + "px"
        }
    }
}

export enum Priority {
    Blocker = "Blocker",
    Critical = "Critical",
    Major = "Major",
    Minor = "Minor",
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
    currentPriority: Priority;
    isDialogOpen: boolean;

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
    @observable currentEditLocation?= new ReviewLocation({});
    @observable currentCommentText = "";
    @observable currentIsDone = false;
    @observable currentPriority = Priority.Minor;

    @observable private initialDoneChecked = false;
    @observable private initialPriority = Priority.Minor;

    @computed
    get canSave(): boolean {
        return this.currentCommentText.trim() !== "" ||
            this.currentIsDone !== this.initialDoneChecked ||
            this.currentPriority !== this.initialPriority;
    }

    @action.bound
    showDialog(location: ReviewLocation): void {
        this.currentCommentText = "";
        this.currentIsDone = location.isDone;
        this.currentPriority = location.priority;
        this.initialDoneChecked = location.isDone;
        this.initialPriority = location.priority;

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

    closeDialog(action: string): void;

    load(): void;
}

class ReviewComponentStore implements IReviewComponentStore {
    @observable reviewLocations = [];
    @observable dialog = new DialogState();

    //TODO: read user from identity
    currentUser = "John";

    @action.bound
    load(): void {
        //TODO: load from episerver store
        this.reviewLocations = [
            new ReviewLocation({
                id: "1",
                positionX: 10,
                positionY: 10,
                propertyName: "Page name",
                isDone: false
            }),
            new ReviewLocation({
                id: "2",
                positionX: 100,
                positionY: 150,
                propertyName: "Page body",
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
        const comment = Comment.create(this.currentUser, this.dialog.currentCommentText);
        if (editedReview.firstComment.date) {
            editedReview.comments.push(comment);
        } else {
            editedReview.firstComment = comment;
        }
        this.dialog.currentEditLocation = new ReviewLocation({});
    }
}

export const stores = {
    reviewStore: new ReviewComponentStore()
}
