import type Category from '../Models/Categories';
import getQueryParam from '../Utility/query-params';
import {formatAmountForEvent, formatAmountOnChange} from './format-amount';
import {
	autoSuggestIncomeSourceNames,
	autoSuggestSourceEventListener,
} from './source-autocomplete';

export function formatInputOnPageLoad(): void {
	const queryValue: number = parseInt((document.getElementById('querySelect') as HTMLSelectElement).value, 10);
	adjustFormBasedOnQueryValue(queryValue);
}

export function formatInputOnSelectListChange(): void {
	const selectListElement = (document.getElementById('querySelect') as HTMLSelectElement);
	selectListElement.addEventListener(
		'change',
		addSelectChangeEventListener,
		false,
	);
}

function adjustFormBasedOnQueryValue(queryValue: number): void {
	const firstInput = document.getElementById('qInput') as HTMLInputElement;
	const categorySelect = (
		document.getElementById('categorySelect') as HTMLSelectElement);
	const q: string | undefined = getQueryParam('Q');
	const query: string | undefined = getQueryParam('Query');
	const currentYear: string = new Date().getFullYear().toString();
	switch (queryValue) {
		// Date
		case 0:
			resetCategorySelect();
			resetNumbersForm();
			firstInput.type = 'date';
			break;
			// Month
		case 1:
			resetCategorySelect();
			resetNumbersForm();
			firstInput.type = 'month';
			break;
			// Quarter
		case 2:
			resetCategorySelect();
			resetNumbersForm();
			firstInput.type = 'month';
			break;
			// Year
		case 3:
			resetCategorySelect();
			firstInput.step = 'any';
			firstInput.min = '2012';
			firstInput.max = currentYear;
			firstInput.value = q && query === '3' ? q : currentYear;
			firstInput.type = 'number';
			break;
			// Amount
		case 4:
			resetCategorySelect();
			firstInput.value = '';
			firstInput.step = 'any';
			firstInput.min = '0.00';
			firstInput.value = q && query === '4' ? q : '0.00';
			firstInput.type = 'number';
			firstInput.classList.add('format-amount-js');
			formatAmountOnChange();
			break;
			// Notes
		case 5:
			firstInput.classList.remove('source-autosuggest-js');
			firstInput.classList.remove('ui-autocomplete-input');
			firstInput.removeEventListener(
				'input',
				autoSuggestSourceEventListener,
				true,
			);
			autoSuggestIncomeSourceNames();
			resetNumbersForm();
			resetCategorySelect();
			firstInput.value = q && query === '5' ? q : '';
			firstInput.type = 'text';

			break;
			// Source
		case 6:
			resetCategorySelect();
			resetNumbersForm();
			firstInput.type = 'text';
			firstInput.classList.add('source-autosuggest-js');
			autoSuggestIncomeSourceNames();
			firstInput.value = q && query === '6' ? q : '';
			break;
			// Category
		case 7:
			resetNumbersForm();
			firstInput.classList.add('display-none');
			categorySelect.classList.remove('display-none');
			fetch('/api/incomecategory')
				.then(async response => response.json())
				.then((data: Category[]) => {
					for (const category of data) {
						if (category.id.toString() === q) {
							categorySelect.add(
								new Option(
									category.category,
									category.id.toString(),
									true,
									true,
								),
							);
						} else {
							categorySelect.add(
								new Option(category.category, category.id.toString()),
							);
						}
					}
				})
				.catch(err => {
					console.log(err);
				});
			firstInput.type = 'text';
			break;
			// Subcategory
		default:
			firstInput.type = 'date';
			console.log('default... something went wrong');
	}
}

const addSelectChangeEventListener = (e: Event): void => {
	const queryValue = parseInt((e.target as HTMLSelectElement).value, 10);
	adjustFormBasedOnQueryValue(queryValue);
};

function resetNumbersForm(): void {
	const firstInput = document.getElementById('qInput') as HTMLInputElement;
	firstInput.removeAttribute('min');
	firstInput.removeAttribute('step');
	firstInput.value = '';
	firstInput.classList.remove('format-amount-js');
	firstInput.removeEventListener('change', formatAmountForEvent, true);
}

function resetCategorySelect(): void {
	const firstInput = document.getElementById('qInput') as HTMLInputElement;
	firstInput.classList.remove('display-none');
	const categorySelectList = (document.getElementById('categorySelect') as HTMLInputElement);
	categorySelectList.classList.add('display-none');

	while (categorySelectList.firstChild) {
		categorySelectList.removeChild(categorySelectList.firstChild);
	}
}
