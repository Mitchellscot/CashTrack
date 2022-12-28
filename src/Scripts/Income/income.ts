import {autoSuggestIncomeSourceNames} from '../Utility/source-autocomplete';
import 'jquery-validation';
import 'jquery-validation-unobtrusive';
import {
	formatInputOnPageLoad,
	formatInputOnSelectListChange,
} from '../Utility/handle-income-dropdown';
import {formatAmountOnChange} from '../Utility/format-amount';
import {
	forceRefundCategoryWhenRefundIsCheckedAddModal,
	forceRefundCategoryWhenRefundIsCheckedEditModal,
} from '../Utility/force-refund';

autoSuggestIncomeSourceNames();
formatInputOnPageLoad();
formatInputOnSelectListChange();
formatAmountOnChange();
forceRefundCategoryWhenRefundIsCheckedAddModal();
forceRefundCategoryWhenRefundIsCheckedEditModal();
