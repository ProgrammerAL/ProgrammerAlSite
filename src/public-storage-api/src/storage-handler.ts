import auth from './auth'

const STORAGE_PATH_START = `/storage/`;

function determineFilePath(requestPath: string): string {
	return requestPath.substring(STORAGE_PATH_START.length).toLowerCase();
}

interface completeBody {
	parts: R2UploadedPart[];
}

interface CopyObjectRequestBodyObject {
	fromPath: string;
	toPath: string;
}

export default {
	isValidStorageFilePath(requestPath: string) {
		//Valid if the path to the file starts with /storage/ and also if there is more text after that value
		return requestPath.startsWith(STORAGE_PATH_START)
			&& requestPath.length > (STORAGE_PATH_START.length + 1);
	},

	async getFileAsync(request: Request, env: Env, ctx: ExecutionContext, pathName: string): Promise<Response> {
		const filePath = determineFilePath(pathName);

		const storageObject = await env.STORAGE_BUCKET.get(filePath);
		if (storageObject === null) {
			return new Response(`Object Not Found at path: ${filePath}`, { status: 404 });
		}

		const headers = new Headers();
		storageObject.writeHttpMetadata(headers);
		headers.set('etag', storageObject.httpEtag);

		return new Response(storageObject.body, {
			headers,
		});
	},

	async storeObject(request: Request, env: Env, ctx: ExecutionContext, pathName: string): Promise<Response> {
		const filePath = determineFilePath(pathName);

		var isAuthorized = await auth.canWriteFileAsync(request, env, filePath);
		if (!isAuthorized) {
			return new Response('Unauthorized. Auth token is missing or invalid.', { status: 401 });
		}

		const putResult = await env.STORAGE_BUCKET.put(filePath, request.body);
		if (putResult) {
			return new Response("", { status: 204 });
		}

		return new Response(`An error occurred storing the file to the new location ${filePath}`, { status: 500 });
	},
};
