import React from 'react';
import Gallery from './Gallery';
import { makeStyles } from '@material-ui/core/styles';
import { useTheme } from '@material-ui/core/styles';
import useMediaQuery from '@material-ui/core/useMediaQuery';

const useStyles = makeStyles(theme => ({
    root: {
        display: 'flex',
        justifyContent: 'center',
        flexDirection: 'column',
        maxWidth: '100%',
    },
    header: {
        display: 'flex',
        justifyContent: 'center',
        alignItems: 'center',
        maxWidth:'100%',
    },  
    image: {
        maxWidth: 960,
        [theme.breakpoints.down('sm')]: {
            maxWidth: '100%',
        },
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
    const theme = useTheme();
    const isSmall = useMediaQuery(theme.breakpoints.down('sm'));
    return (
        <div className={classes.root}>
            <div className={classes.header}>
                <img className={classes.image} src='/header.png' alt='bags on sale' />
            </div>
            { !isSmall && (
                <div className={classes.menu}>
                    <ul>
                        <li>All</li>
                        <li>Bags</li>
                        <li>Dresses</li>
                        <li>Shoes</li>
                        <li>Coats</li>
                    </ul>
                </div>
            )}
            <div className={classes.gallery}>
                <Gallery />
            </div>
        </div>
    )
}

export default App;