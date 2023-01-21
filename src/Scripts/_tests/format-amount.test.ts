import {formatAmount, formatAmountOnChange, formatAmountForEvent} from '../Utility/format-amount';

describe('formatAmountOnChange', () => {

	beforeEach(() => {
		const input = document.createElement('input');
		input.classList.add('format-amount-js');
		input.value = '100.575928';
		document.body.appendChild(input);
	});

	afterEach(() => {
		document.querySelectorAll('.format-amount-js').forEach(x => {
			x.remove();
		});
	});

	it('should format the input to one decimal place', () => {
		const input = document.querySelector('.format - amount - js') as HTMLInputElement;
		input.setAttribute('data-is-integer', 'true');
		formatAmountOnChange();
		input.dispatchEvent(new Event('change'));
		expect(input.value).toBe('101');
	});
	it('should format the input to two decimal places', () => {
		const input = document.querySelector('.format - amount - js') as HTMLInputElement;
		formatAmountOnChange();
		input.dispatchEvent(new Event('change'));
		expect(input.value).toBe('100.58');
	});

});

describe('format amount should format amount', () => {
	it('should format to two decimal places', () => {
		expect(formatAmount(100.569)).toBe(100.57);
		expect(formatAmount(100.569651981)).toBe(100.57);
		expect(formatAmount(0.489651981)).toBe(0.49);
	});
	it('should format to one decimal place', () => {
		expect(formatAmount(100.569651981, true)).toBe(101);
		expect(formatAmount(0.589651981, true)).toBe(1);
	});
});