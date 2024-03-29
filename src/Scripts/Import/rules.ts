import * as ImportRuleModal from '../Utility/importrule-modal';

ImportRuleModal.formatCategorySelectListOnAddButtonClick();
ImportRuleModal.removeSelectListsWhenFilterOptionIsChecked();
ImportRuleModal.setSelectListOptionsOnTransactionTypeChange();
ImportRuleModal.setSelectListOptionsOnEditButtonsClick();

goBack();

function goBack() {
	const backButton = document.getElementById('backButton');
	backButton?.addEventListener(
		'click',
		() => {
			window.history.back();
		},
		false,
	);
}
