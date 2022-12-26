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
	const formattedAmount = formatAmount(input.value);
	input.value = formattedAmount.toString();
};

export const formatAmount = (amount: string | number): number => parseFloat(
	(
		(Math.round(parseFloat(amount.toString()) * 10) ** 2) / (10 ** 2)
	).toFixed(2),
);
