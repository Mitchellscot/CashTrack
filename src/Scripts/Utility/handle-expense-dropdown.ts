import type Category from '../Models/Categories';
import getQueryParam from '../Utility/query-params';
import {
	autoSuggestMerchantNames,
	autoSuggestEventListener,
} from './merchant-autocomplete';
import {formatAmountOnChange, formatAmountForEvent} from './format-amount';

const adjustFormBasedOnQueryValue = (queryValue: number) => {
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
			resetSecondInputForm();
			firstInput.type = 'date';
			break;
			// Date range
		case 1:
			resetCategorySelect();
			resetNumbersForm();
			showSecondInput();
			firstInput.type = 'date';
			break;
			// Month
		case 2:
			resetCategorySelect();
			resetNumbersForm();
			resetSecondInputForm();
			firstInput.type = 'month';
			break;
			// Quarter
		case 3:
			resetCategorySelect();
			resetNumbersForm();
			resetSecondInputForm();
			firstInput.type = 'month';
			break;
			// Year
		case 4:
			resetCategorySelect();
			resetSecondInputForm();
			firstInput.step = 'any';
			firstInput.min = '1900';
			firstInput.max = currentYear;
			firstInput.value = q && query === '4' ? q : currentYear;
			firstInput.type = 'number';
			break;
			// Amount
		case 5:
			firstInput.value = '';
			firstInput.min = '0.00';
			resetCategorySelect();
			resetSecondInputForm();
			firstInput.value = q && query === '5' ? q : '0.00';
			firstInput.type = 'number';
			firstInput.classList.add('format-amount-js');
			formatAmountOnChange();
			break;
			// Notes
		case 6:
			firstInput.classList.remove('merchant-autosuggest-js');
			firstInput.classList.remove('ui-autocomplete-input');
			// This doesn't really work but whatever... works for formatAmount event listener
			firstInput.removeEventListener('input', autoSuggestEventListener, true);
			autoSuggestMerchantNames();
			resetCategorySelect();
			resetNumbersForm();
			resetSecondInputForm();
			firstInput.value = q && query === '6' ? q : '';
			firstInput.type = 'text';
			break;
			// Merchant
		case 7:
			resetCategorySelect();
			resetNumbersForm();
			resetSecondInputForm();
			firstInput.type = 'text';
			firstInput.classList.add('merchant-autosuggest-js');
			autoSuggestMerchantNames();
			firstInput.value = q && query === '7' ? q : '';
			break;
			// Subcategory
		case 8:
			resetCategorySelect();
			resetNumbersForm();
			resetSecondInputForm();
			firstInput.classList.add('display-none');
			categorySelect.classList.remove('display-none');
			fetch('/api/subcategory')
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
					console.error(err);
				});
			firstInput.type = 'text';
			break;
			// Main category
		case 9:
			resetCategorySelect();
			resetNumbersForm();
			resetSecondInputForm();
			firstInput.classList.add('display-none');
			categorySelect.classList.remove('display-none');
			fetch('/api/maincategory')
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
					console.error(err);
				});
			firstInput.type = 'text';
			break;
			// Tag
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
};

const formatInputsOnSelectListChange = (): void => {
	const selectListElement = (
		document.getElementById('querySelect') as HTMLSelectElement);
	selectListElement.addEventListener(
		'change',
		addSelectChangeEventListener,
		false,
	);
};

const addSelectChangeEventListener = (e: Event): void => {
	const queryValue = parseInt((e.target as HTMLSelectElement).value, 10);
	adjustFormBasedOnQueryValue(queryValue);
};

const formatInputsOnPageLoad = (): void => {
	const queryValue: number = parseInt(
		(document.getElementById('querySelect') as HTMLSelectElement).value, 10);
	adjustFormBasedOnQueryValue(queryValue);
};

function showSecondInput(): void {
	const secondInput = document.getElementById('q2Input') as HTMLInputElement;
	secondInput.classList.remove('display-none');
	secondInput.classList.add('w-25');
	const firstInput = document.getElementById('qInput') as HTMLInputElement;
	firstInput.classList.remove('w-50');
	firstInput.classList.add('w-25');
}

function resetSecondInputForm(): void {
	document.getElementById('qInput')?.classList.add('w-50');
	const secondInput = document.getElementById('q2Input') as HTMLInputElement;
	secondInput.value = '';
	secondInput.classList.remove('w-25');
	secondInput.classList.add('display-none');
}

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

export {formatInputsOnSelectListChange, formatInputsOnPageLoad};
