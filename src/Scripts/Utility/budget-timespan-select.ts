export function forceMonthSelectionWhenIncomeIsCheckedAddModal() {
    const checkboxes = document.querySelectorAll('.handle-timespan-select-js');
    checkboxes.forEach(x => x.addEventListener('change', handleMonthSelectionForAddModal, false));
}

function handleMonthSelectionForAddModal(e: Event): void {
    const isChecked = (e.target as HTMLInputElement).checked && (e.target as HTMLInputElement).value == "2";
    const monthElement = document.getElementById('monthSelectList') as HTMLSelectElement;
    if (isChecked) {
        for (let i = 0; i < monthElement.options.length; i++) {
            if (monthElement.options[i].text === "Every") {
                monthElement.options[i].selected = true;
                monthElement.setAttribute('disabled', '');
            }
        }
    }
    else {
        monthElement.removeAttribute('disabled');
        monthElement.value = (new Date().getMonth() + 1).toString();
    }
}