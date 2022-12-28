// Jquery is loaded here:
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
// Import Modal from "bootstrap/js/dist/modal";

autoSuggestMerchantNames();
formatInputsOnPageLoad();
formatInputsOnSelectListChange();
loadMainCategoryOnSubCategorySelect();
loadMainCategoryOnEditModalLoad();
formatAmountOnChange();
// ShowModalFromHash();

// the Add Expense modal has an Add Category button on it that opens the Add Sub Category modal
// when you click save on that model, it redirects to Expenses/Index and adds a hash to the URL
// This function checks for that hash and if it exists, opens up the Add Expense Modal so you can
// continue where you left off.
// I'm smart.
// function showModalFromHash() {
//    if (window.location.hash === "#addExpenseModal") {
//        const modal = new Modal('#addExpenseModal')
//        modal.show();
//    }
// }
