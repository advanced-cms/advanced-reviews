import React from "react";
import { observer, inject } from 'mobx-react';
import { IReviewComponentStore } from './reviewStore';
import Dialog, {
  DialogTitle,
  DialogContent,
  DialogFooter,
  DialogButton,
} from '@material/react-dialog';
import Checkbox from '@material/react-checkbox';
import TextField, { Input } from '@material/react-text-field';
import '@material/react-dialog/index.scss';
import '@material/react-text-field/index.scss';
import '@material/react-checkbox/index.scss';
import '@material/react-button/index.scss';

interface ReviewDialogProps {
  reviewStore?: IReviewComponentStore
}

@inject('reviewStore')
@observer
export default class ReviewDialog extends React.Component<ReviewDialogProps, any> {
  render() {
    const { closeDialog, isDialogOpen, currentEditLocation, dialog } = this.props.reviewStore!;

    const customAttribute = {
      title: currentEditLocation.isDone ? "Uncheck to reopen the task" : "Mark task as done"
    };

    return (
      <Dialog open={isDialogOpen} scrimClickAction="" escapeKeyAction="" onClose={closeDialog} >
        <DialogTitle>
          {currentEditLocation.propertyName}
          <Checkbox nativeControlId='my-checkbox' {...customAttribute} checked={currentEditLocation.isDone}
            onChange={(e) => currentEditLocation.isDone = e.target.checked} />
        </DialogTitle>
        <DialogContent>
            {currentEditLocation.comments.map((comment, idx) => (
              <div key={idx}>{comment.text}</div>
            ))}
            <TextField label='Add comment...' dense textarea><Input value={dialog.currentComment}
              onChange={(e) => dialog.currentComment = e.currentTarget.value } />
            </TextField>
        </DialogContent>
        <DialogFooter>
          <DialogButton dense action='cancel'>close</DialogButton>
          <DialogButton raised dense action='save' isDefault>Save</DialogButton>
        </DialogFooter>
      </Dialog>
    );
  }
}