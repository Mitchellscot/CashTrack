import bootstrap from "../../wwwroot/lib/bootstrap/dist/js/bootstrap";
//jquery is loaded here:
import { autoSuggestMerchantNames } from "../Utility/merchant-autocomplete";
import 'jquery-validation';
import 'jquery-validation-unobtrusive';
import { loadMainCategoryOnSubCategorySelect } from "../Utility/load-main-category";
import { formatAmountOnChange } from '../Utility/format-amount';


autoSuggestMerchantNames();
loadMainCategoryOnSubCategorySelect();
formatAmountOnChange();
showModalFromHash();

//the Add Expense modal has an Add Category button on it that opens the Add Sub Category modal
//when you click save on that model, it redirects to Expenses/Index and adds a hash to the URL
//This function checks for that hash and if it exists, opens up the Add Expense Modal so you can 
//continue where you left off. 
//I'm smart.
function showModalFromHash() {
    if (window.location.hash === "#addExpenseModal") {
        const modal = new bootstrap.Modal('#addExpenseModal', { show: true })
        modal.show();
    }
}
