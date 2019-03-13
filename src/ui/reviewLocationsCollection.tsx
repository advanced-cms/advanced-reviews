import React from "react";
import { observer, inject } from 'mobx-react';
import style from "./Styles.css";
import { IReviewComponentStore } from './reviewStore';
import ReviewEditorDialog from "./reviewEditorDialog";

interface ReviewLocationCollectionProps {
  reviewStore?: IReviewComponentStore
}

@inject('reviewStore')
@observer
export default class ReviewLocationCollection extends React.Component<ReviewLocationCollectionProps, any> {
  render() {
    const {reviewLocations, showDialog} = this.props.reviewStore!;

    return (
      <div>
        {reviewLocations.map(location => (
          <div key={location.id} 
          style={location.style} 
          className={style.reviewLocation} 
          onClick={() => showDialog(location)}>{location.id}</div>
        ))}

        <ReviewEditorDialog />
      </div>)
  };
}