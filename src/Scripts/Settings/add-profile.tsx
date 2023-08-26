import React from 'react';
import {useState} from 'react';
import {createRoot} from 'react-dom/client';

const container = document.getElementById('add-profile-modal');
const root = createRoot(container!);
root.render(<AddProfileModal />);

function AddProfileModal() {
	let [counter, setCounter] = useState(0);

	return (
		<div className="container">
			<div className="row d-flex justify-content-center mb-2">
				<label htmlFor="" className="form-label lead">
					Date Column Name (required)
				</label>
				<input type="text" className="form-control" />
			</div>
			<div className="row d-flex justify-content-center mb-2">
				<label htmlFor="" className="form-label lead">
					Description Column Name (required)
				</label>
				<input type="text" className="form-control" />
			</div>
			<div className="row d-flex justify-content-center">
				<span className="lead mb-2">
					Does your CSV Import file contain Income, Expenses, or both?
				</span>
				<div className="row mb-2">
					<div className="form-check d-flex justify-content-center col">
						<input className="form-check-input mx-3 lead" type="radio" name="containsIncome" id="firstQ" />
						<label className="form-check-label lead" htmlFor="firstQ">
							Both
						</label>
					</div>
					<div className="form-check d-flex justify-content-center col">
						<input className="form-check-input mx-3 lead" type="radio" name="containsIncome" id="secondQ" />
						<label className="form-check-label lead" htmlFor="secondQ">
							Expenses Only
						</label>
					</div>
					<div className="form-check d-flex justify-content-center col">
						<input className="form-check-input mx-3 lead" type="radio" name="containsIncome" id="thirdQ" />
						<label className="form-check-label lead" htmlFor="thirdQ">
							Income Only
						</label>
					</div>
				</div>
			</div>
			<div className="row d-flex justify-content-center">
				<span className="lead mb-2">
					Does your file contain negative values that indicate whether it's an expense or income?
				</span>
				<div className="row mb-2">
					<div className="form-check d-flex justify-content-center col">
						<input className="form-check-input mx-3 lead" type="radio" name="containsIncome" id="firstQ" />
						<label className="form-check-label lead" htmlFor="firstQ">
							Yes
						</label>
					</div>
					<div className="form-check d-flex justify-content-center col">
						<input className="form-check-input mx-3 lead" type="radio" name="containsIncome" id="secondQ" />
						<label className="form-check-label lead" htmlFor="secondQ">
							No
						</label>
					</div>
				</div>
			</div>
			<div className="row d-flex justify-content-center">
				<span className="lead mb-2">
					What does a negative value indicate? (required)
				</span>
				<div className="row mb-2">
					<div className="form-check d-flex justify-content-center col">
						<input className="form-check-input mx-3 lead" type="radio" name="containsIncome" id="firstQ" />
						<label className="form-check-label lead" htmlFor="firstQ">
							Expenses are negative
						</label>
					</div>
					<div className="form-check d-flex justify-content-center col">
						<input className="form-check-input mx-3 lead" type="radio" name="containsIncome" id="secondQ" />
						<label className="form-check-label lead" htmlFor="secondQ">
							Income is negative
						</label>
					</div>
				</div>
			</div>
			<div className="row d-flex justify-content-center mb-2">
				<label htmlFor="" className="form-label lead">
					Income Column Name (required)
				</label>
				<input type="text" className="form-control" />
			</div>
			<div className="row d-flex justify-content-center mb-2">
				<label htmlFor="" className="form-label lead">
					Expense Column Name (required)
				</label>
				<input type="text" className="form-control" />
			</div>
		</div>
	);
}