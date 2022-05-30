export function forceRefundCategoryWhenRefundIsCheckedAddModal() {
    const checkboxes = document.querySelectorAll('.refund-switch-add-js');
    checkboxes.forEach(x => x.addEventListener('change', handleCategoryChangeForAddModal, false));
}

function handleCategoryChangeForAddModal(e: Event): void {
    const isChecked = (e.target as HTMLInputElement).checked;
    const categoryElement = document.getElementById('addIncomeCategory') as HTMLSelectElement;
    const submitButton = document.getElementById('addEditIncomeSubmitButton') as HTMLButtonElement;
    if (isChecked) {
        for (let i = 0; i < categoryElement.options.length; i++) {
            console.log(categoryElement.options[i].text);
            if (categoryElement.options[i].text === "Refund") {
                categoryElement.options[i].selected = true;
                categoryElement.setAttribute('disabled', '');
                submitButton.innerHTML = 'Apply Refund';
            }
        }
    }
    else {
        submitButton.innerHTML = 'Add Income';
        categoryElement.removeAttribute('disabled');
        categoryElement.value = "1";
    }
}

export function forceRefundCategoryWhenRefundIsCheckedEditModal() {
    const checkboxes = document.querySelectorAll('.refund-switch-edit-js');
    checkboxes.forEach(x => x.addEventListener('change', handleCategoryChangeForEditModal, false));
}

function handleCategoryChangeForEditModal(e: Event): void {
    const checkboxElement = e.target as HTMLInputElement;
    const isChecked = checkboxElement.checked;
    const id = checkboxElement.dataset.id;
    console.log(id);
    const categoryElement = document.getElementById(`editIncomeCategory-${id}`) as HTMLSelectElement;

    const submitButton = document.getElementById(`editIncomeSubmitButton-${id}`) as HTMLButtonElement;
    if (isChecked) {
        console.log(`it's checked`);
        console.log(categoryElement);
        console.log(submitButton);
        for (let i = 0; i < categoryElement.options.length; i++) {
            console.log(categoryElement.options[i].text);
            if (categoryElement.options[i].text === "Refund") {
                categoryElement.options[i].selected = true;
                categoryElement.setAttribute('disabled', '');
                submitButton.innerHTML = 'Apply Refund';
            }
        }
    }
    else {
        submitButton.innerHTML = 'Add Income';
        categoryElement.removeAttribute('disabled');
        categoryElement.value = "1";
    }
}