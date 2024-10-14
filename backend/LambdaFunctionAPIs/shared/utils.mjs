import { SecretsManagerClient, GetSecretValueCommand } from '@aws-sdk/client-secrets-manager';
import pkg from 'pg';  // PostgreSQL client
const { Client } = pkg;

// Database Credentials
export const DBCREDENTIALS = {
    host: "seimensgame.cluster-ro-cby6ieo44z27.us-east-1.rds.amazonaws.com",  // RDS Proxy endpoint
    port: 5432,
    database: "seimensgame",
};

// Secret Name
export const secret_name = "rds!cluster-88080b8d-e111-4714-8ce4-0c8ef81b966d";

// Function to get secret from AWS Secrets Manager
export const getSecret = async (secretName) => {
    console.log("Creating Secrets Manager Client");
    const client = new SecretsManagerClient({
        region: "us-east-1",
    });

    try {
        console.log("Fetching secret");
        const response = await client.send(
            new GetSecretValueCommand({
                SecretId: secretName,
                VersionStage: "AWSCURRENT", // VersionStage defaults to AWSCURRENT if unspecified
            })
        );
        const secret = JSON.parse(response.SecretString);
        return secret;
    } catch (error) {
        console.log("Error fetching secret:", error);
        throw error;
    }
};

// Function to create a PostgreSQL client
export const createDbClient = (credentials) => {
    return new Client({
        host: DBCREDENTIALS.host,
        user: credentials.username,
        password: credentials.password,
        database: DBCREDENTIALS.database,
        port: DBCREDENTIALS.port,
    });
};
