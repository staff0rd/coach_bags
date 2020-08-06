import React, { useEffect, useState } from 'react';
import { makeStyles } from '@material-ui/core'
import ReactFullpage from '@fullpage/react-fullpage';
import psl from 'psl';
import { Currency } from './Currency';
import { extractHostname } from './extractHostname';
import { config } from './config';

const useStyles = makeStyles(() => ({
  product: {
    padding: 15,
  },
  name: {
    margin: 0,
  },
  detail: {
    marginTop: 0,
  },
  images: {
    overflowX: 'scroll',
  },
  image: {
    height: '70vh',
  },
  button: {
    display: 'block',
  },
}));

const App = () => {
  const [bucket, setBucket] = useState();
  const [directory, setDirectory] = useState();
  const [products, setProducts] = useState([]); 
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
    <ReactFullpage
      licenseKey = {config().fullPage}
      scrollingSpeed = {1000}
      render={({ state, fullpageApi }) => {
        return (
          <ReactFullpage.Wrapper>
            { products.map((p, ix) => (
              <div key={ix.toString()} className="section">
                <div className={classes.product}>
                  <h1 className={classes.name}>{p.brand} - {p.name}</h1>
                  <p className={classes.detail}>
                    Was <Currency value={p.price} />, now <Currency value={p.salePrice} />, save <Currency value={p.savings} />.&nbsp;
                    <a href={p.link}>{psl.parse(extractHostname(p.link)).domain}</a>
                    <button className={classes.button} onClick={() => fullpageApi.moveSectionDown()}>
                      Next
                    </button>
                  </p>
                </div>
                <div className={classes.images}>
                  <div style={{display: 'inline-flex'}} className="images">
                    { p.images.map((img, ix) => (
                      <img className={classes.image} key={ix.toString()} src={`${bucket}/${img}`} alt={`${p.brand} - ${p.name}`} />
                    ))}
                  </div>
                </div>
              </div>
            ))}
          </ReactFullpage.Wrapper>
        );
      }}
    />
  ) : <h1>Loading...</h1>;
}

export default App;
