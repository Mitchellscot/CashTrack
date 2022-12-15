import { autoSuggestMerchantNames } from "../Utility/merchant-autocomplete";
import { loadMainCategoryOnSubCategorySelect } from "../Utility/load-main-category";
import { formatAmountOnChange } from '../Utility/format-amount';


autoSuggestMerchantNames();
loadMainCategoryOnSubCategorySelect();
formatAmountOnChange();


