import Tooltip from 'bootstrap/js/dist/tooltip';

export default function initializeTooltips() {
    const tooltipTriggerList = document.querySelectorAll('[data-bs-toggle="tooltip"]') as unknown as HTMLElement[]
    const tooltipList = [...tooltipTriggerList].map(x => new Tooltip(x))
}