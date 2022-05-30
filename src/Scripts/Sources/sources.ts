import disableLocationIfSourceIsOnline from '../Utility/online-source';
import { autoSuggestIncomeSourceNames } from '../Utility/source-autocomplete';
import 'jquery-validation';
import 'jquery-validation-unobtrusive';

autoSuggestIncomeSourceNames();
disableLocationIfSourceIsOnline();