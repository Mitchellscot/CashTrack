import { autoSuggestIncomeSourceNames } from '../Utility/source-autocomplete';
import { formatInputOnPageLoad, formatInputOnSelectListChange } from '../Utility/handle-income-dropdown';
import { formatAmountOnChange } from '../Utility/format-amount';
import { forceRefundCategoryWhenRefundIsCheckedAddModal, forceRefundCategoryWhenRefundIsCheckedEditModal } from '../Utility/force-refund';

console.log('income page');


autoSuggestIncomeSourceNames();
formatInputOnPageLoad();
formatInputOnSelectListChange();
formatAmountOnChange();
forceRefundCategoryWhenRefundIsCheckedAddModal();
forceRefundCategoryWhenRefundIsCheckedEditModal();
