import { disableSelectInputsOnIncomeSwitch } from "../Utility/budget-income-switch";
import { forceMonthSelectionWhenIncomeIsCheckedAddModal } from "../Utility/budget-timespan-select";
import { hideCategoryWhenSavingsIsSelectedAddModal } from "../Utility/budget-type-switch";
import { loadAveragesOnSelectListChange, loadMainCategoryOnSubCategorySelect } from "../Utility/loadBudgetStatistics";
console.log('Budget Page');

hideCategoryWhenSavingsIsSelectedAddModal();
forceMonthSelectionWhenIncomeIsCheckedAddModal();
disableSelectInputsOnIncomeSwitch();
loadAveragesOnSelectListChange();
loadMainCategoryOnSubCategorySelect();