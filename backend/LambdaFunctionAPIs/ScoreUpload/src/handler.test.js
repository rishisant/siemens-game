import { jest } from '@jest/globals';
import { handler, runHandler } from './index.mjs';
import { getSecret, createDbClient, secret_name } from './shared/utils.mjs';

// Mock utilities
jest.mock('./shared/utils.mjs', () => ({
  getSecret: jest.fn(() => Promise.resolve({ username: 'user', password: 'pass' })),
  createDbClient: jest.fn(() => ({
    connect: jest.fn(),
    query: jest.fn((query, values) => {
      // Mock database behavior
      if (query.includes('SELECT * FROM users WHERE user_id')) {
        return values[0] === 300 ? { rows: [{}] } : { rows: [] }; // User exists if user_id is 300
      }

      if (query.includes('SELECT score FROM game_scores')) {
        return { rows: [{ score: "10.00" }] }; // Existing score of "10.00" for the user
      }

      if (query.includes('INSERT INTO game_scores') || query.includes('DO UPDATE')) {
        // Simulate the UPSERT operation to return expected structure
        return {
          rows: [
            { user_id: values[0], game_id: values[1], score: values[2].toFixed(2) },
          ],
        };
      }

      return { rows: [] };
    }),
    end: jest.fn(),
  })),
}));


let originalData;

// Back up original data and restore it after tests
beforeAll(async () => {
  const client = createDbClient(await getSecret(secret_name));
  await client.connect();

  // Back up the original data
  const backupResult = await client.query('SELECT * FROM test_game_scores');
  originalData = backupResult.rows;

  await client.end();
});

afterAll(async () => {
  const client = createDbClient(await getSecret(secret_name));
  await client.connect();

  // Clear table
  await client.query('DELETE FROM test_game_scores');

  // Restore original data
  for (const row of originalData) {
    await client.query(
      'INSERT INTO test_game_scores (score_id, user_id, game_id, score) VALUES ($1, $2, $3, $4)',
      [row.score_id, row.user_id, row.game_id, row.score]
    );
  }

  await client.end();
});


describe('Score Handler Tests', () => {
  it('should successfully insert a new score if lower for game_id 7', async () => {
    const mockEvent = {
        body: JSON.stringify({ user_id: 300, game_id: 7, score: 1 }), // New lower score
    };

    const response = await runHandler(mockEvent, 'test_game_scores'); // Use test table

    //expect(response.statusCode).toBe(200);
    console.log("Debug:",response);
    const responseBody = JSON.parse(response.body);
    expect(responseBody.message).toBe('Score operation completed successfully');
    expect(responseBody.score.score).toBe("1.00");
});

  it('should not insert a new score if higher for game_id 7', async () => {
    const mockEvent = {
      body: JSON.stringify({ user_id: 300, game_id: 7, score: 15 }), // New higher score
    };

    const response = await handler(mockEvent);

    expect(response.statusCode).toBe(200);
    const responseBody = JSON.parse(response.body);
    expect(responseBody.score.message).toBe('No Score Uploaded, not the lowest score.');
  });

  it('should increment the score by 1 for game_id 6', async () => {
    const mockEvent = {
      body: JSON.stringify({ user_id: 302, game_id: 6 }), // Game_id 6 increments score by 1
    };

    const response = await handler(mockEvent);

    expect(response.statusCode).toBe(200);
    const responseBody = JSON.parse(response.body);
    expect(responseBody.score.score).toBe("1.00"); // Original score was 10
  });

  it('should return 400 if user_id is not found', async () => {
    const mockEvent = {
      body: JSON.stringify({ user_id: 999, game_id: 7, score: 5 }), // Non-existent user_id
    };

    const response = await handler(mockEvent);

    expect(response.statusCode).toBe(400);
    const responseBody = JSON.parse(response.body);
    expect(responseBody.error).toBe('User with ID 999 does not exist.');
  });

  it('should return 400 if request body is missing', async () => {
    const mockEvent = {};

    const response = await handler(mockEvent);

    expect(response.statusCode).toBe(400);
    const responseBody = JSON.parse(response.body);
    expect(responseBody.error).toBe('Request body is missing.');
  });

  it('should return 400 if required parameters are missing', async () => {
    const mockEvent = {
      body: JSON.stringify({ user_id: 1 }), // Missing game_id and score
    };

    const response = await handler(mockEvent);

    expect(response.statusCode).toBe(400);
    const responseBody = JSON.parse(response.body);
    expect(responseBody.error).toBe('User ID, game ID, and score must be provided.');
  });


});
