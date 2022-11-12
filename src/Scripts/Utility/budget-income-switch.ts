export const disableSelectInputsOnIncomeSwitch = (): void => {
    const checkBox: HTMLElement | null = document.getElementById("budgetIncomeSwitch");
    checkBox?.addEventListener('click', (): void => {

        if ((checkBox as HTMLInputElement).checked) {
            document.getElementById("budgetTypeInputs")?.classList.add('visually-hidden');
            document.getElementById("categoryInputs")?.classList.add('visually-hidden');
        }
        else {
            document.getElementById("budgetTypeInputs")?.classList.remove('visually-hidden');
            document.getElementById("categoryInputs")?.classList.remove('visually-hidden');
        }
    }, false);
}