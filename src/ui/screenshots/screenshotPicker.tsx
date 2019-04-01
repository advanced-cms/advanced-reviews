import React from "react";
import { computed } from "mobx";
import Button from "@material/react-button";
import IconButton from "@material/react-icon-button";
import MaterialIcon from "@material/react-material-icon";
import html2canvas from "html2canvas";
import DrawablePreview from "./drawablePreview";

import ReactCrop, { Crop, PixelCrop } from "react-image-crop";
import "react-image-crop/lib/ReactCrop.scss";
import "@material/react-icon-button/index.scss";
import "./screenshotPicker.scss";

interface ScreenshotPickerProps {
    current: string;
    iframe: HTMLIFrameElement;
    onImageSelected: (string, PixelCrop?) => void;
    toggle: () => void;
}

interface ScreenshotPickerState {
    crop: Crop;
    pixelCrop: PixelCrop;
    input: string;
    drawerInput: string;
}

enum Mode {
    Default,
    Crop,
    Highlight,
    Preview
}

export default class ScreenshotPicker extends React.Component<ScreenshotPickerProps, ScreenshotPickerState> {
    defaultCrop = {
        width: 50,
        height: 50,
        x: 10,
        y: 10
    };
    private imageRef: any;

    constructor(props) {
        super(props);
        this.state = {
            crop: this.defaultCrop,
            pixelCrop: null,
            input: null,
            drawerInput: null
        };
    }

    @computed get mode() {
        if (this.state.input) {
            return Mode.Crop;
        }
        if (this.state.drawerInput) {
            return Mode.Highlight;
        }
        if (this.props.current) {
            return Mode.Preview;
        }

        return Mode.Default;
    }

    onCropChange = crop => {
        this.setState({ crop });
    };

    takeScreenshot = () => {
        const body = this.props.iframe.contentDocument.body;

        html2canvas(body).then(canvas => {
            this.setState({ input: canvas.toDataURL() });
            this.props.toggle();
        });
    };

    cancel = () => {
        this.setState({ crop: this.defaultCrop, input: null, drawerInput: null, pixelCrop: null });
        this.props.toggle();
    };

    crop = () => {
        if (!this.state.pixelCrop) {
            return;
        }

        const croppedImg = this.getCroppedImg(this.imageRef, this.state.pixelCrop);
        this.setState({ crop: this.defaultCrop, input: null, drawerInput: croppedImg });
    };

    getCroppedImg(image, pixelCrop) {
        const canvas = document.createElement("canvas");
        canvas.width = pixelCrop.width;
        canvas.height = pixelCrop.height;
        const ctx = canvas.getContext("2d");

        ctx.drawImage(
            image,
            pixelCrop.x,
            pixelCrop.y,
            pixelCrop.width,
            pixelCrop.height,
            0,
            0,
            pixelCrop.width,
            pixelCrop.height
        );

        return canvas.toDataURL();
    }

    onImageLoaded = (image, pixelCrop) => {
        this.imageRef = image;
        this.setState({ pixelCrop });
    };

    onCropComplete = (crop, pixelCrop) => {
        this.setState({ crop, pixelCrop });
    };

    remove = () => {
        this.props.onImageSelected(null);
        this.setState({ crop: this.defaultCrop, input: null, drawerInput: null, pixelCrop: null });
    };

    onApplyDrawing = img => {
        this.props.onImageSelected(img, this.state.pixelCrop);
        this.setState({ crop: this.defaultCrop, input: null, drawerInput: null, pixelCrop: null });
        this.props.toggle();
    };

    render() {
        return (
            <div className="screenshot-picker">
                {this.mode === Mode.Preview && (
                    <>
                        <img alt="" style={{ maxWidth: "100%" }} src={this.props.current} />
                        <IconButton onClick={this.remove} title="Remove screenshot">
                            <MaterialIcon icon="remove" />
                        </IconButton>
                    </>
                )}
                {this.mode === Mode.Default && <Button onClick={this.takeScreenshot}>Attach screenshot</Button>}
                {this.mode === Mode.Crop && (
                    <>
                        <ReactCrop
                            className="screenshot-cropper"
                            crop={this.state.crop}
                            onImageLoaded={this.onImageLoaded}
                            src={this.state.input}
                            onChange={this.onCropChange}
                            onComplete={this.onCropComplete}
                        />
                        <div className="mdc-dialog__actions">
                            <Button onClick={this.cancel}>cancel</Button>
                            <Button onClick={this.crop} disabled={!this.state.crop.width}>
                                Crop
                            </Button>
                        </div>
                    </>
                )}
                {this.mode === Mode.Highlight && (
                    <DrawablePreview
                        src={this.state.drawerInput}
                        width={this.state.pixelCrop.width}
                        height={this.state.pixelCrop.height}
                        onApplyDrawing={this.onApplyDrawing}
                    />
                )}
            </div>
        );
    }
}
