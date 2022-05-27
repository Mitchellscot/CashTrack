export default function getQueryParam(queryParam: string): string | undefined {
    let queries: Array<string> = []
    let values: Array<string> = window.location.href.slice(window.location.href.indexOf('?') + 1).split('&');
    for (let i: number = 0; i < values.length; i++) {
        queries = values[i].split('=');
        if (queries[0] === queryParam) {
            return queries[1];
        }
    }
    return undefined;
}