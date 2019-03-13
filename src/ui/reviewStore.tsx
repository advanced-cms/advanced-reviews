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
        //TODO: pass current user from store - John
        this.currentEditLocation.comments.push(Comment.create("John", this.dialog.currentComment));
        this.currentEditLocation = new ReviewLocation({});
    }
}

export const stores = {
    reviewStore: new ReviewComponentStore()
}
