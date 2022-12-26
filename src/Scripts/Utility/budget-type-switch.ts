export function hideCategoryWhenSavingsIsSelectedAddModal() {
	const radioButtons = document.querySelectorAll('.handle-category-select-js');
	radioButtons.forEach(x => {
		x.addEventListener('change', handleSavingsIsSelectedForAddModal, false);
	},
	);
}

export const handleSavingsIsSelectedForAddModal = (e: Event): void => {
	const isChecked
    = (e.target as HTMLInputElement).checked
    && (e.target as HTMLInputElement).value === '2';
	const averagesTable = (
		document.getElementById('averagesTable') as HTMLTableElement);
	if (isChecked) {
		document.getElementById('categoryInputs')?.classList.add('visually-hidden');
		averagesTable.classList.add('visually-hidden');
	} else {
		document
			.getElementById('categoryInputs')
			?.classList.remove('visually-hidden');
	}
};

export function hideCategoryWhenSavingsIsSelectedEditModal() {
	const radioButtons = document.querySelectorAll(
		'.handle-category-select-edit-js',
	);
	radioButtons.forEach(x => {
		x.addEventListener('change', handleSavingsIsSelectedForEditModal, false);
	},
	);
}

export const handleSavingsIsSelectedForEditModal = (e: Event): void => {
	const isChecked
    = (e.target as HTMLInputElement).checked
    && (e.target as HTMLInputElement).value === '2';
	const {id} = (e.target as HTMLElement).dataset;
	const averagesTable = (
		document.getElementById(`averagesTable-${id!}`) as HTMLTableElement);
	if (isChecked) {
		document
			.getElementById(`categoryInputs-${id!}`)
			?.classList.add('visually-hidden');
		averagesTable.classList.add('visually-hidden');
	} else {
		document
			.getElementById(`categoryInputs-${id!}`)
			?.classList.remove('visually-hidden');
	}
};
