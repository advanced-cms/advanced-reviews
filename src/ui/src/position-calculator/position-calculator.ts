import { Dimensions, PinPositioningDetails } from "../store/review-store";
import offset from "./offset";

export default class PositionCalculator {
    private readonly _documentSize: Dimensions;
    private readonly _document: Document;

    constructor(documentSize: Dimensions, document?: Document) {
        this._documentSize = documentSize;
        this._document = document;
    }

    private positionDomNode(
        selector: string,
        documentRelativeOffset: Dimensions,
        nodePosition: Dimensions,
        nodeSize: Dimensions
    ) {
        const node: HTMLElement = this._document.querySelector(selector);
        if (!node) {
            return;
        }

        const nodeOffset = offset(node, this._document);

        const originalOffsetFromLeft = documentRelativeOffset.x - nodePosition.x;
        const originalOffsetFromTop = documentRelativeOffset.y - nodePosition.y;

        const currentOffsetFromLeft = nodeOffset.left;
        const currentOffsetFromTop = nodeOffset.top;

        const xPropertyFactor = node.offsetWidth / nodeSize.x;
        const yPropertyFactor = node.offsetHeight / nodeSize.y;

        return {
            x: currentOffsetFromLeft + originalOffsetFromLeft * xPropertyFactor,
            y: currentOffsetFromTop + originalOffsetFromTop * yPropertyFactor
        };
    }

    calculate(location: PinPositioningDetails): Dimensions {
        if (this._document) {
            if (location.propertyName && location.propertyPosition && location.propertySize) {
                const selector = `[data-epi-property-name='${location.propertyName}']`;
                return this.positionDomNode(
                    selector,
                    location.documentRelativePosition,
                    location.propertyPosition,
                    location.propertySize
                );
            } else if (
                location.clickedDomNodeSelector &&
                location.clickedDomNodePosition &&
                location.clickedDomNodeSize
            ) {
                return this.positionDomNode(
                    location.clickedDomNodeSelector,
                    location.documentRelativePosition,
                    location.clickedDomNodePosition,
                    location.clickedDomNodeSize
                );
            }
        }

        const xFactor = location.documentRelativePosition.x / location.documentSize.x;
        const yFactor = location.documentRelativePosition.y / location.documentSize.y;

        return {
            x: xFactor * this._documentSize.x,
            y: yFactor * this._documentSize.y
        };
    }
}
