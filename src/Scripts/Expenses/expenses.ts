//jquery is loaded here:
import { autoSuggestMerchantNames } from "../Utility/merchant-autocomplete";
import { formatInputsOnSelectListChange, formatInputsOnPageLoad } from '../Utility/handle-expense-dropdown';
import 'jquery-validation';
import 'jquery-validation-unobtrusive';
import { loadMainCategoryOnSubCategorySelect, loadMainCategoryOnEditModalLoad } from "../Utility/load-main-category";
import { formatAmountOnChange } from '../Utility/format-amount';

console.log('expenses page');

autoSuggestMerchantNames();
formatInputsOnPageLoad();
formatInputsOnSelectListChange();
loadMainCategoryOnSubCategorySelect();
loadMainCategoryOnEditModalLoad()
formatAmountOnChange();



