import CategoryAverages from "../Models/CategoryAverages";

export const loadAveragesOnSelectListChange = (): void => {
    const subCategoryInputs = <NodeListOf<HTMLSelectElement>>document.querySelectorAll('.load-main-category-js');
    subCategoryInputs.forEach(x => x.addEventListener('change', loadAverages, false));
}

const loadAverages = (e: Event): void => {
    const averagesTable = <HTMLTableElement>document.getElementById('averagesTable');
    const subCategoryId = (e.target as HTMLSelectElement).value;
    const sixMonthAverages = <HTMLTableCellElement>document.getElementById(`sixMonthAverages`);
    const thisYearAverages = <HTMLTableCellElement>document.getElementById(`thisYearAverages`);
    const lastYearAverages = <HTMLTableCellElement>document.getElementById(`lastYearAverages`);
    const twoYearsAgoAverages = <HTMLTableCellElement>document.getElementById(`twoYearsAgoAverages`);
    const sixMonthTotals = <HTMLTableCellElement>document.getElementById(`sixMonthTotals`);
    const thisYearTotals = <HTMLTableCellElement>document.getElementById(`thisYearTotals`);
    const lastYearTotals = <HTMLTableCellElement>document.getElementById(`lastYearTotals`);
    const twoYearsAgoTotals = <HTMLTableCellElement>document.getElementById(`twoYearsAgoTotals`);

    fetch(`/api/budget/averages-and-totals/${subCategoryId}`)
        .then(response => response.json())
        .then((averages: CategoryAverages) => {
            averagesTable.classList.remove('visually-hidden');
            sixMonthAverages.textContent = averages.sixMonthAverages.toString();
            thisYearAverages.textContent = averages.thisYearAverages.toString();
            lastYearAverages.textContent = averages.lastYearAverages.toString();
            twoYearsAgoAverages.textContent = averages.twoYearsAgoAverages.toString();
            sixMonthTotals.textContent = averages.sixMonthTotals.toString();
            thisYearTotals.textContent = averages.thisYearTotals.toString();
            lastYearTotals.textContent = averages.lastYearTotals.toString();
            twoYearsAgoTotals.textContent = averages.twoYearsAgoTotals.toString();
        }).catch(err => console.log(err));
}

export const loadMainCategoryOnSubCategorySelect = (): void => {
    const subCategoryInputs = <NodeListOf<HTMLSelectElement>>document.querySelectorAll('.load-main-category-js');
    subCategoryInputs.forEach(x => x.addEventListener('change', loadMainCategory, false));
}

const loadMainCategory = (e: Event): void => {
    const subCategoryId = (e.target as HTMLSelectElement).value;

    fetch(`/api/maincategory/sub-category/${subCategoryId}`)
        .then(response => response.text())
        .then((category: string) => {
            const mainCategoryInput = <HTMLInputElement>document.getElementById(`mainCategoryInput`);
            mainCategoryInput.value = category;
        }).catch(err => console.log(err));
}

export const loadAveragesOnSelectListChangeEdit = (): void => {
    const subCategoryInputs = <NodeListOf<HTMLSelectElement>>document.querySelectorAll('.load-main-category-edit-js');
    subCategoryInputs.forEach(x => x.addEventListener('change', loadAveragesEdit, false));
}

const loadAveragesEdit = (e: Event): void => {
    const id = (e.target as HTMLElement).dataset.id;
    const averagesTable = <HTMLTableElement>document.getElementById(`averagesTable-${id}`);
    const subCategoryId = (e.target as HTMLSelectElement).value;
    console.log(subCategoryId)
    console.log(id)
    const sixMonthAverages = <HTMLTableCellElement>document.getElementById(`sixMonthAverages-${id}`);
    const thisYearAverages = <HTMLTableCellElement>document.getElementById(`thisYearAverages-${id}`);
    const lastYearAverages = <HTMLTableCellElement>document.getElementById(`lastYearAverages-${id}`);
    const twoYearsAgoAverages = <HTMLTableCellElement>document.getElementById(`twoYearsAgoAverages-${id}`);
    const sixMonthTotals = <HTMLTableCellElement>document.getElementById(`sixMonthTotals-${id}`);
    const thisYearTotals = <HTMLTableCellElement>document.getElementById(`thisYearTotals-${id}`);
    const lastYearTotals = <HTMLTableCellElement>document.getElementById(`lastYearTotals-${id}`);
    const twoYearsAgoTotals = <HTMLTableCellElement>document.getElementById(`twoYearsAgoTotals-${id}`);

    fetch(`/api/budget/averages-and-totals/${subCategoryId}`)
        .then(response => response.json())
        .then((averages: CategoryAverages) => {
            averagesTable.classList.remove('visually-hidden');
            sixMonthAverages.textContent = averages.sixMonthAverages.toString();
            thisYearAverages.textContent = averages.thisYearAverages.toString();
            lastYearAverages.textContent = averages.lastYearAverages.toString();
            twoYearsAgoAverages.textContent = averages.twoYearsAgoAverages.toString();
            sixMonthTotals.textContent = averages.sixMonthTotals.toString();
            thisYearTotals.textContent = averages.thisYearTotals.toString();
            lastYearTotals.textContent = averages.lastYearTotals.toString();
            twoYearsAgoTotals.textContent = averages.twoYearsAgoTotals.toString();
        }).catch(err => console.log(err));
}

export const loadMainCategoryOnSubCategorySelectEdit = (): void => {
    const subCategoryInputs = <NodeListOf<HTMLSelectElement>>document.querySelectorAll('.load-main-category-edit-js');
    subCategoryInputs.forEach(x => x.addEventListener('change', loadMainCategoryEdit, false));
}

const loadMainCategoryEdit = (e: Event): void => {
    const subCategoryId = (e.target as HTMLSelectElement).value;
    const id = (e.target as HTMLElement).dataset.id;
    fetch(`/api/maincategory/sub-category/${subCategoryId}`)
        .then(response => response.text())
        .then((category: string) => {
            const mainCategoryInput = <HTMLInputElement>document.getElementById(`mainCategoryInput-${id}`);
            mainCategoryInput.value = category;
        }).catch(err => console.log(err));
}