#include <stdio.h>
#include <stdlib.h>
#include <stdbool.h>
#include <string.h>

// Buildings
const char BASE_GONDOR[] = "BBBB";
const char BASE_MORDOR[] = "MMMM";
const char MINE_SHIRE[] = "HH";
const char MINE_EREBOR[] = "EE";
const char BARRACKS_ROHAN[] = "RR";
const char BARRACKS_ISENGARD[] = "II";
const char STABLES_LOTHLORIEN[] = "LL";
const char STABLES_MIRKWOOD[] = "OO";
const char ARMOURY_GONDORIAN_FORGE[] = "GG";
const char ARMOURY_DARK_FORGE[] = "DD";

// Units
const char INFANTRY_GONDORIAN_GUARDS[] = "G";
const char CAVALRY_SWAN_KNIGHTS[] = "K";
const char ARTILLERY_TREBUCHETS[] = "T";
const char INFANTRY_ORC_WARRIORS[] = "O";
const char CAVALRY_WARGS[] = "W";
const char ARTILLERY_SIEGE_TOWERS[] = "S";

// Economy
#define STARTING_COINS 100
const int MOVEMENT_COST_INFANTRY = 2;
const int MOVEMENT_COST_CAVALRY = 1;
const int MOVEMENT_COST_ARTILLERY = 3;
const int MINE_INCOME = 5;
const int BUILDING_COST_BASE = 30;
const int BUILDING_COST_MINE = 20;
const int BUILDING_COST_BARRACKS = 25;
const int BUILDING_COST_STABLES = 25;
const int BUILDING_COST_ARMOURY = 30;
const int UNIT_COST_INFANTRY = 10;
const int UNIT_COST_CAVALRY = 15;
const int UNIT_COST_ARTILLERY = 20;
const int ATTACK_COST = 5;

// Combat and health
const int ATTACK_POWER_INFANTRY = 5;
const int ATTACK_POWER_CAVALRY = 7;
const int ATTACK_POWER_ARTILLERY = 10;
const int HEALTH_BASE = 100;
const int HEALTH_MINE = 50;
const int HEALTH_BARRACKS_STABLES_ARMOURY = 70;
const int HEALTH_INFANTRY = 30;
const int HEALTH_CAVALRY = 40;
const int HEALTH_ARTILLERY = 20;

int player1Coins = STARTING_COINS;
int player2Coins = STARTING_COINS;
int turn = 1;

#define ROWS 17
#define COLS 26

int unitHealth[ROWS][COLS];
int buildingHealth[ROWS][COLS];

int player1BasePlaced = 0;
int player2BasePlaced = 0;

int player1Mines = 0;
int player2Mines = 0;

void loadGame(char grid[ROWS][COLS], int* player1Coins, int* player2Coins, int* turn);
char grid[ROWS][COLS];

// Function to get the current player based on the turn number
int getCurrentPlayer() {
    return (turn % 2 == 1) ? 1 : 2;
}

// Function to check if the base of the current player is destroyed
int isBaseDestroyed(char grid[ROWS][COLS], int currentPlayer) {
    // Determine the base string for the current player
    const char* base = (currentPlayer == 1) ? BASE_GONDOR : BASE_MORDOR;

    // Iterate through the grid to check for the presence of the base string
    for (int i = 0; i < ROWS; i++) {
        for (int j = 0; j < COLS - 3; j++) {
            // Compare a substring of length 4 with the base string
            if (strncmp(grid[i] + j, base, 4) == 0) {
                return 0; // Base not destroyed
            }
        }
    }

    return 1; // Base destroyed
}

// Function to initialize the game grid with empty spaces
void initializeGrid(char grid[ROWS][COLS]) {
    for (int i = 0; i < ROWS; i++) {
        for (int j = 0; j < COLS; j++) {
            grid[i][j] = ' ';
        }
    }
}

// Function to display the game grid
void displayGrid(char grid[ROWS][COLS]) {
    // Display column headers
    printf("  ");
    for (int j = 0; j < COLS; j++) {
        printf("  %c", 'A' + j);
    }
    printf("\n");

    // Display each row of the grid with square brackets around each cell
    for (int i = 0; i < ROWS; i++) {
        printf("%2d ", i + 1);
        for (int j = 0; j < COLS; j++) {
            printf("[");
            printf("%c", grid[i][j]);
            printf("]");
        }
        printf("\n");
    }
}

// Function to save the current game state to a file
void saveGame(char grid[ROWS][COLS], int* player1Coins, int* player2Coins, int* currentTurn) {
    // Check for null pointers
    if (player1Coins == NULL || player2Coins == NULL || currentTurn == NULL) {
        printf("Error: Null pointer passed to saveGame.\n");
        return;
    }

    // Open the file for writing
    FILE* file = fopen("savegame.txt", "w");

    // Check for file opening errors
    if (file == NULL) {
        printf("Error opening file for writing.\n");
        return;
    }

    // Save the grid state to the file
    for (int i = 0; i < ROWS; i++) {
        for (int j = 0; j < COLS; j++) {
            fprintf(file, "%c", (grid[i][j] == ' ') ? ' ' : grid[i][j]);
        }
        fprintf(file, "\n");
    }

    // Save player coins and current turn to the file
    fprintf(file, "\n%d %d %d", *player1Coins, *player2Coins, *currentTurn);
    printf("Saved coins: Player 1: %d, Player 2: %d\n", *player1Coins, *player2Coins);

    // Close the file
    fclose(file);
}

// Function to check if a cell is empty and within bounds
bool isCellEmpty(char grid[ROWS][COLS], int row, int col, int length) {
    // Check if the cell is out of bounds or the unit won't fit within the row
    if (row < 0 || row >= ROWS || col < 0 || col >= COLS || col + length > COLS) {
        return false;
    }

    // Check if the specified cells are empty
    for (int i = 0; i < length; i++) {
        if (grid[row][col + i] != ' ') {
            return false;
        }
    }

    return true;
}

// Function for a player to place a unit on the game grid
void playerPlaceUnit(char grid[ROWS][COLS], int currentPlayer, int* player1Coins, int* player2Coins) {
    const char* unit = NULL;
    int row = 0, col = 0, unitCost = 0;

    // Display unit options for the current player
    printf("Player %d unit options:\n", currentPlayer);
    printf("1. Infantry\n2. Cavalry\n3. Artillery\n");
    printf("Choose a unit type (1-3): ");
    int choice;
    scanf_s("%d", &choice);

    // Determine unit type, cost, and required structure for placement based on player choice
    switch (choice) {
    case 1:
        // Infantry
        unit = (currentPlayer == 1) ? INFANTRY_GONDORIAN_GUARDS : INFANTRY_ORC_WARRIORS;
        unitCost = UNIT_COST_INFANTRY;
        const char* requiredBarracks = (currentPlayer == 1) ? BARRACKS_ROHAN : BARRACKS_ISENGARD;
        if (!strstr(grid[0], requiredBarracks)) {
            printf("Cannot place infantry units without placing Barracks.\n");
            return;
        }
        break;
    case 2:
        // Cavalry
        unit = (currentPlayer == 1) ? CAVALRY_SWAN_KNIGHTS : CAVALRY_WARGS;
        unitCost = UNIT_COST_CAVALRY;
        const char* requiredCavalry = (currentPlayer == 1) ? STABLES_LOTHLORIEN : STABLES_MIRKWOOD;
        if (!strstr(grid[0], requiredCavalry)) {
            printf("Cannot place cavalry units without placing Stables.\n");
            return;
        }
        break;
    case 3:
        // Artillery
        unit = (currentPlayer == 1) ? ARTILLERY_TREBUCHETS : ARTILLERY_SIEGE_TOWERS;
        unitCost = UNIT_COST_ARTILLERY;
        const char* requiredArmory = (currentPlayer == 1) ? ARMOURY_GONDORIAN_FORGE : ARMOURY_DARK_FORGE;
        if (!strstr(grid[0], requiredArmory)) {
            printf("Cannot place artillery without a required armory.\n");
            return;
        }
        break;
    default:
        printf("Invalid choice.\n");
        return;
    }

    // Get the row and column where the player wants to place the unit
    printf("Enter the row and column (e.g., 3 A) where you want to place the unit: ");
    scanf_s("%d", &row);
    char colLetter;
    scanf_s(" %c", &colLetter, 1);
    col = colLetter - 'A';

    size_t length = 1;

    // Check if the chosen cell is empty and within bounds
    if (isCellEmpty(grid, row, col, length)) {
        // Deduct unit cost from player's coins if they can afford it
        if (currentPlayer == 1 && *player1Coins >= unitCost) {
            *player1Coins -= unitCost;
        }
        else if (currentPlayer == 2 && *player2Coins >= unitCost) {
            *player2Coins -= unitCost;
        }
        else {
            printf("Not enough coins to build %s.\n", unit);
            return;
        }

        // Place the unit on the grid
        grid[row - 1][col] = *unit;

        // Set the initial health for the unit based on its type
        int initialHealth = 0;
        switch (*unit) {
        case 'G':
            initialHealth = HEALTH_INFANTRY;
            break;
        case 'K':
            initialHealth = HEALTH_CAVALRY;
            break;
        case 'T':
            initialHealth = HEALTH_ARTILLERY;
            break;
        case 'O':
            initialHealth = HEALTH_INFANTRY;
            break;
        case 'W':
            initialHealth = HEALTH_CAVALRY;
            break;
        case 'S':
            initialHealth = HEALTH_ARTILLERY;
            break;
        default:
            printf("Invalid unit type.\n");
            return;
        }

        // Update the unit health if the cell is valid
        if (row > 0 && row <= ROWS && col >= 0 && col < COLS && row - 1 < ROWS && col < COLS) {
            unitHealth[row - 1][col] = initialHealth;
        }
        else {
            printf("Invalid row or column values for updating unit health.\n");
        }

        // Display success message
        printf("Unit placed successfully!\n");
    }
    else {
        // Display an error message if the cell is not empty or out of bounds
        printf("Invalid placement. The selected cell is not empty or out of bounds.\n");
    }
}

// Function for a player to move a unit on the game grid
void playerMoveUnit(char grid[ROWS][COLS], int currentPlayer, int* player1Coins, int* player2Coins) {
    int startRow, startCol, endRow, endCol;

    // Get the current position and target position for the unit to be moved
    printf("Enter the current row and column (e.g., 3 A) of the unit you want to move: ");
    scanf_s("%d", &startRow);
    char startColLetter;
    scanf_s(" %c", &startColLetter, 1);

    startCol = startColLetter - 'A';

    printf("Enter the target row and column (e.g., 4 B) where you want to move the unit: ");
    scanf_s("%d", &endRow);
    char endColLetter;
    scanf_s(" %c", &endColLetter, 1);

    endCol = endColLetter - 'A';

    size_t length = 1;

    // Check if the target cell is empty and within bounds
    if (isCellEmpty(grid, endRow - 1, endCol, length)) {
        int movementCost = 0;

        // Determine movement cost based on the unit type
        char unitType = grid[startRow - 1][startCol];
        switch (unitType) {
        case 'G':
            movementCost = MOVEMENT_COST_INFANTRY;
            break;
        case 'K':
            movementCost = MOVEMENT_COST_CAVALRY;
            break;
        case 'T':
            movementCost = MOVEMENT_COST_ARTILLERY;
            break;
        case 'O':
            movementCost = MOVEMENT_COST_INFANTRY;
            break;
        case 'W':
            movementCost = MOVEMENT_COST_CAVALRY;
            break;
        case 'S':
            movementCost = MOVEMENT_COST_ARTILLERY;
            break;
        default:
            printf("Invalid unit type.\n");
            return;
        }

        // Calculate total movement cost based on distance
        int totalMovementCost = movementCost * (abs(endRow - startRow) + abs(endCol - startCol));

        // Check if the player has enough coins to cover the movement cost
        if ((currentPlayer == 1 && *player1Coins >= totalMovementCost) ||
            (currentPlayer == 2 && *player2Coins >= totalMovementCost)) {
            // Deduct movement cost from player's coins
            if (currentPlayer == 1) {
                *player1Coins -= totalMovementCost;
            }
            else {
                *player2Coins -= totalMovementCost;
            }

            // Move the unit to the target cell and update the grid
            grid[endRow - 1][endCol] = unitType;
            grid[startRow - 1][startCol] = ' ';
            printf("Unit moved successfully!\n");
        }
        else {
            printf("Not enough coins to move the unit.\n");
            return;
        }
    }
    else {
        printf("Invalid movement. The target cell is not empty or out of bounds.\n");
    }
}

// Function to place a building on the game grid
bool placeBuilding(char grid[ROWS][COLS], const char building[], int row, int col) {
    // Check for invalid building type
    if (building == NULL) {
        printf("Invalid building type.\n");
        return false;
    }

    int length = strlen(building);
    int buildingHealth = 0;

    // Check if the building is a mine and update mine count accordingly
    if (strncmp(building, MINE_SHIRE, 2) == 0 || strncmp(building, MINE_EREBOR, 2) == 0) {
        if (getCurrentPlayer() == 1) {
            player1Mines++;
        }
        else if (getCurrentPlayer() == 2) {
            player2Mines++;
        }
    }

    // Determine building health based on building type
    if (strncmp(building, BASE_GONDOR, 4) == 0 || strncmp(building, BASE_MORDOR, 4) == 0) {
        buildingHealth = HEALTH_BASE;

        // Check if the player has already placed a base
        if ((getCurrentPlayer() == 1 && player1BasePlaced) || (getCurrentPlayer() == 2 && player2BasePlaced)) {
            printf("You can only place one base per game.\n");
            return false;
        }

        // Update base placement status for the current player
        if (getCurrentPlayer() == 1) {
            player1BasePlaced = 1;
        }
        else {
            player2BasePlaced = 1;
        }
    }
    else if (strncmp(building, MINE_SHIRE, 2) == 0 || strncmp(building, MINE_EREBOR, 2) == 0) {
        buildingHealth = HEALTH_MINE;
    }
    else {
        buildingHealth = HEALTH_BARRACKS_STABLES_ARMOURY;
    }

    // Check if the target cell is empty and within bounds
    if (isCellEmpty(grid, row, col, length)) {
        // Place the building on the grid
        for (int i = 0; i < length; i++) {
            grid[row][col + i] = building[i];
        }
        return true;
    }
    else {
        printf("Invalid placement. The selected cell is not empty or out of bounds.\n");
        return false;
    }
}

// Function to remove a building from the game grid at the specified position
void removeBuilding(char grid[ROWS][COLS], int row, int col) {
    // Check if the provided position is within bounds and contains a building
    if (row >= 0 && row < ROWS && col >= 0 && col < COLS && grid[row][col] != ' ') {
        // Iterate through the building cells and remove the building
        int i = 0;
        while (col + i < COLS && grid[row][col + i] != ' ') {
            grid[row][col + i] = ' ';
            i++;
        }
        printf("Building removed successfully!\n");
    }
    else {
        printf("Invalid selection. No building found at the selected cell.\n");
    }
}

// Function for a player to place a building on the game grid
void playerPlaceBuilding(char grid[ROWS][COLS], int currentPlayer, int* player1Coins, int* player2Coins) {
    const char* building = NULL;
    int row = 0, col = 0, buildingCost = 0;

    // Determine building options and costs based on the current player
    if (currentPlayer == 1) {
        printf("Player 1 (Gondor/Rivendell) building options:\n");
        printf("1. Gondor Base\n2. Shire Mine\n3. Rohan Barracks\n4. Lothlorien Stables\n5. Gondorian Armoury\n");
        printf("Choose a building (1-5): ");
        int choice;
        scanf_s("%d", &choice);

        switch (choice) {
            // Assign the selected building type and its cost
        case 1:
            building = BASE_GONDOR;
            buildingCost = BUILDING_COST_BASE;
            break;
        case 2:
            building = MINE_SHIRE;
            buildingCost = BUILDING_COST_MINE;
            break;
        case 3:
            building = BARRACKS_ROHAN;
            buildingCost = BUILDING_COST_BARRACKS;
            break;
        case 4:
            building = STABLES_LOTHLORIEN;
            buildingCost = BUILDING_COST_STABLES;
            break;
        case 5:
            building = ARMOURY_GONDORIAN_FORGE;
            buildingCost = BUILDING_COST_ARMOURY;
            break;
        default:
            printf("Invalid choice.\n");
            return;
        }
    }
    else if (currentPlayer == 2) {
        printf("Player 2 (Mordor) building options:\n");
        printf("1. Mordor Base\n2. Erebor Mine\n3. Isengard Barracks\n4. Mirkwood Stables\n5. Dark Armoury\n");
        printf("Choose a building (1-5): ");
        int choice;
        scanf_s("%d", &choice);

        switch (choice) {
            // Assign the selected building type and its cost
        case 1:
            building = BASE_MORDOR;
            buildingCost = BUILDING_COST_BASE;
            break;
        case 2:
            building = MINE_EREBOR;
            buildingCost = BUILDING_COST_MINE;
            break;
        case 3:
            building = BARRACKS_ISENGARD;
            buildingCost = BUILDING_COST_BARRACKS;
            break;
        case 4:
            building = STABLES_MIRKWOOD;
            buildingCost = BUILDING_COST_STABLES;
            break;
        case 5:
            building = ARMOURY_DARK_FORGE;
            buildingCost = BUILDING_COST_ARMOURY;
            break;
        default:
            printf("Invalid choice.\n");
            return;
        }
    }
    else {
        printf("Invalid player.\n");
        return;
    }

    // Get the row and column where the player wants to place the building
    printf("Enter the row and column (e.g., 3 A) where you want to place the building: ");
    scanf_s("%d", &row);
    char colLetter;
    scanf_s(" %c", &colLetter, 1);
    col = colLetter - 'A';

    // Check if the target cell is empty and within bounds
    size_t length = strlen(building);
    if (isCellEmpty(grid, row, col, length)) {
        // Check if the player has enough coins to build the selected building
        if (currentPlayer == 1 && *player1Coins >= buildingCost) {
            // Place the building on the grid and deduct the cost from the player's coins
            if (placeBuilding(grid, building, row - 1, col)) {
                *player1Coins -= buildingCost;
                printf("Building placed successfully!\n");
            }
            else {
                printf("Invalid placement. The selected cell is not empty or out of bounds.\n");
            }
        }
        else if (currentPlayer == 2 && *player2Coins >= buildingCost) {
            // Place the building on the grid and deduct the cost from the player's coins
            if (placeBuilding(grid, building, row - 1, col)) {
                *player2Coins -= buildingCost;
                printf("Building placed successfully!\n");
            }
            else {
                printf("Invalid placement. The selected cell is not empty or out of bounds.\n");
            }
        }
        else {
            // Display the "Not enough coins to build" message only if the player doesn't have enough coins
            printf("Not enough coins to build %s.\n", building);
            return;
        }
    }
}

// Function for a player to remove a building from the game grid
void playerRemoveBuilding(char grid[ROWS][COLS]) {
    int row, col;
    printf("Enter the row and column (e.g., 3 A) where you want to remove the building: ");

    // Get the row and column from the user input
    if (scanf_s("%d", &row) != 1) {
        printf("Invalid input for row.\n");
        return;
    }

    char colLetter;
    scanf_s(" %c", &colLetter, 1);
    col = colLetter - 'A';

    // Call the removeBuilding function to remove the building at the specified position
    removeBuilding(grid, row - 1, col);
}

// Function to check if the attacking unit is valid based on the current player
// Valid attacking units for Player 1: G, K, T; for Player 2: O, W, S, G, F
bool isValidAttack(char attackingUnit, char attackedUnit, int currentPlayer) {
    if ((currentPlayer == 1 && strchr("GKT", attackingUnit) != NULL) ||
        (currentPlayer == 2 && strchr("OWSGF", attackingUnit) != NULL)) {
        return true;
    }
    return false;
}

// Function to update the health of a unit at a specified position on the grid
void updateUnitHealth(int row, int col, int updatedHealth) {
    unitHealth[row][col] = updatedHealth;
}

// Function to perform an attack with a unit on the game grid
void attackWithUnit(char grid[ROWS][COLS], int currentPlayer, int* player1Coins, int* player2Coins) {
    int startRow, startCol, endRow, endCol;

    // Get the current row and column of the attacking unit from the player
    printf("Enter the current row and column (e.g., 3 A) of the attacking unit: ");
    scanf_s("%d", &startRow);
    char startColLetter;
    scanf_s(" %c", &startColLetter, 1);
    startCol = startColLetter - 'A';

    char attackingUnit = grid[startRow - 1][startCol];

    // Check if the selected unit belongs to the current player
    if ((currentPlayer == 1 && strchr("GKT", attackingUnit) == NULL) ||
        (currentPlayer == 2 && strchr("OWS", attackingUnit) == NULL)) {
        printf("Invalid selection. You can only attack with your own units.\n");
        return;
    }

    // Get the target row and column of the unit to attack
    printf("Enter the target row and column (e.g., 4 B) of the unit to attack: ");
    scanf_s("%d", &endRow);
    char endColLetter;
    scanf_s(" %c", &endColLetter, 1);
    endCol = endColLetter - 'A';

    char attackedUnit = grid[endRow - 1][endCol];

    // Check if the attack is valid based on the attacking unit and current player
    if (isValidAttack(attackingUnit, attackedUnit, currentPlayer)) {
        int attackCost = ATTACK_COST;
        int attackPower = 0;

        // Deduct the attack cost from the player's coins
        if (currentPlayer == 1) {
            if (*player1Coins >= attackCost) {
                *player1Coins -= attackCost;
            }
            else {
                printf("Player 1: Not enough coins to perform the attack.\n");
                return;
            }
        }
        else {
            if (*player2Coins >= attackCost) {
                *player2Coins -= attackCost;
            }
            else {
                printf("Player 2: Not enough coins to perform the attack.\n");
                return;
            }
        }

        // Determine the attack power based on the attacking unit type
        switch (attackingUnit) {
        case 'G':
            attackPower = ATTACK_POWER_INFANTRY;
            break;
        case 'K':
            attackPower = ATTACK_POWER_CAVALRY;
            break;
        case 'T':
            attackPower = ATTACK_POWER_ARTILLERY;
            break;
        case 'O':
            attackPower = ATTACK_POWER_INFANTRY;
            break;
        case 'W':
            attackPower = ATTACK_POWER_CAVALRY;
            break;
        case 'S':
            attackPower = ATTACK_POWER_ARTILLERY;
            break;
        default:
            printf("Invalid unit type to attack.\n");
            return;
        }

        // Check if the target unit is within attack range
        if (abs(endRow - startRow) <= 1 && abs(endCol - startCol) <= 1) {
            if (attackedUnit != ' ') {
                int row = endRow - 1;
                int col = endCol;

                // Check if the attacked unit is an infantry, cavalry, or artillery unit
                if (strchr("GKT", attackedUnit) != NULL) {
                    // Check if the target unit has already been defeated
                    if (unitHealth[row][col] <= 0) {
                        printf("Unit defeated! The target unit has already been defeated.\n");
                    }
                    else {
                        // Update the health of the target unit after the attack
                        int updatedHealth = unitHealth[row][col] - attackPower;
                        if (updatedHealth <= 0) {
                            grid[row][col] = ' ';
                            printf("Unit defeated! The target unit has been removed from the grid.\n");
                        }
                        else {
                            printf("Unit attacked successfully! %d health points deducted from the target. Remaining Health: %d\n", attackPower, updatedHealth);
                            updateUnitHealth(row, col, updatedHealth);
                        }
                    }
                }
                else {
                    // Check if the attacked unit is a building
                    int buildingTypeIndex = strchr(BASE_GONDOR, attackedUnit) ? 0 : 1;
                    int initialHealth = (buildingTypeIndex == 0) ? HEALTH_BASE : HEALTH_MINE;

                    // Check if the target building has already been defeated
                    if (buildingHealth[row][col] <= 0) {
                        // Remove the target building from the grid
                        if (grid[row][col] != ' ') {
                            for (int i = 0; i < COLS; i++) {
                                if (grid[row][i] == attackedUnit) {
                                    grid[row][i] = ' ';
                                }
                            }
                            printf("Building defeated! The target building has been removed from the grid.\n");
                        }
                        else {
                            printf("Building defeated! The target building has already been defeated.\n");
                        }
                    }
                    else {
                        // Update the health of the target building after the attack
                        int updatedHealth = buildingHealth[row][col] - attackPower;
                        if (updatedHealth <= 0) {
                            updatedHealth = 0;
                        }
                        printf("Building attacked successfully! %d health points deducted from the target. Remaining Health: %d\n", attackPower, updatedHealth);
                        buildingHealth[row][col] = updatedHealth;
                    }
                }
            }
        }
        else {
            printf("Invalid attack. The target unit is not within attack range.\n");
        }
    }
}

// Function to update the mine income for the current player based on the number of mines they own
void updateMineIncome(int currentPlayer) {
    if (currentPlayer == 1) {
        // Update Player 1's coins based on the number of mines and MINE_INCOME
        player1Coins += player1Mines * MINE_INCOME;
    }
    else {
        // Update Player 2's coins based on the number of mines and MINE_INCOME
        player2Coins += player2Mines * MINE_INCOME;
    }
}

// Function to display the game grid and provide options for the current player
void displayGameGrid(int* player1Coins, int* player2Coins, int* currentPlayer, char grid[ROWS][COLS]) {
    int option = 0;

    // Update mine income at the beginning of the turn for the current player
    if (*currentPlayer == 1) {
        *player1Coins += player1Mines * MINE_INCOME;
    }
    else {
        *player2Coins += player2Mines * MINE_INCOME;
    }

    do {
        // Display the game grid
        displayGrid(grid);

        // Display options for the current player along with their current coins
        if (*currentPlayer == 1) {
            printf("\nOptions (Player 1 - Gondor/Rivendell - %d coins):\n", *player1Coins);
        }
        else {
            printf("\nOptions (Player 2 - Mordor - %d coins):\n", *player2Coins);
        }
        printf("1. Place Building\n2. Remove Building\n3. Place Unit\n4. Move Unit\n5. Attack with Unit\n6. End Turn\n7. Save Game\n");

        // Get the player's choice
        printf("Enter your choice: ");
        scanf_s("%d", &option);

        // Perform actions based on the player's choice
        switch (option) {
        case 1:
            playerPlaceBuilding(grid, getCurrentPlayer(), player1Coins, player2Coins);
            break;
        case 2:
            playerRemoveBuilding(grid);
            break;
        case 3:
            playerPlaceUnit(grid, getCurrentPlayer(), player1Coins, player2Coins);
            break;
        case 4:
            playerMoveUnit(grid, getCurrentPlayer(), player1Coins, player2Coins);
            break;
        case 5:
            attackWithUnit(grid, getCurrentPlayer(), player1Coins, player2Coins);
            break;
        case 6:
            // End the turn and switch to the other player
            printf("Ending the turn...\n");
            *currentPlayer = (*currentPlayer == 1) ? 2 : 1;

            // Check if the opponent's base is placed, update mine income, and check for a winner
            if ((*currentPlayer == 1 && player2BasePlaced == 1) || (*currentPlayer == 2 && player1BasePlaced == 1)) {
                updateMineIncome(*currentPlayer);
                // Check if the opponent's base is destroyed, declare the winner if true
                if ((*currentPlayer == 1 && isBaseDestroyed(grid, 2)) || (*currentPlayer == 2 && isBaseDestroyed(grid, 1))) {
                    printf("Player %d wins!\n", *currentPlayer);
                    return;
                }
            }
            else {
                printf("Opponent's base not placed yet. Cannot declare a winner.\n");
            }
            break;
        case 7:
            // Save the game state to a file
            saveGame(grid, player1Coins, player2Coins, currentPlayer);
            break;
        default:
            printf("Invalid option. Please try again.\n");
            break;
        }
    } while (option != 7);  // Repeat the loop until the player chooses to save the game (option 7)
}

// Function to start a new game
void startNewGame() {
    int gameMode;
    char grid[ROWS][COLS];

    // Initialize the game grid with empty spaces
    initializeGrid(grid);

    printf("\n=== New Game Setup ===\nChoose the game mode:\n1. Two Players\nEnter your choice: ");
    if (scanf_s("%d", &gameMode) == 1) {
        if (gameMode == 1) {
            char playerChoice[20];

            printf("\nChoose your side:\n1. Gondor/Rivendell\n2. Mordor\nEnter your choice: ");

            if (scanf_s("%19s", playerChoice, (unsigned int)sizeof(playerChoice)) == 1) {
                // Set the turn based on the chosen side
                if (playerChoice[0] == '2') {
                    printf("You have chosen to play as Mordor.\n");
                    turn = 2;
                }
                else {
                    printf("You have chosen to play as Gondor/Rivendell.\n");
                    turn = 1;
                }

                // Display the initial game grid
                displayGameGrid(&player1Coins, &player2Coins, &turn, grid);
                printf("Starting the new game...\n");
            }
            else {
                printf("Error reading player choice.\n");
                exit(EXIT_FAILURE);
            }
        }
        else {
            printf("Invalid game mode.\n");
            exit(EXIT_FAILURE);
        }
    }
    else {
        printf("Error reading game mode.\n");
        exit(EXIT_FAILURE);
    }
}

// Function to load a saved game from a file
void loadGame(char grid[ROWS][COLS], int* player1Coins, int* player2Coins, int* turn) {
    FILE* file = fopen("savegame.txt", "r");

    // Check if the file was successfully opened
    if (file == NULL) {
        printf("No saved game found.\n");
        return;
    }

    // Read the game grid from the file
    for (int i = 0; i < ROWS; i++) {
        fgets(grid[i], COLS + 1, file);
    }

    // Read player coins and current turn
    fscanf_s(file, "%d %d %d", player1Coins, player2Coins, turn);
    printf("Loaded coins: Player 1: %d, Player 2: %d\n", *player1Coins, *player2Coins);

    fclose(file);

    printf("Game loaded successfully!\n");

    displayGameGrid(player1Coins, player2Coins, turn, grid);
}

// Main function of the game
int main() {
    int choice;

    // Main loop to display the menu and handle user choices
    do {
        printf("=== Main Menu ===\n1. Start New Game\n2. Load Game\n3. Exit\n");
        printf("Enter your choice: ");
        scanf_s("%d", &choice);

        switch (choice) {
        case 1:
            startNewGame();
            break;
        case 2:
            loadGame(grid, &player1Coins, &player2Coins, &turn);
            break;
        case 3:
            printf("Exiting the game. Goodbye!\n");
            break;
        default:
            printf("Invalid choice. Please try again.\n");
            break;
        }

    } while (choice != 3);  // Continue the loop until the user chooses to exit

    return 0;  // Return 0 to indicate successful execution
}