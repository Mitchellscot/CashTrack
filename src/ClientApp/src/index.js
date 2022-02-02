import 'bootstrap/dist/css/bootstrap.css';
import React from 'react';
import ReactDOM from 'react-dom';
import { BrowserRouter } from 'react-router-dom';
import App from './components/App/App';
import { createStore, applyMiddleware, compose  } from 'redux';
import { Provider } from 'react-redux';
import createSagaMiddleware from 'redux-saga';
import rootReducer from './redux/reducers/_rootReducer';
import rootSaga from './redux/sagas/_rootSaga';
import logger from 'redux-logger';

const baseUrl = document.getElementsByTagName('base')[0].getAttribute('href');
const rootElement = document.getElementById('root');

const sagaMiddleware = createSagaMiddleware();
const composeEnhancers = window.__REDUX_DEVTOOLS_EXTENSION_COMPOSE__ || compose;
const middlewareList = process.env.NODE_ENV === 'development' ?
  [sagaMiddleware,  logger ] :
  [sagaMiddleware ];

const store = createStore(
    rootReducer,
    composeEnhancers(applyMiddleware(...middlewareList)),
);

sagaMiddleware.run(rootSaga);

ReactDOM.render(

    <BrowserRouter basename={baseUrl}>
        <Provider store={store}>
          <App />
        </Provider>
    </BrowserRouter>
    ,
    rootElement);