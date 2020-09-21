import React, { useEffect, useState } from 'react';
import { makeStyles } from '@material-ui/core'
import ProductDetail from './ProductDetail';
import ProductImage from './ProductImage';
import { flipOnIndex } from './flipOnIndex';
import Placeholder from './Placeholder';

const useStyles = makeStyles((theme) => ({
  root: {
    display: 'flex',
    flexWrap: 'wrap',
    justifyContent: 'center',
    maxWidth: 960,
  },
  leftContainer: {
    display: 'flex',
    flexDirection: 'row-reverse',
    margin: theme.spacing(3, 0),
  },
  selectedContainer: {
    display: 'flex',
    flexDirection: 'row',
    margin: theme.spacing(3, 0),
    maxWidth: 960,
    textAlign: 'right',
    flexFlow: 'wrap',
  },
  rightContainer: {
    display: 'flex',
    textAlign: 'right',
    margin: theme.spacing(3, 0),
  }
}));

const Gallery = () => {
  const [bucket, setBucket] = useState();
  const [directory, setDirectory] = useState();
  const [products, setProducts] = useState([]); 
  const [selected, setSelected] = useState();
  const classes = useStyles();

  useEffect(() => {
    async function fetchData() {
      var response = await fetch('config.json');
      const json = await response.json();
      setBucket(json.bucket);
      setDirectory(json.directory);
    }
    fetchData();
  }, [])

  useEffect(() => {
    async function fetchData() {
      const index = `${bucket}/${directory}/json/index.json`;
      var response = await fetch(index);
      const page1 = await response.json();
      response = await fetch(`${bucket}/${directory}/json/${page1.nextPage}.json`);
      const page2 = await response.json();
      setProducts([...page1.products, ...page2.products].slice(0, 12))
    }
    if (directory) {
      fetchData();
    }
  }, [bucket, directory])

  const handleSelect = (ix) => {
    if (ix === selected) ix = undefined;
    setSelected(ix);
  }
  
  let tiles = 0;

  return products.length ? ( 
    <div className={classes.root}>
      { products.map((p, productId) => { 
        const rightAlign = flipOnIndex(tiles, window.innerWidth >= 960 ? 4 : 2);
        tiles++;
        return (
        <>
          { rightAlign && (
            <ProductDetail key={productId.toString()} product={p} rightAlign={rightAlign} />
          )}
          { p.images.slice(0, 1).map((image) => {
            tiles++;
            return (
              <ProductImage key={`${productId}-first`} product={p} image={image} bucket={bucket} handleSelect={() => handleSelect(productId)} />
              )})}
          { selected === productId && p.images.slice(1, p.images.length).map((image, ix) => { 
            tiles++;
            return (
            <ProductImage key={`${productId}-${ix}`} product={p} image={image} bucket={bucket} handleSelect={() => handleSelect(productId)} />
          )})}
          { selected === productId && p.images.length % 2 === 0 && (
            <Placeholder />
          )}
          { !rightAlign && (
            <ProductDetail key={productId.toString()} product={p} rightAlign={rightAlign} />
          )}
          
        </>
        
      )})}
    </div>  
  ) : <h1>Loading...</h1>;
}

export default Gallery;


