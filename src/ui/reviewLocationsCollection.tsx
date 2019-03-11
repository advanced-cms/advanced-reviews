import React from "react";
import style from "./Styles.css";
import '@material/react-dialog/index.scss';
import ReviewEditorDialog from "./reviewEditorDialog";

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
        <ReviewEditorDialog isOpen={this.state.dialogSettings.isOpen} />
      </div>)
  };
}