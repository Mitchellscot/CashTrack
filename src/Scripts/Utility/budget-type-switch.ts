export function hideCategoryWhenSavingsIsSelectedAddModal() {
    const radioButtons = document.querySelectorAll('.handle-category-select-js');
    radioButtons.forEach(x => x.addEventListener('change', handleSavingsIsSelectedForAddModal, false));
}
export const handleSavingsIsSelectedForAddModal = (e: Event): void => {
    const isChecked = (e.target as HTMLInputElement).checked && (e.target as HTMLInputElement).value == "2";
    if (isChecked) {
        document.getElementById("categoryInputs")?.classList.add('visually-hidden');
    }
    else {
        document.getElementById("categoryInputs")?.classList.remove('visually-hidden');
    };
}