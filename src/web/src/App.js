import React from 'react';
import Gallery from './Gallery';
import { makeStyles } from '@material-ui/core/styles';

const useStyles = makeStyles(theme => ({
    root: {
        display: 'flex',
        justifyContent: 'center',
        flexDirection: 'column',
    },
    header: {
        display: 'flex',
        justifyContent: 'center',
    },  
    menu: {
        display: 'flex',
        justifyContent: 'center',
        '& ul': {
            listStyleType: 'none',
            display: 'flex',
            margin: 0,
        },
        '& li': {
            display: 'inline-block',
            padding: theme.spacing(3, 6, 0, 6),
        },
    },
    gallery: {
        display: 'flex',
        justifyContent: 'center',
        width: '100%',
    }
}));

const App = () => {
    const classes = useStyles();
    return (
        <div className={classes.root}>
            <div className={classes.header}>
                <img width='960' src='/header.png' alt='bags on sale' />
            </div>
            <div className={classes.menu}>
                <ul>
                    <li>All</li>
                    <li>Bags</li>
                    <li>Dresses</li>
                    <li>Shoes</li>
                    <li>Coats</li>
                </ul>
            </div>
            <div className={classes.gallery}>
                <Gallery />
            </div>
        </div>
    )
}

export default App;