import {formatAmount} from '../Utility/format-amount';

export const updateTotalsWhenAmountChanges = (): void => {
	const amountInputs = document.querySelectorAll('.split-amount-js');
	amountInputs.forEach(x => {
		x.addEventListener('change', adjustTotals, false);
	},
	);
};

export const updateTotalsWhenTaxStatusChanges = (): void => {
	const taxCheckboxes = document.querySelectorAll('.split-tax-js');
	taxCheckboxes.forEach(x => {
		x.addEventListener('change', adjustTotalsOnTaxChange, false);
	},
	);
};

const adjustTotalsOnTaxChange = (e: Event): void => {
	const checkbox = e.target as HTMLInputElement;
	const {index} = checkbox.dataset;
	const amount: number = formatAmount(
		(document.getElementById(`amount-${index!}`) as HTMLInputElement).value,
	);
	const taxAmount: number = parseFloat(checkbox.dataset.taxAmount!);
	const isTaxed = checkbox.checked;

	updateExpenseTotalAmount(index!, amount, taxAmount, isTaxed);
	updateTheForm();
};

const adjustTotals = (e: Event): void => {
	const input = e.target as HTMLInputElement;
	const amount: number = formatAmount(input.value);
	const taxAmount: number = parseFloat(input.dataset.taxAmount!);
	const {index} = input.dataset;
	const isTaxed = ((
		document.getElementById(`isTaxed-${index!}`) as HTMLInputElement
	)).checked;

	updateExpenseTotalAmount(index!, amount, taxAmount, isTaxed);
	updateTheForm();
};

function updateExpenseTotalAmount(
	index: string,
	amount: number,
	taxAmount: number,
	isTaxed: boolean,
): void {
	const totalAmountElement = document.getElementById(`totalAmount-${index}`)!;
	const calculatedAmount = formatAmount((amount + amount) * taxAmount);
	totalAmountElement.innerHTML = isTaxed
		? isNaN(calculatedAmount)
			? '0'
			: calculatedAmount.toString()
		: isNaN(amount)
			? '0'
			: amount.toString();
}

function updateTheForm(): void {
	const totalElement = document.getElementById('total')!;
	const originalTotal: number = parseFloat(totalElement.dataset.originalTotal!);
	const allTotalElements = (
		document.querySelectorAll('.total-amount-js')
	);
	let sumOfTotals = 0;
	allTotalElements.forEach(x => {
		(sumOfTotals += parseFloat(x.innerHTML));
	});
	sumOfTotals = isNaN(sumOfTotals) ? 0 : sumOfTotals;
	const leftoverTotal: number = formatAmount(originalTotal - sumOfTotals);
	totalElement.innerHTML = leftoverTotal.toString();
	adjustPageOnAmountChange(leftoverTotal, originalTotal);
}

function adjustPageOnAmountChange(amount: number, originalTotal: number): void {
	const totalBox = document.getElementById('totalBox')!;
	const submitButton = (
		document.getElementById('submitButton') as HTMLButtonElement
	);
	if (amount !== Number(0.0)) {
		submitButton.setAttribute('disabled', '');
		totalBox.classList.remove('border-success');
	}

	if (amount > Number(0.0) && amount <= originalTotal) {
		totalBox.classList.remove('border-danger');
	}

	if (amount < Number(0.0)) {
		totalBox.classList.add('border-danger');
	}

	if (amount > originalTotal) {
		totalBox.classList.add('border-danger');
	}

	if (amount === Number(0.0)) {
		submitButton.removeAttribute('disabled');
		totalBox.classList.remove('border-danger');
		totalBox.classList.add('border-success');
	}
}
