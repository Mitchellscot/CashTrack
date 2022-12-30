import {formatAmount} from '../Utility/format-amount';

describe('test', () => {
	it('should do stuff', () => {
		expect(formatAmount(100.569)).toBe(100.57);
	});
});