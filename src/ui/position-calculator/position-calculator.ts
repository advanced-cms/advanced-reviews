import { Dimensions, PinLocation } from "../store/review-store";

export default class PositionCalculator {
    private readonly _iframe: HTMLIFrameElement;

    constructor(iframe: HTMLIFrameElement) {
        this._iframe = iframe;
    }

    calculate(location: PinLocation): Dimensions {
        if (!this._iframe || !location.documentSize) {
            return location.documentRelativePosition;
        }

        return location.documentRelativePosition;
    }
}
