import type Category from '../Models/Categories';
import type MerchantSource from '../Models/MerchantSource';

// Sets the select lists display status on edit button click
function setSelectListOptionsOnEditButtonsClick() {
	const editButtons = document.querySelectorAll('.edit-rule-button-js');
	editButtons.forEach(x => {
		x.addEventListener('click', handleEditButtonClick, false);
	},
	);
}

function handleEditButtonClick(e: Event) {
	const element = e.target as HTMLElement;
	const {ruleType} = element.dataset;
	const selectLists: HTMLElement[] = Array.from(
		document.querySelectorAll('.filter-rule-hides-this-js'),
	);
	if (ruleType === 'Filter') {
		selectLists.forEach(x => {
			x.classList.add('display-none');
		});
	} else {
		selectLists.forEach(x => {
			x.classList.remove('display-none');
		});
	}
}

// Sets the select list options when you select the transaction type radio button
function setSelectListOptionsOnTransactionTypeChange() {
	const checkboxes = document.querySelectorAll('.transaction-type-radio-js');
	checkboxes.forEach(x => {
		x.addEventListener('change', handleTransactionTypeChange, false);
	},
	);
}

function handleTransactionTypeChange(e: Event) {
	const isExpense = Boolean((e.target as HTMLInputElement).value === '0');
	const isEdit = Boolean((e.target as HTMLElement).dataset.isEdit === 'True');
	if (isEdit) {
		const {id} = (e.target as HTMLElement).dataset;
		const categorySelect = (
			document.getElementById(`categorySelectList-${id!}`) as HTMLSelectElement);
		const merchantSourceSelect = (
			document.getElementById(`merchantSourceSelectList-${id!}`) as HTMLSelectElement);
		adjustSelectListOptions(isExpense, categorySelect, merchantSourceSelect);
	}

	if (!isEdit) {
		const categorySelect = (
			document.getElementById('categorySelectList') as HTMLSelectElement);
		const merchantSourceSelect = (
			document.getElementById('merchantSourceSelectList') as HTMLSelectElement);
		adjustSelectListOptions(isExpense, categorySelect, merchantSourceSelect);
	}
}

// Private helper function
function adjustSelectListOptions(
	isExpense: boolean,
	categorySelect: HTMLSelectElement,
	merchantSourceSelect: HTMLSelectElement,
): void {
	if (isExpense) {
		while (categorySelect.firstChild) {
			categorySelect.removeChild(categorySelect.firstChild);
		}

		while (merchantSourceSelect.firstChild) {
			merchantSourceSelect.removeChild(merchantSourceSelect.firstChild);
		}

		fetch('/api/subcategory')
			.then(async response => response.json())
			.then((data: Category[]) => {
				categorySelect.add(new Option('Select', undefined, true, false));
				categorySelect.add(new Option('NULL', undefined, false, false));
				for (const item of data) {
					categorySelect.add(
						new Option(item.category, item.id.toString(), false, false),
					);
				}
			})
			.catch(err => {
				console.error(err);
			});

		fetch('/api/merchants/dropdown')
			.then(async response => response.json())
			.then((data: MerchantSource[]) => {
				merchantSourceSelect.add(new Option('Select', undefined, true, false));
				merchantSourceSelect.add(new Option('NULL', undefined, false, false));
				for (const item of data) {
					merchantSourceSelect.add(
						new Option(item.name, item.id.toString(), false, false),
					);
				}
			})
			.catch(err => {
				console.error(err);
			});
	} else {
		while (categorySelect.firstChild) {
			categorySelect.removeChild(categorySelect.firstChild);
		}

		while (merchantSourceSelect.firstChild) {
			merchantSourceSelect.removeChild(merchantSourceSelect.firstChild);
		}

		fetch('/api/IncomeCategory')
			.then(async response => response.json())
			.then((data: Category[]) => {
				categorySelect.add(new Option('Select', undefined, true, false));
				categorySelect.add(new Option('NULL', undefined, false, false));
				for (const item of data) {
					categorySelect.add(
						new Option(item.category, item.id.toString(), false, false),
					);
				}
			})
			.catch(err => {
				console.error(err);
			});

		fetch('/api/incomesource/dropdown')
			.then(async response => response.json())
			.then((data: MerchantSource[]) => {
				merchantSourceSelect.add(new Option('Select', undefined, true, false));
				merchantSourceSelect.add(new Option('NULL', undefined, false, false));
				for (const item of data) {
					merchantSourceSelect.add(
						new Option(item.name, item.id.toString(), false, false),
					);
				}
			})
			.catch(err => {
				console.error(err);
			});
	}
}

// Removes Select Lists When Filter Option Is Checked
function removeSelectListsWhenFilterOptionIsChecked() {
	const ruleTypeRadio = document.querySelectorAll('.filter-rule-radio-js');
	ruleTypeRadio.forEach(x => {
		x.addEventListener('change', handleRuleTypeChange, false);
	},
	);
}

function handleRuleTypeChange(e: Event): void {
	const radioValue = (e.target as HTMLInputElement).value;
	const selectLists: HTMLElement[] = Array.from(
		document.querySelectorAll('.filter-rule-hides-this-js'),
	);

	if (radioValue === '1') {
		selectLists.forEach(x => {
			x.classList.add('display-none');
		});
	} else {
		selectLists.forEach(x => {
			x.classList.remove('display-none');
		});
	}
}

// These two add the category and merchant list when the add import rule button is clicked. Defaults to expense.
function formatCategorySelectListOnAddButtonClick(): void {
	const addImportRuleButton = (
		document.getElementById('addImportRuleButton') as HTMLButtonElement);
	addImportRuleButton.addEventListener(
		'click',
		addCategorySelectListValuesForExpense,
		false,
	);
}

function addCategorySelectListValuesForExpense(): void {
	const categorySelect = (
		document.getElementById('categorySelectList') as HTMLSelectElement);
	const merchantSourceSelect = (
		document.getElementById('merchantSourceSelectList') as HTMLSelectElement);
	fetch('/api/subcategory')
		.then(async response => response.json())
		.then((data: Category[]) => {
			for (const item of data) {
				categorySelect.add(
					new Option(item.category, item.id.toString(), false, false),
				);
			}
		})
		.catch(err => {
			console.error(err);
		});

	fetch('/api/merchants/dropdown')
		.then(async response => response.json())
		.then((data: MerchantSource[]) => {
			for (const item of data) {
				merchantSourceSelect.add(
					new Option(item.name, item.id.toString(), false, false),
				);
			}
		})
		.catch(err => {
			console.error(err);
		});
}

export {
	formatCategorySelectListOnAddButtonClick,
	removeSelectListsWhenFilterOptionIsChecked,
	setSelectListOptionsOnEditButtonsClick,
	setSelectListOptionsOnTransactionTypeChange,
};
