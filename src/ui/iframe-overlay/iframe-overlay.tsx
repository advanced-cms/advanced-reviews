import React, { CSSProperties } from "react";
import { inject, observer } from "mobx-react";
import { IReviewComponentStore, PinLocation } from "../store/review-store";

interface IframeOverlayProps {
    iframe: HTMLIFrameElement;
    reviewStore?: IReviewComponentStore;
    reviewLocationCreated(location: PinLocation): void;
}

const getClosest = (element, selector): HTMLElement => {
    for (; element && element !== document; element = element.parentNode) {
        if (typeof element.matches === "function" && element.matches(selector)) return element;
    }
    return null;
};

@inject("reviewStore")
@observer
export default class IframeOverlay extends React.Component<IframeOverlayProps, any> {
    calculatePositionInterval: number;
    private overlayRef: React.RefObject<HTMLDivElement>;
    private overlayDocumentRef: React.RefObject<HTMLDivElement>;

    constructor(props: IframeOverlayProps) {
        super(props);
        this.overlayRef = React.createRef<HTMLDivElement>();
        this.overlayDocumentRef = React.createRef<HTMLDivElement>();
        this.state = {
            offsetHeight: 0
        };
    }

    componentDidMount() {
        const checkTime = 1000;
        this.calculatePositionInterval = window.setInterval(
            function() {
                //TODO: when changing context very fast, sometimes we get into here before the component is unmounted, maybe can be solved differently?
                if (!this.props.iframe.contentDocument || !this.props.iframe.contentDocument.body) {
                    return;
                }

                const { offsetHeight } = this.props.iframe.contentDocument.body;
                if (this.state.offsetHeight === offsetHeight) {
                    return;
                }

                this.setState({
                    offsetHeight: offsetHeight
                });
            }.bind(this),
            checkTime
        );
        this.overlayRef.current.addEventListener("scroll", this.scroll.bind(this), true);
        this.overlayRef.current.addEventListener("click", this.addReviewLocation.bind(this));
    }

    componentWillUnmount() {
        clearInterval(this.calculatePositionInterval);
        this.overlayRef.current.removeEventListener("scroll", this.scroll.bind(this), true);
        this.overlayRef.current.removeEventListener("click", this.addReviewLocation.bind(this));
    }

    scroll(e) {
        if (this.overlayRef.current !== e.target) {
            return;
        }

        const previewContainer = this.props.iframe.parentNode as HTMLElement;
        previewContainer.scrollTop = e.srcElement.scrollTop;
    }

    addReviewLocation(e) {
        //TODO: why we have to hack like this? preventDefault should take care of this
        if (this.overlayDocumentRef.current !== e.target) {
            return;
        }

        const clickedElement = this.props.iframe.contentDocument.elementFromPoint(e.offsetX, e.offsetY) as HTMLElement;

        let reviewLocation = new PinLocation(this.props.reviewStore, {
            documentRelativePosition: {
                x: e.offsetX,
                y: e.offsetY
            },
            documentSize: {
                x: this.overlayDocumentRef.current.offsetWidth,
                y: this.overlayDocumentRef.current.offsetHeight
            },
            isDone: false
        });

        const propertyElement = getClosest(clickedElement, "[data-epi-property-name]");
        if (propertyElement) {
            // if property is found we want to remember its offsets as well
            reviewLocation.propertyName = propertyElement.dataset.epiPropertyName;
            reviewLocation.propertyPosition = { x: propertyElement.offsetLeft, y: propertyElement.offsetTop };
            reviewLocation.propertySize = { x: propertyElement.offsetWidth, y: propertyElement.offsetHeight };

            const blockElement = getClosest(clickedElement, "[data-epi-block-id]");
            if (blockElement) {
                reviewLocation.blockId = blockElement.dataset.epiBlockId;
                reviewLocation.blockName = blockElement.dataset.epiContentName;
                reviewLocation.blockPosition = { x: blockElement.offsetLeft, y: blockElement.offsetTop };
                reviewLocation.blockSize = { x: blockElement.offsetWidth, y: blockElement.offsetHeight };
            }
        }

        this.props.reviewLocationCreated(reviewLocation);
    }

    render() {
        const { height, top, width } = this.props.iframe.parentElement.style;

        let styles: CSSProperties = {
            position: "absolute",
            zIndex: 600,
            overflowY: "auto",
            top: top,
            height: height,
            maxHeight: height,
            width: width,
            cursor: "crosshair"
        };

        let documentStyles: CSSProperties = {
            height: this.state.offsetHeight
        };

        return (
            <div ref={this.overlayRef} style={styles}>
                <div ref={this.overlayDocumentRef} style={documentStyles}>
                    {this.props.children}
                </div>
            </div>
        );
    }
}
