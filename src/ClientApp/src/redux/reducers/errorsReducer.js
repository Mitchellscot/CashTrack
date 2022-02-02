import { errorConst } from '../../_constants';
const initialState = {type: 'none', message: ''}

const loginMessage = (state = initialState, action) => {
    switch (action.type) {
      case errorConst.CLEAR:
        return state;
      case errorConst.FAILED:
        return {
          type: 'alert-danger',
          message: action.message
        }
      default:
        return state;
    }
  };
  
  export default loginMessage;