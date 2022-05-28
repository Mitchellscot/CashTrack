const loadMainCategoryOnSubCategorySelect = (): void => {
    const subCategoryInputs = <NodeListOf<HTMLSelectElement>>document.querySelectorAll('.load-main-category-js');
    subCategoryInputs.forEach(x => x.addEventListener('change', loadMainCategory, false));
}

const loadMainCategory = (e: Event): void => {
    const subCategoryId = (e.target as HTMLSelectElement).value;
    //edit modals have an expenseId set as the data attribute - the add modal does not have that.
    const expenseId = (e.target as HTMLSelectElement).dataset.id;
    fetch(`/api/maincategory/sub-category/${subCategoryId}`)
        //a string is returned instead of json, so we just response.text() instead of response.json()
        .then(response => response.text())
        .then((category: string) => {
            if (expenseId !== '') {
                const mainCategoryInput = <HTMLInputElement>document.getElementById(`editExpenseMainCategory-${expenseId}`);
                mainCategoryInput.value = category;
            }
            else {
                //if there is no expenseId, then it's for the add expense modal
                const mainCategoryInput = <HTMLInputElement>document.getElementById(`addExpenseMainCategory`);
                mainCategoryInput.value = category;
            }
        }).catch(err => console.log(err));
}

const loadMainCategoryOnEditModalLoad = (): void => {
    const editModals = <NodeListOf<HTMLElement>>document.querySelectorAll('.load-main-category-edit-modal-js');
    editModals.forEach(x => x.addEventListener('click', loadMainCategoryForEditModal, false));
}

const loadMainCategoryForEditModal = (e: Event): void => {
    //these are two data attributes on every icon that loads the edit modal. Don't ask me why they aren't on the button...
    const element = e.target as HTMLElement;
    console.log(element);
    const subCategoryId = element.dataset.subCategoryId;
    console.log(subCategoryId);
    const expenseId = (e.target as HTMLElement).dataset.id;
    console.log(expenseId);
    fetch(`/api/maincategory/sub-category/${subCategoryId}`)
        .then(response => response.text())
        .then((category: string) => {
            const mainCategoryInput = <HTMLInputElement>document.getElementById(`editExpenseMainCategory-${expenseId}`);
            console.log(category);
            mainCategoryInput.value = category;

        }).catch(err => console.log(err));
}

export { loadMainCategoryOnSubCategorySelect, loadMainCategoryOnEditModalLoad };