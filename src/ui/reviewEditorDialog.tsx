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

interface ReviewDialogProps {
  reviewStore?: IReviewComponentStore
}

@inject('reviewStore')
@observer
export default class ReviewDialog extends React.Component<ReviewDialogProps, any> {

  constructor(props: ReviewDialogProps) {
    super(props);
    this.state = {value: 'Woof'};
  }

  

  render() {
    const { closeDialog, isDialogOpen, currentEditLocation } = this.props.reviewStore!;

    return (
      <Dialog open={isDialogOpen} scrimClickAction="" escapeKeyAction="" onClose={closeDialog} >
        <DialogTitle>
          {currentEditLocation.propertyName}
          <Checkbox nativeControlId='my-checkbox' 
            title={currentEditLocation.isDone ? "Uncheck to reopen the task": "Mark task as done"} 
            checked={currentEditLocation.isDone} onChange={(e) => currentEditLocation.isDone = e.target.checked }
          />
        </DialogTitle>
        <DialogContent>
          <div>
            <TextField
              label='Comment...'
              textarea={true}
              //onTrailingIconSelect={() => this.setState({ value: '' })}
              //trailingIcon={<MaterialIcon role="button" icon="delete" />}
            ><Input
                value={this.state.value}
                onChange={(e) => this.setState({ value: e.currentTarget.value })} />
            </TextField>

          </div>
        </DialogContent>
        <DialogFooter>
          <DialogButton action='dismiss'>Dismiss</DialogButton>
          <DialogButton action='accept' isDefault>Accept</DialogButton>
        </DialogFooter>
      </Dialog>
    );
  }
}