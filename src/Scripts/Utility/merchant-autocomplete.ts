import 'jquery';
import 'jquery-ui/ui/widgets/autocomplete.js';
import type MerchantSource from '../Models/MerchantSource';

const autoSuggestMerchantNames = (): void => {
	const inputs: NodeListOf<HTMLElement> | undefined = document.querySelectorAll(
		'.merchant-autosuggest-js',
	);
	inputs.forEach(x => {
		x.addEventListener('input', autoSuggestEventListener, true);
	},
	);
};

function autoSuggestEventListener(x: Event) {
	const searchTerm = (x.target as HTMLInputElement).value;

	// I really wanted to use vanilla javascript but this autocomplete library insists I use ajax WHATEVER
	$.ajax({
		url: `/api/merchants?merchantName=${searchTerm}`,
		method: 'GET',
	}).then(response => {
		$(x.target as HTMLInputElement).empty();
		$(x.target as HTMLInputElement).autocomplete({source: response as MerchantSource[]});
	}).catch(e => {
		console.log(e);
	});
}

export {autoSuggestMerchantNames, autoSuggestEventListener};
