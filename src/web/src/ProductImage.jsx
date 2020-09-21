import React from 'react';
import { makeStyles } from '@material-ui/core'

const useStyles = makeStyles((theme) => ({
  container: {
    border: 'solid 2px #F0F0F0',
    width: 236,
    height: 236,
    display: 'flex',
    overflow: 'hidden',
    justifyContent: 'center',
    cursor: 'pointer',
    marginTop: theme.spacing(3),
  },
  image: {
    height: '100%',
  },
  
}));

const ProductImage = ({product, bucket, image, handleSelect}) => {
  const classes = useStyles();
  return (
    <div className={classes.container}>
      <img
        className={classes.image}
        src={`${bucket}/${image}`}
        onClick={handleSelect}
        alt={`${product.brand} - ${product.name}`}
      />
    </div>
  )
};

export default ProductImage;