import fs from 'fs';
import archiver from 'archiver';
import AWS from 'aws-sdk';
import path from 'path';
import { fileURLToPath } from 'url';

// Set up the equivalent of __dirname in ES module
const __filename = fileURLToPath(import.meta.url);
const __dirname = path.dirname(__filename);

// Set up AWS Lambda client
const lambda = new AWS.Lambda({
    region: 'us-east-1' // Change this to your region
});

// Lambda function name (replace this with your actual function name)
const functionName = 'Login';

// Build zip file
const outputZipPath = path.join(__dirname, 'lambda_function.zip');

const zipLambdaFunction = async () => {
    return new Promise((resolve, reject) => {
        const output = fs.createWriteStream(outputZipPath);
        const archive = archiver('zip', {
            zlib: { level: 9 }
        });

        output.on('close', () => {
            console.log(`Zipped ${archive.pointer()} total bytes`);
            resolve();
        });

        archive.on('error', (err) => {
            console.error('Error during file archiving:', err);
            reject(err);
        });

        archive.pipe(output);

        // Check if src/index.mjs exists
        const indexPath = path.join(__dirname, 'src/index.mjs');
        if (fs.existsSync(indexPath)) {
            archive.file(indexPath, { name: 'index.mjs' });
        } else {
            console.error(`File not found: ${indexPath}`);
            reject(new Error(`File not found: ${indexPath}`));
        }

        // Check if shared/utils.js exists
        const utilsPath = path.join(__dirname, '../shared/utils.mjs');
        if (fs.existsSync(utilsPath)) {
            archive.file(utilsPath, { name: 'shared/utils.mjs' });
        } else {
            console.error(`File not found: ${utilsPath}`);
            reject(new Error(`File not found: ${utilsPath}`));
        }

        // Include the node_modules directory
        const nodeModulesPath = path.join(__dirname, 'node_modules/');
        if (fs.existsSync(nodeModulesPath)) {
            archive.directory(nodeModulesPath, 'node_modules');
        } else {
            console.error(`Directory not found: ${nodeModulesPath}`);
            reject(new Error(`Directory not found: ${nodeModulesPath}`));
        }

        archive.finalize();
    });
};

const updateLambdaFunction = async () => {
    try {
        const zipData = fs.readFileSync(outputZipPath);
        const params = {
            FunctionName: functionName,
            ZipFile: zipData
        };
        await lambda.updateFunctionCode(params).promise();
        console.log('Lambda function updated successfully!');
    } catch (error) {
        console.error('Error updating Lambda function:', error);
        throw error;
    }
};

const deployLambda = async () => {
    try {
        console.log('Zipping the Lambda function...');
        await zipLambdaFunction();

        console.log('Deploying the Lambda function...');
        await updateLambdaFunction();

        console.log('Deployment successful!');
    } catch (err) {
        console.error('Error during Lambda deployment:', err);
    }
};

// Run the deployment
deployLambda();
