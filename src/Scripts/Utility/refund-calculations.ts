import {formatAmount} from '../Utility/format-amount';

export function refundExpenseOnAmountChange() {
	const refundAmountInputs = document.querySelectorAll('.refund-amount-js');
	refundAmountInputs.forEach(x => {
		x.addEventListener('change', refundExpense, false);
	},
	);
}

export function refundEntireExpenseWhenChecked(): void {
	const applyFullCheckboxes = document.querySelectorAll(
		'.apply-full-amount-js',
	);
	applyFullCheckboxes.forEach(x => {
		x.addEventListener('change', refundEntireExpense, false);
	},
	);
}

function refundExpense(e: Event): void {
	const input = e.target as HTMLInputElement;
	const {index} = input.dataset;
	const applyFullIsChecked = ((
		document.getElementById(`applyFull-${index!}`) as HTMLInputElement
	)).checked;
	const modifiedAmountElement = document.getElementById(`modifiedAmount-${index!}`)!;
	const originalAmountElement = document.getElementById(`originalAmount-${index!}`)!;
	const totalAmountElement = document.getElementById('total')!;
	const incomeAmount: number = parseFloat(totalAmountElement.dataset.total!);
	const originalAmount: number = parseFloat(originalAmountElement.innerHTML);
	if (!applyFullIsChecked) {
		const refundAmount = input.value;
		input.value = formatAmount(refundAmount).toString();
		modifiedAmountElement.innerHTML = formatAmount(
			originalAmount - formatAmount(refundAmount),
		).toString();
		const total = findTotal(incomeAmount);
		adjustFormOnTotalChange(total, incomeAmount);
		totalAmountElement.innerHTML = total.toString();
	}
}

function refundEntireExpense(e: Event): void {
	const checkbox = e.target as HTMLInputElement;
	const {index} = checkbox.dataset;
	const originalAmountElement = document.getElementById(`originalAmount-${index!}`)!;
	const modifiedAmountElement = document.getElementById(`modifiedAmount-${index!}`)!;
	const refundAmountElement = (
		document.getElementById(`refundAmount-${index!}`) as HTMLInputElement
	);
	const totalAmountElement = document.getElementById('total')!;
	const incomeAmount: number = parseFloat(totalAmountElement.dataset.total!);
	const originalAmount: number = parseFloat(originalAmountElement.innerHTML);

	if (checkbox.checked) {
		modifiedAmountElement.innerHTML = modifiedAmountWithFullRefund(
			originalAmount,
			incomeAmount,
		);
		refundAmountElement.value = applyFullAmount(originalAmount, incomeAmount);
		refundAmountElement.readOnly = true;
		const total = findTotal(incomeAmount);
		adjustFormOnTotalChange(total, incomeAmount);
		totalAmountElement.innerHTML = total.toString();
	} else {
		refundAmountElement.readOnly = false;
		refundAmountElement.value = '0.00';
		modifiedAmountElement.innerHTML = originalAmountElement.innerHTML;
		const total = findTotal(incomeAmount);
		adjustFormOnTotalChange(total, incomeAmount);
		totalAmountElement.innerHTML = total.toString();
	}
}

function findTotal(incomeAmount: number): number {
	const allTotalElements = document.querySelectorAll('.refund-amount-js') as unknown as HTMLInputElement[];
	let sumOfTotals = 0;
	allTotalElements.forEach(x => {
		(sumOfTotals += parseFloat(x.value));
	});
	sumOfTotals = isNaN(sumOfTotals) ? 0 : sumOfTotals;
	return formatAmount(incomeAmount - sumOfTotals);
}

function modifiedAmountWithFullRefund(
	original: number,
	incomeAmount: number,
): string {
	return original > incomeAmount
		? formatAmount(original - incomeAmount).toString()
		: '0.00';
}

function applyFullAmount(original: number, incomeAmount: number): string {
	return original < incomeAmount
		? formatAmount(original).toString()
		: formatAmount(incomeAmount).toString();
}

function adjustFormOnTotalChange(total: number, incomeAmount: number): void {
	const totalBox = document.getElementById('totalBox')!;
	const submitButton = (
		document.getElementById('submitButton') as HTMLButtonElement
	);
	if (total !== 0.0) {
		submitButton.setAttribute('disabled', '');
		totalBox.classList.remove('border-success');
	}

	if (total > 0.0 && total <= incomeAmount) {
		totalBox.classList.remove('border-danger');
	}

	if (total < 0.0) {
		totalBox.classList.add('border-danger');
	}

	if (total > incomeAmount) {
		totalBox.classList.add('border-danger');
	}

	if (total === 0.0) {
		submitButton.removeAttribute('disabled');
		totalBox.classList.remove('border-danger');
		totalBox.classList.add('border-success');
	}
}
