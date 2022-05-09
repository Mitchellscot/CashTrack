const getName = () => "Mitchell";

const hideLoadingSpinner = (): void => {
    const loadingSpinner: HTMLElement | null = document.getElementById("#loadingSpinner");
    if (loadingSpinner)
    {
        loadingSpinner.style.display = 'none';
    }
}

export { getName, hideLoadingSpinner };