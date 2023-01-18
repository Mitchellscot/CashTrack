import {
	disableSelectInputsOnEditIncomeSwitch,
	disableSelectInputsOnIncomeSwitch,
} from '../Utility/budget-income-switch';
import {
	forceMonthSelectionWhenIncomeIsCheckedAddModal,
	forceMonthSelectionWhenIncomeIsCheckedEditModal,
} from '../Utility/budget-timespan-select';
import {
	hideCategoryWhenSavingsIsSelectedAddModal,
	hideCategoryWhenSavingsIsSelectedEditModal,
} from '../Utility/budget-type-switch';
import {formatAmountOnChange} from '../Utility/format-amount';
import {
	loadAveragesOnSelectListChange,
	loadAveragesOnSelectListChangeEdit,
	loadMainCategoryOnSubCategorySelect,
	loadMainCategoryOnSubCategorySelectEdit,
} from '../Utility/loadBudgetStatistics';
import showModalFromUrlHash from '../Utility/show-modal';

hideCategoryWhenSavingsIsSelectedEditModal();
hideCategoryWhenSavingsIsSelectedAddModal();
forceMonthSelectionWhenIncomeIsCheckedAddModal();
forceMonthSelectionWhenIncomeIsCheckedEditModal();
disableSelectInputsOnEditIncomeSwitch();
disableSelectInputsOnIncomeSwitch();
loadAveragesOnSelectListChange();
loadMainCategoryOnSubCategorySelect();
loadMainCategoryOnSubCategorySelectEdit();
loadAveragesOnSelectListChangeEdit();
showModalFromUrlHash('#addBudgetModal');
formatAmountOnChange();