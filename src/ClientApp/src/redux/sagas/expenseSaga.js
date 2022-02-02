import { put, takeLatest } from 'redux-saga/effects';
import axios from 'axios';
import { expenseConst } from '../../_constants';

function* fetchAllExpenses() {
    try {
        const expenseResponse = yield axios.get('/api/expense?dateoptions=1&pageSize=20');
        yield put({type: expenseConst.SET_EXPENSES, payload: {
            totalAmount: expenseResponse.data.totalAmount,
            pageNumber: expenseResponse.data.pageNumber,
            pageSize: expenseResponse.data.pageSize,
            totalCount: expenseResponse.data.totalCount,
            totalPages: expenseResponse.data.totalPages,
            listItems: expenseResponse.data.listItems
        }})
    } catch (error) {
        console.log(`HEY MITCH - COULDN'T GET THE EXPENSES ${error}`);
    }
}
function* searchExpenses(action) {
    try {
        const pageNumber = action.payload.pageNumber
        const urlPath = `/api/expense?dateoptions=1&pageSize=20&pageNumber=${pageNumber}`;
        console.log(urlPath);
        const response = yield axios.get(urlPath);
        yield put({type: expenseConst.SET_EXPENSES, payload: {
            totalAmount: response.data.totalAmount,
            pageNumber: response.data.pageNumber,
            pageSize: response.data.pageSize,
            totalCount: response.data.totalCount,
            totalPages: response.data.totalPages,
            listItems: response.data.listItems
        }})
    } catch (error) {
        console.log(`HEY MITCH - COULDN'T GET THE EXPENSES ${error}`);
    }
}

function* expenseSaga() {
    yield takeLatest(expenseConst.FETCH_ALL, fetchAllExpenses);
    yield takeLatest(expenseConst.SEARCH_RESULTS, searchExpenses);
  }

export default expenseSaga;