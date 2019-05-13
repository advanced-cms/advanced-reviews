import React from "react";
import { computed } from "mobx";
import Button from "@material/react-button";
import html2canvas from "html2canvas";
import DrawablePreview from "../drawable-preview/drawable-preview";

import ReactCrop, { Crop, PixelCrop } from "react-image-crop";
import "react-image-crop/lib/ReactCrop.scss";
import "@material/react-icon-button/index.scss";
import "./screenshot-dialog.scss";

import Dialog, { DialogContent, DialogTitle } from "@material/react-dialog";
import { observer } from "mobx-react";

interface ScreenshotPickerProps {
    iframe: HTMLIFrameElement;
    onImageSelected: (string, PixelCrop?) => void;
    toggle: () => void;
    maxWidth: number;
    maxHeight: number;
}

interface ScreenshotPickerState {
    crop: Crop;
    pixelCrop: PixelCrop;
    input: string;
    drawerInput: ResizeResult;
}

enum Mode {
    Default,
    Crop,
    Highlight
}

interface ResizeResult {
    image: string;
    width: number;
    height: number;
}

function resize(base64Str: string, maxWidth: number, maxHeight: number): Promise<ResizeResult> {
    return new Promise(resolve => {
        var img = new Image();
        img.src = base64Str;
        img.onload = function() {
            var canvas = document.createElement("canvas");
            var width = img.width;
            var height = img.height;

            if (width > height) {
                if (width > maxWidth) {
                    height *= maxWidth / width;
                    width = maxWidth;
                }
            } else {
                if (height > maxHeight) {
                    width *= maxHeight / height;
                    height = maxHeight;
                }
            }
            canvas.width = width;
            canvas.height = height;
            var ctx = canvas.getContext("2d");
            ctx.drawImage(img, 0, 0, width, height);
            resolve({
                image: canvas.toDataURL(),
                width: width,
                height: height
            });
        };
    });
}

@observer
export default class ScreenshotDialog extends React.Component<ScreenshotPickerProps, ScreenshotPickerState> {
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

        return Mode.Default;
    }

    onCropChange = crop => {
        this.setState({ crop });
    };

    componentDidMount(): void {
        html2canvas(this.props.iframe.contentDocument.body).then(canvas => {
            this.setState({ input: canvas.toDataURL() });
        });
    }

    cancel = () => {
        this.setState({ crop: this.defaultCrop, input: null, drawerInput: null, pixelCrop: null });
        this.props.toggle();
    };

    crop = async () => {
        if (!this.state.pixelCrop) {
            return;
        }

        const croppedImg = this.getCroppedImg(this.imageRef, this.state.pixelCrop);
        const resizedImage = await resize(croppedImg, this.props.maxWidth, this.props.maxHeight);
        this.setState({ crop: this.defaultCrop, input: null, drawerInput: resizedImage });
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

    onCancel = () => {
        this.remove();
        this.props.toggle();
    };

    onApplyDrawing = img => {
        this.props.onImageSelected(img, this.state.pixelCrop);
        this.setState({ crop: this.defaultCrop, input: null, drawerInput: null, pixelCrop: null });
        this.props.toggle();
    };

    render() {
        return (
            <Dialog
                className="screenshot-picker-dialog"
                open={this.mode !== Mode.Default}
                scrimClickAction=""
                escapeKeyAction=""
            >
                <DialogTitle>
                    <div className="header">Crop and highlight the area you want to comment</div>
                </DialogTitle>
                <DialogContent>
                    <div className="screenshot-picker">
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
                                src={this.state.drawerInput.image}
                                width={this.state.drawerInput.width}
                                height={this.state.drawerInput.height}
                                onCancel={this.onCancel}
                                onApplyDrawing={this.onApplyDrawing}
                            />
                        )}
                    </div>
                </DialogContent>
            </Dialog>
        );
    }
}
