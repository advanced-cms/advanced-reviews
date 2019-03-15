import React from "react";
import { observer, inject } from 'mobx-react';
import { IReviewComponentStore, Priority } from './reviewStore';
import Dialog, {
  DialogTitle,
  DialogContent,
  DialogFooter,
  DialogButton,
} from '@material/react-dialog';
import Checkbox from '@material/react-checkbox';
import Select from '@material/react-select';
import TextField, { Input } from '@material/react-text-field';
import '@material/react-dialog/index.scss';
import '@material/react-text-field/index.scss';
import '@material/react-checkbox/index.scss';
import '@material/react-button/index.scss';
import '@material/react-select/index.scss';

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

    const options = Object.keys(Priority).map(priority => {
      return {
        value: priority,
        label: Priority[priority]
      };
    });

    return (
      <Dialog open={dialog.isDialogOpen} scrimClickAction="" escapeKeyAction="" onClose={closeDialog} >
        <DialogTitle>
          {dialog.currentEditLocation.propertyName}
          <Checkbox nativeControlId='my-checkbox' {...customAttribute} checked={dialog.currentIsDone}
            onChange={(e) => dialog.currentIsDone = e.target.checked} />
        </DialogTitle>
        <DialogContent>
            <Select
                value={Priority[dialog.currentPriority]}
                label='Priority'
                onChange={(e) => dialog.currentPriority = Priority[e.currentTarget.value] }
                options={options} />
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
