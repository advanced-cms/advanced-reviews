import {CSSProperties} from "react";
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

interface IDialogState {
    currentComment: string;
    isDoneChecked: boolean;
}

class DialogState implements IDialogState {
    @observable currentComment = "";
    @observable isDoneChecked = false;
}

export interface IReviewComponentStore {
    reviewLocations: ReviewLocation[];

    readonly dialog: IDialogState;

    /**
     * Currently logged user. 
     * Field is used when saving comment author
     */
    currentUser: string;

    //TODO: move to dialogState
    isDialogOpen: boolean;
    currentEditLocation?: ReviewLocation;
    showDialog(location: ReviewLocation): void;
    closeDialog(action: string): void;

    load(): void;
}

class ReviewComponentStore implements IReviewComponentStore {
    @observable isDialogOpen = false;
    @observable reviewLocations = [];
    @observable currentEditLocation? = new ReviewLocation({});
    @observable dialog = new DialogState();

    //TODO: read user from identity
    currentUser = "John";

    @action.bound
    load(): void {
        //TODO: load from episerver store
        this.isDialogOpen = false;
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
    showDialog(location: ReviewLocation): void {
        this.dialog.currentComment = "";
        this.dialog.isDoneChecked = location.isDone;
        this.currentEditLocation = location;
        this.isDialogOpen = true;
    }

    @action.bound
    closeDialog(action: string): void {
        this.isDialogOpen = false;
        if (action !== "save") {
            return;
        }
        this.currentEditLocation.isDone = this.dialog.isDoneChecked;
        const comment = Comment.create(this.currentUser, this.dialog.currentComment);
        if (this.currentEditLocation.firstComment.date) {
            this.currentEditLocation.comments.push(comment);
        } else {
            this.currentEditLocation.firstComment = comment;
        }
        this.currentEditLocation = new ReviewLocation({});
    }
}

export const stores = {
    reviewStore: new ReviewComponentStore()
}
