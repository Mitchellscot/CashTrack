import disableLocationIfMerchantIsOnline from "../Utility/online-merchant";
import { autoSuggestMerchantNames } from "../Utility/merchant-autocomplete";

console.log('merchants page');

disableLocationIfMerchantIsOnline();
autoSuggestMerchantNames();