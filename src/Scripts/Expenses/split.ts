import {loadMainCategoryOnSplitLoad} from '../Utility/load-main-category';
import {
	updateTotalsWhenAmountChanges,
	updateTotalsWhenTaxStatusChanges,
} from '../Utility/split-calculations';

loadMainCategoryOnSplitLoad();
updateTotalsWhenAmountChanges();
updateTotalsWhenTaxStatusChanges();
