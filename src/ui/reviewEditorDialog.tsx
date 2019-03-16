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




interface ReviewDialogProps {
  reviewStore?: IReviewComponentStore
}

@inject('reviewStore')
@observer
export default class ReviewDialog extends React.Component<ReviewDialogProps, any> {
  render() {
    const { closeDialog, dialog } = this.props.reviewStore!;

    const customAttribute = {
      title: dialog.currentEditLocation.isDone ? "Uncheck to reopen the task" : "Mark task as done"
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

    return (
      <Dialog open={dialog.isDialogOpen} scrimClickAction="" escapeKeyAction="" onClose={closeDialog} >
        <DialogTitle>
          {dialog.currentEditLocation.propertyName}
          <Checkbox nativeControlId='my-checkbox' {...customAttribute} checked={dialog.currentIsDone}
            onChange={(e) => dialog.currentIsDone = e.target.checked} />
          <ContextMenu icon={icons[dialog.currentPriority]} menuItems={options} />
        </DialogTitle>
        <DialogContent>
            <div>
              <strong>{dialog.currentEditLocation.firstComment.text}</strong>
            </div>
            {dialog.currentEditLocation.comments.map((comment, idx) => (
              <div key={idx}>{comment.text}</div>
            ))}
            <TextField label='Add comment...' dense textarea><Input value={dialog.currentCommentText}
              onChange={(e) => dialog.currentCommentText = e.currentTarget.value } />
            </TextField>
        </DialogContent>
        <DialogFooter>
          <DialogButton dense action='cancel'>close</DialogButton>
          <DialogButton raised dense action='save' isDefault disabled={!dialog.canSave}>Save</DialogButton>
        </DialogFooter>
      </Dialog>
    );
  }
}
