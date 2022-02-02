import { combineReducers } from 'redux';
import login from './loginReducer';
import errors from './errorsReducer';
import expenses from './expenseReducer';

const rootReducer = combineReducers({
    login,
    errors,
    expenses
});

export default rootReducer;