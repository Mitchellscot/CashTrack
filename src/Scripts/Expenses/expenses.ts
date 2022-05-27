//jquery is loaded here:
import { autoSuggestMerchantNames } from "../Utility/merchant-autocomplete";
import { formatInputsOnSelectListChange, formatInputsOnPageLoad } from '../Utility/handle-expense-dropdown';
import 'jquery-validation';
import 'jquery-validation-unobtrusive';

console.log('expenses page');

autoSuggestMerchantNames();
formatInputsOnPageLoad();
formatInputsOnSelectListChange();


////loads the main category after sub category is chosen on add expense modal
//$("#addExpenseSubCategory").change(() => {
//    let categoryId = $("#addExpenseSubCategory").val();
//    $.ajax({
//        url: `/api/maincategory/sub-category/${categoryId}`,
//        method: 'GET'
//    }).then((response) => {
//        $("#addExpenseMainCategory").prop(`value`, `${response}`);
//    }).catch((error) => console.log(error));
//});

////EDIT EXPENSE MODALS
////dynamically loaded code for each edit modal
//@if (@Model.ExpenseResponse != null && @Model.ExpenseResponse.ListItems.Count() > 0)
//{
//    var expenseIds = Model.ExpenseResponse.ListItems.ToDictionary(x => x.Id, x => x.SubCategory);
//    foreach(var id in expenseIds)
//    {
//        //changes main category when edit modal opens
//        <text>$("#editExpenseSubCategory-@id.Key").change(() => {
//            let categoryId = $("#editExpenseSubCategory-@id.Key").val();
//            $.ajax({
//                url: `/api/maincategory/sub-category/${categoryId}`,
//                method: 'GET'
//            }).then((response) => {
//                $("#editExpenseMainCategory-@id.Key").prop(`value`, `${response}`);
//            }).catch((error) => console.log(error));
//        }); </text>
//            //Loads the sub category select list
//            < text > $("#editExpenseButton-@id.Key").click(() => {
//                let subCategory = $("#editExpenseSubCategory-@id.Key").val();
//                $.ajax({
//                    url: `/api/subcategory`,
//                    method: 'GET'
//                }).then((response) => {
//                    $("#editExpenseSubCategory-@id.Key").empty();
//                    for (let category of response) {
//                        if (category.category === "@id.Value") {
//                            $("#editExpenseSubCategory-@id.Key").append(`<option selected value=${category.id}>${category.category}</option>`);
//                        }
//                        else {
//                            $("#editExpenseSubCategory-@id.Key").append(`<option value=${category.id}>${category.category}</option>`);
//                        }
//                    }
//                });
//            });
//        </text>
//            //Autocomplete for merchant names
//            < text > $("#editExpenseMerchantName-@id.Key").on("input", () => {
//                let editSearchTerm = $("#editExpenseMerchantName-@id.Key").val();
//                $.ajax({
//                    url: `/api/merchants?merchantName=${editSearchTerm}`,
//                    method: 'GET'
//                }).then((response) => {
//                    $("#editExpenseMerchantName-@id.Key").empty();
//                    $("#editExpenseMerchantName-@id.Key").autocomplete({ source: response });
//                });
//            })
//            < /text>
//            //When updating the expense amount, make it a nice looking decimal
//            < text > $('#editExpenseAmount-@id.Key').change(() => {
//                let amount = $("#editExpenseAmount-@id.Key").val();
//                let rounded = Math.round(amount * 100) / 100;
//                let finalNumber = rounded.toFixed(2);
//                $("#editExpenseAmount-@id.Key").val(finalNumber);
//            });
//        </text>
//    }
//}



