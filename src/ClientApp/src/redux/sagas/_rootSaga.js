import { all } from 'redux-saga/effects';
import loginSaga from './loginSaga';
import expenseSaga from './expenseSaga';

export default function* rootSaga() {
    yield all([
        loginSaga(),
        expenseSaga()
    ]);
}