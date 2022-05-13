import { getName } from '../Utility/utils';
import { hideLoadingSpinner, activateSpinnerOnClick } from '../Utility/loading-spinner';

function printName(): void {
    return console.log(`Hello ${getName()}`);
}

printName();
hideLoadingSpinner();
activateSpinnerOnClick();