import Category from '../Models/Categories';
import getQueryParam from '../Utility/query-params';
import { formatAmountForEvent, formatAmountOnChange } from './format-amount';
import { autoSuggestIncomeSourceNames, autoSuggestSourceEventListener } from './source-autocomplete';

export function formatInputOnPageLoad(): void {
    let queryValue: number = parseInt((<HTMLSelectElement>document.getElementById("querySelect")).value);
    adjustFormBasedOnQueryValue(queryValue);
}

export function formatInputOnSelectListChange(): void {
    let selectListElement = <HTMLSelectElement>document.getElementById("querySelect");
    selectListElement.addEventListener('change', addSelectChangeEventListener, false);
}

function adjustFormBasedOnQueryValue(queryValue: number): void {
    const firstInput = <HTMLInputElement>document.getElementById('qInput');
    const categorySelect = <HTMLSelectElement>document.getElementById('categorySelect');
    const Q: string | undefined = getQueryParam('Q');
    const Query: string | undefined = getQueryParam('Query');

    switch (queryValue) {
        //date
        case 0:
            resetCategorySelect();
            resetNumbersForm();
            firstInput.type = 'date';
            break;
        //month
        case 1:
            resetCategorySelect();
            resetNumbersForm();
            firstInput.type = 'month';
            break;
        //quarter
        case 2:
            resetCategorySelect();
            resetNumbersForm();
            firstInput.type = 'month';
            break;
        //year
        case 3:
            resetCategorySelect();
            firstInput.step = 'any';
            firstInput.min = '2012';
            const currentYear: string = (new Date().getFullYear()).toString();
            firstInput.max = currentYear;
            Q && Query === '3' ? firstInput.value = Q! : firstInput.value = currentYear;
            firstInput.type = 'number';
            break;
        //amount
        case 4:
            resetCategorySelect();
            firstInput.value = '';
            firstInput.step = 'any';
            firstInput.min = '0.00';
            Q && Query === '4' ? firstInput.value = Q : firstInput.value = '0.00';
            firstInput.type = 'number';
            firstInput.classList.add('format-amount-js');
            formatAmountOnChange();
            break;
        //notes
        case 5:
            firstInput.classList.remove('source-autosuggest-js');
            firstInput.classList.remove('ui-autocomplete-input');
            firstInput.removeEventListener('input', autoSuggestSourceEventListener, true);
            autoSuggestIncomeSourceNames();
            resetNumbersForm();
            resetCategorySelect();
            Q && Query === '5' ? firstInput.value = Q : firstInput.value = '';
            firstInput.type = 'text';

            break;
        //source
        case 6:
            resetCategorySelect();
            resetNumbersForm();
            firstInput.type = 'text';
            firstInput.classList.add('source-autosuggest-js');
            autoSuggestIncomeSourceNames();
            Q && Query === '6' ? firstInput.value = Q : firstInput.value = '';
            break;
        //category
        case 7:
            resetNumbersForm();
            firstInput.classList.add('display-none');
            categorySelect.classList.remove('display-none');
            fetch('/api/incomecategory')
                .then(response => response.json())
                .then((data: Array<Category>) => {
                    for (var category of data) {
                        if (category.id.toString() === Q) {
                            categorySelect.add(new Option(category.category, category.id.toString(), true, true))
                        }
                        else {
                            categorySelect.add(new Option(category.category, category.id.toString()))
                        }
                    }
                }).catch(err => console.log(err));
            firstInput.type = 'text';
            break;
        //subcategory
        default:
            firstInput.type = 'date';
            console.log('default... something went wrong');
    }
}

const addSelectChangeEventListener = (e: Event): void => {
    const queryValue = parseInt((e.target as HTMLSelectElement).value);
    adjustFormBasedOnQueryValue(queryValue);
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