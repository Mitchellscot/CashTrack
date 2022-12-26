export default function getQueryParam(queryParam: string): string | undefined {
	let queries: string[] = [];
	const values: string[] = window.location.href
		.slice(window.location.href.indexOf('?') + 1)
		.split('&');
	for (const v of values) {
		queries = v.split('=');
		if (queries[0] === queryParam) {
			// If you aren't feeling lazy, add a way to remove the + from a url string when a param has multiple words...
			// like Alliance+Redwoods
			return decodeURIComponent(queries[1]);
		}
	}

	return undefined;
}
