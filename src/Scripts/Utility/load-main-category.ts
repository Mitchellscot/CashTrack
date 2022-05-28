const loadMainCategoryOnSubCategorySelect = (): void => {
    const subCategoryInputs = <NodeListOf<HTMLSelectElement>>document.querySelectorAll('.load-main-category-js');
    subCategoryInputs.forEach(x => x.addEventListener('change', (e: Event) => {
        const subCategoryId = (e.target as HTMLSelectElement).value;
        //edit modals have an expenseId set as the data attribute - the add modal does not have that.
        const expenseId = (e.target as HTMLSelectElement).dataset.id;
        fetch(`/api/maincategory/sub-category/${subCategoryId}`)
            //a string is returned instead of json, so we just response.text() instead of response.json()
            .then(response => response.text())
            .then((category: string) => {
                if (expenseId !== undefined) {
                    const mainCategoryInput = <HTMLInputElement>document.getElementById(`editExpenseMainCategory-${expenseId}`);
                    mainCategoryInput.value = category;
                }
                else {
                    //if there is no expenseId, then it's for the add expense modal
                    const mainCategoryInput = <HTMLInputElement>document.getElementById(`addExpenseMainCategory`);
                    mainCategoryInput.value = category;
                }

            }).catch(err => console.log(err));

    }), false);
}

export default loadMainCategoryOnSubCategorySelect;