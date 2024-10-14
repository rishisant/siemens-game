import { getSecret, createDbClient, secret_name } from './shared/utils.mjs';

const getAllItems = async (client) => {
    try {
        const result = await client.query('SELECT * FROM store');
        console.log("All items retrieved from the store.");
        return result.rows;
    } catch (err) {
        console.error('Error fetching items from store:', err);
        throw err;
    }
};

export const handler = async (event) => {
    let client;

    try {
        const secret = await getSecret(secret_name);
        console.log("Secret Fetched");

        client = createDbClient(secret);
        await client.connect();
        console.log("Client connected to DB");

        const items = await getAllItems(client);
        return {
            statusCode: 200,
            body: JSON.stringify(items),
        };
    } catch (err) {
        console.error('Error querying the database:', err);
        return {
            statusCode: 500,
            body: 'Error querying the database',
        };
    } finally {
        if (client) {
            await client.end();
        }
    }
};
