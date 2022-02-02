import './ExpenseTable.css';
import { expenseConst } from '../../_constants';

import React, { useState, useEffect } from 'react';
import { useSelector, useDispatch } from 'react-redux';
import { Link } from 'react-router-dom';

import ExpenseRow from './ExpenseRow';
import ExpensePagination from './ExpensePagination';

import Table from 'react-bootstrap/Table';
import Col from 'react-bootstrap/Col';
import Row from 'react-bootstrap/Row';
import Form from 'react-bootstrap/Form';

import Container from 'react-bootstrap/Container';
import InputGroup from 'react-bootstrap/InputGroup';
import FormControl from 'react-bootstrap/FormControl';
import Button from 'react-bootstrap/Button';
import axios from 'axios';
import FormSelect from 'react-bootstrap/FormSelect';

function ExpenseTable() {
    const dispatch = useDispatch();
    const expenseStore = useSelector(store => store.expenses.expenseReducer);

    useEffect(() => {
        expenseStore.listItems.length === 0 && dispatch({ type: expenseConst.FETCH_ALL });
    }, []);

    return (
        <Container fluid>
            <Col className="align-items-center justify-content-center">
            <Row>
                <Col lg={8}>
                    <InputGroup className="mb-3" size={"sm"}>
                        <Form.Select>
                            <option>Return All</option>
                            <option>Specific Date</option>
                            <option>Specific Month</option>
                        </Form.Select>
                        <FormControl value={""}
                        
                        ></FormControl>
                    </InputGroup>
                </Col>
                <Col></Col>
                    <Col><ExpensePagination expenseStore={expenseStore} /></Col>
                </Row>
                <Table bordered hover className="expenseTable" size="sm">
                    <thead>
                        <tr class="table-primary">
                            <th scope="col">
                                Purchase Date
                            </th>
                            <th scope="col">
                                Amount
                            </th>
                            <th scope="col">
                                Merchant
                            </th>
                            <th scope="col">
                                Sub Category
                            </th>
                            <th scope="col">
                                Main Category
                            </th>
                            <th scope="col">
                                Actions
                            </th>
                        </tr>
                    </thead>
                    {expenseStore.isLoading === true ? <div>Loading...</div> : expenseStore.listItems.map(expense => {
                        return (
                            <tbody key={expense.Id}>
                                <ExpenseRow
                                    expense={expense}
                                />
                            </tbody>
                        );
                    })}
                </Table>

            </Col>
        </Container>
    );
}

export default ExpenseTable;