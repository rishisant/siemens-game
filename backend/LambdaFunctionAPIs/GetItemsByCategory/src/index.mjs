import { getSecret, createDbClient, secret_name } from './shared/utils.mjs';

const getItemsByCategory = async (client, category) => {
    try {
        const result = await client.query('SELECT * FROM store WHERE item_type = $1', [category]);
        console.log(`Items of category '${category}' retrieved from the store.`);
        return result.rows;
    } catch (err) {
        console.error('Error fetching items by category from store:', err);
        throw err;
    }
};

const errorCheck = async (event) => {
    if (!event.body) {
        throw new Error('Request body is missing.');
    }
    const requestBody = JSON.parse(event.body);
    const category = requestBody.category;

    // Validate that the category parameter is provided
    if (!category) {
        throw new Error('Category parameter is missing or invalid.');
    }
    return category;
};

export const handler = async (event) => {
    let client;

    try {
        // Await the errorCheck function to handle asynchronous code properly
        const category = await errorCheck(event);

        const secret = await getSecret(secret_name);
        console.log("Secret Fetched");

        client = createDbClient(secret);
        await client.connect();
        console.log("Client connected to DB");

        const items = await getItemsByCategory(client, category);

        return {
            statusCode: 200,
            category: category,
            body: JSON.stringify(items),
        };
    } catch (err) {
        console.error('Error querying the database:', err);
        return {
            statusCode: 400, // Use 400 status code to indicate a bad request
            body: JSON.stringify({ error: err.message }),
        };
    } finally {
        if (client) {
            await client.end();
        }
    }
};
