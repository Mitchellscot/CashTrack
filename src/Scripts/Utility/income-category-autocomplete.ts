import 'jquery';
import 'jquery-ui/ui/widgets/autocomplete.js';

const autoSuggestIncomeCategoryNames = (): void => {
    const inputs: NodeListOf<HTMLElement> | null = document.querySelectorAll(".incomecategory-autosuggest-js");
    inputs.forEach(x => x.addEventListener('input', autoSuggestIncomeCategoryEventListener, true));
}

function autoSuggestIncomeCategoryEventListener(x: Event) {
    const searchTerm = (x.target as HTMLInputElement).value;

    $.ajax({
        url: `/api/incomecategory/autocomplete?categoryName=${searchTerm}`,
        method: 'GET'
    }).then((response) => {
        $(x.target as HTMLInputElement).empty();
        $(x.target as HTMLInputElement).autocomplete({ source: response });
    });
}

export { autoSuggestIncomeCategoryNames, autoSuggestIncomeCategoryEventListener };