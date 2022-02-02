import { loginConst, errorConst } from '../../_constants';
import React, { useState } from 'react';
import { useSelector, useDispatch } from 'react-redux';
import Button from 'react-bootstrap/Button';
import Form from 'react-bootstrap/Form';

function LoginForm() {
    const errors = useSelector(store => store.errors)
    const dispatch = useDispatch();
    const [inputs, setInputs] = useState({
        name: '',
        password: ''
    });
    const { name, password } = inputs;

    const handleChange = (e) => {
        const { name, value } = e.target;
        setInputs(inputs => ({ ...inputs, [name]: value }));
    }

    const handleSubmit = (e) => {
        e.preventDefault();
        if (name && password) {
            dispatch({
                type: loginConst.LOGIN, payload: {
                    name: inputs.name,
                    password: inputs.password }
            });
        }
        else {
            dispatch({ type: errorConst.INPUT });
        }
    }

    return (
        <Form onSubmit={handleSubmit}>
            {<div className={`alert ${errors.type} text-center py-0 mx-3`} role="alert">{errors.message}</div>}
            <Form.Group>
                <Form.Control
                    className="my-3 text-center"
                    placeholder="Name"
                    type="text"
                    name="name"
                    required
                    value={inputs.name}
                    onChange={handleChange}
                >
                </Form.Control>
                <Form.Control
                    className="my-3 text-center"
                    placeholder="Password"
                    type="password"
                    name="password"
                    required
                    value={inputs.password}
                    onChange={handleChange}
                ></Form.Control>
            </Form.Group>

            <div className="d-flex justify-content-center mb-3">
                <Button variant="info" type="submit" name="submit">Log In</Button>
            </div>

            {/*             <div className="form-row d-flex">
                <div className="col mx-2">
                    <input placeholder="Name" className="form-control" id="nameInput" required type="text" name="name" value={inputs.name} onChange={handleChange} /></div>
                {submitted && !name && <div className="invalid-feedback">Name is required</div>}
                <div className="col mx-2">
                    <input placeholder="password" className="form-control" id="passwordInput" required type="text" name="password" value={inputs.password} onChange={handleChange} />
                    {submitted && !password && <div className="invalid-feedback">Password is required</div>}
                </div>
            </div>
            <div className="form-group d-flex justify-content-between m-2">
                <button className="btn btn-primary btn" type="submit" name="submit">
                    {loggingIn && <span className="spinner-border spinner-border-sm mr-1"></span>}
                        Login
                    </button>
            </div> */}
        </Form>
    );
}

export default LoginForm;