import {forceRefundCategoryWhenRefundIsCheckedAddModal} from '../Utility/force-refund';
import {formatAmountOnChange} from '../Utility/format-amount';
import {loadMainCategoryOnSubCategorySelect} from '../Utility/loadBudgetStatistics';
import {autoSuggestMerchantNames} from '../Utility/merchant-autocomplete';
import disableLocationIfMerchantIsOnline from '../Utility/online-merchant';
import disableLocationIfSourceIsOnline from '../Utility/online-source';
import {autoSuggestIncomeSourceNames} from '../Utility/source-autocomplete';

autoSuggestMerchantNames();
loadMainCategoryOnSubCategorySelect();
formatAmountOnChange();
autoSuggestIncomeSourceNames();
disableLocationIfMerchantIsOnline();
forceRefundCategoryWhenRefundIsCheckedAddModal();
disableLocationIfSourceIsOnline();