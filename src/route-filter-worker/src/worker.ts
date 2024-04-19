// User agents handled by Prerender
const BOT_AGENTS = [
	"googlebot",
	"yahoo! slurp",
	"bingbot",
	"yandex",
	"baiduspider",
	"facebookexternalhit",
	"twitterbot",
	"rogerbot",
	"linkedinbot",
	"embedly",
	"quora link preview",
	"showyoubot",
	"outbrain",
	"pinterest/0.",
	"developers.google.com/+/web/snippet",
	"slackbot",
	"vkshare",
	"w3c_validator",
	"redditbot",
	"applebot",
	"whatsapp",
	"flipboard",
	"tumblr",
	"bitlybot",
	"skypeuripreview",
	"nuzzel",
	"discordbot",
	"google page speed",
	"qwantify",
	"pinterestbot",
	"bitrix link preview",
	"xing-contenttabreceiver",
	"chrome-lighthouse",
	"telegrambot",
	"integration-test", // Integration testing
	"google-inspectiontool"
];

// These are the extensions that the worker will skip prerendering
// even if any other conditions pass.
// const IGNORE_EXTENSIONS = [
// 	".js",
// 	".css",
// 	".xml",
// 	".less",
// 	".png",
// 	".jpg",
// 	".jpeg",
// 	".gif",
// 	".pdf",
// 	".doc",
// 	".txt",
// 	".ico",
// 	".rss",
// 	".zip",
// 	".mp3",
// 	".rar",
// 	".exe",
// 	".wmv",
// 	".doc",
// 	".avi",
// 	".ppt",
// 	".mpg",
// 	".mpeg",
// 	".tif",
// 	".wav",
// 	".mov",
// 	".psd",
// 	".ai",
// 	".xls",
// 	".mp4",
// 	".m4a",
// 	".swf",
// 	".dat",
// 	".dmg",
// 	".iso",
// 	".flv",
// 	".m4v",
// 	".torrent",
// 	".woff",
// 	".ttf",
// 	".svg",
// 	".webmanifest",
// ];

export default {
	/**
	 * Hooks into the request, and changes origin if needed
	 */
	async fetch(request: Request, env: Env): Promise<Response> {
		return await handleRequest(request, env).catch(
			(err) => new Response(err.stack, { status: 500 })
		);
	},
};

async function handleRequest(request: Request, env: Env): Promise<Response> {
	const url = new URL(request.url);
	const userAgent = request.headers.get("User-Agent")?.toLowerCase() || "";
	const pathName = url.pathname.toLowerCase();
	// const extension = pathName
	// 	.substring(pathName.lastIndexOf(".") || pathName.length)
	// 	?.toLowerCase();

	// console.log(`Received request from url '${request.url}' with pathname '${pathName}'`);

	//TODO: Once metatags are made for comics pages, include /Comics/
	// Non robot user agent
	// Ignore extensions
	if (request.method.toLowerCase() == "get"
		&& (pathName.startsWith('/posts/') || pathName.startsWith('/comics/'))
		&& BOT_AGENTS.some((bot) => userAgent == bot)) {

		//example: 		https://programmeral.com/posts/20240409-WhyAzureManagedIdentitiesNoMoreSecrets
		//Redirects to: https://storage.programmeral.com/posts/20240409-WhyAzureManagedIdentitiesNoMoreSecrets/metatags.html
		const newUrl = `${env.STORAGE_API_ENDPOINT}/storage/${url.pathname}/metatags.html`;

		// console.log(`Redirecting request from '${request.url}' to '${newUrl}' because it has User-Agent ${userAgent}`);

		return fetch(new Request(newUrl, {
			headers: request.headers,
			redirect: "manual",
		}));
	}


	return fetch(request);
}