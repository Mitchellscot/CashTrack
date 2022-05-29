const disableLocationIfSourceIsOnline = (): void => {
    const checkBox: HTMLElement | null = document.getElementById("addEditSourceIsOnline");
    checkBox?.addEventListener('click', (): void => {

        if ((checkBox as HTMLInputElement).checked) {
            document.getElementById("addEditSourceCity")?.setAttribute('disabled', '');
            document.getElementById("addEditSourceState")?.setAttribute('disabled', '');
        }
        else {
            document.getElementById("addEditSourceCity")?.removeAttribute('disabled');
            document.getElementById("addEditSourceState")?.removeAttribute('disabled');
        }
    }, false);

}
export default disableLocationIfSourceIsOnline;