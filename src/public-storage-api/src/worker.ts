import storageHandler from './storage-handler';

const corsHeaders = {
	"Access-Control-Allow-Origin": "*",
	"Access-Control-Allow-Methods": "GET,HEAD,POST,PUT,OPTIONS",
	"Access-Control-Max-Age": "86400",
}

function handleOptions(request: Request): Response {
	// Make sure the necessary headers are present
	// for this to be a valid pre-flight request
	let headers = request.headers
	if (
		headers.get("Origin") !== null &&
		headers.get("Access-Control-Request-Method") !== null &&
		headers.get("Access-Control-Request-Headers") !== null
	) {
		// Handle CORS pre-flight request.
		// If you want to check or reject the requested method + headers
		// you can do that here.
		const respHeaders = new Headers({
			...corsHeaders,
		});

		// Allow all future content Request headers to go back to browser
		// such as Authorization (Bearer) or X-Client-Name-Version
		var accessControlAllowHeaders = request.headers.get("Access-Control-Request-Headers");
		if (accessControlAllowHeaders) {
			respHeaders.append("Access-Control-Allow-Headers", accessControlAllowHeaders);
		}

		return new Response(null, {
			headers: respHeaders,
		})
	}
	else {
		// Handle standard OPTIONS request.
		// If you want to allow other HTTP Methods, you can do that here.
		return new Response(null, {
			headers: {
				Allow: "GET, HEAD, POST, PUT, OPTIONS",
			},
		})
	}
}

function addCorsHeadersToStandardResponse(response: Response) {
	response.headers.set("Access-Control-Allow-Origin", "*")
	response.headers.set("Access-Control-Allow-Methods", "GET, POST, PUT, DELETE, OPTIONS")
}

const worker = {
	async fetch(request: Request, env: Env, ctx: ExecutionContext): Promise<Response> {
		try {

			const url = new URL(request.url);
			const pathName = url.pathname.toLowerCase();
			const method = request.method.toLowerCase();
			const action = url.searchParams.get("action");

			let response: Response | null = null;

			if (method === "options") {
				return handleOptions(request);
			}
			else if (storageHandler.isValidStorageFilePath(pathName)) {
				if (method === "get") {
					response = await storageHandler.getFileAsync(request, env, ctx, pathName);
				}
				else if (method === "put") {
					if (action === "store-object") {
						response = await storageHandler.storeObject(request, env, ctx, pathName);
					}
				}
			}

			if (!response) {
				response = new Response(`Route not found`, { status: 404 });
			}

			addCorsHeadersToStandardResponse(response);
			return response;
		}
		catch (error: any) {
			return new Response(error.message, { status: 500 });
		}
	},
};

export default worker;
