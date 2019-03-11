import React from "react";

import Dialog, {
    DialogTitle,
    DialogContent,
    DialogFooter,
    DialogButton,
  } from '@material/react-dialog';
  
export default class ReviewDialog extends React.Component<any, any> {
    constructor(props) {
      super(props);
      this.state = { isOpen: props.isOpen };
    }
  
    componentDidUpdate(prevProps, prevState) {
      if (this.state.isOpen === this.props.isOpen) {
        return;
      }
      this.setState({
        isOpen: this.props.isOpen
      });
    }
  
    onDialogClose(action) {
      
    }
  
    render() {
      return (
        <Dialog open={this.state.isOpen} scrimClickAction="" escapeKeyAction=""
        onClose={this.onDialogClose.bind(this)}
        >
          <DialogTitle>My Dialog</DialogTitle>
          <DialogContent>
            <div>adasdasdas dasdas dasdas</div>
          </DialogContent>
          <DialogFooter>
            <DialogButton action='dismiss'>Dismiss</DialogButton>
            <DialogButton action='accept' isDefault>Accept</DialogButton>
          </DialogFooter>
        </Dialog>
      );
    }
  }