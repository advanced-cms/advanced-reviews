import React from "react";
import Button from "@material/react-button";
import IconButton from "@material/react-icon-button";
import MaterialIcon from "@material/react-material-icon";
import html2canvas from "html2canvas";

import ReactCrop, { Crop, PixelCrop } from "react-image-crop";
import "react-image-crop/lib/ReactCrop.scss";
import "@material/react-icon-button/index.scss";

interface ScreenshotPickerProps {
    current: string;
    iframe: HTMLIFrameElement;
    onImageSelected: (string) => void;
    toggle: () => void;
}

interface ScreenshotPickerState {
    crop: Crop;
    pixelCrop: PixelCrop;
    input: string;
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
            input: null
        };
    }

    onCropChange = crop => {
        this.setState({ crop });
    };

    takeScreenshot = () => {
        const body = this.props.iframe.contentWindow.document.body;

        html2canvas(body).then(canvas => {
            this.setState({ input: canvas.toDataURL() });
            this.props.toggle();
        });
    };

    cancel = () => {
        this.setState({ crop: this.defaultCrop, input: null });
        this.props.toggle();
    };

    save = () => {
        if (!this.state.pixelCrop) {
            return;
        }

        let croppedImg = this.getCroppedImg(this.imageRef, this.state.pixelCrop);
        this.props.onImageSelected(croppedImg);
        this.setState({ crop: this.defaultCrop, input: null });
        this.props.toggle();
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

    onImageLoaded = image => {
        this.imageRef = image;
    };

    onCropComplete = (crop, pixelCrop) => {
        this.setState({ crop, pixelCrop });
    };

    remove = () => {
        this.props.onImageSelected(null);
        this.setState({ crop: this.defaultCrop, input: null });
    };

    render() {
        return (
            <div>
                {!this.state.input && this.props.current && (
                    <>
                        <img alt="" style={{ maxWidth: "100%" }} src={this.props.current} />
                        <IconButton onClick={this.remove} title="Remove screenshot">
                            <MaterialIcon icon="remove" />
                        </IconButton>
                    </>
                )}
                {!this.state.input && !this.props.current && (
                    <Button onClick={this.takeScreenshot}>Attach screenshot</Button>
                )}
                {this.state.input && (
                    <>
                        <ReactCrop
                            crop={this.state.crop}
                            onImageLoaded={this.onImageLoaded}
                            src={this.state.input}
                            onChange={this.onCropChange}
                            onComplete={this.onCropComplete}
                        />
                        <div>
                            <Button onClick={this.cancel}>cancel</Button>
                            <Button onClick={this.save}>Save</Button>
                        </div>
                    </>
                )}
            </div>
        );
    }
}
