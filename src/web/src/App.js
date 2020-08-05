import React, { useEffect, useState } from 'react';
import './App.css';

function App() {
  const root = "remote";
  const [products, setProducts] = useState([]); 

  useEffect(() => {
    async function fetchData() {
      var response = await fetch(`${root}/json/index.json`);
      const page1 = await response.json();
      response = await fetch(`${root}/json/${page1.nextPage}.json`);
      const page2 = await response.json();
      setProducts([...page1.products, ...page2.products].slice(0, 12))
    }
    fetchData();
  }, [])

  return (
    <div className="App" style={{padding: 15}}>
      { products.map(p => (
        <div>
          <h1>{p.brand} - {p.name}</h1>
          { p.images.map(img => (
              <img style={{width: 100}} src={`/${img}`} alt={`${p.brand} - ${p.name}`} />
          ))}
        </div>
      ))}
    </div>
  );
}

export default App;
