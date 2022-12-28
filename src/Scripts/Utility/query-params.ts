export default function getQueryParam(queryParam: string): string | undefined {
	let queries: string[] = [];
	const values: string[] = window.location.href
		.slice(window.location.href.indexOf('?') + 1)
		.split('&');
	for (const v of values) {
		queries = v.split('=');
		if (queries[0] === queryParam) {
			const param = queries[1].replace(/[+]/gi, ' ');
			return decodeURIComponent(param);
		}
	}

	return undefined;
}
