//jquery is loaded here:
import autoSuggestMerchantNames from "../Utility/merchant-autocomplete";
import 'jquery-validation';
import 'jquery-validation-unobtrusive';

console.log('expenses page');

autoSuggestMerchantNames();


////get query values for dropdown lists
//let q = getQueryValue();
//let queryType = getQueryType();
////fires on load
//let selectListValue = $("#querySelect").val();
//handleQuerySelectChange(selectListValue);

////GET REQUEST METHODS
////When the query selection option changes, change the input type box
//$("#querySelect").change(() => {
//    let selectListValue = $("#querySelect").val();
//    handleQuerySelectChange(selectListValue);
//});
////massive switch statement handling qet requests
//function handleQuerySelectChange(selectListValue) {
//    switch (selectListValue) {
//        //date
//        case "0":
//            resetCategorySelect();
//            resetNumbersForm();
//            resetSecondInputForm();
//            $("#qInput").prop("type", "date");
//            break;
//        //date range
//        case "1":
//            resetCategorySelect();
//            resetNumbersForm();
//            $("#qInput").prop("type", "date");
//            $("#q2Input").show();
//            $("#qInput").removeClass("w-50");
//            $("#qInput").addClass("w-25");
//            $("#q2Input").addClass("w-25");
//            break;
//        //month
//        case "2":
//            resetCategorySelect();
//            resetNumbersForm();
//            resetSecondInputForm();
//            $("#qInput").prop("type", "month");
//            break;
//        //quarter
//        case "3":
//            resetCategorySelect();
//            resetNumbersForm();
//            resetSecondInputForm();
//            $("#qInput").prop("type", "month");
//            break;
//        //year
//        case "4":
//            resetCategorySelect();
//            resetSecondInputForm();
//            $("#qInput").prop("type", "number");
//            $("#qInput").prop('min', "2012");
//            let currentYear = new Date().getFullYear();
//            $("#qInput").prop('max', currentYear);
//            if (q !== undefined && queryType === "4") {
//                console.log(q)
//                $("#qInput").val(q);
//            }
//            else {
//                $("#qInput").val(parseInt(new Date().getFullYear()));
//            }
//            break;
//        //amount
//        case "5":
//            resetCategorySelect();
//            resetSecondInputForm();
//            $("#qInput").val('');
//            $("#qInput").prop("type", "number");
//            $("#qInput").prop('step', "any");
//            $("#qInput").prop('min', "0.00");
//            if (q !== undefined && queryType === "5") {
//                $("#qInput").val(q);
//            }
//            break;
//        //notes
//        case "6":
//            resetCategorySelect();
//            resetNumbersForm();
//            resetSecondInputForm();
//            $("#qInput").prop("type", "text");
//            if (q !== undefined && queryType === "6") {
//                $("#qInput").val(q);
//            }
//            break;
//        //merchant
//        case "7":
//            resetCategorySelect();
//            resetNumbersForm();
//            resetSecondInputForm();
//            $("#qInput").prop("type", "text");
//            break;
//        //sub category
//        case "8":
//            resetNumbersForm();
//            resetSecondInputForm();
//            $("#qInput").hide();
//            $("#categorySelect").show();
//            $.ajax({
//                url: `/api/subcategory`,
//                method: 'GET'
//            }).then((response) => {
//                $("#categorySelect").empty();
//                for (let category of response) {
//                    if (q !== undefined && queryType == "8") {
//                        if (Number(category.id) === Number(q)) {
//                            $("#categorySelect").append(`<option selected value=${category.id}>${category.category}</option>`);
//                        }
//                        else {
//                            $("#categorySelect").append(`<option value=${category.id}>${category.category}</option>`);
//                        }
//                    }
//                    else {
//                        $("#categorySelect").append(`<option value=${category.id}>${category.category}</option>`);
//                    }
//                }
//            }).catch((error) => alert(error));
//            break;
//        //main category
//        case "9":
//            resetNumbersForm();
//            resetSecondInputForm();
//            $("#qInput").hide();
//            $("#categorySelect").show();
//            $.ajax({
//                url: `/api/maincategory`,
//                method: 'GET'
//            }).then((response) => {
//                $("#categorySelect").empty();
//                for (let category of response) {
//                    if (q !== undefined && queryType == "9") {
//                        if (Number(category.id) === Number(q)) {
//                            $("#categorySelect").append(`<option selected value=${category.id}>${category.category}</option>`);
//                        }
//                        else {
//                            $("#categorySelect").append(`<option value=${category.id}>${category.category}</option>`);
//                        }
//                    }
//                    else {
//                        $("#categorySelect").append(`<option value=${category.id}>${category.category}</option>`);
//                    }
//                }
//            }).catch((error) => alert(error));
//            break;
//        //tags
//        case "10":
//            resetCategorySelect();
//            resetNumbersForm();
//            resetSecondInputForm();
//            $("#qInput").prop("type", "text");
//            break;
//        default:
//            resetSecondInputForm();
//            resetNumbersForm();
//            resetCategorySelect();
//            $("#qInput").prop("type", "date");
//            break;
//    }
//}

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


//function resetSecondInputForm() {
//    $("#qInput").addClass("w-50");
//    $("#q2Input").val("");
//    $("#q2Input").removeClass("w-25");
//    $("#q2Input").hide();
//}
//function resetNumbersForm() {
//    $("#qInput").removeAttr('min');
//    $("#qInput").removeAttr('step');
//    $("#qInput").removeAttr('min');
//    $("#qInput").val('');
//}
//function resetCategorySelect() {
//    $("#qInput").show();
//    $("#categorySelect").empty();
//    $("#categorySelect").hide();
//}
