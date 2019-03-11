import React from "react";
import style from "./Styles.css";
import '@material/react-dialog/index.scss';
import Dialog, {
  DialogTitle,
  DialogContent,
  DialogFooter,
  DialogButton,
} from '@material/react-dialog';

class ReviewDialog extends React.Component<any, any> {
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
      <Dialog open={this.state.isOpen}
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

export default class Index extends React.Component<any, any> {
  constructor(props) {
    super(props);

    this.state = {
      dialogSettings: {
        isOpen: false,
      }
    }
  }

  onDialogSave(action: String) {
    alert(action);
    this.setState({action, isOpen: false});
  }

  onLocationClick() {
    this.setState({
      "dialogSettings": {
        isOpen: true
      }
    });
  };

  render() {
    return (
      <div>
        <div style={{
          top: "100px",
          left: "50px"
        }} className={style.reviewLocation} onClick={this.onLocationClick.bind(this)}>32</div>
        <ReviewDialog isOpen={this.state.dialogSettings.isOpen} />
      </div>)
  };
}