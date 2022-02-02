import { put, takeLatest } from 'redux-saga/effects';
import axios from 'axios';
import { loginConst, errorConst } from '../../_constants';

function* loginUser(action){

    try {
        yield put({type: errorConst.CLEAR });
        const config = {
            headers: { 'Content-Type': 'application/json' },
            withCredentials: true,
          };
          let user = {};
        yield axios.post('/api/authenticate', action.payload, config)
        .then(response => {
                  localStorage.setItem('user', JSON.stringify(response.data));
                  user = response.data;
              });
        yield put({type: loginConst.SET_USER, user: user});
    } catch (error) {
        console.log('HEY MITCH - ERROR LOGGING IN', error);
        if (error.response.status === 401){
            console.log(error.response.data.message);
            yield put({ type: errorConst.FAILED, message: error.response.data.message });
        }
        else {
            console.log(error.response.data.message);
            yield put({ type: errorConst.FAILED, message: error.response.data.message });
        }
    }
}

function* logoutUser(){
    try {
        localStorage.removeItem('user');
        yield put({type: loginConst.REMOVE_USER});
    } catch (error) {
        console.log('HEY MITCH - COULD NOT LOG OUT USER', error);
    }
}

function* loginSaga(){
    yield takeLatest(loginConst.LOGIN, loginUser);
    yield takeLatest(loginConst.LOGOUT, logoutUser);
}

export default loginSaga;