import React from "react";
import TextField, { Input } from "@material/react-text-field";
import IconButton from "@material/react-icon-button";
import MaterialIcon from "@material/react-material-icon";
import { DropDownMenu } from "../common/drop-down-menu";
import { inject } from "mobx-react";

interface LocationCommentProps {
    currentScreenshot: string;
    resources?: ReviewResources;
    onToggle: () => void;
    onChange: (comment: string, screenshot: string) => void;
}

interface LocationCommentState {
    currentCommentText: string;
}

@inject("resources")
export default class LocationComment extends React.Component<LocationCommentProps, LocationCommentState> {
    commentInput: any;

    constructor(props: LocationCommentProps) {
        super(props);
        this.state = {
            currentCommentText: ""
        };
    }

    componentDidMount(): void {
        setTimeout(() => {
            if (this.commentInput) {
                this.commentInput.inputElement.focus();
            }
        });
    }

    onTextChange(comment: string): void {
        this.setState({ currentCommentText: comment }, () => {
            this.props.onChange(comment, this.props.currentScreenshot);
        });
    }

    onRemoveScreenshot(): void {
        this.props.onChange(this.state.currentCommentText, null);
    }

    render() {
        const res = this.props.resources!;

        return (
            <>
                <TextField label={`${res.dialog.addcomment}...`} dense textarea>
                    <Input
                        ref={(input: any) => (this.commentInput = input)}
                        value={this.state.currentCommentText}
                        onChange={e => this.onTextChange(e.currentTarget.value)}
                    />
                </TextField>
                {!this.props.currentScreenshot && (
                    <IconButton title="Attach screenshot" onClick={() => this.props.onToggle()}>
                        <MaterialIcon icon="image" />
                    </IconButton>
                )}
                {this.props.currentScreenshot && (
                    <>
                        <DropDownMenu icon="image">
                            <img src={this.props.currentScreenshot} />
                        </DropDownMenu>
                        <IconButton onClick={() => this.onRemoveScreenshot()} title="Remove screenshot">
                            <MaterialIcon icon="remove" />
                        </IconButton>
                    </>
                )}
            </>
        );
    }
}
