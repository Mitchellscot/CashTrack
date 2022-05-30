import 'jquery';
import 'jquery-ui/ui/widgets/autocomplete.js';

const autoSuggestIncomeSourceNames = (): void => {
    const inputs: NodeListOf<HTMLElement> | null = document.querySelectorAll(".source-autosuggest-js");
    inputs.forEach(x => x.addEventListener('input', autoSuggestSourceEventListener, true));
}

function autoSuggestSourceEventListener(x: Event) {
    const searchTerm = (x.target as HTMLInputElement).value;

    $.ajax({
        url: `/api/incomesource?sourceName=${searchTerm}`,
        method: 'GET'
    }).then((response) => {
        $(x.target as HTMLInputElement).empty();
        $(x.target as HTMLInputElement).autocomplete({ source: response });
    }).catch(err => console.log(err));
}

export { autoSuggestIncomeSourceNames, autoSuggestSourceEventListener };