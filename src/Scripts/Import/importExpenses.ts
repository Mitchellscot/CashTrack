import {autoSuggestMerchantNames} from '../Utility/merchant-autocomplete';
import disableLocationIfMerchantIsOnline from '../Utility/online-merchant';

disableLocationIfMerchantIsOnline();
autoSuggestMerchantNames();
