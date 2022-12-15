import { autoSuggestMerchantNames } from "../Utility/merchant-autocomplete";
import { loadMainCategoryOnSubCategorySelect } from "../Utility/load-main-category";
import { formatAmountOnChange } from '../Utility/format-amount';
import { autoSuggestIncomeSourceNames } from '../Utility/source-autocomplete';
import { forceRefundCategoryWhenRefundIsCheckedAddModal } from "../Utility/force-refund";
import disableLocationIfMerchantIsOnline from "../Utility/online-merchant";
import disableLocationIfSourceIsOnline from "../Utility/online-source";

autoSuggestMerchantNames();
loadMainCategoryOnSubCategorySelect();
formatAmountOnChange();
autoSuggestIncomeSourceNames();
disableLocationIfMerchantIsOnline();
forceRefundCategoryWhenRefundIsCheckedAddModal();
disableLocationIfSourceIsOnline();


