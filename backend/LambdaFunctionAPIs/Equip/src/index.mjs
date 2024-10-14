import { getSecret, createDbClient, secret_name } from './shared/utils.mjs';

// Function to update the items based on the action (equip/unequip)
const updateItems = async (client, employeeId, items, action) => {
    try {
        // Get the current items_owned and items_equipped for the user
        const result = await client.query(
            'SELECT items_owned, items_equipped FROM users WHERE employee_id = $1',
            [employeeId]
        );

        if (result.rows.length === 0) {
            throw new Error('User not found with the specified employee_id.');
        }

        let { items_owned, items_equipped } = result.rows[0];

        // Ensure the user owns all the items
        const itemsNotOwned = items.filter(item => !items_owned.includes(item));
        if (itemsNotOwned.length > 0) {
            throw new Error(`User does not own these items: ${itemsNotOwned}`);
        }

        if (action === 'equip') {
            // Equip: Add the items to items_equipped if they are not already equipped
            items_equipped = [...new Set([...items_equipped, ...items])]; // Use Set to ensure uniqueness
        } else if (action === 'unequip') {
            // Unequip: Remove the items from items_equipped
            items_equipped = items_equipped.filter(item => !items.includes(item));
        } else {
            throw new Error('Invalid action. Use "equip" or "unequip".');
        }

        // Update the user's items_equipped in the database
        await client.query(
            'UPDATE users SET items_equipped = $1 WHERE employee_id = $2',
            [items_equipped, employeeId]
        );

        return {
            statusCode: 200,
            message: `Successfully ${action}ped items for employee ID ${employeeId}.`,
            items_equipped
        };
    } catch (err) {
        console.error('Error updating items:', err);
        throw err;
    }
};

export const handler = async (event) => {
    let client;
    
    try {
        // Parse the event body to get action, items, and employee_id
        const requestBody = JSON.parse(event.body);
        const { action, items, employee_id } = requestBody;

        if (!action || !Array.isArray(items) || items.length === 0 || !employee_id) {
            throw new Error('Invalid request payload. Ensure action, items, and employee_id are provided.');
        }

        // Fetch secrets from Secrets Manager
        const secret = await getSecret(secret_name);
        console.log("Secret Fetched");

        // Create a PostgreSQL client using the fetched credentials
        client = createDbClient(secret);
        await client.connect();
        console.log("Client connected to DB");

        // Update items based on the action (equip/unequip)
        const response = await updateItems(client, employee_id, items, action);

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
