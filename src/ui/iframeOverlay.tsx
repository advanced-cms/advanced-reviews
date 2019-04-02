import React, { CSSProperties } from "react";
import { inject, observer } from "mobx-react";
import { IReviewComponentStore, ReviewLocation } from "./reviewStore";

interface IframeOverlayProps {
    iframe: HTMLIFrameElement;
    reviewStore?: IReviewComponentStore;
}

const getClosest = (element, selector) => {
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
        const previewContainer = this.props.iframe.parentNode as HTMLElement;
        previewContainer.scrollTop = e.srcElement.scrollTop;
    }

    addReviewLocation(e) {
        //TODO: why we have to hack like this? preventDefault should take care of this
        if (this.overlayDocumentRef.current !== e.target) {
            return;
        }

        const clickedElement = this.props.iframe.contentDocument.elementFromPoint(e.offsetX, e.offsetY) as HTMLElement;
        const propertyElement = getClosest(clickedElement, "[data-epi-property-name]");

        let reviewLocation = new ReviewLocation(this.props.reviewStore, {
            positionX: e.offsetX - 12,
            positionY: e.offsetY - 12,
            propertyName: propertyElement ? propertyElement.dataset.epiPropertyName : null,
            isDone: false
        });
        this.props.reviewStore.reviewLocations.push(reviewLocation);
        //TODO: show dialog right after adding a review location
        //this.props.reviewStore.dialog.showDialog(reviewLocation);
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
            width: width
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
