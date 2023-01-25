import {hideCategoryWhenSavingsIsSelectedAddModal} from '../Utility/budget-type-switch';

describe('Hide Category When Savings Is Selected Add Modal', () => {
	beforeEach(() => {
		const radioButton = document.createElement('input');
		const table = document.createElement('table');
		const categoryInputs = document.createElement('input');
		radioButton.classList.add('handle-category-select-js');

		categoryInputs.setAttribute('id', 'categoryInputs')
		table.setAttribute('id', 'averagesTable');

		document.body.appendChild(radioButton);
		document.body.appendChild(categoryInputs);
		document.body.appendChild(table);
	});
	afterEach(() => {
		document.querySelectorAll('.handle-category-select-js').forEach(x => {
			x.remove();
		});
		document.querySelectorAll('#categoryInputs').forEach(x => {
			x.remove();
		});
		document.querySelectorAll('#averagesTable').forEach(x => {
			x.remove();
		});
	})

	it('hides inputs when checked', () => {
		hideCategoryWhenSavingsIsSelectedAddModal();
		const radioButton = document.querySelector('.handle-category-select-js') as HTMLInputElement;
		radioButton.checked = true;
		radioButton.value = '2';
		radioButton.dispatchEvent(new Event('change'));

		const table = document.getElementById('averagesTable') as HTMLTableElement;
		const categoryInputs = document.getElementById('categoryInputs') as HTMLInputElement;
		expect(table.classList.contains('visually-hidden')).toBe(true);
		expect(categoryInputs.classList.contains('visually-hidden')).toBe(true);
	});
	it('doesnt hide when inputs when checked with wrong value', () => {
		hideCategoryWhenSavingsIsSelectedAddModal();
		const radioButton = document.querySelector('.handle-category-select-js') as HTMLInputElement;
		radioButton.checked = true;
		radioButton.value = '1';
		radioButton.dispatchEvent(new Event('change'));

		const table = document.getElementById('averagesTable') as HTMLTableElement;
		const categoryInputs = document.getElementById('categoryInputs') as HTMLInputElement;
		expect(table.classList.contains('visually-hidden')).toBe(false);
		expect(categoryInputs.classList.contains('visually-hidden')).toBe(false);
	});
	it('unhides inputs when checked again', () => {
		hideCategoryWhenSavingsIsSelectedAddModal();
		const radioButton = document.querySelector('.handle-category-select-js') as HTMLInputElement;
		radioButton.checked = true;
		radioButton.value = '2';
		radioButton.dispatchEvent(new Event('change'));

		const table = document.getElementById('averagesTable') as HTMLTableElement;
		const categoryInputs = document.getElementById('categoryInputs') as HTMLInputElement;
		expect(table.classList.contains('visually-hidden')).toBe(true);
		expect(categoryInputs.classList.contains('visually-hidden')).toBe(true);
		radioButton.checked = false;
		radioButton.value = '1';
		radioButton.dispatchEvent(new Event('change'));
		const table2 = document.getElementById('averagesTable') as HTMLTableElement;
		const categoryInputs2 = document.getElementById('categoryInputs') as HTMLInputElement;
		expect(categoryInputs2.classList.contains('visually-hidden')).toBe(false);
	});
});





//export function hideCategoryWhenSavingsIsSelectedAddModal() {
//	const radioButtons = document.querySelectorAll('.handle-category-select-js');
//	radioButtons.forEach(x => {
//		x.addEventListener('change', handleSavingsIsSelectedForAddModal, false);
//	},
//	);
//}

//export const handleSavingsIsSelectedForAddModal = (e: Event): void => {
//	const isChecked
//		= (e.target as HTMLInputElement).checked
//		&& (e.target as HTMLInputElement).value === '2';
//	const averagesTable = (
//		document.getElementById('averagesTable') as HTMLTableElement);
//	if (isChecked) {
//		document.getElementById('categoryInputs')?.classList.add('visually-hidden');
//		averagesTable.classList.add('visually-hidden');
//	} else {
//		document
//			.getElementById('categoryInputs')
//			?.classList.remove('visually-hidden');
//	}
//};