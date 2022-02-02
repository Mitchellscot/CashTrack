import { combineReducers } from 'redux';
import { expenseConst } from '../../_constants';

const initialState = { isLoading: true, totalAmount: 0, pageNumber: 1, pageSize: 25, totalCount: 0, totalPages: 0, listItems: [] };
const expenseReducer = (state = initialState, action) => {
    switch (action.type) {
        case expenseConst.SET_EXPENSES:
            console.log(action.payload);
            return {
                isLoading: false,
                totalAmount: action.payload.totalAmount,
                pageNumber: action.payload.pageNumber,
                pageSize: action.payload.pageSize,
                totalCount: action.payload.totalCount,
                totalPages: action.payload.totalPages,
                listItems: action.payload.listItems,
            };
        case expenseConst.CLEAR_EXPENSES:
            return initialState;
        default:
            return state;
    }
};

export default combineReducers({
    expenseReducer,
});