import { loginConst } from '../../_constants';
let user = JSON.parse(localStorage.getItem('user'));
const initialState = user ? { loggedIn: false, user: "" } : {};

const login = (state = initialState, action) => {
    switch (action.type) {
        case loginConst.SET_USER:
            return {
                loggedIn: true,
                user: action.user
            };
        case loginConst.REMOVE_USER:
            return initialState;
        default:
            return state
    }
}

export default login;