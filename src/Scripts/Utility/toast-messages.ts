
export const getToastInfoMessage = (): void => {
    const infoMessage: HTMLElement | null = document.querySelector("#info-toast");
    if (infoMessage)
    {
        if (infoMessage.dataset.show && infoMessage.dataset.show.toLowerCase() === "true")
        {
            infoMessage.style.display = '';
            infoMessage.classList.add('hide');
            infoMessage.classList.add('show');
            console.log(infoMessage.innerText);
            console.log(infoMessage.dataset.show);
        }
    }
}
export const getSuccessMessage = (): void => {
    const successMessage: HTMLElement | null = document.querySelector("#success-toast");
    if (successMessage)
    {
        if (successMessage.dataset.show && successMessage.dataset.show.toLowerCase() === "true")
        {
            /*START HERE - display:none is still on the style list when show=true*/
            successMessage.style.display='';
            successMessage.classList.add('hide');
            successMessage.classList.add('show');
            console.log(successMessage.innerText);
            console.log(successMessage.dataset.show);
        }
    }
}