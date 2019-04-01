import React from "react";
import Button from "@material/react-button";
import CanvasDraw from "react-canvas-draw";
import mergeImages from "merge-images";

interface DrawablePreviewProps {
    width: number;
    height: number;
    src: string;
    onApplyDrawing: (string) => void;
}

export default class DrawablePreview extends React.Component<DrawablePreviewProps, any> {
    constructor(props: any) {
        super(props);
        this.state = {
            canvas: null
        };
    }

    setCanvas = canvasDraw => {
        if (this.state.canvasDraw) {
            return;
        }
        this.setState({ canvasDraw: canvasDraw });
    };

    undo = () => {
        this.state.canvasDraw.undo();
    };

    clear = () => {
        this.state.canvasDraw.clear();
    };

    done = () => {
        mergeImages([this.props.src, this.state.canvasDraw.canvas.drawing.toDataURL()]).then(result => {
            this.props.onApplyDrawing(result);
        });
    };

    render() {
        let canvasWidth = this.props.width;
        let canvasHeight = this.props.height;

        return (
            <>
                <CanvasDraw
                    ref={this.setCanvas}
                    hideGrid={true}
                    canvasWidth={canvasWidth}
                    canvasHeight={canvasHeight}
                    imgSrc={this.props.src}
                    brushRadius={2}
                    brushColor="#f00"
                />
                <div className="mdc-dialog__actions">
                    <Button onClick={this.undo}>Undo</Button>
                    <Button onClick={this.clear}>Clear</Button>
                    <Button onClick={this.done}>Done</Button>
                </div>
            </>
        );
    }
}
