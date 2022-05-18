const hideLoadingSpinner = (): void => {
    const loadingSpinner: HTMLElement | null = document.getElementById("loadingSpinner");
    if (loadingSpinner)
    {
        loadingSpinner.style.display = 'none';
    }
}

const activateSpinnerOnClick = (): void => {
    const anchorLinks: NodeListOf<HTMLAnchorElement> | null = document.querySelectorAll(".spin-it");
    const loadingSpinner: HTMLElement | null = document.querySelector("#loadingSpinner");
    anchorLinks.forEach(x => x.addEventListener('click', function () {
        if (loadingSpinner)
            loadingSpinner.style.display = '';
    }, false))
}

export { hideLoadingSpinner, activateSpinnerOnClick };