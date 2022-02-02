import './ExpenseRow.css';
import React, { useState } from 'react';
import { useDispatch } from 'react-redux';
import axios from 'axios';
import Modal from 'react-bootstrap/Modal';

function ExpenseRow({ expense }) {
    const [modal, setModal] = useState(false);
    const handleShowModal = () => {
        setModal(!modal);
    }

    const formatDate = (orderDate) => {
        const date = new Date(orderDate);
        const options = { month: "numeric", day: "numeric", year: "numeric" }
        const fd = new Intl.DateTimeFormat('en-us', options).format(date);
        return fd.toString();
    }

    return (
        <tr scope="row">
            <td className="align-middle text-center">
            {expense.date !== undefined ? formatDate(expense.date) : <span>loading...</span>}
            </td>
            <td className="align-middle text-center">
            {expense.amount}
            </td>
            <td className="align-middle text-center">
            {expense.merchant}
            </td>
            <td className="align-middle text-center">
            {expense.subCategory}
            </td>
            <td className="align-middle text-center">
            {expense.mainCategory}
            </td>
            <td>actions go here</td>
        </tr>
    );
}

export default ExpenseRow;