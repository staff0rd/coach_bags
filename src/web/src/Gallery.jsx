import React, { useEffect, useState } from 'react';
import { makeStyles } from '@material-ui/core'
import ProductDetail from './ProductDetail';
import ProductImage from './ProductImage';
import { flipOnIndex } from './flipOnIndex';

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

  return products.length ? ( 
    <div className={classes.root}>
      { products.map((p, id) => (
        <div key={id.toString()} className={flipOnIndex(id, classes.leftContainer, classes.rightContainer, id===selected, classes.selectedContainer)}>
          <ProductDetail product={p} />
            
          { selected === id && p.images.slice(1).map((img, ix) => (
            <div className={classes.image}>
              <img
                className={classes.img}
                key={ix.toString()} 
                src={`${bucket}/${img}`}
                alt={`${p.brand} - ${p.name}`}
              />
            </div>
          ))}
          { p.images.slice(0, 1).map((image, ix) => (
            <ProductImage key={ix.toString()} product={p} image={image} bucket={bucket} />
          ))}
          
        </div>
        
      ))}
    </div>  
  ) : <h1>Loading...</h1>;
}

export default Gallery;


