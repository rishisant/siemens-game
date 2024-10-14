# Makefile for creating a new Lambda function directory

# Variables
FUNCTION_NAME = NewLambdaFunction # Replace with the name of your new Lambda function

# Directories
BASE_DIR = .
SRC_DIR = $(BASE_DIR)/$(FUNCTION_NAME)/src
TEMPLATE_DIR = Test

# Targets
all: create-structure

create-structure: create-directories copy-files

# Create the directories for the new Lambda function
create-directories:
	@echo "Creating directories for $(FUNCTION_NAME)..."
	mkdir -p $(SRC_DIR)

# Copy the necessary files from the template
copy-files:
	@echo "Copying files from template..."
	cp $(TEMPLATE_DIR)/build.js $(BASE_DIR)/$(FUNCTION_NAME)/
	cp $(TEMPLATE_DIR)/src/index.mjs $(SRC_DIR)/
	cp $(TEMPLATE_DIR)/src/handler.test.js $(SRC_DIR)/
	@echo "Setup complete for $(FUNCTION_NAME)."

clean:
	@echo "Cleaning up $(FUNCTION_NAME)..."
	rm -rf $(BASE_DIR)/$(FUNCTION_NAME)
	@echo "$(FUNCTION_NAME) has been removed."

# To create a new Lambda function structure, run: `make FUNCTION_NAME=YourFunctionName`
