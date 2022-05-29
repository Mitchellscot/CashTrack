import Category from '../Models/Categories';
import getQueryParam from '../Utility/query-params';
import { autoSuggestMerchantNames, autoSuggestEventListener } from './merchant-autocomplete';
import { formatAmountOnChange, formatAmountForEvent } from './format-amount';

const adjustFormBasedOnQueryValue = (queryValue: number) => {
    const firstInput = <HTMLInputElement>document.getElementById('qInput');
    const categorySelect = <HTMLSelectElement>document.getElementById('categorySelect');
    const Q: string | undefined = getQueryParam('Q');
    const Query: string | undefined = getQueryParam('Query');

    switch (queryValue) {
        //date
        case 0:
            resetCategorySelect();
            resetNumbersForm();
            resetSecondInputForm();
            firstInput.type = 'date';
            break;
        //date range
        case 1:
            resetCategorySelect();
            resetNumbersForm();
            showSecondInput();
            firstInput.type = 'date';
            break;
        //month
        case 2:
            resetCategorySelect();
            resetNumbersForm();
            resetSecondInputForm();
            firstInput.type = 'month';
            break;
        //quarter
        case 3:
            resetCategorySelect();
            resetNumbersForm();
            resetSecondInputForm();
            firstInput.type = 'month';
            break;
        //year
        case 4:
            resetCategorySelect();
            resetSecondInputForm();
            firstInput.type = 'number';
            firstInput.step = 'any';
            firstInput.min = '2012';
            const currentYear: string = (new Date().getFullYear()).toString();
            firstInput.max = currentYear;
            Q && Query === '4' ? firstInput.value = Q! : firstInput.value = currentYear;
            firstInput.type = 'number';
            break;
        //amount
        case 5:
            firstInput.value = '';
            firstInput.min = '0.00';
            resetCategorySelect();
            resetSecondInputForm();
            Q && Query === '5' ? firstInput.value = Q : firstInput.value = '0.00';
            firstInput.type = 'number';
            firstInput.classList.add('format-amount-js');
            formatAmountOnChange();
            break;
        //notes
        case 6:
            firstInput.classList.remove('merchant-autosuggest-js');
            firstInput.classList.remove('ui-autocomplete-input');
            //this doesn't really work but whatever... works for formatAmount event listener
            firstInput.removeEventListener('input', autoSuggestEventListener, true);
            autoSuggestMerchantNames();
            resetCategorySelect();
            resetNumbersForm();
            resetSecondInputForm();
            Q && Query === '6' ? firstInput.value = Q : firstInput.value = '';
            firstInput.type = 'text';
            break;
        //merchant
        case 7:
            resetCategorySelect();
            resetNumbersForm();
            resetSecondInputForm();
            firstInput.type = 'text';
            firstInput.classList.add('merchant-autosuggest-js');
            autoSuggestMerchantNames();
            Q && Query === '7' ? firstInput.value = Q : firstInput.value = '';
            break;
        //subcategory
        case 8:
            resetNumbersForm();
            resetSecondInputForm();
            firstInput.classList.add('display-none');
            categorySelect.classList.remove('display-none');
            fetch('/api/subcategory')
                .then(response => response.json())
                .then((data: Array<Category>) => {
                    for (var category of data) {
                        if (category.id.toString() === Q) {
                            categorySelect.add(new Option(category.category, category.id.toString(), true, true))
                        }
                        categorySelect.add(new Option(category.category, category.id.toString()))
                    }
                }).catch(err => console.log(err));
            firstInput.type = 'text';
            break;
        //main category
        case 9:
            resetNumbersForm();
            resetSecondInputForm();
            firstInput.classList.add('display-none');
            categorySelect.classList.remove('display-none');
            fetch('/api/maincategory')
                .then(response => response.json())
                .then((data: Array<Category>) => {
                    for (var category of data) {
                        if (category.id.toString() === Q) {
                            categorySelect.add(new Option(category.category, category.id.toString(), true, true))
                        }
                        categorySelect.add(new Option(category.category, category.id.toString()))
                    }
                }).catch(err => console.log(err));
            firstInput.type = 'text';
            break;
        //tag
        case 10:
            resetCategorySelect();
            resetNumbersForm();
            resetSecondInputForm();
            firstInput.type = 'text';
            console.log('not implemented yet');
            break;
        default:
            firstInput.type = 'date';
            console.log('default... something went wrong');
    }
}

const formatInputsOnSelectListChange = (): void => {
    let selectListElement = <HTMLSelectElement>document.getElementById("querySelect");
    selectListElement.addEventListener('change', addSelectChangeEventListener, false);
}

const addSelectChangeEventListener = (e: Event): void => {
    const queryValue = parseInt((e.target as HTMLSelectElement).value);
    adjustFormBasedOnQueryValue(queryValue);
}

const formatInputsOnPageLoad = (): void => {
    let queryValue: number = parseInt((<HTMLSelectElement>document.getElementById("querySelect")).value);
    adjustFormBasedOnQueryValue(queryValue);
}

function showSecondInput(): void {
    const secondInput = <HTMLInputElement>document.getElementById('q2Input');
    secondInput.classList.remove('display-none');
    secondInput.classList.add('w-25');
    const firstInput = <HTMLInputElement>document.getElementById('qInput');
    firstInput.classList.remove('w-50');
    firstInput.classList.add('w-25');
}
function resetSecondInputForm(): void {
    document.getElementById("qInput")?.classList.add("w-50");
    const secondInput = <HTMLInputElement>document.getElementById('q2Input');
    secondInput.value = '';
    secondInput.classList.remove('w-25');
    secondInput.classList.add('display-none');
}
function resetNumbersForm(): void {
    const firstInput = <HTMLInputElement>document.getElementById('qInput');
    firstInput.removeAttribute('min');
    firstInput.removeAttribute('step');
    firstInput.value = '';
    firstInput.classList.remove('format-amount-js');
    firstInput.removeEventListener('change', formatAmountForEvent, true);
}
function resetCategorySelect(): void {
    const firstInput = <HTMLInputElement>document.getElementById('qInput');
    firstInput.classList.remove('display-none');
    const categorySelectList = <HTMLInputElement>document.getElementById('categorySelect');
    categorySelectList.classList.add('display-none');

    while (categorySelectList.firstChild) {
        categorySelectList.removeChild(categorySelectList.firstChild);
    }
}

export { formatInputsOnSelectListChange, formatInputsOnPageLoad };