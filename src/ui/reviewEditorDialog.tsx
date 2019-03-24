import React from "react";
import { observer, inject } from 'mobx-react';
import { IReviewComponentStore, Priority } from './reviewStore';

import { ContextMenu } from "./context-menu";

import Dialog, {
  DialogTitle,
  DialogContent,
  DialogFooter,
  DialogButton,
} from '@material/react-dialog';
import Checkbox from '@material/react-checkbox';
import TextField, { Input } from '@material/react-text-field';

import '@material/react-material-icon/index.scss';
import '@material/react-dialog/index.scss';
import '@material/react-text-field/index.scss';
import '@material/react-checkbox/index.scss';
import '@material/react-button/index.scss';
import "@material/react-list/index.scss";
import "@material/react-menu-surface/index.scss";
import '@material/react-icon-button/index.scss';
import "./reviewEditorDialog.scss"
import ScreenshotPicker from "./screenshotPicker";
import {DropDownMenu} from "./drop-down-menu";



interface ReviewDialogProps {
  reviewStore?: IReviewComponentStore
}

@inject('reviewStore')
@observer
export default class ReviewDialog extends React.Component<ReviewDialogProps, any> {
  render() {
    const { closeDialog, dialog } = this.props.reviewStore!;

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
      <Dialog className="review-dialog" open={dialog.isDialogOpen} scrimClickAction="" escapeKeyAction="" onClose={closeDialog} >
        <DialogTitle>
            {!dialog.isScreenshotMode && (
            <>
                {dialog.currentEditLocation.propertyName}
                <Checkbox nativeControlId='my-checkbox' {...customAttribute} checked={dialog.currentIsDone}
                    onChange={(e) => dialog.currentIsDone = e.target.checked} />
                <ContextMenu icon={icons[dialog.currentPriority]} title={dialog.currentPriority} menuItems={options} />
            </>
            )}
            {dialog.isScreenshotMode && (
                <>Crop the area you want to comment:</>
            )}
        </DialogTitle>
        <DialogContent>
            {!dialog.isScreenshotMode && (
                <>
                    <div>
                        <strong>{dialog.currentEditLocation.firstComment.text}</strong>
                        {dialog.currentEditLocation.firstComment.screenshot && <DropDownMenu icon="image">
                            <img src={dialog.currentEditLocation.firstComment.screenshot} />
                        </DropDownMenu>}
                    </div>
                    {dialog.currentEditLocation.comments.map((comment, idx) => (
                        <div className="comment" key={idx}>
                            <div>
                                <span className="author">{comment.author}</span>
                                <span className="date" title={comment.formattedDate}>{comment.userFriendlyDate}</span>
                                {comment.screenshot && <DropDownMenu icon="image">
                                    <img src={comment.screenshot} />
                                </DropDownMenu>}
                            </div>
                            <p>{comment.text}</p>
                        </div>
                    ))}
                    <TextField label='Add comment...' dense textarea><Input value={dialog.currentCommentText}
                                                                            onChange={(e) => dialog.currentCommentText = e.currentTarget.value} />
                    </TextField>
            </>
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
                  <>
                      <DialogButton dense action='cancel'>close</DialogButton>
                      <DialogButton raised dense action='save' isDefault disabled={!dialog.canSave}>Save</DialogButton>
                  </>
              )}
          </DialogFooter>
      </Dialog>
    );
  }
}
