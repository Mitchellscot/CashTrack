import 'jquery';
import 'jquery-ui/ui/widgets/autocomplete.js';
import type Category from '../Models/Categories';

const autoSuggestIncomeCategoryNames = (): void => {
	const inputs: NodeListOf<HTMLElement> | undefined = document.querySelectorAll(
		'.incomecategory-autosuggest-js',
	);
	inputs.forEach(x => {
		x.addEventListener('input', autoSuggestIncomeCategoryEventListener, true);
	},
	);
};

function autoSuggestIncomeCategoryEventListener(x: Event) {
	const searchTerm = (x.target as HTMLInputElement).value;

	$.ajax({
		url: `/api/incomecategory/autocomplete?categoryName=${searchTerm}`,
		method: 'GET',
	}).then(response => {
		$(x.target as HTMLInputElement).empty();
		$(x.target as HTMLInputElement).autocomplete({source: response as Category[]});
	}).catch(e => {
		console.error(e);
	});
}

export {autoSuggestIncomeCategoryNames, autoSuggestIncomeCategoryEventListener};
