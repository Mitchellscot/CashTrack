import { autoSuggestIncomeSourceNames } from '../Utility/source-autocomplete';
console.log('income page');


autoSuggestIncomeSourceNames();


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
//            $("#qInput").prop("type", "date");
//            break;
//        //month
//        case "1":
//            resetCategorySelect();
//            resetNumbersForm();
//            $("#qInput").prop("type", "month");
//            break;
//        //quarter
//        case "2":
//            resetCategorySelect();
//            resetNumbersForm();
//            $("#qInput").prop("type", "month");
//            break;
//        //year
//        case "3":
//            resetCategorySelect();
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
//        case "4":
//            resetCategorySelect();
//            $("#qInput").val('');
//            $("#qInput").prop("type", "number");
//            $("#qInput").prop('step', "any");
//            $("#qInput").prop('min', "0.00");
//            if (q !== undefined && queryType === "5") {
//                $("#qInput").val(q);
//            }
//            break;
//        //notes
//        case "5":
//            resetCategorySelect();
//            resetNumbersForm();
//            $("#qInput").prop("type", "text");
//            if (q !== undefined && queryType === "6") {
//                $("#qInput").val(q);
//            }
//            break;
//        //source
//        case "6":
//            resetCategorySelect();
//            resetNumbersForm();
//            $("#qInput").prop("type", "text");
//            break;
//        //category
//        case "7":
//            resetNumbersForm();
//            $("#qInput").hide();
//            $("#categorySelect").show();
//            $.ajax({
//                url: `/api/incomecategory`,
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
//        default:
//            resetNumbersForm();
//            resetCategorySelect();
//            $("#qInput").prop("type", "date");
//            break;
//    }
//}
////if it's a query of the amount, make it a nice loking decimal
//$('#qInput').change(() => {
//    let selectListValue = $("#querySelect").val();
//    if (selectListValue == "4") {
//        let amount = $("#qInput").val();
//        let rounded = Math.round(amount * 100) / 100;
//        let finalNumber = rounded.toFixed(2);
//        $("#qInput").val(finalNumber);
//    }
//});
////if it's a query of the income source, show a dropdown list of matching income source names
//$("#qInput").on("input", () => {
//    let selectListValue = $("#querySelect").val();
//    if (selectListValue === "6") {
//        let searchTerm = $("#qInput").val();
//        $.ajax({
//            url: `/api/incomesource?sourceName=${searchTerm}`,
//            method: 'GET'
//        }).then((response) => {
//            $("#qInput").empty();
//            $("#qInput").autocomplete({ source: response });
//        }).catch((error) => console.log(error));
//    }
//});


////ADD INCOME MODAl
////when adding a new income amount, make it a nice loking decimal
//$('#addIncomeAmount').change(() => {
//    let amount = $("#addIncomeAmount").val();
//    let rounded = Math.round(amount * 100) / 100;
//    let finalNumber = rounded.toFixed(2);
//    $("#addIncomeAmount").val(finalNumber);
//});
////loads sub category list when add income modal opens
//$("#addIncomeButton").click(() => {
//    $.ajax({
//        url: `/api/incomecategory`,
//        method: 'GET'
//    }).then((response) => {
//        $("#addIncomeCategory").empty();
//        $("#addIncomeCategory").append(`<option value="" selected disabled hidden>Select</option>`);
//        for (let category of response) {
//            $("#addIncomeCategory").append(`<option value=${category.category}>${category.category}</option>`);
//        }
//    });
//});

////Autocomplete for income sources on add income modal
//$("#addIncomeSource").on("input", () => {
//    let sourceName = $("#addIncomeSource").val();
//    $.ajax({
//        url: `/api/incomesource?sourceName=${sourceName}`,
//        method: 'GET'
//    }).then((response) => {
//        console.log(response);
//        $("#addIncomeSource").empty();
//        $("#addIncomeSource").autocomplete({ source: response });
//    });
//});

////changes form if it's a refund (forces refund category)
//$("#addIncomeRefundBool").click(() => {
//    if ($("#addIncomeRefundBool").is(":checked")) {
//        $("#addIncomeCategory").empty();
//        $("#addIncomeCategory").append(`<option value="9" selected>Refund</option>`);
//        $("#addIncomeSubmitButton").text("Apply Refund");
//    }
//    else {
//        $("#addIncomeSubmitButton").text("Add New Income");
//        $.ajax({
//            url: `/api/incomecategory`,
//            method: 'GET'
//        }).then((response) => {
//            $("#addIncomeCategory").empty();
//            $("#addIncomeCategory").append(`<option value="" selected disabled hidden>Select</option>`);
//            for (let category of response) {
//                $("#addIncomeCategory").append(`<option value=${category.category}>${category.category}</option>`);
//            }
//        });
//    }
//});

////EDIT INCOME MODALS
////dynamically loaded code for each edit modal
//@if (@Model.IncomeResponse != null && @Model.IncomeResponse.ListItems.Count() > 0)
//{
//    var incomeIds = Model.IncomeResponse.ListItems.ToDictionary(x => x.Id, x => x.Category);
//    foreach(var id in incomeIds)
//    {
//        //Loads the category select list
//        <text>$("#editIncomeButton-@id.Key").click(() => {
//            if ($("#editIncomeRefundBool-@id.Key").is(":checked")) {
//                $("#refundButton-@id.Key").show();
//            }

//            $.ajax({
//                url: `/api/incomecategory`,
//                method: 'GET'
//            }).then((response) => {
//                $("#editIncomeCategory-@id.Key").empty();
//                for (let category of response) {
//                    if (category.category === "@id.Value") {
//                        $("#editIncomeCategory-@id.Key").append(`<option selected value=${category.category}>${category.category}</option>`);
//                    }
//                    else {
//                        $("#editIncomeCategory-@id.Key").append(`<option value=${category.category}>${category.category}</option>`);
//                    }
//                }
//            });
//        });
//        </text>
//            //Autocomplete for income source names
//            < text > $("#editIncomeSource-@id.Key").on("input", () => {
//                let editSearchTerm = $("#editIncomeSource-@id.Key").val();
//                $.ajax({
//                    url: `/api/incomesource?sourceName=${editSearchTerm}`,
//                    method: 'GET'
//                }).then((response) => {
//                    $("#editIncomeSource-@id.Key").empty();
//                    $("#editIncomeSource-@id.Key").autocomplete({ source: response });
//                });
//            })
//            < /text>
//            //When updating the income amount, make it a nice looking decimal
//            < text > $('#editIncomeAmount-@id.Key').change(() => {
//                let amount = $("#editIncomeAmount-@id.Key").val();
//                let rounded = Math.round(amount * 100) / 100;
//                let finalNumber = rounded.toFixed(2);
//                $("#editIncomeAmount-@id.Key").val(finalNumber);
//            });
//        </text>
//            //changes form if it's a refund (forces refund category)
//            < text >
//            $("#editIncomeRefundBool-@id.Key").click(() => {
//                if ($("#editIncomeRefundBool-@id.Key").is(":checked")) {
//                    $("#refundButton-@id.Key").show();
//                    $("#editIncomeCategory-@id.Key").empty();
//                    $("#editIncomeCategory-@id.Key").append(`<option value="9" selected>Refund</option>`);
//                }
//                else {
//                    $("#refundButton-@id.Key").hide();
//                    $.ajax({
//                        url: `/api/incomecategory`,
//                        method: 'GET'
//                    }).then((response) => {
//                        $("#editIncomeCategory-@id.Key").empty();
//                        $("#editIncomeCategory-@id.Key").append(`<option value="" selected disabled hidden>Select</option>`);
//                        for (let category of response) {
//                            $("#editIncomeCategory-@id.Key").append(`<option value=${category.category}>${category.category}</option>`);
//                        }
//                    });
//                }
//            });
//        </text>
//    }
//}

////general helper methods
//function getQueryValue() {
//    let queries = [], hash;
//    let values = window.location.href.slice(window.location.href.indexOf('?') + 1).split('&');
//    for (let i = 0; i < values.length; i++) {
//        hash = values[i].split('=');
//        if (hash[0] === "Q") {
//            return hash[1];
//        }
//    }
//    return undefined;
//}
//function getQueryType() {
//    let queries = [], hash;
//    let values = window.location.href.slice(window.location.href.indexOf('?') + 1).split('&');
//    for (let i = 0; i < values.length; i++) {
//        hash = values[i].split('=');
//        if (hash[0] === "Query") {
//            return hash[1];
//        }
//    }
//    return undefined;
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
//    });