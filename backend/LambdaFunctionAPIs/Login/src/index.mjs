import { getSecret, createDbClient, secret_name } from './shared/utils.mjs';

const getRandomUser = async (client) => {
    try {
        const result = await client.query('SELECT * FROM users ORDER BY RANDOM() LIMIT 1');
        console.log("Query Finished");
        if (result.rows.length > 0) {
            return result.rows[0];
        } else {
            throw new Error('No users found');
        }
    } catch (err) {
        console.error('Error fetching random user:', err);
        throw err;
    }
};

export const handler = async (event) => {
    let client; // Declare the client variable here

    try {
        // Fetch secrets from Secrets Manager
        const secret = await getSecret(secret_name);
        console.log("Secret Fetched");

        // Create a PostgreSQL client using the fetched credentials
        client = createDbClient(secret);

        await client.connect();
        console.log("Client has connected to DB");

        const randomUser = await getRandomUser(client);
        return {
            statusCode: 200,
            body: JSON.stringify(randomUser),
        };
    } catch (err) {
        console.error('Error querying the database:', err);
        return {
            statusCode: 500,
            body: 'Error querying the database',
        };
    } finally {
        if (client) { // Check if the client is defined before calling end()
            await client.end();
        }
    }
};
