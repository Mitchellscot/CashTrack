console.log('split page');
import { loadMainCategoryOnSplitLoad } from '../Utility/load-main-category';


loadMainCategoryOnSplitLoad();


//            //When updating the expense amount, make it a nice looking decimal
//            <text>$('#amount-@i').change(() => {
//                //format the number in the input box to 2 decimals
//                $("#amount-@i").val(formatNumber($("#amount-@i").val()));
//                //apply tax if tax is checked and then update the Total Table Cell
//                let totalAmountAfterTax = applyTax($("#amount-@i").val(), $("#isTaxed-@i").is(":checked"));
//                $("#totalAmount-@i").text(totalAmountAfterTax);
//                //Update the form, if all totals equal the original amount, save changes button is displayed.
//                $("#total").text(adjustFormBasedOnTotal(findTotal()));
//            });
//            </text>
//            <text>$("#isTaxed-@i").change(() => {
//                let taxedOrNot = applyTax($("#amount-@i").val(), $("#isTaxed-@i").is(":checked"));
//                $("#totalAmount-@i").text(taxedOrNot);
//                $("#total").text(adjustFormBasedOnTotal(findTotal()));
//            });

//            </text>
//        }
//        function applyTax(num, isTaxed)
//        {
//            let amount = Number(num);
//            let taxAmount = Number(@Model.Tax);
//            if(isTaxed){
//                return formatNumber(amount + (amount * taxAmount));
//            }
//            else{
//                return num;
//            }
//        }
//        function adjustFormBasedOnTotal(num)
//        {
//            let originalTotal = @Model.Total;
//            if(Number(num) !== Number(0.00)){
//                $("#submitButton").prop('disabled', true);
//                $("#totalBox").removeClass("border-success");
//            }
//            if(Number(num) > Number(0.00) && num <= originalTotal){
//                $("#totalBox").removeClass("border-danger");
//            }
//            if(Number(num) < Number(0.00)){
//                $("#totalBox").addClass("border-danger")
//                return num
//            }
//            if(Number(num) > Number(originalTotal)){
//                $("#totalBox").addClass("border-danger")
//                return num
//            }
//            if(Number(num) === Number(0.00)){
//                $("#submitButton").removeAttr('disabled');
//                $("#totalBox").removeClass("border-danger");
//                $("#totalBox").addClass("border-success")
//            }
//            return num
//        }
//        function findTotal()
//        {
//            let originalTotal = @Model.Total;
//            let totals = $('[id^=totalAmount]').map((i, el) => { return Number($(el).text()); }).get();
//            let sumOfTotals = totals.reduce((a, b) => a + b);
//            return formatNumber((originalTotal - sumOfTotals));
//        }
//        function formatNumber(num)
//        {
//            let rounded = Math.round(Number(num) * 100) / 100;
//            return rounded.toFixed(2);
//        }
//    });