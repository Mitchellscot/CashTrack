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

export function disableSelectInputsOnEditIncomeSwitch() {
    const radioButtons = document.querySelectorAll('.disable-inputs-on-check-edit-js');
    radioButtons.forEach(x => x.addEventListener('change', handleSelectInputsOnEditIncomeSwitch, false));
}

export const handleSelectInputsOnEditIncomeSwitch = (e: Event): void => {
    const id = (e.target as HTMLElement).dataset.id;
    const checkBox: HTMLElement | null = document.getElementById(`budgetIncomeSwitch-${id}`);
    const modalButton: HTMLElement | null = document.getElementById(`addBudgetButton-${id}`);

    if ((checkBox as HTMLInputElement).checked) {
        const averagesTable = <HTMLTableElement>document.getElementById(`averagesTable-${id}`);
        averagesTable.classList.add('visually-hidden');
        document.getElementById(`budgetTypeInputs-${id}`)?.classList.add('visually-hidden');
        document.getElementById(`categoryInputs-${id}`)?.classList.add('visually-hidden');
    }
    else {
        document.getElementById(`budgetTypeInputs-${id}`)?.classList.remove('visually-hidden');
        document.getElementById(`categoryInputs-${id}`)?.classList.remove('visually-hidden');
    }

    if ((checkBox as HTMLInputElement).checked) {
        const averagesTable = <HTMLTableElement>document.getElementById(`averagesTable-${id}`);
        averagesTable.classList.add('visually-hidden');
        document.getElementById(`budgetTypeInputs-${id}`)?.classList.add('visually-hidden');
        document.getElementById(`categoryInputs-${id}`)?.classList.add('visually-hidden');
    }
    else {
        document.getElementById(`budgetTypeInputs-${id}`)?.classList.remove('visually-hidden');
        document.getElementById(`categoryInputs-${id}`)?.classList.remove('visually-hidden');
    }

}