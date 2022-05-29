console.log('split page');
import { formatAmount } from '../Utility/format-amount';
import { loadMainCategoryOnSplitLoad } from '../Utility/load-main-category';


loadMainCategoryOnSplitLoad();

const updateTotalsWhenAmountChanges = () => {
    const amountInputs = document.querySelectorAll('.split-amount-js');
    amountInputs.forEach(x => x.addEventListener('change', adjustTotals, false));
}

const adjustTotals = (e: Event) => {
    const input = e.target as HTMLInputElement;
    const formattedAmount: number = formatNumber(input.value);
    console.log(formattedAmount);
    const taxAmount = parseFloat(input.dataset.taxAmount!);
    const index = input.dataset.index;
    const isTaxed = (<HTMLInputElement>document.getElementById(`isTaxed-${index}`)).checked;
    console.log(isTaxed);
    const totalAmountElement = <HTMLElement>document.getElementById(`totalAmount-${index}`);
    const overallTotalElement = <HTMLElement>document.getElementById(`total`);
    const amountAfterTaxes = formatNumber(formattedAmount + (formattedAmount * taxAmount));
    console.log(amountAfterTaxes);
    totalAmountElement.innerHTML = isTaxed ? amountAfterTaxes.toString() : formattedAmount.toString();
    const totalAmounts = findTotals();
    overallTotalElement.innerHTML = totalAmounts.toString();
    adjustPageOnAmountChange(totalAmounts);
}

function findTotals(): number {
    const originalTotal: number = parseFloat((<HTMLElement>document.getElementById('total')).dataset.originalAmount!);
    const allTotalElements = <NodeListOf<HTMLElement>>document.querySelectorAll('.total-amount-js');
    let sumOfTotals = 0;
    allTotalElements.forEach(x => sumOfTotals + Number(x.innerHTML));
    console.log(sumOfTotals);
    return formatNumber((originalTotal - sumOfTotals));
}

function adjustPageOnAmountChange(amount: number): void {
    const originalTotal: number = parseFloat((<HTMLElement>document.getElementById('total')).dataset.originalAmount!);
    const totalBox = <HTMLElement>document.getElementById("totalBox");
    const submitButton = <HTMLButtonElement>document.getElementById('submitButton');
    if (amount !== Number(0.00)) {
        submitButton.setAttribute('disabled', '');
        totalBox.classList.remove("border-success");
    }
    if (amount > Number(0.00) && amount <= originalTotal) {
        totalBox.classList.remove("border-danger");
    }
    if (amount < Number(0.00)) {
        totalBox.classList.add("border-danger");
    }
    if (amount > Number(originalTotal)) {
        totalBox.classList.add("border-danger");
    }
    if (amount === Number(0.00)) {
        submitButton.removeAttribute('disabled');
        totalBox.classList.remove("border-danger");
        totalBox.classList.add("border-success");
    }
}

function formatNumber(num: number | string): number {
    return parseFloat((Math.round(parseFloat(num.toString()) * Math.pow(10, 2)) / Math.pow(10, 2)).toFixed(2));
}

updateTotalsWhenAmountChanges();

//$('#amount-@i').change(() => {
//    //format the number in the input box to 2 decimals
//    $("#amount-@i").val(formatNumber($("#amount-@i").val()));
//    //apply tax if tax is checked and then update the Total Table Cell
//    let totalAmountAfterTax = applyTax($("#amount-@i").val(), $("#isTaxed-@i").is(":checked"));
//    $("#totalAmount-@i").text(totalAmountAfterTax);
//    //Update the form, if all totals equal the original amount, save changes button is displayed.
//    $("#total").text(adjustFormBasedOnTotal(findTotal()));
//});

//$("#isTaxed-@i").change(() => {
//    let taxedOrNot = applyTax($("#amount-@i").val(), $("#isTaxed-@i").is(":checked"));
//    $("#totalAmount-@i").text(taxedOrNot);
//    $("#total").text(adjustFormBasedOnTotal(findTotal()));
//});


//function applyTax(num, isTaxed) {
//    let amount = Number(num);
//    let taxAmount = Number(@Model.Tax);
//    if (isTaxed) {
//        return formatNumber(amount + (amount * taxAmount));
//    }
//    else {
//        return num;
//    }
//}

//function adjustFormBasedOnTotal(num) {
//    let originalTotal = @Model.Total;
//    if (Number(num) !== Number(0.00)) {
//        $("#submitButton").prop('disabled', true);
//        $("#totalBox").removeClass("border-success");
//    }
//    if (Number(num) > Number(0.00) && num <= originalTotal) {
//        $("#totalBox").removeClass("border-danger");
//    }
//    if (Number(num) < Number(0.00)) {
//        $("#totalBox").addClass("border-danger")
//        return num
//    }
//    if (Number(num) > Number(originalTotal)) {
//        $("#totalBox").addClass("border-danger")
//        return num
//    }
//    if (Number(num) === Number(0.00)) {
//        $("#submitButton").removeAttr('disabled');
//        $("#totalBox").removeClass("border-danger");
//        $("#totalBox").addClass("border-success")
//    }
//    return num
//}

//function findTotal() {
//    let originalTotal = @Model.Total;
//    let totals = $('[id^=totalAmount]').map((i, el) => { return Number($(el).text()); }).get();
//    let sumOfTotals = totals.reduce((a, b) => a + b);
//    return formatNumber((originalTotal - sumOfTotals));
//}
//function formatNumberzzzzzz(num) {
//    let rounded = Math.round(Number(num) * 100) / 100;
//    return rounded.toFixed(2);
//}
