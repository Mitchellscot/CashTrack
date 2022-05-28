export const formatAmountOnChange = (): void => {
    const amountInputs = <NodeListOf<HTMLInputElement>>document.querySelectorAll('.format-amount-js');
    amountInputs.forEach(x => x.addEventListener('change', formatAmount, true));
}

export const formatAmount = (e: Event): void => {
    const input = e.target as HTMLInputElement;
    const formattedAmount = (Math.round(parseFloat(input.value) * Math.pow(10, 2)) / Math.pow(10, 2)).toFixed(2)
    input.value = formattedAmount;
}