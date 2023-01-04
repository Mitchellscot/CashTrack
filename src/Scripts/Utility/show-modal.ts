import {Modal} from 'bootstrap';

export default function showModalFromUrlHash(modalId: string) {
	if (window.location.hash === modalId) {
		const modal = new Modal(modalId);
		modal.show();
	}
}