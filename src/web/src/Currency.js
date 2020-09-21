import { makeStyles } from '@material-ui/core';
import React from 'react';
const useStyles = makeStyles(theme => ({
  strikethrough: {
    display: 'inline-block',
    position: 'relative',
    '&:before': {
      content: "''",
      borderBottom: '2px solid #E66300',
      width: '100%',
      position: 'absolute',
      right: 0,
      top: '50%',
    },
  },
}));
export const Currency = ({ value, strike }) => {
  const classes = useStyles();
  if (strike) {
    return (<div className={classes.strikethrough}>
        ${value.toFixed(2)}
      </div>);
  } else {
    return (
      <span>${value.toFixed(2)}</span>
    );
  }
};
