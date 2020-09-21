import React from 'react';
import { makeStyles } from '@material-ui/core/styles';
import TwitterIcon from '@material-ui/icons/Twitter';
import Typography from '@material-ui/core/Typography';
import Button from '@material-ui/core/Button';

const useStyles = makeStyles(theme => ({
  root: {
    width: 240,
    height: 240,
    marginTop: theme.spacing(3),
    display: 'flex',
    justifyContent: 'center',
    alignItems: 'center',
    [theme.breakpoints.down('sm')]: {
      width: '50vw',
      height: '50vw',
    },
  },
  button: {
    color: 'gray',
  },
  buttonText: {
    marginLeft: theme.spacing(1),
  }
}));

const Placeholder = () => {
  const classes = useStyles();
  return (
    <div className={classes.root}>
      <Button
        className={classes.button}
        color='default'
        variant='contained'
        href='https://twitter.com/bagsonsale'
        disableElevation
        size='small'
      >
        <TwitterIcon />
        <Typography className={classes.buttonText}>
          bagsonsale
        </Typography>
      </Button>
    </div>
  )
};

export default Placeholder;
