import 'jquery';
import 'jquery-ui/ui/widgets/autocomplete.js';

const autoSuggestSubCategoryNames = (): void => {
    const inputs: NodeListOf<HTMLElement> | null = document.querySelectorAll(".subcategory-autosuggest-js");
    inputs.forEach(x => x.addEventListener('input', autoSuggestSubCategoryEventListener, true));
}

function autoSuggestSubCategoryEventListener(x: Event) {
    const searchTerm = (x.target as HTMLInputElement).value;

    $.ajax({
        url: `/api/subcategory/autocomplete?categoryName=${searchTerm}`,
        method: 'GET'
    }).then((response) => {
        $(x.target as HTMLInputElement).empty();
        $(x.target as HTMLInputElement).autocomplete({ source: response });
    });
}

export { autoSuggestSubCategoryNames, autoSuggestSubCategoryEventListener };