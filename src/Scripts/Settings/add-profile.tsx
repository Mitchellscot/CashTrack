import { hide } from '@popperjs/core';
import React from 'react';
import {useState} from 'react';
import {createRoot} from 'react-dom/client';

const container = document.getElementById('add-profile-modal');
const root = createRoot(container!);
root.render(<AddProfileModal />);

function AddProfileModal() {
	const BOTH = 'both';
	const EXPENSE_ONLY = 'expenses-only';
	const INCOME_ONLY = 'income-only';
	const allQuestions = ['q2', 'q3', 'q4', 'q5', 'q6', 'q7']
	let [showIncomeInput, setShowIncomeInput] = useState(false);
	let [amountColumnName, setAmountColumnName] = useState('Expenses');

	const hideAllQuestions = () => {
		allQuestions.forEach(x => {
			const el = document.getElementById(x) as HTMLInputElement;
			el.value = '';
			el.classList.add('display-none');
		});
	}

	const showNext = (question: Number) => {
		const el = document.getElementById(`q${question}`);
		el?.classList.remove('display-none');
	}
	const hideQuestion = (question: Number) => {
		const el = document.getElementById(`q${question}`) as HTMLInputElement;
		el?.classList.remove('d-flex');
		el?.classList.add('display-none');
	}
	const handleContainsNegativevalue = (answer: Boolean) => {
		const input = document.getElementById(`AddEditImportProfile.ContainsNegativeValue`) as HTMLInputElement;
		const nextQuestionId = answer === true ? `q3` : `q4`;
		const nextQuestion = document.getElementById(nextQuestionId);
		if (answer)

			nextQuestion?.classList.remove('display-none');
		else
		{
			const hideThisInput = document.getElementById('q3') as HTMLInputElement;
			hideThisInput.classList.add('display-none')
		}
	}
	const handleTransactionQuestion = (answer: string) => {
		switch (answer)
		{
			case BOTH:
				setShowIncomeInput(true);
				setAmountColumnName('Expenses');
				hideAllQuestions();
				showNext(4);
				break;
			case EXPENSE_ONLY:
				setShowIncomeInput(false);
				setAmountColumnName('Expenses');
				hideAllQuestions();
				showNext(2);
				break;
			case INCOME_ONLY:
				setShowIncomeInput(false);
				setAmountColumnName('Income');
				hideAllQuestions();
				showNext(2);
				const input = document.getElementById("AddEditImportProfile.DefaultTransactionType") as HTMLInputElement;
				input.value = '1';
				break;
		}
	}

	return (
		<div className="container">
			<form method="post" action='/settings?handler=addProfile' id="addProfileForm">
				<input type="hidden" name="AddEditImportProfile.ContainsNegativeValue" id="AddEditImportProfile.ContainsNegativeValue" value="false" />
				<input type="hidden" name="AddEditImportProfile.NegativeValueTransactionType" id="AddEditImportProfile.NegativeValueTransactionType" value="0" />
				<input type="hidden" name="AddEditImportProfile.DefaultTransactionType" id="AddEditImportProfile.DefaultTransactionType" value="0" />

				<div className="row" id="q1" >
					<span className="lead mb-2">
						Does your file contain seperate columns for income and expenses?
					</span>
					<div className="row mb-2">
						<div className="form-check col">
							<input className="form-check-input mx-3" type="radio" id="both" name="transaction-type"
								onChange={() => handleTransactionQuestion(BOTH)} />
							<label className="form-check-label text-center" htmlFor="both">
								Two Columns
							</label>
						</div>
						<div className="form-check col">
							<input className="form-check-input mx-3" type="radio" name="transaction-type" id="expenses-only" onChange={() => handleTransactionQuestion(EXPENSE_ONLY)} />
							<label className="form-check-label text-center" htmlFor="expenses-only">
								One (Expenses)
							</label>
						</div>
						<div className="form-check col">
							<input className="form-check-input mx-3" type="radio" name="transaction-type" id="thirdQ"
								onChange={() => handleTransactionQuestion(INCOME_ONLY)} />
							<label className="form-check-label text-center" htmlFor="thirdQ">
								One (Income)
							</label>
						</div>
					</div>
				</div>

				<div className="row display-none" id="q2">
					<span className="mb-2">
						Does your file contain negative values that indicate whether it's an expense or income?
					</span>
					<div className="row mb-2">
						<div className="form-check  col">
							<input className="form-check-input mx-3" type="radio" name="containsNegativeValue" onChange={() => handleContainsNegativevalue(true)} />
							<label className="form-check-label" htmlFor="firstQ">
								Yes
							</label>
						</div>
						<div className="form-check col">
							<input className="form-check-input mx-3" type="radio" name="containsNegativeValue" onChange={() => handleContainsNegativevalue(false)} />
							<label className="form-check-label" htmlFor="secondQ">
								No
							</label>
						</div>
					</div>
				</div>

				<div className="row display-none" id="q3">
					<span className="lead mb-2">
						What does a negative value indicate? (required)
					</span>
					<div className="row mb-2">
						<div className="form-check  col">
							<input className="form-check-input mx-3 lead" type="radio" name="AddEditImportProfile.NegativeValueTransactionType" id="firstQ" value="0" />
							<label className="form-check-label lead" htmlFor="firstQ">
								Expenses are negative
							</label>
						</div>
						<div className="form-check col">
							<input className="form-check-input mx-3 lead" type="radio" name="AddEditImportProfile.NegativeValueTransactionType" id="secondQ" value="1" />
							<label className="form-check-label lead" htmlFor="secondQ">
								Income is negative
							</label>
						</div>
					</div>
				</div>
				<div className="row mb-2 display-none" id="q4">
					<label htmlFor="" className="form-label lead">
						Income Column Name (required)
					</label>
					<input type="text" className="form-control" name="AddEditImportProfile.IncomeColumn" onChange={() => showNext(5)} />
				</div>
				<div className="row mb-2 display-none" id="q5">
					<label htmlFor="" className="form-label lead">
						{amountColumnName} Column Name (required)
					</label>
					<input type="text" className="form-control" name="AddEditImportProfile.AmountColumn" onChange={() => showNext(6)} />
				</div>
				<div className="row mb-2 display-none" id="q6">
					<label htmlFor="" className="form-label lead">
						Date Column Name (required)
					</label>
					<input onChange={() => showNext(7)} type="text" className="form-control" name="AddEditImportProfile.DateColumn" />
				</div>
				<div className="row mb-2 display-none" id="q7">
					<label htmlFor="" className="form-label lead">
						Transaction Notes Column Name (required)
					</label>
					<input type="text" className="form-control" name="AddEditImportProfile.NotesColumn" />
				</div>
			</form>
		</div>
	);
}