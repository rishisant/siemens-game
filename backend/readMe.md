backend/
│
├── LambdaFunctionAPIs/
│   ├── userManagement/
│   │   ├── src/
│   │   │   ├── index.mjs  # Lambda function code
│   │   │   ├── handler.test.js  # Unit tests for this Lambda
│   │   ├── node_modules/  # Dependencies for this specific Lambda
│   │   ├── package.json  # Dependencies and scripts for this Lambda
│   │   ├── package-lock.json  # Lock file for this Lambda's dependencies
│   │   └── build.js  # Deployment logic for this Lambda
│   │
│   ├── storeAPI/
│   │   ├── src/
│   │   │   ├── index.mjs  # Lambda function code
│   │   │   ├── handler.test.js  # Unit tests for this Lambda
│   │   ├── node_modules/  # Dependencies for this specific Lambda
│   │   ├── package.json  # Dependencies and scripts for this Lambda
│   │   ├── package-lock.json  # Lock file for this Lambda's dependencies
│   │   └── build.js  # Deployment logic for this Lambda
│   │
│   ├── inventoryManagement/
│   │   ├── src/
│   │   │   ├── index.mjs  # Lambda function code
│   │   │   ├── handler.test.js  # Unit tests for this Lambda
│   │   ├── node_modules/  # Dependencies for this specific Lambda
│   │   ├── package.json  # Dependencies and scripts for this Lambda
│   │   ├── package-lock.json  # Lock file for this Lambda's dependencies
│   │   └── build.js  # Deployment logic for this Lambda
│   │
│   └── shared/
│       ├── utils.js  # Shared code or utilities used by multiple Lambdas
│
├── gitignore/
│   └── .gitignore
└── README.md


Workflow Example
Installing Dependencies:

Navigate to your Lambda function directory:
cd backend/LambdaFunctionAPIs/userManagement

Install dependencies using:
npm install

Running Tests:
npm test

Deploying the Lambda Function:
npm run deploy


make FUNCTION_NAME=GetUserItems