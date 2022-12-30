import type CategoryAverages from '../Models/CategoryAverages';

export const loadAveragesOnSelectListChange = (): void => {
	const subCategoryInputs = (
		document.querySelectorAll('.load-main-category-js')
	);
	subCategoryInputs.forEach(x => {
		x.addEventListener('change', loadAverages, false);
	},
	);
};

const loadAverages = (e: Event): void => {
	const averagesTable = (
		document.getElementById('averagesTable') as HTMLTableElement
	);
	const subCategoryId = (e.target as HTMLSelectElement).value;
	const sixMonthAverages = (
		document.getElementById('sixMonthAverages') as HTMLTableCellElement
	);
	const thisYearAverages = (
		document.getElementById('thisYearAverages') as HTMLTableCellElement
	);
	const lastYearAverages = (
		document.getElementById('lastYearAverages') as HTMLTableCellElement
	);
	const twoYearsAgoAverages = (
		document.getElementById('twoYearsAgoAverages') as HTMLTableCellElement
	);
	const sixMonthTotals = (
		document.getElementById('sixMonthTotals') as HTMLTableCellElement
	);
	const thisYearTotals = (
		document.getElementById('thisYearTotals') as HTMLTableCellElement
	);
	const lastYearTotals = (
		document.getElementById('lastYearTotals') as HTMLTableCellElement
	);
	const twoYearsAgoTotals = (
		document.getElementById('twoYearsAgoTotals') as HTMLTableCellElement
	);

	fetch(`/api/budget/averages-and-totals/${subCategoryId}`)
		.then(async response => response.json())
		.then((averages: CategoryAverages) => {
			averagesTable.classList.remove('visually-hidden');
			sixMonthAverages.textContent = averages.sixMonthAverages.toString();
			thisYearAverages.textContent = averages.thisYearAverages.toString();
			lastYearAverages.textContent = averages.lastYearAverages.toString();
			twoYearsAgoAverages.textContent = averages.twoYearsAgoAverages.toString();
			sixMonthTotals.textContent = averages.sixMonthTotals.toString();
			thisYearTotals.textContent = averages.thisYearTotals.toString();
			lastYearTotals.textContent = averages.lastYearTotals.toString();
			twoYearsAgoTotals.textContent = averages.twoYearsAgoTotals.toString();
		})
		.catch(err => {
			console.error(err);
		});
};

export const loadMainCategoryOnSubCategorySelect = (): void => {
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

	fetch(`/api/maincategory/sub-category/${subCategoryId}`)
		.then(async response => response.text())
		.then((category: string) => {
			const mainCategoryInput = (
				document.getElementById('mainCategoryInput') as HTMLInputElement
			);
			mainCategoryInput.value = category;
		})
		.catch(err => {
			console.error(err);
		});
};

export const loadAveragesOnSelectListChangeEdit = (): void => {
	const subCategoryInputs = (
		document.querySelectorAll('.load-main-category-edit-js')
	);
	subCategoryInputs.forEach(x => {
		x.addEventListener('change', loadAveragesEdit, false);
	},
	);
};

const loadAveragesEdit = (e: Event): void => {
	const {id} = (e.target as HTMLElement).dataset;
	const averagesTable = (
		document.getElementById(`averagesTable-${id!}`) as HTMLTableElement
	);
	const subCategoryId = (e.target as HTMLSelectElement).value;
	const sixMonthAverages = (
		document.getElementById(`sixMonthAverages-${id!}`) as HTMLTableCellElement
	);
	const thisYearAverages = (
		document.getElementById(`thisYearAverages-${id!}`) as HTMLTableCellElement
	);
	const lastYearAverages = (
		document.getElementById(`lastYearAverages-${id!}`) as HTMLTableCellElement
	);
	const twoYearsAgoAverages = (
		document.getElementById(`twoYearsAgoAverages-${id!}`) as HTMLTableCellElement
	);
	const sixMonthTotals = (
		document.getElementById(`sixMonthTotals-${id!}`) as HTMLTableCellElement
	);
	const thisYearTotals = (
		document.getElementById(`thisYearTotals-${id!}`) as HTMLTableCellElement
	);
	const lastYearTotals = (
		document.getElementById(`lastYearTotals-${id!}`) as HTMLTableCellElement
	);
	const twoYearsAgoTotals = (
		document.getElementById(`twoYearsAgoTotals-${id!}`) as HTMLTableCellElement
	);

	fetch(`/api/budget/averages-and-totals/${subCategoryId}`)
		.then(async response => response.json())
		.then((averages: CategoryAverages) => {
			averagesTable.classList.remove('visually-hidden');
			sixMonthAverages.textContent = averages.sixMonthAverages.toString();
			thisYearAverages.textContent = averages.thisYearAverages.toString();
			lastYearAverages.textContent = averages.lastYearAverages.toString();
			twoYearsAgoAverages.textContent = averages.twoYearsAgoAverages.toString();
			sixMonthTotals.textContent = averages.sixMonthTotals.toString();
			thisYearTotals.textContent = averages.thisYearTotals.toString();
			lastYearTotals.textContent = averages.lastYearTotals.toString();
			twoYearsAgoTotals.textContent = averages.twoYearsAgoTotals.toString();
		})
		.catch(err => {
			console.error(err);
		});
};

export const loadMainCategoryOnSubCategorySelectEdit = (): void => {
	const subCategoryInputs = (
		document.querySelectorAll('.load-main-category-edit-js')
	);
	subCategoryInputs.forEach(x => {
		x.addEventListener('change', loadMainCategoryEdit, false);
	},
	);
};

const loadMainCategoryEdit = (e: Event): void => {
	const subCategoryId = (e.target as HTMLSelectElement).value;
	const {id} = (e.target as HTMLElement).dataset;
	fetch(`/api/maincategory/sub-category/${subCategoryId}`)
		.then(async response => response.text())
		.then((category: string) => {
			const mainCategoryInput = (
				document.getElementById(`mainCategoryInput-${id!}`) as HTMLInputElement
			);
			mainCategoryInput.value = category;
		})
		.catch(err => {
			console.error(err);
		});
};
