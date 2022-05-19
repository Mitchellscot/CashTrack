
const disableLocationIfMerchantIsOnline = (): void => {
    const checkBox: HTMLElement | null = document.getElementById("addEditMerchantIsOnline");
    checkBox?.addEventListener('click', (): void => {

        if ((checkBox as HTMLInputElement).checked)
        {
            document.getElementById("addEditMerchantCity")?.setAttribute('disabled', '');
            document.getElementById("addEditMerchantState")?.setAttribute('disabled', '');
        }
        else
        {
            document.getElementById("addEditMerchantCity")?.removeAttribute('disabled');
            document.getElementById("addEditMerchantState")?.removeAttribute('disabled');
        }
    }, false);
    
}
export default disableLocationIfMerchantIsOnline;