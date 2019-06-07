import PositionCalculator from "./position-calculator";
import { Dimensions, PinPositioningDetails } from "../store/review-store";

const originalPinLocation: Dimensions = { x: 100, y: 200 };
const originalOverlaySize: Dimensions = { x: 800, y: 800 };

const getPin = (extraProps?): PinPositioningDetails => {
    const pin = {
        documentRelativePosition: originalPinLocation,
        documentSize: originalOverlaySize
    };
    Object.assign(pin, extraProps || {});
    return pin;
};

describe("when property position data is not available", () => {
    let pin: PinPositioningDetails;
    let positionCalculator: PositionCalculator;

    beforeEach(() => {
        pin = getPin();
        positionCalculator = new PositionCalculator(originalOverlaySize);
    });

    test("it should return position relative to the document", () => {
        const dimensions = positionCalculator.calculate(pin);
        expect(dimensions).toBe(originalPinLocation);
    });
});

//TODO: add more tests with faked HTML document

// describe("when property position data is available", () => {
//     let pin: PinPositioningDetails;
//     let positionCalculator: PositionCalculator;
//
//     beforeEach(() => {
//         pin = getPin({
//             propertyName: "myproperty",
//             propertyPosition: { x: 1, y: 1 },
//             propertySize: { x: 1, y: 1 }
//         });
//         positionCalculator = new PositionCalculator(originalOverlaySize);
//     });
//
//     test("it should return position relative to the document", () => {
//         const dimensions = positionCalculator.calculate(pin);
//         expect(dimensions).toBe(originalPinLocation);
//     });
// });
