import $ from "jquery";
import "jquery-ui";

const autoSuggestMerchantNames = (): void => {
    const inputs: NodeListOf<HTMLElement> | null = document.querySelectorAll(".merchant-name-input");
    inputs.forEach(x => x.addEventListener('input', x => {
        const searchTerm = (x.target as HTMLInputElement).value;
        const request = new XMLHttpRequest();
        request.open('GET', `/api/merchants?merchantName=${searchTerm}`);
        request.onload = () => {
            if (request?.status >= 200 && request?.status < 400)
            {
                const response = request.response;
                console.log($(".merchant-name-input").val());
                $(x.target as HTMLInputElement).autocomplete({ source: response });
            } else
            {
                console.log(`error while attempting to fetch merchant names ${request.response}`);
            }
        };
        request.onerror = function () {
            console.log(`an error occured while trying to autocomplete merchant names with value: ${searchTerm}`);
        };
        request.send();
    }, false));
}

export default autoSuggestMerchantNames;