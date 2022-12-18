import bootstrap from "../../wwwroot/lib/bootstrap/dist/js/bootstrap";

export default function initializeTooltips() {
    const tooltipTriggerList = document.querySelectorAll('[data-bs-toggle="tooltip"]') as unknown as HTMLElement[]
    const tooltipList = [...tooltipTriggerList].map(x => new bootstrap.Tooltip(x))
}