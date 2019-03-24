import React from "react";
import { reaction, IReactionDisposer } from "mobx";
import { observer, inject } from 'mobx-react';
import { IReviewComponentStore, Priority } from './../reviewStore';

import { ContextMenu } from "./../context-menu";

import Dialog, {
  DialogTitle,
  DialogContent,
  DialogFooter,
  DialogButton,
} from '@material/react-dialog';
import { Cell, Grid, Row } from '@material/react-layout-grid';
import Checkbox from '@material/react-checkbox';
import TextField, { Input } from '@material/react-text-field';
import PageNavigator from "./pageNavigator";
import Comment from "./comment";

import '@material/react-button/index.scss';
import '@material/react-checkbox/index.scss';
import '@material/react-dialog/index.scss';
import '@material/react-layout-grid/index.scss';
import "@material/react-list/index.scss";
import "@material/react-menu-surface/index.scss";
import '@material/react-text-field/index.scss';
import "./reviewEditorDialog.scss"
import ScreenshotPicker from "./../screenshotPicker";
import { DropDownMenu } from "./../drop-down-menu";

interface ReviewDialogProps {
  reviewStore?: IReviewComponentStore,
  isDialogOpen: boolean,
  onPrevClick(): void,
  onNextClick(): void,
  onCloseDialog(action: string): void
}

@inject('reviewStore')
@observer
export default class ReviewDialog extends React.Component<ReviewDialogProps, any> {

  commentsChangedReaction: IReactionDisposer;

  constructor(props: ReviewDialogProps) {
    super(props);
    this.commentsChangedReaction = reaction(() => {
      //uncomment the below code to see the trigger message
      //return state.messages.map((a) => a)
      return this.props.reviewStore!.dialog.currentEditLocation.comments.slice();
    }, () => {
      this.scrollToBottom();
    });
  }

  componentWillUnmount() {
    this.commentsChangedReaction();
  }

  static defaultProps = {
    isDialogOpen: false
  }

  messagesEnd: HTMLDivElement;

  scrollToBottom = () => {
    setTimeout(() => {
      this.messagesEnd.scrollIntoView({ behavior: "smooth" });
    }, 0);
  }

  onDialogOpen(): void {
    this.scrollToBottom();
  }

  render() {
    const { dialog } = this.props.reviewStore!;

    const customAttribute = {
      title: dialog.currentIsDone ? "Uncheck to reopen the task" : "Mark task as done"
    };

    const icons = {};
    icons[Priority.Important] = "error_outline";
    icons[Priority.Normal] = "assignment";
    icons[Priority.Trivial] = "low_priority";

    const options = Object.keys(Priority).map(priority => {
      return {
        name: priority,
        icon: icons[priority],
        onSelected: () => {
          dialog.currentPriority = Priority[priority]
        }
      };
    });

    //TODO: get iframe from props?
    const iframe: HTMLIFrameElement = document.getElementById("iframe") as HTMLIFrameElement;

    return (
      <Dialog className="review-dialog"
        open={this.props.isDialogOpen}
        scrimClickAction=""
        escapeKeyAction=""
        onOpen={() => this.onDialogOpen()}
        onClose={this.props.onCloseDialog} >
        <DialogTitle>{!dialog.isScreenshotMode && (
          <>
            {dialog.currentEditLocation.propertyName}
          </>
        )}
          {dialog.isScreenshotMode && (
            <>Crop the area you want to comment:</>
          )}</DialogTitle>
        <DialogContent>
          {!dialog.isScreenshotMode && (
            <Grid className="dialog-grid">
              <Row>
                <Cell columns={8} className="review-actions left-align">
                  <PageNavigator reviewLocation={dialog.currentEditLocation} onPrevClick={this.props.onPrevClick} onNextClick={this.props.onNextClick} />
                </Cell>
                <Cell columns={4} className="review-actions">
                  <Checkbox nativeControlId='my-checkbox' {...customAttribute} checked={dialog.currentIsDone}
                    onChange={(e) => dialog.currentIsDone = e.target.checked} />
                  <ContextMenu icon={icons[dialog.currentPriority]} title={dialog.currentPriority} menuItems={options} />
                </Cell>
              </Row>
              <Row>
                <Cell columns={12}>
                  <strong>{dialog.currentEditLocation.firstComment.text}</strong>
                  {dialog.currentEditLocation.firstComment.screenshot && <DropDownMenu icon="image">
                    <img src={dialog.currentEditLocation.firstComment.screenshot} />
                  </DropDownMenu>}</Cell>
              </Row>
              <Row>
                <Cell columns={12} className="comments-list">
                  {dialog.currentEditLocation.comments.map((comment, idx) =>
                    <Comment key={idx} comment={comment} />
                  )}
                  <div style={{ float: "left", clear: "both" }} ref={(el) => { this.messagesEnd = el; }}>
                  </div>
                </Cell>
              </Row>
              <Row>
                <Cell columns={12}>
                  <TextField label='Add comment...' dense textarea><Input value={dialog.currentCommentText}
                    onChange={(e) => dialog.currentCommentText = e.currentTarget.value} />
                  </TextField>
                </Cell>
              </Row>
            </Grid>
          )}
                  <ScreenshotPicker
                    current={dialog.currentScreenshot}
                    iframe={iframe}
                    onImageSelected={(output) => dialog.currentScreenshot = output}
                    toggle={() => dialog.isScreenshotMode = !dialog.isScreenshotMode}
                  />
        </DialogContent>
        <DialogFooter>
          {!dialog.isScreenshotMode && (
            <><DialogButton dense action='cancel'>close</DialogButton>
              <DialogButton raised dense action='save' isDefault disabled={!dialog.canSave}>Save</DialogButton></>
          )}
        </DialogFooter>
      </Dialog>
    );
  }
}
