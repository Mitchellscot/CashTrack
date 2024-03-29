
const shorOrHideToast = (messages: HTMLElement) => {
	if (messages?.dataset.show?.toLowerCase() === 'true') {
		messages.style.display = 'block';
		setTimeout(() => {
			messages.classList.add('hide');
		}, 3500);
		setTimeout(() => {
			messages.style.display = 'none';
		}, 4000);
	} else {
		messages.style.display = 'none';
	}
};

const getToastMessages = (): void => {
	const messages: NodeListOf<HTMLElement> = document.querySelectorAll(
		'#success-toast, #info-toast, #danger-toast',
	);
	messages.forEach(x => {
		shorOrHideToast(x);
	});
};

export default getToastMessages;
