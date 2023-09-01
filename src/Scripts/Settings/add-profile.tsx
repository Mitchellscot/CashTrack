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
	const DISPLAY_NONE = 'display-none';
	const EXPENSES = 'expenses';
	const INCOME = 'income';
	let [showIncomeInput, setShowIncomeInput] = useState(false);
	let [amountColumnName, setAmountColumnName] = useState('Expenses');

	const hideAllQuestions = () => {
		allQuestions.forEach(x => {
			const el = document.getElementById(x) as HTMLInputElement;
			el.value = '';
			el.classList.add(DISPLAY_NONE);
		});
	}
	const showNext = (question: Number) => {
		const el = document.getElementById(`q${question}`);
		el?.classList.remove(DISPLAY_NONE);
	}
	const handleContainsNegativevalue = (answer: Boolean | null) => {
		if (Boolean(answer) == undefined)
			return;
		const input = document.getElementById(`AddProfileModal_ContainsNegativeValue`) as HTMLInputElement;
		input.value = Boolean(answer).toString();
		const nextQuestionId = answer ? `q3` : !answer && showIncomeInput ? `q4` : `q5`;
		const nextQuestion = document.getElementById(nextQuestionId);
		if (answer) {
			nextQuestion?.classList.remove(DISPLAY_NONE);
			allQuestions.filter(i => i != 'q2' && i != 'q3').forEach(i => document.getElementById(i)?.classList.add(DISPLAY_NONE))
		}
		else {
			const hideThisInput = document.getElementById('q3') as HTMLInputElement;
			hideThisInput.classList.add(DISPLAY_NONE)
			nextQuestion?.classList.remove(DISPLAY_NONE);
		}
	}
	const handleNegativeValueTransactionType = (answer: string | null) => {
		if (answer === undefined || answer === null)
			return;
		const input = document.getElementById(`AddProfileModal_NegativeValueTransactionType`) as HTMLInputElement;
		input.value = answer;
		const nextQuestionId = `q5`;
		const nextQuestion = document.getElementById(nextQuestionId);
		nextQuestion?.classList.remove(DISPLAY_NONE);
	}
	const resetForm = () => {
		const containsNegativeValueInput = document.getElementById(`AddProfileModal_ContainsNegativeValue`) as HTMLInputElement;
		containsNegativeValueInput.value = 'false';
		const radioButton1 = document.getElementById('negativeQuestion1') as HTMLInputElement;
		const radioButton2 = document.getElementById('negativeQuestion2') as HTMLInputElement;
		radioButton1.checked = false;
		radioButton2.checked = false;
		const negativeValueInput = document.getElementById(`AddProfileModal_NegativeValueTransactionType`) as HTMLInputElement;
		negativeValueInput.value = EXPENSES;
		const radioButton3 = document.getElementById('negativeValueQuestion1') as HTMLInputElement;
		const radioButton4 = document.getElementById('negativeValueQuestion2') as HTMLInputElement;
		radioButton3.checked = false;
		radioButton4.checked = false;
		const incomeColumn = document.getElementById(`AddProfileModal_IncomeColumn`) as HTMLInputElement;
		incomeColumn.value = '';
		const amountColumn = document.getElementById(`AddProfileModal_AmountColumn`) as HTMLInputElement;
		amountColumn.value = '';
		const dateColumn = document.getElementById(`AddProfileModal_DateColumn`) as HTMLInputElement;
		dateColumn.value = '';
		const notesColumn = document.getElementById(`AddProfileModal_NotesColumn`) as HTMLInputElement;
		notesColumn.value = '';
		const nameColumn = document.getElementById(`AddProfileModal_Name`) as HTMLInputElement;
		nameColumn.value = '';
	}

	const handleTransactionQuestion = (answer: string) => {
		switch (answer)
		{
			case BOTH:
				resetForm()
				setShowIncomeInput(true);
				setAmountColumnName('Expenses');
				hideAllQuestions();
				showNext(4);
				break;
			case EXPENSE_ONLY:
				resetForm()
				setShowIncomeInput(false);
				setAmountColumnName('Expenses');
				hideAllQuestions();
				showNext(2);
				let expensesDefault = document.getElementById("AddProfileModal_DefaultTransactionType") as HTMLInputElement;
				expensesDefault.value = EXPENSES;
				break;
			case INCOME_ONLY:
				resetForm()
				setShowIncomeInput(false);
				setAmountColumnName('Income');
				hideAllQuestions();
				showNext(2);
				let incomeDefault = document.getElementById("AddProfileModal_DefaultTransactionType") as HTMLInputElement;
				incomeDefault.value = INCOME;
				break;
		}
	}
	const enableSave = () => {
		const button = document.getElementById('profile-save');
		button?.removeAttribute('disabled');
		showNext(9);
	}

	return (
		<>
			<input type="hidden" name="ContainsNegativeValue" id="AddProfileModal_ContainsNegativeValue" />
			<input type="hidden" name="NegativeValueTransactionType" id="AddProfileModal_NegativeValueTransactionType" />
			<input type="hidden" name="DefaultTransactionType" id="AddProfileModal_DefaultTransactionType" />
				<div className="row" id="q1" >
					<span className="lead mb-2">
						Does your file contain seperate columns for income and expenses?
					</span>
					<div className="row mb-2">
					<div className="form-check col">
						<input className="form-check-input mx-3" type="radio" id="both" name="TransactionType"
							onChange={() => handleTransactionQuestion(BOTH)} value={BOTH} />
							<label className="form-check-label text-center" htmlFor="both">
								Two Columns
							</label>
						</div>
					<div className="form-check col">
						<input className="form-check-input mx-3" type="radio" name="TransactionType" id="expenses-only" onChange={() => handleTransactionQuestion(EXPENSE_ONLY)} value={EXPENSE_ONLY} />
							<label className="form-check-label text-center" htmlFor="expenses-only">
								One (Expenses)
							</label>
						</div>
						<div className="form-check col">
						<input className="form-check-input mx-3" type="radio" id="thirdQ" name="TransactionType"
							onChange={() => handleTransactionQuestion(INCOME_ONLY)} value={INCOME_ONLY} />
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
							<input className="form-check-input mx-3" type="radio" name="negative-value" id="negativeQuestion1" onChange={() => handleContainsNegativevalue(true)} />
							<label className="form-check-label" htmlFor="firstQ">
								Yes
							</label>
						</div>
						<div className="form-check col">
						<input className="form-check-input mx-3" type="radio" name="negative-value" id="negativeQuestion2"  onChange={() => handleContainsNegativevalue(false)} />
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
						<input className="form-check-input mx-3 lead" type="radio" name="negativeValue" id="negativeValueQuestion1" value="0" onChange={() => handleNegativeValueTransactionType(EXPENSES)} />
							<label className="form-check-label lead" htmlFor="firstQ">
								Expenses are negative
							</label>
						</div>
						<div className="form-check col">
						<input className="form-check-input mx-3 lead" type="radio" name="negativeValue"  id="negativeValueQuestion2" value="1" onChange={() => handleNegativeValueTransactionType(INCOME)} />
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
					<input type="text" className="form-control" name="IncomeColumn"
						id="AddProfileModal_IncomeColumn"
						onChange={() => showNext(5)} />
				</div>
				<div className="row mb-2 display-none" id="q5">
					<label htmlFor="" className="form-label lead">
						{amountColumnName} Column Name (required)
					</label>
					<input type="text" className="form-control" name="AmountColumn"
						id="AddProfileModal_AmountColumn"
						onChange={() => showNext(6)} />
				</div>
				<div className="row mb-2 display-none" id="q6">
					<label htmlFor="" className="form-label lead">
						Date Column Name (required)
					</label>
					<input onChange={() => showNext(7)} type="text" className="form-control" name="DateColumn" id="AddProfileModal_DateColumn" />
				</div>
				<div className="row mb-2 display-none" id="q7">
					<label htmlFor="" className="form-label lead">
						Transaction Notes Column Name (required)
					</label>
					<input type="text" className="form-control" name="NotesColumn"
					onChange={() => showNext(8)} id="AddProfileModal_NotesColumn" />
			</div>
			<div className="row mb-3 display-none" id="q8">
				<label htmlFor="" className="form-label lead">
					Please give this profile a name (required. example: bank, credit, etc.)
				</label>
				<input type="text" className="form-control" name="Name"
					onChange={enableSave} id="AddProfileModal_Name" />
			</div>
				<div className="modal-footer display-none" id="q9">
					<button type="submit" disabled className="btn btn-primary spin-it" id="profile-save">Save</button>
			</div>
		</>
	);
}