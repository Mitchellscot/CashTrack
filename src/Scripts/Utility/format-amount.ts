export const formatAmountOnChange = (): void => {
	const amountInputs = (
		document.querySelectorAll('.format-amount-js')
	);
	amountInputs.forEach(x => {
		x.addEventListener('change', formatAmountForEvent, true);
	},
	);
};

export const formatAmountForEvent = (e: Event): void => {
	const input = e.target as HTMLInputElement;
	const isInteger = Boolean((e.target as HTMLElement).dataset.isInteger);
	const formattedAmount = formatAmount(input.value, isInteger);
	input.value = formattedAmount.toString();
};

export const formatAmount = (amount: string | number, isInteger = false): number => parseFloat(
	(
		// eslint-disable-next-line prefer-exponentiation-operator
		(Math.round(parseFloat(amount.toString()) * Math.pow(10, 2)) / Math.pow(10, 2)
		).toFixed(isInteger ? 0 : 2)
	));
