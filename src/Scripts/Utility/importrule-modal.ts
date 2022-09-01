import Category from "../Models/Categories";
import MerchantSource from "../Models/MerchantSource";

//sets the select lists display status on edit button click
function setSelectListOptionsOnEditButtonsClick() {
    const editButtons = document.querySelectorAll('.edit-rule-button-js');
    editButtons.forEach(x => x.addEventListener('click', handleEditButtonClick, false));
}
function handleEditButtonClick(e: Event) {
    const element = e.target as HTMLElement;
    const ruleType = element.dataset.ruleType;
    const selectLists: HTMLElement[] = Array.from(document.querySelectorAll('.filter-rule-hides-this-js'));
    ruleType === 'Filter' ?
        selectLists.forEach((x) => x.classList.add('display-none')) :
        selectLists.forEach((x) => x.classList.remove('display-none'));
}
//sets the select list options when you select the transaction type radio button
function setSelectListOptionsOnTransactionTypeChange() {
    const checkboxes = document.querySelectorAll('.transaction-type-radio-js');
    checkboxes.forEach(x => x.addEventListener('change', handleTransactionTypeChange, false));
}

function handleTransactionTypeChange(e: Event) {
    const isExpense = Boolean((e.target as HTMLInputElement).value === "0");
    const isEdit = Boolean((e.target as HTMLElement).dataset.isEdit === "True");
    if (isEdit) {
        const Id = (e.target as HTMLElement).dataset.id;
        const categorySelect = <HTMLSelectElement>document.getElementById(`categorySelectList-${Id}`);
        const merchantSourceSelect = <HTMLSelectElement>document.getElementById(`merchantSourceSelectList-${Id}`);
        adjustSelectListOptions(isExpense, categorySelect, merchantSourceSelect);
    }
    if (!isEdit) {
        const categorySelect = <HTMLSelectElement>document.getElementById('categorySelectList');
        const merchantSourceSelect = <HTMLSelectElement>document.getElementById('merchantSourceSelectList');
        adjustSelectListOptions(isExpense, categorySelect, merchantSourceSelect);
    }

}
//private helper function
function adjustSelectListOptions(isExpense: boolean, categorySelect: HTMLSelectElement, merchantSourceSelect: HTMLSelectElement): void {
    if (isExpense) {
        while (categorySelect.firstChild) {
            categorySelect.removeChild(categorySelect.firstChild);
        }
        while (merchantSourceSelect.firstChild) {
            merchantSourceSelect.removeChild(merchantSourceSelect.firstChild);
        }
        fetch('/api/subcategory')
            .then(response => response.json())
            .then((data: Array<Category>) => {
                categorySelect.add(new Option('Select', undefined, true, false))
                categorySelect.add(new Option('NULL', undefined, false, false))
                for (var item of data) {
                    categorySelect.add(new Option(item.category, item.id.toString(), false, false))
                }
            }).catch(err => console.log(err));

        fetch('/api/merchants/dropdown')
            .then(response => response.json())
            .then((data: Array<MerchantSource>) => {
                merchantSourceSelect.add(new Option('Select', undefined, true, false))
                merchantSourceSelect.add(new Option('NULL', undefined, false, false))
                for (var item of data) {
                    merchantSourceSelect.add(new Option(item.name, item.id.toString(), false, false))
                }
            }).catch(err => console.log(err));
    }
    else {
        while (categorySelect.firstChild) {
            categorySelect.removeChild(categorySelect.firstChild);
        }
        while (merchantSourceSelect.firstChild) {
            merchantSourceSelect.removeChild(merchantSourceSelect.firstChild);
        }
        fetch('/api/IncomeCategory')
            .then(response => response.json())
            .then((data: Array<Category>) => {
                categorySelect.add(new Option('Select', undefined, true, false))
                categorySelect.add(new Option('NULL', undefined, false, false))
                for (var item of data) {
                    categorySelect.add(new Option(item.category, item.id.toString(), false, false))
                }
            }).catch(err => console.log(err));

        fetch('/api/incomesource/dropdown')
            .then(response => response.json())
            .then((data: Array<MerchantSource>) => {
                merchantSourceSelect.add(new Option('Select', undefined, true, false))
                merchantSourceSelect.add(new Option('NULL', undefined, false, false))
                for (var item of data) {
                    merchantSourceSelect.add(new Option(item.name, item.id.toString(), false, false))
                }
            }).catch(err => console.log(err));
    }
}

//Removes Select Lists When Filter Option Is Checked
function removeSelectListsWhenFilterOptionIsChecked() {
    const ruleTypeRadio = document.querySelectorAll('.filter-rule-radio-js');
    ruleTypeRadio.forEach(x => x.addEventListener('change', handleRuleTypeChange, false));
}

function handleRuleTypeChange(e: Event): void {
    const radioValue = (e.target as HTMLInputElement).value;
    const selectLists: HTMLElement[] = Array.from(document.querySelectorAll('.filter-rule-hides-this-js'));

    if (radioValue === "1") {
        selectLists.forEach((x) => x.classList.add('display-none'));
    }
    else {
        selectLists.forEach((x) => x.classList.remove('display-none'));
    }
}

//these two add the category and merchant list when the add import rule button is clicked. Defaults to expense.
function formatCategorySelectListOnAddButtonClick(): void {
    let addImportRuleButton = <HTMLButtonElement>document.getElementById("addImportRuleButton");
    addImportRuleButton.addEventListener('click', addCategorySelectListValuesForExpense, false);
}

function addCategorySelectListValuesForExpense(): void {
    const categorySelect = <HTMLSelectElement>document.getElementById('categorySelectList');
    const merchantSourceSelect = <HTMLSelectElement>document.getElementById('merchantSourceSelectList');
    fetch('/api/subcategory')
        .then(response => response.json())
        .then((data: Array<Category>) => {
            for (var item of data) {
                categorySelect.add(new Option(item.category, item.id.toString(), false, false))
            }
        }).catch(err => console.log(err));

    fetch('/api/merchants/dropdown')
        .then(response => response.json())
        .then((data: Array<MerchantSource>) => {
            for (var item of data) {
                merchantSourceSelect.add(new Option(item.name, item.id.toString(), false, false))
            }
        }).catch(err => console.log(err));
}

export { formatCategorySelectListOnAddButtonClick, removeSelectListsWhenFilterOptionIsChecked, setSelectListOptionsOnEditButtonsClick, setSelectListOptionsOnTransactionTypeChange }