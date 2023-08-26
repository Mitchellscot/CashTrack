import React from 'react';
import {useState} from 'react';
import {createRoot} from 'react-dom/client';

const container = document.getElementById('add-profile-modal');
const root = createRoot(container!);
root.render(<AddProfileModal />);

function AddProfileModal() {
	let [counter, setCounter] = useState(0);

	const showNext = (question: Number) =>
	{
		const el = document.getElementById(`q${question}`);
		el?.classList.remove('visibility-hidden');
	}
	const showIncomeOnlyNext = (question: Number) => {
		const el = document.getElementById(`q${question}`);
		el?.classList.remove('visibility-hidden');
	}

	return (
		<div className="container">
			<div className="row d-flex justify-content-center mb-2">
				<label htmlFor="" className="form-label lead">
					Date Column Name (required)
				</label>
				<input onChange={() => showNext(2)} type="text" className="form-control" name="AddEditImportProfile.DateColumn" />
			</div>
			<div className="row d-flex justify-content-center mb-2 visibility-hidden" id="q2">
				<label htmlFor="" className="form-label lead">
					Transaction Notes Column Name (required)
				</label>
				<input onChange={() => showNext(3)} type="text" className="form-control" name="AddEditImportProfile.NotesColumn" />
			</div>
			<div className="row d-flex justify-content-center visibility-hidden" id="q3" >
				<span className="lead mb-2">
					Does your file contain Income, Expenses, or both?
				</span>
				<div className="row mb-2">
					<div className="form-check d-flex justify-content-center col">
						<input className="form-check-input mx-3 lead" type="radio" id="firstQ" name="transaction-type" onChange={() => showNext(4)} />
						<label className="form-check-label lead" htmlFor="firstQ">
							Both
						</label>
					</div>
					<div className="form-check d-flex justify-content-center col">
						<input className="form-check-input mx-3 lead" type="radio" name="transaction-type" id="secondQ" />
						<label className="form-check-label lead" htmlFor="secondQ">
							Expenses Only
						</label>
					</div>
					<div className="form-check d-flex justify-content-center col">
						<input className="form-check-input mx-3 lead" type="radio" name="transaction-type" id="thirdQ" onChange={() => showIncomeOnlyNext(4)} />
						<label className="form-check-label lead" htmlFor="thirdQ">
							Income Only
						</label>
					</div>
				</div>
			</div>
			<div className="row d-flex justify-content-center visibility-hidden" id="q4">
				<span className="lead mb-2">
					Does your file contain negative values that indicate whether it's an expense or income?
				</span>
				<div className="row mb-2">
					<div className="form-check d-flex justify-content-center col">
						<input className="form-check-input mx-3 lead" type="radio" name="AddEditImportProfile.ContainsNegativeValue" id="firstQ" value="0"/>
						<label className="form-check-label lead" htmlFor="firstQ">
							Yes
						</label>
					</div>
					<div className="form-check d-flex justify-content-center col">
						<input className="form-check-input mx-3 lead" type="radio" name="AddEditImportProfile.ContainsNegativeValue" id="secondQ" value="1" />
						<label className="form-check-label lead" htmlFor="secondQ">
							No
						</label>
					</div>
				</div>
			</div>
			<div className="row d-flex justify-content-center visibility-hidden" id="q5">
				<span className="lead mb-2">
					What does a negative value indicate? (required)
				</span>
				<div className="row mb-2">
					<div className="form-check d-flex justify-content-center col">
						<input className="form-check-input mx-3 lead" type="radio" name="AddEditImportProfile.NegativeValueTransactionType" id="firstQ" value="0" />
						<label className="form-check-label lead" htmlFor="firstQ">
							Expenses are negative
						</label>
					</div>
					<div className="form-check d-flex justify-content-center col">
						<input className="form-check-input mx-3 lead" type="radio" name="AddEditImportProfile.NegativeValueTransactionType" id="secondQ" value="1"  />
						<label className="form-check-label lead" htmlFor="secondQ">
							Income is negative
						</label>
					</div>
				</div>
			</div>
			<div className="row d-flex justify-content-center mb-2 visibility-hidden" id="q6">
				<label htmlFor="" className="form-label lead">
					Income Column Name (required)
				</label>
				<input type="text" className="form-control" name="AddEditImportProfile.IncomeColumn" />
			</div>
			<div className="row d-flex justify-content-center mb-2 visibility-hidden" id="q7">
				<label htmlFor="" className="form-label lead">
					Expense Column Name (required)
				</label>
				<input type="text" className="form-control" name="AddEditImportProfile.AmountColumn" />
			</div>
		</div>
	);
}