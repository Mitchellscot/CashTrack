export default function getQueryParam(queryParam: string): string | undefined {
    let queries: Array<string> = []
    let values: Array<string> = window.location.href.slice(window.location.href.indexOf('?') + 1).split('&');
    for (let i: number = 0; i < values.length; i++) {
        queries = values[i].split('=');
        if (queries[0] === queryParam) {
            //if you aren't feeling lazy, add a way to remove the + from a url string when a param has multiple words... 
            //like Alliance+Redwoods
            return decodeURIComponent(queries[1]);
        }
    }
    return undefined;
}