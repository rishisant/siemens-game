/* jslint ignore:start */
import { getSecret, createDbClient, secret_name } from './shared/utils.mjs';
/* jslint ignore:end */

// Function to check if the user exists in the database
/**
 * Checks if a user exists in the database.
 *
 * @param {Object} client - PostgreSQL client for database operations.
 * @param {number} userId - The ID of the user to check.
 * @returns {Promise<boolean>} - Resolves to true if the user exists, false otherwise.
 */
const checkUserExists = async (client, userId) => {
    const result = await client.query('SELECT * FROM users WHERE user_id = $1', [userId]);
    return result.rows.length > 0;
};

// Function to get the existing score
/**
 * Retrieves the existing score for a user and game from the database.
 *
 * @param {Object} client - PostgreSQL client for database operations.
 * @param {number} userId - The ID of the user.
 * @param {number} gameId - The ID of the game.
 * @param {string} [tableName='game_scores'] - The name of the table to query.
 * @returns {Promise<number|null>} - The existing score or null if no score exists.
 */
const getExistingScore = async (client, userId, gameId, tableName = 'game_scores') => {
    const result = await client.query(
        `SELECT score FROM ${tableName} WHERE user_id = $1 AND game_id = $2`,
        [userId, gameId]
    );
    return result.rows[0]?.score || null;
};


// Function to insert or update the score using UPSERT logic
/**
 * Inserts or updates the score using UPSERT logic.
 *
 * @param {Object} client - PostgreSQL client for database operations.
 * @param {number} userId - The ID of the user.
 * @param {number} gameId - The ID of the game.
 * @param {number} score - The score to insert or update.
 * @param {string} [tableName='game_scores'] - The name of the table to update.
 * @returns {Promise<Object>} - The inserted or updated score record.
 */
const upsertGameScore = async (client, userId, gameId, score, tableName = 'game_scores') => {
    const query = `
        INSERT INTO ${tableName} (user_id, game_id, score)
        VALUES ($1, $2, $3)
        ON CONFLICT (user_id, game_id) 
        DO UPDATE SET score = $3
        RETURNING *;
    `;
    const result = await client.query(query, [userId, gameId, score]);
    return result.rows[0];
};

/**
 * Handles the score logic based on game-specific conditions.
 *
 * @param {Object} client - PostgreSQL client for database operations.
 * @param {number} userId - The ID of the user.
 * @param {number} gameId - The ID of the game.
 * @param {number} newScore - The new score to evaluate.
 * @param {string} [tableName='game_scores'] - The name of the table to update.
 * @returns {Promise<Object>} - The result of the score operation, including the upserted score.
 */
const handleScoreInsert = async (client, userId, gameId, newScore, tableName = 'game_scores') => {
    const existingScore = await getExistingScore(client, userId, gameId, tableName);

    if (gameId === 5 || gameId === 7) {
        if (existingScore !== null && newScore >= existingScore) {
            return { message: "No Score Uploaded, not the lowest score." };
        }
    } else if (gameId === 6) {
        const updatedScore = (existingScore || 0) + 1;
        return await upsertGameScore(client, userId, gameId, updatedScore, tableName);
    }

    return await upsertGameScore(client, userId, gameId, newScore, tableName);
};

/**
 * Runs the score upload flow against a specific table. Exported for tests;
 * production traffic goes through `handler`, which pins the table name.
 *
 * @param {Object} event - The Lambda event object containing the request data.
 * @param {string} [tableName='game_scores'] - The name of the table to operate on.
 * @returns {Promise<Object>} - HTTP response object with status and score operation result.
 */
export const runHandler = async (event, tableName = 'game_scores') => {
    let client;

    try {
        if (!event.body) {
            throw new Error('Request body is missing.');
        }

        const requestBody = JSON.parse(event.body);
        const { user_id, game_id, score } = requestBody;

        if (!user_id || !game_id || (score === undefined && game_id !== 6)) {
            throw new Error('User ID, game ID, and score must be provided.');
        }

        // Fetch secrets from Secrets Manager
        const secret = await getSecret(secret_name);
        console.log("Secret Fetched");

        // Create a PostgreSQL client using the fetched credentials
        client = createDbClient(secret);
        await client.connect();
        console.log("Client connected to DB");

        // Check if the user exists
        const userExists = await checkUserExists(client, user_id);
        if (!userExists) {
            throw new Error(`User with ID ${user_id} does not exist.`);
        }

        // Handle the score insertion logic
        const upsertedScore = await handleScoreInsert(client, user_id, game_id, score, tableName);

        return {
            statusCode: 200,
            body: JSON.stringify({
                message: 'Score operation completed successfully',
                score: upsertedScore,
            }),
        };
    } catch (err) {
        console.error('Error processing request:', err);
        return {
            statusCode: 400,
            body: JSON.stringify({ error: err.message }),
        };
    } finally {
        if (client) {
            await client.end();
        }
    }
};

/**
 * Lambda entry point. AWS invokes handler(event, context), so the table name
 * must never be taken from the second argument — it would receive the Lambda
 * context object and corrupt the SQL.
 *
 * @param {Object} event - The Lambda event object containing the request data.
 * @returns {Promise<Object>} - HTTP response object with status and score operation result.
 */
export const handler = async (event) => runHandler(event);
