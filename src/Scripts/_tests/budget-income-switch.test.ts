import {disableSelectInputsOnIncomeSwitch, disableSelectInputsOnEditIncomeSwitch} from '../Utility/budget-income-switch';

describe('Disable Select Inputs On Income Switch', () => {
	beforeEach(() => {
		const checkBox = document.createElement('input');
		const modalButton = document.createElement('button');
		const table = document.createElement('table');
		const budgetTypeInputs = document.createElement('input');
		const categoryInputs = document.createElement('input');
		budgetTypeInputs.setAttribute('id', 'budgetTypeInputs');
		categoryInputs.setAttribute('id', 'categoryInputs')
		checkBox.setAttribute('id', 'budgetIncomeSwitch');
		modalButton.setAttribute('id', 'addBudgetButton');
		table.setAttribute('id', 'averagesTable');
		document.body.appendChild(checkBox);
		document.body.appendChild(modalButton);
		document.body.appendChild(budgetTypeInputs);
		document.body.appendChild(categoryInputs);
		document.body.appendChild(table);
	});

	it('hides inputs when checked', () => {
		disableSelectInputsOnIncomeSwitch();
		const button = document.getElementById('addBudgetButton') as HTMLButtonElement;
		const checkbox = document.getElementById('budgetIncomeSwitch') as HTMLInputElement;
		checkbox.checked = true;
		button.dispatchEvent(new Event('click'));
		const table = document.getElementById('averagesTable') as HTMLTableElement;
		const budgetTypeInputs = document.getElementById('budgetTypeInputs') as HTMLInputElement;
		const categoryInputs = document.getElementById('categoryInputs') as HTMLInputElement;
		expect(table.classList.contains('visually-hidden')).toBe(true);
		expect(budgetTypeInputs.classList.contains('visually-hidden')).toBe(true);
		expect(categoryInputs.classList.contains('visually-hidden')).toBe(true);
	});
	it('unhides inputs when checked again', () => {
		disableSelectInputsOnIncomeSwitch();
		const button = document.getElementById('addBudgetButton') as HTMLButtonElement;
		const checkbox = document.getElementById('budgetIncomeSwitch') as HTMLInputElement;
		checkbox.checked = true;
		button.dispatchEvent(new Event('click'));
		const table = document.getElementById('averagesTable') as HTMLTableElement;
		const budgetTypeInputs = document.getElementById('budgetTypeInputs') as HTMLInputElement;
		const categoryInputs = document.getElementById('categoryInputs') as HTMLInputElement;
		expect(table.classList.contains('visually-hidden')).toBe(true);
		expect(budgetTypeInputs.classList.contains('visually-hidden')).toBe(true);
		expect(categoryInputs.classList.contains('visually-hidden')).toBe(true);
		checkbox.checked = false;
		checkbox.dispatchEvent(new Event('click'));
		const budgetTypeInputs2 = document.getElementById('budgetTypeInputs') as HTMLInputElement;
		const categoryInputs2 = document.getElementById('categoryInputs') as HTMLInputElement;
		expect(budgetTypeInputs2.classList.contains('visually-hidden')).toBe(false);
		expect(categoryInputs2.classList.contains('visually-hidden')).toBe(false);
	});
});

describe('Disable Select Inputs On Edit Income Switch', () => {
	beforeEach(() => {
		const checkBox = document.createElement('input');
		const table = document.createElement('table');
		const budgetTypeInputs = document.createElement('input');
		const categoryInputs = document.createElement('input');
		budgetTypeInputs.setAttribute('id', 'budgetTypeInputs-1');
		categoryInputs.setAttribute('id', 'categoryInputs-1')
		checkBox.setAttribute('id', 'budgetIncomeSwitch-1');
		checkBox.setAttribute('data-id', '1');
		checkBox.classList.add('disable-inputs-on-check-edit-js');
		table.setAttribute('id', 'averagesTable-1');
		document.body.appendChild(checkBox);
		document.body.appendChild(budgetTypeInputs);
		document.body.appendChild(categoryInputs);
		document.body.appendChild(table);
	});

	it('hides inputs when checked', () => {
		disableSelectInputsOnEditIncomeSwitch();
		const checkbox = document.getElementById('budgetIncomeSwitch-1') as HTMLInputElement;
		checkbox.checked = true;
		checkbox.dispatchEvent(new Event('change'));
		const table = document.getElementById('averagesTable-1') as HTMLTableElement;
		const budgetTypeInputs = document.getElementById('budgetTypeInputs-1') as HTMLInputElement;
		const categoryInputs = document.getElementById('categoryInputs-1') as HTMLInputElement;
		expect(table.classList.contains('visually-hidden')).toBe(true);
		expect(budgetTypeInputs.classList.contains('visually-hidden')).toBe(true);
		expect(categoryInputs.classList.contains('visually-hidden')).toBe(true);
	});
	it('hides inputs when checked', () => {
		disableSelectInputsOnEditIncomeSwitch();
		const checkbox = document.getElementById('budgetIncomeSwitch-1') as HTMLInputElement;
		checkbox.checked = true;
		checkbox.dispatchEvent(new Event('change'));
		const table = document.getElementById('averagesTable-1') as HTMLTableElement;
		const budgetTypeInputs = document.getElementById('budgetTypeInputs-1') as HTMLInputElement;
		const categoryInputs = document.getElementById('categoryInputs-1') as HTMLInputElement;
		expect(table.classList.contains('visually-hidden')).toBe(true);
		expect(budgetTypeInputs.classList.contains('visually-hidden')).toBe(true);
		expect(categoryInputs.classList.contains('visually-hidden')).toBe(true);
		checkbox.checked = false;
		checkbox.dispatchEvent(new Event('change'));
		expect(budgetTypeInputs.classList.contains('visually-hidden')).toBe(false);
		expect(categoryInputs.classList.contains('visually-hidden')).toBe(false);
	});
});

