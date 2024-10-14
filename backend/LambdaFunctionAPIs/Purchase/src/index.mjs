import { getSecret, createDbClient, secret_name } from './shared/utils.mjs';

// Function to check if the user owns the item
const checkItemOwnership = (itemsOwned, itemId) => {
    return itemsOwned.includes(itemId);
};

// Function to fetch item details from the store
const getItemDetails = async (client, itemId) => {
    const result = await client.query('SELECT * FROM store WHERE item_id = $1', [itemId]);
    if (result.rows.length === 0) {
        throw new Error('Item not found in the store.');
    }
    return result.rows[0];
};

// Function to purchase an item for a user
const purchaseItem = async (client, employeeId, itemId) => {
    try {
        // Fetch the user's current data
        const userResult = await client.query(
            'SELECT current_coins, items_owned FROM users WHERE employee_id = $1',
            [employeeId]
        );
        if (userResult.rows.length === 0) {
            throw new Error('User not found with the specified employee_id.');
        }

        let { current_coins, items_owned } = userResult.rows[0];

        // Check if the user already owns the item
        if (checkItemOwnership(items_owned, itemId)) {
            throw new Error('User already owns this item.');
        }

        // Fetch the item's details from the store
        const itemDetails = await getItemDetails(client, itemId);
        const itemPrice = itemDetails.item_price;

        // Check if the user has enough coins
        if (current_coins < itemPrice) {
            throw new Error('Not enough coins to purchase this item.');
        }

        // Subtract the item price from the user's current coins and add the item to items_owned
        current_coins -= itemPrice;
        items_owned = [...items_owned, itemId];

        // Update the user's current_coins and items_owned in the database
        await client.query(
            'UPDATE users SET current_coins = $1, items_owned = $2 WHERE employee_id = $3',
            [current_coins, items_owned, employeeId]
        );

        return {
            statusCode: 200,
            message: `Successfully purchased item ID ${itemId} for employee ID ${employeeId}.`,
            current_coins,
            items_owned
        };
    } catch (err) {
        console.error('Error purchasing item:', err);
        throw err;
    }
};

export const handler = async (event) => {
    let client;

    try {
        if (!event.body) {
            throw new Error('Request body is missing.');
        }

        const requestBody = JSON.parse(event.body);
        const { employee_id, item_id } = requestBody;

        if (!employee_id || !item_id) {
            throw new Error('Employee ID and item ID must be provided.');
        }

        // Fetch secrets from Secrets Manager
        const secret = await getSecret(secret_name);
        console.log("Secret Fetched");

        // Create a PostgreSQL client using the fetched credentials
        client = createDbClient(secret);
        await client.connect();
        console.log("Client connected to DB");

        // Process the item purchase
        const response = await purchaseItem(client, employee_id, item_id);

        return {
            statusCode: 200,
            body: JSON.stringify(response)
        };
    } catch (err) {
        console.error('Error processing request:', err);
        return {
            statusCode: 400,
            body: JSON.stringify({ error: err.message })
        };
    } finally {
        if (client) {
            await client.end();
        }
    }
};
