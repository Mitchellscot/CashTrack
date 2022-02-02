import './Dashboard.css';
import Container from 'react-bootstrap/Container';
import Col from 'react-bootstrap/Col';
import DashboardHeader from './DashboardHeader';
import DashboardSidebar from './DashboardSidebar';
import Row from 'react-bootstrap/Row';
import React, { useEffect } from 'react';
import { Switch, Route } from "react-router-dom";
import ExpenseTable from '../Expenses/ExpenseTable';

function Dashboard() {
    useEffect(() => {
        document.body.style='background-color: none;';
    }, []);
    
    return (
        <>
            <DashboardHeader />
            <Container fluid>
                <Row>
                    <DashboardSidebar />
                    <Col md={10} lg={10} className="ml-sm-auto px-md-2 py-2">
                        <main className="d-flex flex-wrap flex-md-nowrap pt-4 border border-bottom-0 rounded-top">
                         <Switch>
                        <Route path="/dashboard/expenses">
                            <ExpenseTable />
                            </Route>
                        </Switch> 
                        </main>
                    </Col>
                </Row>
            </Container>
        </>
    );
}

export default Dashboard;