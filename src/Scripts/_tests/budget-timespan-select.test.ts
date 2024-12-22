describe('Hide Category When Savings Is Selected Add Modal', () => {
	beforeEach(() => {
		const checkbox = document.createElement('input');
		const selectList = document.createElement('select');
		checkbox.classList.add('handle-timespan-select-js');
		selectList.setAttribute('id', 'monthSelectList')

		document.body.appendChild(checkbox);
		document.body.appendChild(selectList);
	});
	afterEach(() => {
		document.querySelectorAll('.handle-timespan-select-js').forEach(x => {
			x.remove();
		});
		document.querySelectorAll('#monthSelectList').forEach(x => {
			x.remove();
		});
	})

	it('hides inputs when checked', () => {
		forceMonthSelectionWhenIncomeIsCheckedAddModal();
		const radioButton = document.querySelector('.handle-category-select-js') as HTMLInputElement;
		radioButton.checked = true;
		radioButton.value = '2';
		radioButton.dispatchEvent(new Event('change'));

		const table = document.getElementById('averagesTable') as HTMLTableElement;
		const categoryInputs = document.getElementById('categoryInputs') as HTMLInputElement;
		expect(table.classList.contains('visually-hidden')).toBe(true);
		expect(categoryInputs.classList.contains('visually-hidden')).toBe(true);
	});
});

export function forceMonthSelectionWhenIncomeIsCheckedAddModal() {
	const checkboxes = document.querySelectorAll('.handle-timespan-select-js');
	checkboxes.forEach(x => {
		x.addEventListener('change', handleMonthSelectionForAddModal, false);
	},
	);
}

function handleMonthSelectionForAddModal(e: Event): void {
	const isChecked
		= (e.target as HTMLInputElement).checked
		&& (e.target as HTMLInputElement).value === '2';
	const monthElement = document.getElementById(
		'monthSelectList',
	) as HTMLSelectElement;
	if (isChecked) {
		// eslint-disable-next-line @typescript-eslint/prefer-for-of
		for (let i = 0; i < monthElement.options.length; i++) {
			if (monthElement.options[i].text === 'Every') {
				monthElement.options[i].selected = true;
				monthElement.setAttribute('disabled', '');
			}
		}
	} else {
		monthElement.removeAttribute('disabled');
		monthElement.value = (new Date().getMonth() + 1).toString();
	}
}