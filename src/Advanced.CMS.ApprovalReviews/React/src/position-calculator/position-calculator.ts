import { Dimensions, PinPositioningDetails } from "../store/review-store";
import offset from "./offset";

export default class PositionCalculator {
    private readonly _documentSize: Dimensions;
    private readonly _external: boolean;
    private readonly _document: Document;

    constructor(documentSize: Dimensions, external: boolean, document?: Document) {
        this._documentSize = documentSize;
        this._external = external;
        this._document = document;
    }

    private positionDomNode(
        node: HTMLElement,
        documentRelativeOffset: Dimensions,
        nodePosition: Dimensions,
        nodeSize: Dimensions,
    ) {
        const nodeOffset = offset(node, this._external);

        const originalOffsetFromLeft = documentRelativeOffset.x - nodePosition.x;
        const originalOffsetFromTop = documentRelativeOffset.y - nodePosition.y;

        const currentOffsetFromLeft = nodeOffset.left;
        const currentOffsetFromTop = nodeOffset.top;

        const xPropertyFactor = node.offsetWidth / nodeSize.x;
        const yPropertyFactor = node.offsetHeight / nodeSize.y;

        return {
            x: currentOffsetFromLeft + originalOffsetFromLeft * xPropertyFactor,
            y: currentOffsetFromTop + originalOffsetFromTop * yPropertyFactor,
        };
    }

    private resize(location: PinPositioningDetails) {
        const xFactor = location.documentRelativePosition.x / location.documentSize.x;
        const yFactor = location.documentRelativePosition.y / location.documentSize.y;

        return {
            x: xFactor * this._documentSize.x,
            y: yFactor * this._documentSize.y,
        };
    }

    calculate(location: PinPositioningDetails): Dimensions {
        if (this._document) {
            if (location.clickedDomNodeSelector && location.clickedDomNodePosition && location.clickedDomNodeSize) {
                const node: HTMLElement = this._document.querySelector(location.clickedDomNodeSelector);
                if (!node) {
                    return this.resize(location);
                }

                return this.positionDomNode(
                    node,
                    location.documentRelativePosition,
                    location.clickedDomNodePosition,
                    location.clickedDomNodeSize,
                );
            } else if (location.propertyName && location.propertyPosition && location.propertySize) {
                const node: HTMLElement = this._document.querySelector(
                    `[data-epi-property-name='${location.propertyName}']`,
                );
                if (!node) {
                    return this.resize(location);
                }
                return this.positionDomNode(
                    node,
                    location.documentRelativePosition,
                    location.propertyPosition,
                    location.propertySize,
                );
            }
        }

        return this.resize(location);
    }
}
