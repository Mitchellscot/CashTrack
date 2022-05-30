import disableLocationIfSourceIsOnline from '../Utility/online-source';
import { autoSuggestIncomeSourceNames } from '../Utility/source-autocomplete';
console.log('income sources page');

autoSuggestIncomeSourceNames();
disableLocationIfSourceIsOnline();