import showModalFromUrlHash from '../Utility/show-modal';
import {autoSuggestMerchantNames} from '../Utility/merchant-autocomplete';
import {
	formatInputsOnSelectListChange,
	formatInputsOnPageLoad,
} from '../Utility/handle-expense-dropdown';
import 'jquery-validation';
import 'jquery-validation-unobtrusive';
import {
	loadMainCategoryOnSubCategorySelect,
	loadMainCategoryOnEditModalLoad,
} from '../Utility/load-main-category';
import {formatAmountOnChange} from '../Utility/format-amount';

autoSuggestMerchantNames();
formatInputsOnPageLoad();
formatInputsOnSelectListChange();
loadMainCategoryOnSubCategorySelect();
loadMainCategoryOnEditModalLoad();
formatAmountOnChange();
showModalFromUrlHash('#addExpenseModal');

