const loadMainCategoryOnSubCategorySelect = (): void => {
	const subCategoryInputs = (
		document.querySelectorAll('.load-main-category-js')
	);
	subCategoryInputs.forEach(x => {
		x.addEventListener('change', loadMainCategory, false);
	},
	);
};

const loadMainCategory = (e: Event): void => {
	const subCategoryId = (e.target as HTMLSelectElement).value;
	console.log(subCategoryId);
	// Edit modals have an expenseId set as the data attribute - the add modal does not have that.
	const expenseId = (e.target as HTMLSelectElement).dataset.id;
	fetch(`/api/maincategory/sub-category/${subCategoryId}`)
	// A string is returned instead of json, so we just response.text() instead of response.json()
		.then(async response => response.text())
		.then((category: string) => {
			if (expenseId === '') {
				// If there is no expenseId, then it's for the add expense modal
				const mainCategoryInput = (
					document.getElementById('addExpenseMainCategory') as HTMLInputElement
				);
				mainCategoryInput.value = category;
			} else {
				const mainCategoryInput = (
					document.getElementById(`editExpenseMainCategory-${expenseId!}`) as HTMLInputElement
				);
				mainCategoryInput.value = category;
			}
		})
		.catch(err => {
			console.log(err);
		});
};

const loadMainCategoryOnEditModalLoad = (): void => {
	const editModals = (
		document.querySelectorAll('.load-main-category-edit-modal-js')
	);
	editModals.forEach(x => {
		x.addEventListener('click', loadMainCategoryForEditModal, false);
	},
	);
};

const loadMainCategoryForEditModal = (e: Event): void => {
	// These are two data attributes on every icon that loads the edit modal.
	const element = e.target as HTMLElement;
	const {subCategoryId} = element.dataset;
	const expenseId = (e.target as HTMLElement).dataset.id;
	fetch(`/api/maincategory/sub-category/${subCategoryId!}`)
		.then(async response => response.text())
		.then((category: string) => {
			const mainCategoryInput = (
				document.getElementById(`editExpenseMainCategory-${expenseId!}`) as HTMLInputElement
			);
			mainCategoryInput.value = category;
		})
		.catch(err => {
			console.log(err);
		});
};

const loadMainCategoryOnSplitLoad = (): void => {
	const subCategories = (
		document.querySelectorAll<HTMLSelectElement>('.load-main-category-js')
	);
	subCategories.forEach(x => {
		x.addEventListener('change', loadMainCategoryForSplit, false);
	},
	);
	subCategories.forEach(x => {
		loadMainCategoriesForSplit(x);
	});
};

const loadMainCategoriesForSplit = (element: HTMLElement): void => {
	const {subCategoryId} = element.dataset;
	const {index} = element.dataset;
	fetch(`/api/maincategory/sub-category/${subCategoryId!}`)
		.then(async response => response.text())
		.then((category: string) => {
			const mainCategory = document.getElementById(`mainCategory-${index!}`)!;
			mainCategory.innerHTML = category;
		})
		.catch(err => {
			console.log(err);
		});
};

const loadMainCategoryForSplit = (e: Event): void => {
	const element = e.target as HTMLSelectElement;
	const subCategoryId = element.value;
	const {index} = element.dataset;
	fetch(`/api/maincategory/sub-category/${subCategoryId}`)
		.then(async response => response.text())
		.then((category: string) => {
			const mainCategory = document.getElementById(`mainCategory-${index!}`)!;
			mainCategory.innerHTML = '';
			mainCategory.innerHTML = category;
		})
		.catch(err => {
			console.log(err);
		});
};

export {
	loadMainCategoryOnSubCategorySelect,
	loadMainCategoryOnEditModalLoad,
	loadMainCategoryOnSplitLoad,
};
