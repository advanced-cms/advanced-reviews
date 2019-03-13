import {CSSProperties} from "react";
import { action, computed, observable } from 'mobx';

class ReviewLocation {
    id: string;
    propertyName: string;
    @observable isDone: boolean;
    positionX: number;
    positionY: number;

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
}

class DialogState implements IDialogState {
    @observable currentComment = "";
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
        this.currentEditLocation = location;
        this.isDialogOpen = true;
    }

    @action.bound
    closeDialog(action: string): void {
        this.isDialogOpen = false;
        this.currentEditLocation = new ReviewLocation({});
        alert(action);
    }
}

export const stores = {
    reviewStore: new ReviewComponentStore()
}
