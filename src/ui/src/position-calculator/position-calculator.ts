import { Dimensions, PinPositioningDetails } from "../store/review-store";

export default class PositionCalculator {
    private readonly _documentSize: Dimensions;
    private _document: Document;

    constructor(documentSize: Dimensions, document?: Document) {
        this._documentSize = documentSize;
        this._document = document;
    }

    private rescale(location: PinPositioningDetails): Dimensions {
        const xFactor = location.documentRelativePosition.x / location.documentSize.x;
        const yFactor = location.documentRelativePosition.y / location.documentSize.y;

        return {
            x: xFactor * this._documentSize.x,
            y: yFactor * this._documentSize.y
        };
    }

    calculate(location: PinPositioningDetails): Dimensions {
        if (location.propertyName && location.propertyPosition && location.propertySize && this._document) {
            const originalOffsetFromPropertyLeft = location.documentRelativePosition.x - location.propertyPosition.x;
            const originalOffsetFromPropertyTop = location.documentRelativePosition.y - location.propertyPosition.y;

            const propertyNode: HTMLElement = this._document.querySelector(
                `[data-epi-property-name='${location.propertyName}']`
            );
            if (propertyNode) {
                const currentOffsetFromPropertyLeft = propertyNode.offsetLeft;
                const currentOffsetFromPropertyTop = propertyNode.offsetTop;

                const xPropertyFactor = propertyNode.offsetWidth / location.propertySize.x;
                const yPropertyFactor = propertyNode.offsetHeight / location.propertySize.y;

                return {
                    x: currentOffsetFromPropertyLeft + originalOffsetFromPropertyLeft * xPropertyFactor,
                    y: currentOffsetFromPropertyTop + originalOffsetFromPropertyTop * yPropertyFactor
                };
            }
        }

        //TODO: rescale in a smart way? return this.rescale(location);
        return location.documentRelativePosition;
    }
}
