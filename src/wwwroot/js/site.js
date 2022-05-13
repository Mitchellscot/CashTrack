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

/***/ "./Scripts/Utility/toast-messages.ts":
/*!*******************************************!*\
  !*** ./Scripts/Utility/toast-messages.ts ***!
  \*******************************************/
/***/ ((__unused_webpack_module, __webpack_exports__, __webpack_require__) => {

__webpack_require__.r(__webpack_exports__);
/* harmony export */ __webpack_require__.d(__webpack_exports__, {
/* harmony export */   "getSuccessMessage": () => (/* binding */ getSuccessMessage),
/* harmony export */   "getToastInfoMessage": () => (/* binding */ getToastInfoMessage)
/* harmony export */ });
var getToastInfoMessage = function () {
    var infoMessage = document.querySelector("#info-toast");
    if (infoMessage) {
        if (infoMessage.dataset.show && infoMessage.dataset.show.toLowerCase() === "true") {
            infoMessage.style.display = '';
            infoMessage.classList.add('hide');
            infoMessage.classList.add('show');
            console.log(infoMessage.innerText);
            console.log(infoMessage.dataset.show);
        }
    }
};
var getSuccessMessage = function () {
    var successMessage = document.querySelector("#success-toast");
    if (successMessage) {
        if (successMessage.dataset.show && successMessage.dataset.show.toLowerCase() === "true") {
            /*START HERE - display:none is still on the style list when show=true*/
            successMessage.style.display = '';
            successMessage.classList.add('hide');
            successMessage.classList.add('show');
            console.log(successMessage.innerText);
            console.log(successMessage.dataset.show);
        }
    }
};


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
  !*** ./Scripts/Site/site.ts ***!
  \******************************/
__webpack_require__.r(__webpack_exports__);
/* harmony import */ var _Utility_loading_spinner__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! ../Utility/loading-spinner */ "./Scripts/Utility/loading-spinner.ts");
/* harmony import */ var _Utility_toast_messages__WEBPACK_IMPORTED_MODULE_1__ = __webpack_require__(/*! ../Utility/toast-messages */ "./Scripts/Utility/toast-messages.ts");


(0,_Utility_loading_spinner__WEBPACK_IMPORTED_MODULE_0__.hideLoadingSpinner)();
(0,_Utility_loading_spinner__WEBPACK_IMPORTED_MODULE_0__.activateSpinnerOnClick)();
(0,_Utility_toast_messages__WEBPACK_IMPORTED_MODULE_1__.getToastInfoMessage)();
(0,_Utility_toast_messages__WEBPACK_IMPORTED_MODULE_1__.getSuccessMessage)();

})();

/******/ })()
;
//# sourceMappingURL=data:application/json;charset=utf-8;base64,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Q0w7QUFFbkYsNEVBQWtCLEVBQUUsQ0FBQztBQUNyQixnRkFBc0IsRUFBRSxDQUFDO0FBQ3pCLDRFQUFtQixFQUFFLENBQUM7QUFDdEIsMEVBQWlCLEVBQUUsQ0FBQyIsInNvdXJjZXMiOlsid2VicGFjazovL2Nhc2gtdHJhY2svLi9TY3JpcHRzL1V0aWxpdHkvbG9hZGluZy1zcGlubmVyLnRzIiwid2VicGFjazovL2Nhc2gtdHJhY2svLi9TY3JpcHRzL1V0aWxpdHkvdG9hc3QtbWVzc2FnZXMudHMiLCJ3ZWJwYWNrOi8vY2FzaC10cmFjay93ZWJwYWNrL2Jvb3RzdHJhcCIsIndlYnBhY2s6Ly9jYXNoLXRyYWNrL3dlYnBhY2svcnVudGltZS9kZWZpbmUgcHJvcGVydHkgZ2V0dGVycyIsIndlYnBhY2s6Ly9jYXNoLXRyYWNrL3dlYnBhY2svcnVudGltZS9oYXNPd25Qcm9wZXJ0eSBzaG9ydGhhbmQiLCJ3ZWJwYWNrOi8vY2FzaC10cmFjay93ZWJwYWNrL3J1bnRpbWUvbWFrZSBuYW1lc3BhY2Ugb2JqZWN0Iiwid2VicGFjazovL2Nhc2gtdHJhY2svLi9TY3JpcHRzL1NpdGUvc2l0ZS50cyJdLCJzb3VyY2VzQ29udGVudCI6WyJjb25zdCBoaWRlTG9hZGluZ1NwaW5uZXIgPSAoKTogdm9pZCA9PiB7XHJcbiAgICBjb25zdCBsb2FkaW5nU3Bpbm5lcjogSFRNTEVsZW1lbnQgfCBudWxsID0gZG9jdW1lbnQuZ2V0RWxlbWVudEJ5SWQoXCJsb2FkaW5nU3Bpbm5lclwiKTtcclxuICAgIGlmIChsb2FkaW5nU3Bpbm5lcilcclxuICAgIHtcclxuICAgICAgICBsb2FkaW5nU3Bpbm5lci5zdHlsZS5kaXNwbGF5ID0gJ25vbmUnO1xyXG4gICAgfVxyXG59XHJcblxyXG5jb25zdCBhY3RpdmF0ZVNwaW5uZXJPbkNsaWNrID0gKCk6IHZvaWQgPT4ge1xyXG4gICAgY29uc3QgYW5jaG9yTGlua3M6IE5vZGVMaXN0T2Y8SFRNTEFuY2hvckVsZW1lbnQ+IHwgbnVsbCA9IGRvY3VtZW50LnF1ZXJ5U2VsZWN0b3JBbGwoXCJhXCIpO1xyXG4gICAgY29uc3QgbG9hZGluZ1NwaW5uZXI6IEhUTUxFbGVtZW50IHwgbnVsbCA9IGRvY3VtZW50LnF1ZXJ5U2VsZWN0b3IoXCIjbG9hZGluZ1NwaW5uZXJcIik7XHJcbiAgICBhbmNob3JMaW5rcy5mb3JFYWNoKHggPT4geC5hZGRFdmVudExpc3RlbmVyKCdjbGljaycsIGZ1bmN0aW9uICgpIHtcclxuICAgICAgICBpZiAobG9hZGluZ1NwaW5uZXIpXHJcbiAgICAgICAgICAgIGxvYWRpbmdTcGlubmVyLnN0eWxlLmRpc3BsYXkgPSAnJztcclxuICAgIH0sIGZhbHNlKSlcclxufVxyXG5cclxuZXhwb3J0IHsgaGlkZUxvYWRpbmdTcGlubmVyLCBhY3RpdmF0ZVNwaW5uZXJPbkNsaWNrIH07IiwiXHJcbmV4cG9ydCBjb25zdCBnZXRUb2FzdEluZm9NZXNzYWdlID0gKCk6IHZvaWQgPT4ge1xyXG4gICAgY29uc3QgaW5mb01lc3NhZ2U6IEhUTUxFbGVtZW50IHwgbnVsbCA9IGRvY3VtZW50LnF1ZXJ5U2VsZWN0b3IoXCIjaW5mby10b2FzdFwiKTtcclxuICAgIGlmIChpbmZvTWVzc2FnZSlcclxuICAgIHtcclxuICAgICAgICBpZiAoaW5mb01lc3NhZ2UuZGF0YXNldC5zaG93ICYmIGluZm9NZXNzYWdlLmRhdGFzZXQuc2hvdy50b0xvd2VyQ2FzZSgpID09PSBcInRydWVcIilcclxuICAgICAgICB7XHJcbiAgICAgICAgICAgIGluZm9NZXNzYWdlLnN0eWxlLmRpc3BsYXkgPSAnJztcclxuICAgICAgICAgICAgaW5mb01lc3NhZ2UuY2xhc3NMaXN0LmFkZCgnaGlkZScpO1xyXG4gICAgICAgICAgICBpbmZvTWVzc2FnZS5jbGFzc0xpc3QuYWRkKCdzaG93Jyk7XHJcbiAgICAgICAgICAgIGNvbnNvbGUubG9nKGluZm9NZXNzYWdlLmlubmVyVGV4dCk7XHJcbiAgICAgICAgICAgIGNvbnNvbGUubG9nKGluZm9NZXNzYWdlLmRhdGFzZXQuc2hvdyk7XHJcbiAgICAgICAgfVxyXG4gICAgfVxyXG59XHJcbmV4cG9ydCBjb25zdCBnZXRTdWNjZXNzTWVzc2FnZSA9ICgpOiB2b2lkID0+IHtcclxuICAgIGNvbnN0IHN1Y2Nlc3NNZXNzYWdlOiBIVE1MRWxlbWVudCB8IG51bGwgPSBkb2N1bWVudC5xdWVyeVNlbGVjdG9yKFwiI3N1Y2Nlc3MtdG9hc3RcIik7XHJcbiAgICBpZiAoc3VjY2Vzc01lc3NhZ2UpXHJcbiAgICB7XHJcbiAgICAgICAgaWYgKHN1Y2Nlc3NNZXNzYWdlLmRhdGFzZXQuc2hvdyAmJiBzdWNjZXNzTWVzc2FnZS5kYXRhc2V0LnNob3cudG9Mb3dlckNhc2UoKSA9PT0gXCJ0cnVlXCIpXHJcbiAgICAgICAge1xyXG4gICAgICAgICAgICAvKlNUQVJUIEhFUkUgLSBkaXNwbGF5Om5vbmUgaXMgc3RpbGwgb24gdGhlIHN0eWxlIGxpc3Qgd2hlbiBzaG93PXRydWUqL1xyXG4gICAgICAgICAgICBzdWNjZXNzTWVzc2FnZS5zdHlsZS5kaXNwbGF5PScnO1xyXG4gICAgICAgICAgICBzdWNjZXNzTWVzc2FnZS5jbGFzc0xpc3QuYWRkKCdoaWRlJyk7XHJcbiAgICAgICAgICAgIHN1Y2Nlc3NNZXNzYWdlLmNsYXNzTGlzdC5hZGQoJ3Nob3cnKTtcclxuICAgICAgICAgICAgY29uc29sZS5sb2coc3VjY2Vzc01lc3NhZ2UuaW5uZXJUZXh0KTtcclxuICAgICAgICAgICAgY29uc29sZS5sb2coc3VjY2Vzc01lc3NhZ2UuZGF0YXNldC5zaG93KTtcclxuICAgICAgICB9XHJcbiAgICB9XHJcbn0iLCIvLyBUaGUgbW9kdWxlIGNhY2hlXG52YXIgX193ZWJwYWNrX21vZHVsZV9jYWNoZV9fID0ge307XG5cbi8vIFRoZSByZXF1aXJlIGZ1bmN0aW9uXG5mdW5jdGlvbiBfX3dlYnBhY2tfcmVxdWlyZV9fKG1vZHVsZUlkKSB7XG5cdC8vIENoZWNrIGlmIG1vZHVsZSBpcyBpbiBjYWNoZVxuXHR2YXIgY2FjaGVkTW9kdWxlID0gX193ZWJwYWNrX21vZHVsZV9jYWNoZV9fW21vZHVsZUlkXTtcblx0aWYgKGNhY2hlZE1vZHVsZSAhPT0gdW5kZWZpbmVkKSB7XG5cdFx0cmV0dXJuIGNhY2hlZE1vZHVsZS5leHBvcnRzO1xuXHR9XG5cdC8vIENyZWF0ZSBhIG5ldyBtb2R1bGUgKGFuZCBwdXQgaXQgaW50byB0aGUgY2FjaGUpXG5cdHZhciBtb2R1bGUgPSBfX3dlYnBhY2tfbW9kdWxlX2NhY2hlX19bbW9kdWxlSWRdID0ge1xuXHRcdC8vIG5vIG1vZHVsZS5pZCBuZWVkZWRcblx0XHQvLyBubyBtb2R1bGUubG9hZGVkIG5lZWRlZFxuXHRcdGV4cG9ydHM6IHt9XG5cdH07XG5cblx0Ly8gRXhlY3V0ZSB0aGUgbW9kdWxlIGZ1bmN0aW9uXG5cdF9fd2VicGFja19tb2R1bGVzX19bbW9kdWxlSWRdKG1vZHVsZSwgbW9kdWxlLmV4cG9ydHMsIF9fd2VicGFja19yZXF1aXJlX18pO1xuXG5cdC8vIFJldHVybiB0aGUgZXhwb3J0cyBvZiB0aGUgbW9kdWxlXG5cdHJldHVybiBtb2R1bGUuZXhwb3J0cztcbn1cblxuIiwiLy8gZGVmaW5lIGdldHRlciBmdW5jdGlvbnMgZm9yIGhhcm1vbnkgZXhwb3J0c1xuX193ZWJwYWNrX3JlcXVpcmVfXy5kID0gKGV4cG9ydHMsIGRlZmluaXRpb24pID0+IHtcblx0Zm9yKHZhciBrZXkgaW4gZGVmaW5pdGlvbikge1xuXHRcdGlmKF9fd2VicGFja19yZXF1aXJlX18ubyhkZWZpbml0aW9uLCBrZXkpICYmICFfX3dlYnBhY2tfcmVxdWlyZV9fLm8oZXhwb3J0cywga2V5KSkge1xuXHRcdFx0T2JqZWN0LmRlZmluZVByb3BlcnR5KGV4cG9ydHMsIGtleSwgeyBlbnVtZXJhYmxlOiB0cnVlLCBnZXQ6IGRlZmluaXRpb25ba2V5XSB9KTtcblx0XHR9XG5cdH1cbn07IiwiX193ZWJwYWNrX3JlcXVpcmVfXy5vID0gKG9iaiwgcHJvcCkgPT4gKE9iamVjdC5wcm90b3R5cGUuaGFzT3duUHJvcGVydHkuY2FsbChvYmosIHByb3ApKSIsIi8vIGRlZmluZSBfX2VzTW9kdWxlIG9uIGV4cG9ydHNcbl9fd2VicGFja19yZXF1aXJlX18uciA9IChleHBvcnRzKSA9PiB7XG5cdGlmKHR5cGVvZiBTeW1ib2wgIT09ICd1bmRlZmluZWQnICYmIFN5bWJvbC50b1N0cmluZ1RhZykge1xuXHRcdE9iamVjdC5kZWZpbmVQcm9wZXJ0eShleHBvcnRzLCBTeW1ib2wudG9TdHJpbmdUYWcsIHsgdmFsdWU6ICdNb2R1bGUnIH0pO1xuXHR9XG5cdE9iamVjdC5kZWZpbmVQcm9wZXJ0eShleHBvcnRzLCAnX19lc01vZHVsZScsIHsgdmFsdWU6IHRydWUgfSk7XG59OyIsImltcG9ydCB7IGhpZGVMb2FkaW5nU3Bpbm5lciwgYWN0aXZhdGVTcGlubmVyT25DbGljayB9IGZyb20gJy4uL1V0aWxpdHkvbG9hZGluZy1zcGlubmVyJztcclxuaW1wb3J0IHsgZ2V0VG9hc3RJbmZvTWVzc2FnZSwgZ2V0U3VjY2Vzc01lc3NhZ2UgfSBmcm9tICcuLi9VdGlsaXR5L3RvYXN0LW1lc3NhZ2VzJztcclxuXHJcbmhpZGVMb2FkaW5nU3Bpbm5lcigpO1xyXG5hY3RpdmF0ZVNwaW5uZXJPbkNsaWNrKCk7XHJcbmdldFRvYXN0SW5mb01lc3NhZ2UoKTtcclxuZ2V0U3VjY2Vzc01lc3NhZ2UoKTsiXSwibmFtZXMiOltdLCJzb3VyY2VSb290IjoiIn0=