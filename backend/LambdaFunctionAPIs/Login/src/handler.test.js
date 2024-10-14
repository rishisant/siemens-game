import { handler } from './index.mjs';
import { SecretsManagerClient, GetSecretValueCommand } from '@aws-sdk/client-secrets-manager';
import pkg from 'pg';

const { Client } = pkg;

// Mock the AWS SDK and PostgreSQL client
jest.mock('@aws-sdk/client-secrets-manager');
jest.mock('pg');

describe('Lambda Handler Tests', () => {
    beforeEach(() => {
        jest.clearAllMocks();
    });

    test('should successfully fetch a random user', async () => {
        // Mock the SecretsManagerClient to return fake credentials
        SecretsManagerClient.mockImplementation(() => ({
            send: jest.fn().mockResolvedValue({
                SecretString: JSON.stringify({
                    username: 'mock_user',
                    password: 'mock_password',
                }),
            }),
        }));

        // Mock the PostgreSQL client
        Client.mockImplementation(() => ({
            connect: jest.fn().mockResolvedValue(),
            query: jest.fn().mockResolvedValue({
                rows: [{ user_id: 1, user_name: 'test_user' }],
            }),
            end: jest.fn().mockResolvedValue(),
        }));

        const event = {}; // Dummy event for testing
        const result = await handler(event);

        expect(result.statusCode).toBe(200);
        expect(JSON.parse(result.body)).toEqual({ user_id: 1, user_name: 'test_user' });
    });

    test('should handle error when no users are found', async () => {
        // Mock the SecretsManagerClient to return fake credentials
        SecretsManagerClient.mockImplementation(() => ({
            send: jest.fn().mockResolvedValue({
                SecretString: JSON.stringify({
                    username: 'mock_user',
                    password: 'mock_password',
                }),
            }),
        }));

        // Mock the PostgreSQL client to return an empty result
        Client.mockImplementation(() => ({
            connect: jest.fn().mockResolvedValue(),
            query: jest.fn().mockResolvedValue({ rows: [] }),
            end: jest.fn().mockResolvedValue(),
        }));

        const event = {}; // Dummy event for testing
        const result = await handler(event);

        expect(result.statusCode).toBe(500);
        expect(result.body).toBe('Error querying the database');
    });

    test('should handle error when Secrets Manager fails', async () => {
        // Mock the SecretsManagerClient to throw an error
        SecretsManagerClient.mockImplementation(() => ({
            send: jest.fn().mockRejectedValue(new Error('Secrets Manager error')),
        }));

        const event = {}; // Dummy event for testing
        await expect(handler(event)).rejects.toThrow('Secrets Manager error');
    });

    test('should handle error when database connection fails', async () => {
        // Mock the SecretsManagerClient to return fake credentials
        SecretsManagerClient.mockImplementation(() => ({
            send: jest.fn().mockResolvedValue({
                SecretString: JSON.stringify({
                    username: 'mock_user',
                    password: 'mock_password',
                }),
            }),
        }));

        // Mock the PostgreSQL client to throw an error on connect
        Client.mockImplementation(() => ({
            connect: jest.fn().mockRejectedValue(new Error('Database connection error')),
            end: jest.fn().mockResolvedValue(),
        }));

        const event = {}; // Dummy event for testing
        const result = await handler(event);

        expect(result.statusCode).toBe(500);
        expect(result.body).toBe('Error querying the database');
    });
});
