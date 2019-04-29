import { ReviewLink, ExternalReviewStore } from "../externalReviews/external-review-store";

function getDate(days: number) {
    var result = new Date();
    result.setDate(result.getDate() + days);
    return result;
}

function uuidv4() {
    return 'xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx'.replace(/[xy]/g, function (c) {
        var r = Math.random() * 16 | 0, v = c == 'x' ? r : (r & 0x3 | 0x8);
        return v.toString(16);
    });
}

export default class FakeReviewLinksStore extends ExternalReviewStore {

    constructor() {
        super(null);

        this.links = [
            new ReviewLink("581888c096584a60bc51dfeab1095d62", "http://www.google.com", getDate(1).toString(), true),
            new ReviewLink("747ae7bb415f47c0813b0052c62b8511", "http://www.google.com", getDate(-1).toString(), true),
            new ReviewLink("193a02bda7a64672b2489171a87c72b1", "http://www.google.com", getDate(1).toString(), false),
            new ReviewLink("39dfc3781d6148849af37bfb728dc9cf", "http://www.google.com", getDate(1).toString(), true),
            new ReviewLink("038be60a9a9444a38954a1f114584927", "http://www.google.com", getDate(1).toString(), true),
            new ReviewLink("774904d9720b45e9b32bc997f74fa041", "http://www.google.com", getDate(-1).toString(), false),
            new ReviewLink("d947b5c0d14c4a5a8c6ab0c224f8817e", "http://www.google.com", getDate(2).toString(), true),
            new ReviewLink("8e1b68777417425284e3ea2c4b9aec7e", "http://www.google.com", getDate(4).toString(), true),
            new ReviewLink("c72b45508ace4e518a7ba9ffe0cb0d12", "http://www.google.com", getDate(10).toString(), false),
            new ReviewLink("bb08a4c991574ac499e90937a5c036f5", "http://www.google.com", getDate(1).toString(), true)
        ];
    }

    addLink(isEditable: boolean): void {
        this.links.push(new ReviewLink(uuidv4(), "http://www.google.com", getDate(5).toString(), isEditable));
    }

    delete(item: ReviewLink): void {
        var itemIndex = this.links.indexOf(item);
        if (itemIndex === -1) {
            return;
        }
        this.links.splice(itemIndex, 1);
    }
}