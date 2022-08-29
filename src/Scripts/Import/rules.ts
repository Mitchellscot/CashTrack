import Category from "../Models/Categories";
import MerchantSource from "../Models/MerchantSource";

console.log('rules page');

formatCategorySelectListOnAddButtonClick();
removeSelectListsWhenFilterOptionIsChecked();
setSelectListOptionsOnTransactionTypeChange();

//this works fine for the add import rule modal but you need to make one for the edit modal that grabs the data-id
function setSelectListOptionsOnTransactionTypeChange() {
    const checkboxes = document.querySelectorAll('.transaction-type-radio-js');
    checkboxes.forEach(x => x.addEventListener('change', handleTransactionTypeChange, false));
}

function handleTransactionTypeChange(e: Event) {
    const radioValue = (e.target as HTMLInputElement).value;
    const categorySelect = <HTMLSelectElement>document.getElementById('categorySelectList');
    const merchantSourceSelect = <HTMLSelectElement>document.getElementById('merchantSourceSelectList');
    if (radioValue === "0") {
        while (categorySelect.firstChild) {
            categorySelect.removeChild(categorySelect.firstChild);
        }
        while (merchantSourceSelect.firstChild) {
            merchantSourceSelect.removeChild(merchantSourceSelect.firstChild);
        }
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
                for (var item of data) {
                    categorySelect.add(new Option(item.category, item.id.toString(), false, false))
                }
            }).catch(err => console.log(err));

        fetch('/api/incomesource/dropdown')
            .then(response => response.json())
            .then((data: Array<MerchantSource>) => {
                for (var item of data) {
                    merchantSourceSelect.add(new Option(item.name, item.id.toString(), false, false))
                }
            }).catch(err => console.log(err));
    }
}

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