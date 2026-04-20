export class AuthenticationError extends Error {
    status: number;
    data: any;

    constructor(data?: any) {
        super('The user is not authenticated.');
        this.name = 'AuthenticationError';
        this.status = 401;
        this.data = data;

        Object.setPrototypeOf(this, AuthenticationError.prototype);
    }
}