/* eslint-disable  @typescript-eslint/no-non-null-assertion */

const shorOrHideToast = (messages: HTMLElement) => {
    if (messages?.dataset.show?.toLowerCase() === "true")
    {
        messages.classList.add('show');
        setTimeout(() => {
            messages.classList.add('hide');
        }, 2500);
        setTimeout(() => {
            messages.style.display = 'none';
        }, 3000);
    }
    else
    {
        messages!.style.display = 'none';
    }
}

const getToastMessages = (): void => {
    const messages: NodeListOf<HTMLElement> = document.querySelectorAll("#success-toast, #info-toast, #danger-toast");
    messages.forEach(x => shorOrHideToast(x));
}

export default getToastMessages;