import {autoSuggestIncomeSourceNames} from '../Utility/source-autocomplete';
import {
	formatInputOnPageLoad,
	formatInputOnSelectListChange,
} from '../Utility/handle-income-dropdown';
import {formatAmountOnChange} from '../Utility/format-amount';
import {
	forceRefundCategoryWhenRefundIsCheckedAddModal,
	forceRefundCategoryWhenRefundIsCheckedEditModal,
} from '../Utility/force-refund';
import showModalFromUrlHash from '../Utility/show-modal';

autoSuggestIncomeSourceNames();
formatInputOnPageLoad();
formatInputOnSelectListChange();
formatAmountOnChange();
forceRefundCategoryWhenRefundIsCheckedAddModal();
forceRefundCategoryWhenRefundIsCheckedEditModal();
showModalFromUrlHash('#addIncomeModal');