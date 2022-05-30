//jquery is loaded here:
import { autoSuggestIncomeSourceNames } from "../Utility/source-autocomplete";
import disableLocationIfSourceIsOnline from "../Utility/online-source";

disableLocationIfSourceIsOnline();
autoSuggestIncomeSourceNames();