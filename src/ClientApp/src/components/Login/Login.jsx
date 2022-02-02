import './Login.css';
import React, { useEffect } from 'react';
import LoginForm from './LoginForm';
import { useDispatch } from 'react-redux';
import { loginConst, errorConst } from '../../_constants';
import Container from 'react-bootstrap/Container';

export default function Login() {
    const dispatch = useDispatch();

     useEffect(() => {
        dispatch({ type: errorConst.CLEAR });
        dispatch({ type: loginConst.LOGOUT });
        document.body.style='background-color: #2B3C4C;';
    }, [dispatch]);


    return (
        <Container>
            <div className="col-md-6 offset-md-4 mt-5">
            <div className="col-lg-8 offset-lg-0 border rounded bg-light">
            <div className="d-flex justify-content-center mt-3 ">
                        <img src="\images\cash-track.png" height="200px" width="200px" alt="Cash Track Logo" />
                    </div>
                    <Container className="pt-3">
                    <LoginForm />
                    </Container>
                </div>
            </div>
            <div className="col-lg-6 offset-lg-3 rounded mt-0">
            </div>
        </Container>
    );
}