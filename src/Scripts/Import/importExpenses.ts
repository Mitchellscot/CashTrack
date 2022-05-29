//jquery is loaded here:
import { autoSuggestMerchantNames } from "../Utility/merchant-autocomplete";
import disableLocationIfMerchantIsOnline from "../Utility/online-merchant";

console.log('import-expenses page');

disableLocationIfMerchantIsOnline();
autoSuggestMerchantNames();