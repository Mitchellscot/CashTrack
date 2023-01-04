const hideLoadingSpinner = (): void => {
	const loadingSpinner = document.getElementById('loadingSpinner');
	loadingSpinner!.style.display = 'none';
};

const activateSpinnerOnClick = (): void => {
	const anchorLinks: NodeListOf<HTMLAnchorElement> | undefined = document.querySelectorAll('.spin-it');
	const loadingSpinner = document.querySelector<HTMLElement>('#loadingSpinner')!;
	anchorLinks.forEach(x => {
		x.addEventListener(
			'click',
			() => {
				if (loadingSpinner) {
					loadingSpinner.style.display = '';
				}
			},
			false,
		);
	},
	);
};

export {hideLoadingSpinner, activateSpinnerOnClick};
