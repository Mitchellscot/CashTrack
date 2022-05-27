import getQueryParam from '../Utility/query-params';

const adjustFormBasedOnQueryValue = (queryValue: number) => {
    switch (queryValue) {
        //date
        case 0:
            resetCategorySelect();
            resetNumbersForm();
            resetSecondInputForm();
            (<HTMLInputElement>document.getElementById('qInput')).type = 'date';
            break;
        //date range
        case 1:
            resetCategorySelect();
            resetNumbersForm();
            showSecondInput();
            (<HTMLInputElement>document.getElementById('qInput')).type = 'date';
            break;
        //month
        case 2:
            resetCategorySelect();
            resetNumbersForm();
            resetSecondInputForm();
            (<HTMLInputElement>document.getElementById('qInput')).type = 'month';
            break;
        //quarter
        case 3:
            resetCategorySelect();
            resetNumbersForm();
            resetSecondInputForm();
            (<HTMLInputElement>document.getElementById('qInput')).type = 'month';
            break;
        //year
        case 4:
            resetCategorySelect();
            resetSecondInputForm();
            const firstInput = <HTMLInputElement>document.getElementById('qInput');
            firstInput.type = 'number';
            firstInput.step = 'any';
            firstInput.min = '2012';
            const currentYear: string = (new Date().getFullYear()).toString();
            firstInput.max = currentYear;
            const Q = getQueryParam('Q');
            Q ? firstInput.value = Q : firstInput.value = currentYear;
            (<HTMLInputElement>document.getElementById('qInput')).type = 'number';
            break;
        //amount
        case 5:
            resetCategorySelect();
            resetSecondInputForm();
            //            $("#qInput").val('');
            //            $("#qInput").prop('step', "any");
            //            $("#qInput").prop('min', "0.00");
            //            if (q !== undefined && queryType === "5") {
            //                $("#qInput").val(q);
            //            }
            (<HTMLInputElement>document.getElementById('qInput')).type = 'number';
            break;
        //notes
        case 6:
            resetCategorySelect();
            resetNumbersForm();
            resetSecondInputForm();
            //            if (q !== undefined && queryType === "6") {
            //                $("#qInput").val(q);
            //            }
            (<HTMLInputElement>document.getElementById('qInput')).type = 'text';
            break;
        //merchant
        case 7:
            resetCategorySelect();
            resetNumbersForm();
            resetSecondInputForm();
            (<HTMLInputElement>document.getElementById('qInput')).type = 'text';
            break;
        //subcategory
        case 8:
            resetNumbersForm();
            resetSecondInputForm();
            //add 'display-none' class here
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
            (<HTMLInputElement>document.getElementById('qInput')).type = 'text';
            break;
        //main category
        case 9:
            resetNumbersForm();
            resetSecondInputForm();
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
            (<HTMLInputElement>document.getElementById('qInput')).type = 'text';
            break;
        //tag
        case 10:
            resetCategorySelect();
            resetNumbersForm();
            resetSecondInputForm();
            (<HTMLInputElement>document.getElementById('qInput')).type = 'text';
            console.log('not implemented yet');
            break;
        default:
            resetSecondInputForm();
            resetNumbersForm();
            resetCategorySelect();
            (<HTMLInputElement>document.getElementById('qInput')).type = 'date';
            console.log('default... something went wrong');
    }
}

const formatInputsOnSelectListChange = (): void => {
    let selectListElement = <HTMLSelectElement>document.getElementById("querySelect");
    selectListElement.addEventListener('change', addSelectChangeEventListener, false);
}

const addSelectChangeEventListener = (e: Event): void => {
    const queryValue = parseInt((e.target as HTMLSelectElement).value);
    adjustFormBasedOnQueryValue(queryValue);
}

const formatInputsOnPageLoad = (): void => {
    let queryValue: number = parseInt((<HTMLSelectElement>document.getElementById("querySelect")).value);
    adjustFormBasedOnQueryValue(queryValue);
}

function showSecondInput(): void {
    const secondInput = <HTMLInputElement>document.getElementById('q2Input');
    secondInput.classList.remove('display-none');
    secondInput.classList.add('w-25');
    const firstInput = <HTMLInputElement>document.getElementById('qInput');
    firstInput.classList.remove('w-50');
    firstInput.classList.add('w-25');
}
function resetSecondInputForm(): void {
    document.getElementById("qInput")?.classList.add("w-50");
    const secondInput = <HTMLInputElement>document.getElementById('q2Input');
    secondInput.value = '';
    secondInput.classList.remove('w-25');
    secondInput.classList.add('display-none');
}
function resetNumbersForm(): void {
    const firstInput = <HTMLInputElement>document.getElementById('qInput');
    firstInput.removeAttribute('min');
    firstInput.removeAttribute('step');
    firstInput.value = '';
}
function resetCategorySelect(): void {
    const firstInput = <HTMLInputElement>document.getElementById('qInput');
    firstInput.classList.remove('display-none');
    const categorySelectList = <HTMLInputElement>document.getElementById('categorySelect');
    categorySelectList.classList.add('display-none');

    while (categorySelectList.firstChild) {
        categorySelectList.removeChild(categorySelectList.firstChild);
    }
}

export { formatInputsOnSelectListChange, formatInputsOnPageLoad };
