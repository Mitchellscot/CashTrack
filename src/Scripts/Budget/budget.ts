import { disableSelectInputsOnEditIncomeSwitch, disableSelectInputsOnIncomeSwitch } from "../Utility/budget-income-switch";
import { forceMonthSelectionWhenIncomeIsCheckedAddModal, forceMonthSelectionWhenIncomeIsCheckedEditModal } from "../Utility/budget-timespan-select";
import { hideCategoryWhenSavingsIsSelectedAddModal, hideCategoryWhenSavingsIsSelectedEditModal } from "../Utility/budget-type-switch";
import { loadAveragesOnSelectListChange, loadAveragesOnSelectListChangeEdit, loadMainCategoryOnSubCategorySelect, loadMainCategoryOnSubCategorySelectEdit } from "../Utility/loadBudgetStatistics";
console.log('Budget Page');

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