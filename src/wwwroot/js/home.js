/******/ (() => { // webpackBootstrap
/******/ 	"use strict";
/******/ 	var __webpack_modules__ = ({

/***/ "./Scripts/Utility/loading-spinner.ts":
/*!********************************************!*\
  !*** ./Scripts/Utility/loading-spinner.ts ***!
  \********************************************/
/***/ ((__unused_webpack_module, __webpack_exports__, __webpack_require__) => {

__webpack_require__.r(__webpack_exports__);
/* harmony export */ __webpack_require__.d(__webpack_exports__, {
/* harmony export */   "activateSpinnerOnClick": () => (/* binding */ activateSpinnerOnClick),
/* harmony export */   "hideLoadingSpinner": () => (/* binding */ hideLoadingSpinner)
/* harmony export */ });
var hideLoadingSpinner = function () {
    var loadingSpinner = document.getElementById("loadingSpinner");
    if (loadingSpinner) {
        loadingSpinner.style.display = 'none';
    }
};
var activateSpinnerOnClick = function () {
    var anchorLinks = document.querySelectorAll("a");
    var loadingSpinner = document.querySelector("#loadingSpinner");
    anchorLinks.forEach(function (x) { return x.addEventListener('click', function () {
        if (loadingSpinner)
            loadingSpinner.style.display = '';
    }, false); });
};



/***/ }),

/***/ "./Scripts/Utility/utils.ts":
/*!**********************************!*\
  !*** ./Scripts/Utility/utils.ts ***!
  \**********************************/
/***/ ((__unused_webpack_module, __webpack_exports__, __webpack_require__) => {

__webpack_require__.r(__webpack_exports__);
/* harmony export */ __webpack_require__.d(__webpack_exports__, {
/* harmony export */   "getName": () => (/* binding */ getName)
/* harmony export */ });
var getName = function () { return "Mitchell"; };



/***/ })

/******/ 	});
/************************************************************************/
/******/ 	// The module cache
/******/ 	var __webpack_module_cache__ = {};
/******/ 	
/******/ 	// The require function
/******/ 	function __webpack_require__(moduleId) {
/******/ 		// Check if module is in cache
/******/ 		var cachedModule = __webpack_module_cache__[moduleId];
/******/ 		if (cachedModule !== undefined) {
/******/ 			return cachedModule.exports;
/******/ 		}
/******/ 		// Create a new module (and put it into the cache)
/******/ 		var module = __webpack_module_cache__[moduleId] = {
/******/ 			// no module.id needed
/******/ 			// no module.loaded needed
/******/ 			exports: {}
/******/ 		};
/******/ 	
/******/ 		// Execute the module function
/******/ 		__webpack_modules__[moduleId](module, module.exports, __webpack_require__);
/******/ 	
/******/ 		// Return the exports of the module
/******/ 		return module.exports;
/******/ 	}
/******/ 	
/************************************************************************/
/******/ 	/* webpack/runtime/define property getters */
/******/ 	(() => {
/******/ 		// define getter functions for harmony exports
/******/ 		__webpack_require__.d = (exports, definition) => {
/******/ 			for(var key in definition) {
/******/ 				if(__webpack_require__.o(definition, key) && !__webpack_require__.o(exports, key)) {
/******/ 					Object.defineProperty(exports, key, { enumerable: true, get: definition[key] });
/******/ 				}
/******/ 			}
/******/ 		};
/******/ 	})();
/******/ 	
/******/ 	/* webpack/runtime/hasOwnProperty shorthand */
/******/ 	(() => {
/******/ 		__webpack_require__.o = (obj, prop) => (Object.prototype.hasOwnProperty.call(obj, prop))
/******/ 	})();
/******/ 	
/******/ 	/* webpack/runtime/make namespace object */
/******/ 	(() => {
/******/ 		// define __esModule on exports
/******/ 		__webpack_require__.r = (exports) => {
/******/ 			if(typeof Symbol !== 'undefined' && Symbol.toStringTag) {
/******/ 				Object.defineProperty(exports, Symbol.toStringTag, { value: 'Module' });
/******/ 			}
/******/ 			Object.defineProperty(exports, '__esModule', { value: true });
/******/ 		};
/******/ 	})();
/******/ 	
/************************************************************************/
var __webpack_exports__ = {};
// This entry need to be wrapped in an IIFE because it need to be isolated against other modules in the chunk.
(() => {
/*!******************************!*\
  !*** ./Scripts/home/home.ts ***!
  \******************************/
__webpack_require__.r(__webpack_exports__);
/* harmony import */ var _Utility_utils__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! ../Utility/utils */ "./Scripts/Utility/utils.ts");
/* harmony import */ var _Utility_loading_spinner__WEBPACK_IMPORTED_MODULE_1__ = __webpack_require__(/*! ../Utility/loading-spinner */ "./Scripts/Utility/loading-spinner.ts");


function printName() {
    return console.log("Hello ".concat((0,_Utility_utils__WEBPACK_IMPORTED_MODULE_0__.getName)()));
}
printName();
(0,_Utility_loading_spinner__WEBPACK_IMPORTED_MODULE_1__.hideLoadingSpinner)();
(0,_Utility_loading_spinner__WEBPACK_IMPORTED_MODULE_1__.activateSpinnerOnClick)();

})();

/******/ })()
;
//# sourceMappingURL=data:application/json;charset=utf-8;base64,eyJ2ZXJzaW9uIjozLCJmaWxlIjoiaG9tZS5qcyIsIm1hcHBpbmdzIjoiOzs7Ozs7Ozs7Ozs7Ozs7QUFBQSxJQUFNLGtCQUFrQixHQUFHO0lBQ3ZCLElBQU0sY0FBYyxHQUF1QixRQUFRLENBQUMsY0FBYyxDQUFDLGdCQUFnQixDQUFDLENBQUM7SUFDckYsSUFBSSxjQUFjLEVBQ2xCO1FBQ0ksY0FBYyxDQUFDLEtBQUssQ0FBQyxPQUFPLEdBQUcsTUFBTSxDQUFDO0tBQ3pDO0FBQ0wsQ0FBQztBQUVELElBQU0sc0JBQXNCLEdBQUc7SUFDM0IsSUFBTSxXQUFXLEdBQXlDLFFBQVEsQ0FBQyxnQkFBZ0IsQ0FBQyxHQUFHLENBQUMsQ0FBQztJQUN6RixJQUFNLGNBQWMsR0FBdUIsUUFBUSxDQUFDLGFBQWEsQ0FBQyxpQkFBaUIsQ0FBQyxDQUFDO0lBQ3JGLFdBQVcsQ0FBQyxPQUFPLENBQUMsV0FBQyxJQUFJLFFBQUMsQ0FBQyxnQkFBZ0IsQ0FBQyxPQUFPLEVBQUU7UUFDakQsSUFBSSxjQUFjO1lBQ2QsY0FBYyxDQUFDLEtBQUssQ0FBQyxPQUFPLEdBQUcsRUFBRSxDQUFDO0lBQzFDLENBQUMsRUFBRSxLQUFLLENBQUMsRUFIZ0IsQ0FHaEIsQ0FBQztBQUNkLENBQUM7QUFFcUQ7Ozs7Ozs7Ozs7Ozs7OztBQ2pCdEQsSUFBTSxPQUFPLEdBQUcsY0FBTSxpQkFBVSxFQUFWLENBQVUsQ0FBQztBQUVkOzs7Ozs7O1VDRm5CO1VBQ0E7O1VBRUE7VUFDQTtVQUNBO1VBQ0E7VUFDQTtVQUNBO1VBQ0E7VUFDQTtVQUNBO1VBQ0E7VUFDQTtVQUNBO1VBQ0E7O1VBRUE7VUFDQTs7VUFFQTtVQUNBO1VBQ0E7Ozs7O1dDdEJBO1dBQ0E7V0FDQTtXQUNBO1dBQ0EseUNBQXlDLHdDQUF3QztXQUNqRjtXQUNBO1dBQ0E7Ozs7O1dDUEE7Ozs7O1dDQUE7V0FDQTtXQUNBO1dBQ0EsdURBQXVELGlCQUFpQjtXQUN4RTtXQUNBLGdEQUFnRCxhQUFhO1dBQzdEOzs7Ozs7Ozs7Ozs7O0FDTjJDO0FBQzZDO0FBRXhGLFNBQVMsU0FBUztJQUNkLE9BQU8sT0FBTyxDQUFDLEdBQUcsQ0FBQyxnQkFBUyx1REFBTyxFQUFFLENBQUUsQ0FBQyxDQUFDO0FBQzdDLENBQUM7QUFFRCxTQUFTLEVBQUUsQ0FBQztBQUNaLDRFQUFrQixFQUFFLENBQUM7QUFDckIsZ0ZBQXNCLEVBQUUsQ0FBQyIsInNvdXJjZXMiOlsid2VicGFjazovL2Nhc2gtdHJhY2svLi9TY3JpcHRzL1V0aWxpdHkvbG9hZGluZy1zcGlubmVyLnRzIiwid2VicGFjazovL2Nhc2gtdHJhY2svLi9TY3JpcHRzL1V0aWxpdHkvdXRpbHMudHMiLCJ3ZWJwYWNrOi8vY2FzaC10cmFjay93ZWJwYWNrL2Jvb3RzdHJhcCIsIndlYnBhY2s6Ly9jYXNoLXRyYWNrL3dlYnBhY2svcnVudGltZS9kZWZpbmUgcHJvcGVydHkgZ2V0dGVycyIsIndlYnBhY2s6Ly9jYXNoLXRyYWNrL3dlYnBhY2svcnVudGltZS9oYXNPd25Qcm9wZXJ0eSBzaG9ydGhhbmQiLCJ3ZWJwYWNrOi8vY2FzaC10cmFjay93ZWJwYWNrL3J1bnRpbWUvbWFrZSBuYW1lc3BhY2Ugb2JqZWN0Iiwid2VicGFjazovL2Nhc2gtdHJhY2svLi9TY3JpcHRzL2hvbWUvaG9tZS50cyJdLCJzb3VyY2VzQ29udGVudCI6WyJjb25zdCBoaWRlTG9hZGluZ1NwaW5uZXIgPSAoKTogdm9pZCA9PiB7XHJcbiAgICBjb25zdCBsb2FkaW5nU3Bpbm5lcjogSFRNTEVsZW1lbnQgfCBudWxsID0gZG9jdW1lbnQuZ2V0RWxlbWVudEJ5SWQoXCJsb2FkaW5nU3Bpbm5lclwiKTtcclxuICAgIGlmIChsb2FkaW5nU3Bpbm5lcilcclxuICAgIHtcclxuICAgICAgICBsb2FkaW5nU3Bpbm5lci5zdHlsZS5kaXNwbGF5ID0gJ25vbmUnO1xyXG4gICAgfVxyXG59XHJcblxyXG5jb25zdCBhY3RpdmF0ZVNwaW5uZXJPbkNsaWNrID0gKCk6IHZvaWQgPT4ge1xyXG4gICAgY29uc3QgYW5jaG9yTGlua3M6IE5vZGVMaXN0T2Y8SFRNTEFuY2hvckVsZW1lbnQ+IHwgbnVsbCA9IGRvY3VtZW50LnF1ZXJ5U2VsZWN0b3JBbGwoXCJhXCIpO1xyXG4gICAgY29uc3QgbG9hZGluZ1NwaW5uZXI6IEhUTUxFbGVtZW50IHwgbnVsbCA9IGRvY3VtZW50LnF1ZXJ5U2VsZWN0b3IoXCIjbG9hZGluZ1NwaW5uZXJcIik7XHJcbiAgICBhbmNob3JMaW5rcy5mb3JFYWNoKHggPT4geC5hZGRFdmVudExpc3RlbmVyKCdjbGljaycsIGZ1bmN0aW9uICgpIHtcclxuICAgICAgICBpZiAobG9hZGluZ1NwaW5uZXIpXHJcbiAgICAgICAgICAgIGxvYWRpbmdTcGlubmVyLnN0eWxlLmRpc3BsYXkgPSAnJztcclxuICAgIH0sIGZhbHNlKSlcclxufVxyXG5cclxuZXhwb3J0IHsgaGlkZUxvYWRpbmdTcGlubmVyLCBhY3RpdmF0ZVNwaW5uZXJPbkNsaWNrIH07IiwiY29uc3QgZ2V0TmFtZSA9ICgpID0+IFwiTWl0Y2hlbGxcIjtcclxuXHJcbmV4cG9ydCB7IGdldE5hbWUgfTsiLCIvLyBUaGUgbW9kdWxlIGNhY2hlXG52YXIgX193ZWJwYWNrX21vZHVsZV9jYWNoZV9fID0ge307XG5cbi8vIFRoZSByZXF1aXJlIGZ1bmN0aW9uXG5mdW5jdGlvbiBfX3dlYnBhY2tfcmVxdWlyZV9fKG1vZHVsZUlkKSB7XG5cdC8vIENoZWNrIGlmIG1vZHVsZSBpcyBpbiBjYWNoZVxuXHR2YXIgY2FjaGVkTW9kdWxlID0gX193ZWJwYWNrX21vZHVsZV9jYWNoZV9fW21vZHVsZUlkXTtcblx0aWYgKGNhY2hlZE1vZHVsZSAhPT0gdW5kZWZpbmVkKSB7XG5cdFx0cmV0dXJuIGNhY2hlZE1vZHVsZS5leHBvcnRzO1xuXHR9XG5cdC8vIENyZWF0ZSBhIG5ldyBtb2R1bGUgKGFuZCBwdXQgaXQgaW50byB0aGUgY2FjaGUpXG5cdHZhciBtb2R1bGUgPSBfX3dlYnBhY2tfbW9kdWxlX2NhY2hlX19bbW9kdWxlSWRdID0ge1xuXHRcdC8vIG5vIG1vZHVsZS5pZCBuZWVkZWRcblx0XHQvLyBubyBtb2R1bGUubG9hZGVkIG5lZWRlZFxuXHRcdGV4cG9ydHM6IHt9XG5cdH07XG5cblx0Ly8gRXhlY3V0ZSB0aGUgbW9kdWxlIGZ1bmN0aW9uXG5cdF9fd2VicGFja19tb2R1bGVzX19bbW9kdWxlSWRdKG1vZHVsZSwgbW9kdWxlLmV4cG9ydHMsIF9fd2VicGFja19yZXF1aXJlX18pO1xuXG5cdC8vIFJldHVybiB0aGUgZXhwb3J0cyBvZiB0aGUgbW9kdWxlXG5cdHJldHVybiBtb2R1bGUuZXhwb3J0cztcbn1cblxuIiwiLy8gZGVmaW5lIGdldHRlciBmdW5jdGlvbnMgZm9yIGhhcm1vbnkgZXhwb3J0c1xuX193ZWJwYWNrX3JlcXVpcmVfXy5kID0gKGV4cG9ydHMsIGRlZmluaXRpb24pID0+IHtcblx0Zm9yKHZhciBrZXkgaW4gZGVmaW5pdGlvbikge1xuXHRcdGlmKF9fd2VicGFja19yZXF1aXJlX18ubyhkZWZpbml0aW9uLCBrZXkpICYmICFfX3dlYnBhY2tfcmVxdWlyZV9fLm8oZXhwb3J0cywga2V5KSkge1xuXHRcdFx0T2JqZWN0LmRlZmluZVByb3BlcnR5KGV4cG9ydHMsIGtleSwgeyBlbnVtZXJhYmxlOiB0cnVlLCBnZXQ6IGRlZmluaXRpb25ba2V5XSB9KTtcblx0XHR9XG5cdH1cbn07IiwiX193ZWJwYWNrX3JlcXVpcmVfXy5vID0gKG9iaiwgcHJvcCkgPT4gKE9iamVjdC5wcm90b3R5cGUuaGFzT3duUHJvcGVydHkuY2FsbChvYmosIHByb3ApKSIsIi8vIGRlZmluZSBfX2VzTW9kdWxlIG9uIGV4cG9ydHNcbl9fd2VicGFja19yZXF1aXJlX18uciA9IChleHBvcnRzKSA9PiB7XG5cdGlmKHR5cGVvZiBTeW1ib2wgIT09ICd1bmRlZmluZWQnICYmIFN5bWJvbC50b1N0cmluZ1RhZykge1xuXHRcdE9iamVjdC5kZWZpbmVQcm9wZXJ0eShleHBvcnRzLCBTeW1ib2wudG9TdHJpbmdUYWcsIHsgdmFsdWU6ICdNb2R1bGUnIH0pO1xuXHR9XG5cdE9iamVjdC5kZWZpbmVQcm9wZXJ0eShleHBvcnRzLCAnX19lc01vZHVsZScsIHsgdmFsdWU6IHRydWUgfSk7XG59OyIsImltcG9ydCB7IGdldE5hbWUgfSBmcm9tICcuLi9VdGlsaXR5L3V0aWxzJztcclxuaW1wb3J0IHsgaGlkZUxvYWRpbmdTcGlubmVyLCBhY3RpdmF0ZVNwaW5uZXJPbkNsaWNrIH0gZnJvbSAnLi4vVXRpbGl0eS9sb2FkaW5nLXNwaW5uZXInO1xyXG5cclxuZnVuY3Rpb24gcHJpbnROYW1lKCk6IHZvaWQge1xyXG4gICAgcmV0dXJuIGNvbnNvbGUubG9nKGBIZWxsbyAke2dldE5hbWUoKX1gKTtcclxufVxyXG5cclxucHJpbnROYW1lKCk7XHJcbmhpZGVMb2FkaW5nU3Bpbm5lcigpO1xyXG5hY3RpdmF0ZVNwaW5uZXJPbkNsaWNrKCk7Il0sIm5hbWVzIjpbXSwic291cmNlUm9vdCI6IiJ9