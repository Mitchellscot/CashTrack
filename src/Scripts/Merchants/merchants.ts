import disableLocationIfMerchantIsOnline from "../Utility/online-merchant";
import { autoSuggestMerchantNames } from "../Utility/merchant-autocomplete";
import 'jquery-validation';
import 'jquery-validation-unobtrusive';

disableLocationIfMerchantIsOnline();
autoSuggestMerchantNames();