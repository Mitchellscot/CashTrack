import 'jquery';
import 'jquery-ui/ui/widgets/autocomplete.js';

const autoSuggestMerchantNames = (): void => {
    const inputs: NodeListOf<HTMLElement> | null = document.querySelectorAll(".merchant-autosuggest-js");
    inputs.forEach(x => x.addEventListener('input', autoSuggestEventListener, false));
}

const removeAutosuggestMerchantName = (element: HTMLElement): void => {
    element.removeEventListener('input', autoSuggestEventListener, false);
}

function autoSuggestEventListener(x: Event) {
    const searchTerm = (x.target as HTMLInputElement).value;

    //I really wanted to use vanilla javascript but this autocomplete library insists I use ajax WHATEVER
    $.ajax({
        url: `/api/merchants?merchantName=${searchTerm}`,
        method: 'GET'
    }).then((response) => {
        $(x.target as HTMLInputElement).empty();
        $(x.target as HTMLInputElement).autocomplete({ source: response });
    });
}

export { autoSuggestMerchantNames, removeAutosuggestMerchantName };