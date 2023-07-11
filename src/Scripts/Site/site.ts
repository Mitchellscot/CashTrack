import 'bootstrap/dist/js/bootstrap.min.js';
import {
	hideLoadingSpinner,
	activateSpinnerOnClick,
} from '../Utility/loading-spinner';
import getToastMessages from '../Utility/toast-messages';
import initializeTooltips from '../Utility/initialize-tooltips';
import { Button } from 'bootstrap';

initializeTooltips();
hideLoadingSpinner();
activateSpinnerOnClick();
getToastMessages();