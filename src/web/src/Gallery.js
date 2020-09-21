import React, { useEffect, useState } from 'react';
import { makeStyles, Typography } from '@material-ui/core'
import Button from '@material-ui/core/Button';
import psl from 'psl';
import { Currency } from './Currency';
import { flipOnIndex } from './flipOnIndex';
import { extractHostname } from './extractHostname';
import { useHistory } from 'react-router-dom';

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
  },
  product: {
    display: 'flex',
    flexDirection: 'column',
    justifyContent: 'center',
    width: 240,
    height: 240,
  },
  productContainer: {
    padding: theme.spacing(0,2),
    display: 'flex',
    flexDirection: 'column',
  },
  brandName: {
    lineHeight: 1,
    fontSize: 14,
  },
  detail: {
    margin: theme.spacing(1, 1, 0, 1),
  },
  image: {
    border: 'solid 2px #F0F0F0',
    width: 236,
    height: 236,
    display: 'flex',
    overflow: 'hidden',
    justifyContent: 'center',
    cursor: 'pointer',
  },
  img: {
    height: '100%',
  },
  priceContainer: {
    paddingBottom: theme.spacing(1),
  },
  saveContainer: {
    borderRadius: 10,
    border: 'solid 1px #E66300',
    padding: theme.spacing(2),
    display: 'inline-block',
    textAlign: 'center',
    color: '#E66300',
    fontSize: 22,
  },
  button: {
    fontSize: '0.5rem',
    paddingTop: 6,
    marginTop: theme.spacing(2),
  },
}));

const Gallery = () => {
  const [bucket, setBucket] = useState();
  const [directory, setDirectory] = useState();
  const [products, setProducts] = useState([]); 
  const [selected, setSelected] = useState();
  const classes = useStyles();
  const history = useHistory();

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

  const handleSelect = (id, a,b ) => {
    console.log('selected: ' + id);
    console.log(a);
    console.log(b);
    setSelected(id);
  }

  return products.length ? ( 
    <div className={classes.root}>
      { products.map((p, id) => (
        <div key={id.toString()} className={flipOnIndex(id, classes.leftContainer, classes.rightContainer, id===selected, classes.selectedContainer)}>
          <div className={classes.product}>
            <div className={classes.productContainer}>
              <Typography className={classes.brandName}>
                {p.brand} - {p.name}
              </Typography>
              <div className={classes.detail}>
                <div className= {classes.priceContainer}>
                  <Currency value={p.price} strike/> <Currency value={p.salePrice} />
                </div>
                <div className={classes.saveContainer}>
                  <Typography>Save</Typography>
                  <Currency value={p.savings} />
                </div>
                <div>
                  <Button
                    className={classes.button}
                    onClick={() => history.push(p.link)}
                    href={p.link}
                    variant="contained"
                    color="default"
                    size="small"
                    disableElevation
                  >
                    {psl.parse(extractHostname(p.link)).domain}
                  </Button>
                </div>
              </div>
            </div>
          </div>
            
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
          { p.images.slice(0, 1).map((img, ix) => (
            <div className={classes.image}>
              <img
                className={classes.img}
                key={ix.toString()} 
                src={`${bucket}/${img}`}
                onClick={(a,b) => handleSelect(id, a, b)}
                alt={`${p.brand} - ${p.name}`}
              />
            </div>
          ))}
          
        </div>
        
      ))}
    </div>  
  ) : <h1>Loading...</h1>;
}

export default Gallery;


