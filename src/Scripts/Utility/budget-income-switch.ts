export const disableSelectInputsOnIncomeSwitch = (): void => {
    const checkBox: HTMLElement | null = document.getElementById("budgetIncomeSwitch");
    const modalButton: HTMLElement | null = document.getElementById('addBudgetButton');
    checkBox?.addEventListener('click', (): void => {
        if ((checkBox as HTMLInputElement).checked) {
            const averagesTable = <HTMLTableElement>document.getElementById('averagesTable');
            averagesTable.classList.add('visually-hidden');
            document.getElementById("budgetTypeInputs")?.classList.add('visually-hidden');
            document.getElementById("categoryInputs")?.classList.add('visually-hidden');
        }
        else {
            document.getElementById("budgetTypeInputs")?.classList.remove('visually-hidden');
            document.getElementById("categoryInputs")?.classList.remove('visually-hidden');
        }
    }, false);
    modalButton?.addEventListener('click', (): void => {
        if ((checkBox as HTMLInputElement).checked) {
            const averagesTable = <HTMLTableElement>document.getElementById('averagesTable');
            averagesTable.classList.add('visually-hidden');
            document.getElementById("budgetTypeInputs")?.classList.add('visually-hidden');
            document.getElementById("categoryInputs")?.classList.add('visually-hidden');
        }
        else {
            document.getElementById("budgetTypeInputs")?.classList.remove('visually-hidden');
            document.getElementById("categoryInputs")?.classList.remove('visually-hidden');
        }
    }, false);
}