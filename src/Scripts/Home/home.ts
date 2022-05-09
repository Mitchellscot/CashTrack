import { getName, hideLoadingSpinner } from '../Utility/utils';

function printName(): void {
    return console.log(`Hello ${getName()}`);
}

printName();
hideLoadingSpinner();