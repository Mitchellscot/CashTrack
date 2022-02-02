import React from 'react';
import { Link } from 'react-router-dom';
import { ChevronDoubleLeft, ChevronLeft, ChevronRight, ChevronDoubleRight } from 'react-bootstrap-icons';
import { expenseConst } from '../../_constants';
import { useSelector, useDispatch } from 'react-redux';

export default function ExpensePagination({ expenseStore: store }) {
    const dispatch = useDispatch();
    const params = new URLSearchParams(document.location.search);
    const page = parseInt(params.get('pageNumber'));
    const pageSize = parseInt(params.get('pageSize'));
    const dateOptions = parseInt(params.get(`dateOptions`));
    const query = parseInt(params.get(`query`));
    console.log(dateOptions);
    console.log(page);
    console.log(query);
    console.log(pageSize);



    const handlePageChange = (setPageNumber) => {
        return dispatch({ type: expenseConst.SEARCH_RESULTS, 
            payload: {
            pageNumber: setPageNumber
        }});
    };

    return (
        <div>
            {store &&
                <ul className="pagination pagination-sm">
                    <li onClick={() => handlePageChange(1)}
                        className={`page-item ${store.pageNumber === 1 ? `disabled` : ``}`}
                    >
                        <Link to={{ search: `?pageNumber=1` }} className="page-link"><ChevronDoubleLeft /></Link>
                    </li>
                    {store.pageNumber > 2 &&
                        <li onClick={() => handlePageChange(store.pageNumber - 2)}>
                            <Link to={{ search: `?pageNumber=${store.pageNumber - 2}` }} className="page-link"><ChevronLeft /></Link>
                        </li>}

                    {store.pageNumber > 1 &&
                        <li className="page-item" onClick={() => handlePageChange(store.pageNumber - 1)}>
                            <Link to={{ search: `?pageNumber=${store.pageNumber - 1}` }} className="page-link">{store.pageNumber - 1}</Link>
                        </li>}

                    <li className="page-item active">
                        <div className="page-link">{store.pageNumber}</div>
                    </li>

                    {store.totalPages > store.pageNumber &&

                        <li className="page-item" onClick={() => handlePageChange(store.pageNumber + 1)}>
                            <Link to={{ search: `?pageNumber=${store.pageNumber + 1}` }} className="page-link">{store.pageNumber + 1}</Link>
                        </li>}

                    {store.totalPages > store.pageNumber + 1 &&
                        <li className="page-item" onClick={() => handlePageChange(store.pageNumber + 2)}>
                            <Link to={{ search: `?pageNumber=${store.pageNumber + 2}` }} className="page-link"><ChevronRight /></Link>
                        </li>}

                    <li onClick={() => handlePageChange(store.totalPages)}
                    className={`page-item ${store.pageNumber === store.totalPages ? `disabled` : ``}`}>
                    <Link to={{ search: `?pageNumber=${store.totalPages}` }} className="page-link"><ChevronDoubleRight /></Link>
                    </li>
                </ul>}
        </div>
    );
}