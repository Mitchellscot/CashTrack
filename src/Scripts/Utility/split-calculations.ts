import {formatAmount} from '../Utility/format-amount';

export const updateTotalsWhenAmountChanges = (): void => {
	const amountInputs = document.querySelectorAll('.split-amount-js');
	amountInputs.forEach(x => {
		x.addEventListener('change', adjustTotalsOnAmountChange, false);
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
	const currentTaxAmount = document.getElementById('Tax') as HTMLInputElement;
	const taxAmount: number = parseFloat(currentTaxAmount.value);
	const isTaxed = checkbox.checked;

	updateExpenseTotalAmount(index!, amount, taxAmount, isTaxed);
	updateTheForm();
};

const adjustTotalsOnAmountChange = (e: Event): void => {
	const input = e.target as HTMLInputElement;
	const amount: number = formatAmount(input.value);
	const currentTaxAmount = document.getElementById('Tax') as HTMLInputElement;
	const taxAmount: number = parseFloat(currentTaxAmount.value);
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
	const totalAmountInputElement = document.getElementById(`totalAmount-${index}`)! as HTMLInputElement;
	const totalAmountElementForDisplay = document.getElementById(`totalAmountForDisplay-${index}`)! as HTMLInputElement;
	const calculatedAmount = formatAmount(amount + (amount * taxAmount));
	const amountAfterTaxIfApplicable = isTaxed
		? isNaN(calculatedAmount)
			? '0'
			: calculatedAmount.toString()
		: isNaN(amount)
			? '0'
			: amount.toString();
	totalAmountInputElement.value = amountAfterTaxIfApplicable;
	totalAmountElementForDisplay.value = amountAfterTaxIfApplicable;
}

function updateTheForm(): void {
	const totalElement = document.getElementById('total')!;
	const originalTotal: number = parseFloat(totalElement.dataset.originalTotal!);
	const allTotalElements = (
		// eslint-disable-next-line @typescript-eslint/no-unnecessary-type-assertion
		document.querySelectorAll('.total-amount-js') as NodeListOf<HTMLInputElement>
	);
	let sumOfTotals = 0;
	allTotalElements.forEach(x => {
		(sumOfTotals += parseFloat(x.value));
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
