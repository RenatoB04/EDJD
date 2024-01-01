#include <stdio.h>
#include <stdlib.h>
#include <stdbool.h>
#include <string.h>

// Buildings
const char BASE_GONDOR[] = "GGGG";
const char BASE_MORDOR[] = "MMMM";
const char MINE_SHIRE[] = "SS";
const char MINE_EREBOR[] = "EE";
const char BARRACKS_ROHAN[] = "RR";
const char BARRACKS_ISENGARD[] = "II";
const char STABLES_LOTHLORIEN[] = "LL";
const char STABLES_MIRKWOOD[] = "MK";
const char ARMOURY_GONDORIAN_FORGE[] = "GF";
const char ARMOURY_DARK_FORGE[] = "DF";

// Units
const char INFANTRY_GONDORIAN_GUARDS = 'G';
const char CAVALRY_SWAN_KNIGHTS[] = "K";
const char ARTILLERY_TREBUCHETS = 'T';
const char INFANTRY_ORC_WARRIORS[] = "O";
const char CAVALRY_WARGS = 'W';
const char ARTILLERY_SIEGE_TOWERS[] = "S";

// Economy
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

#define STARTING_COINS 100
int player1Coins = STARTING_COINS;
int player2Coins = STARTING_COINS;
int turn = 1;

#define ROWS 17
#define COLS 26

int getCurrentPlayer() {
    return (turn % 2 == 1) ? 1 : 2;
}

int unitHealth[ROWS][COLS];
int buildingHealth[ROWS][COLS];

int player1BasePlaced = 0;
int player2BasePlaced = 0;

void saveGame(char grid[ROWS][COLS], int player1Coins, int player2Coins, int turn);
void loadGame(char grid[ROWS][COLS], int* player1Coins, int* player2Coins, int* turn);
char grid[ROWS][COLS];

void displayMainMenu() {
    printf("=== Main Menu ===\n1. Start New Game\n2. Load Game\n3. Settings\n4. Exit\n");
}

void displayGrid(char grid[ROWS][COLS]) {
    printf("  ");
    for (int j = 0; j < COLS; j++) {
        printf("  %c", 'A' + j);
    }
    printf("\n");

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

bool isCellEmpty(char grid[ROWS][COLS], int row, int col, int length) {
    if (row < 0 || row >= ROWS || col < 0 || col >= COLS || col + length > COLS) {
        return false;
    }

    for (int i = 0; i < length; i++) {
        if (grid[row][col + i] != ' ') {
            return false;
        }
    }

    return true;
}

void playerPlaceUnit(char grid[ROWS][COLS], int currentPlayer, int* player1Coins, int* player2Coins) {
    const char* unit = NULL;
    int row = 0, col = 0, unitCost = 0;

    printf("Player %d unit options:\n", currentPlayer);
    printf("1. Infantry\n2. Cavalry\n3. Artillery\n");
    printf("Choose a unit type (1-3): ");
    int choice;
    scanf_s("%d", &choice);

    switch (choice) {
    case 1:
        unit = (currentPlayer == 1) ? &INFANTRY_GONDORIAN_GUARDS : &INFANTRY_ORC_WARRIORS;
        unitCost = UNIT_COST_INFANTRY;
        break;
    case 2:
        unit = (currentPlayer == 1) ? &CAVALRY_SWAN_KNIGHTS : &CAVALRY_WARGS;
        unitCost = UNIT_COST_CAVALRY;
        break;
    case 3:
        unit = (currentPlayer == 1) ? &ARTILLERY_TREBUCHETS : &ARTILLERY_SIEGE_TOWERS;
        unitCost = UNIT_COST_ARTILLERY;
        break;
    default:
        printf("Invalid choice.\n");
        return;
    }

    printf("Enter the row and column (e.g., 3 A) where you want to place the unit: ");
    scanf_s("%d", &row);
    char colLetter;
    scanf_s(" %c", &colLetter);
    col = colLetter - 'A';

    size_t length = 1;

    if (isCellEmpty(grid, row, col, length)) {
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

        grid[row - 1][col] = *unit;
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

        unitHealth[row - 1][col] = initialHealth;

        printf("Unit placed successfully!\n");
    }
    else {
        printf("Invalid placement. The selected cell is not empty or out of bounds.\n");
    }
}

void playerMoveUnit(char grid[ROWS][COLS], int currentPlayer, int* player1Coins, int* player2Coins) {
    int startRow, startCol, endRow, endCol;

    printf("Enter the current row and column (e.g., 3 A) of the unit you want to move: ");
    scanf_s("%d", &startRow);
    char startColLetter;
    scanf_s(" %c", &startColLetter);
    startCol = startColLetter - 'A';

    printf("Enter the target row and column (e.g., 4 B) where you want to move the unit: ");
    scanf_s("%d", &endRow);
    char endColLetter;
    scanf_s(" %c", &endColLetter);
    endCol = endColLetter - 'A';

    size_t length = 1;

    if (isCellEmpty(grid, endRow - 1, endCol, length)) {
        int movementCost = 0;

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

        int totalMovementCost = movementCost * (abs(endRow - startRow) + abs(endCol - startCol));

        if ((currentPlayer == 1 && *player1Coins >= totalMovementCost) ||
            (currentPlayer == 2 && *player2Coins >= totalMovementCost)) {
            if (currentPlayer == 1) {
                *player1Coins -= totalMovementCost;
            }
            else {
                *player2Coins -= totalMovementCost;
            }

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

bool placeBuilding(char grid[ROWS][COLS], const char building[], int row, int col) {
    if (building == NULL) {
        printf("Invalid building type.\n");
        return false;
    }

    int length = strlen(building);
    int buildingHealth = 0;

    if (strncmp(building, BASE_GONDOR, 4) == 0 || strncmp(building, BASE_MORDOR, 4) == 0) {
        buildingHealth = HEALTH_BASE;
        if ((getCurrentPlayer() == 1 && player1BasePlaced) || (getCurrentPlayer() == 2 && player2BasePlaced)) {
            printf("You can only place one base per game.\n");
            return false;
        }
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

    if (isCellEmpty(grid, row, col, length)) {
        for (int i = 0; i < length; i++) {
            grid[row][col + i] = building[i];
            buildingHealth = buildingHealth;
        }
        return true;
    }
    else {
        printf("Invalid placement. The selected cell is not empty or out of bounds.\n");
        return false;
    }
}

void removeBuilding(char grid[ROWS][COLS], int row, int col) {
    if (row >= 0 && row < ROWS && col >= 0 && col < COLS && grid[row][col] != ' ') {
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

void playerPlaceBuilding(char grid[ROWS][COLS], int currentPlayer, int* player1Coins, int* player2Coins) {
    const char* building = NULL;
    int row = 0, col = 0, buildingCost = 0;

    if (currentPlayer == 1) {
        printf("Player 1 (Gondor/Rivendell) building options:\n");
        printf("1. Gondor Base\n2. Shire Mine\n3. Rohan Barracks\n4. Lothlorien Stables\n5. Gondorian Armoury\n");
        printf("Choose a building (1-5): ");
        int choice;
        scanf_s("%d", &choice);

        switch (choice) {
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

    printf("Enter the row and column (e.g., 3 A) where you want to place the building: ");
    scanf_s("%d", &row);
    char colLetter;
    scanf_s(" %c", &colLetter);
    col = colLetter - 'A';

    size_t length = strlen(building);
    if (isCellEmpty(grid, row, col, length)) {
        if (currentPlayer == 1 && *player1Coins >= buildingCost) {
            if (placeBuilding(grid, building, row - 1, col)) {
                *player1Coins -= buildingCost;
                printf("Building placed successfully!\n");
            }
            else {
                printf("Invalid placement. The selected cell is not empty or out of bounds.\n");
            }
        }
        else if (currentPlayer == 2 && *player2Coins >= buildingCost) {
            if (placeBuilding(grid, building, row - 1, col)) {
                *player2Coins -= buildingCost;
                printf("Building placed successfully!\n");
            }
            else {
                printf("Invalid placement. The selected cell is not empty or out of bounds.\n");
            }
        }
        else {
            printf("Not enough coins to build %s.\n", building);
            return;
        }
    }
    else {
        printf("Invalid placement. The selected cell is not empty or out of bounds.\n");
    }
}

void playerRemoveBuilding(char grid[ROWS][COLS]) {
    int row, col;
    printf("Enter the row and column (e.g., 3 A) where you want to remove the building: ");
    if (scanf_s("%d", &row) != 1) {
        printf("Invalid input for row.\n");
        return;
    }

    char colLetter;
    scanf_s(" %c", &colLetter);
    col = colLetter - 'A';

    removeBuilding(grid, row - 1, col);
}

bool isValidAttack(char attackingUnit, char attackedUnit, int currentPlayer) {
    if ((currentPlayer == 1 && strchr("GKT", attackingUnit) != NULL) ||
        (currentPlayer == 2 && strchr("OWSGF", attackingUnit) != NULL)) {
        return true;
    }
    return false;
}

void updateUnitHealth(int row, int col, int updatedHealth) {
    unitHealth[row][col] = updatedHealth;
}

void attackWithUnit(char grid[ROWS][COLS], int currentPlayer, int* player1Coins, int* player2Coins) {
    int startRow, startCol, endRow, endCol;

    printf("Enter the current row and column (e.g., 3 A) of the attacking unit: ");
    scanf_s("%d", &startRow);
    char startColLetter;
    scanf_s(" %c", &startColLetter);
    startCol = startColLetter - 'A';

    char attackingUnit = grid[startRow - 1][startCol];
    if ((currentPlayer == 1 && strchr("GKT", attackingUnit) == NULL) ||
        (currentPlayer == 2 && strchr("OWS", attackingUnit) == NULL)) {
        printf("Invalid selection. You can only attack with your own units.\n");
        return;
    }

    printf("Enter the target row and column (e.g., 4 B) of the unit to attack: ");
    scanf_s("%d", &endRow);
    char endColLetter;
    scanf_s(" %c", &endColLetter);
    endCol = endColLetter - 'A';

    char attackedUnit = grid[endRow - 1][endCol];

    if (isValidAttack(attackingUnit, attackedUnit, currentPlayer)) {
        int attackCost = ATTACK_COST;
        int attackPower = 0;

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

        if (abs(endRow - startRow) <= 1 && abs(endCol - startCol) <= 1) {
            if (attackedUnit != ' ') {
                int row = endRow - 1;
                int col = endCol;

                if (strchr("GKT", attackedUnit) != NULL) {
                    if (unitHealth[row][col] <= 0) {
                        printf("Unit defeated! The target unit has already been defeated.\n");
                    }
                    else {
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
                    int buildingTypeIndex = strchr(BASE_GONDOR, attackedUnit) ? 0 : 1;
                    int initialHealth = (buildingTypeIndex == 0) ? HEALTH_BASE : HEALTH_MINE;

                    if (buildingHealth[row][col] <= 0) {
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

void displayGameGrid(int* player1Coins, int* player2Coins, int* currentPlayer, char grid[ROWS][COLS]) {
    int option = 0;

    do {
        displayGrid(grid);
        if (*currentPlayer == 1) {
            printf("\nOptions (Player 1 - Gondor/Rivendell - %d coins):\n", *player1Coins);
        }
        else {
            printf("\nOptions (Player 2 - Mordor - %d coins):\n", *player2Coins);
        }
        printf("1. Place Building\n2. Remove Building\n3. Place Unit\n4. Move Unit\n5. Attack with Unit\n6. End Turn\n7. Save Game\n");

        printf("Enter your choice: ");
        scanf_s("%d", &option);

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
            printf("Ending the turn...\n");
            *currentPlayer = (*currentPlayer == 1) ? 2 : 1;
            break;
        case 7:
            saveGame(grid, &player1Coins, &player2Coins, &turn);
            break;
        default:
            printf("Invalid option. Please try again.\n");
            break;
        }
    } while (option != 7);
}

void startNewGame() {
    int gameMode;
    char grid[ROWS][COLS];
    for (int i = 0; i < ROWS; i++) {
        for (int j = 0; j < COLS; j++) {
            grid[i][j] = ' ';
        }
    }

    for (int i = 0; i < ROWS; i++) {
        for (int j = 0; j < COLS; j++) {
            unitHealth[i][j] = HEALTH_BASE;
        }
    }

    for (int i = 0; i < ROWS; i++) {
        for (int j = 0; j < COLS; j++) {
            if (grid[i][j] == 'G' || grid[i][j] == 'M') {
                buildingHealth[i][j] = HEALTH_BASE;
            }
            else if (grid[i][j] == 'S' || grid[i][j] == 'E') {
                buildingHealth[i][j] = HEALTH_MINE;
            }
            else if (strchr("RIL", grid[i][j]) != NULL) {
                buildingHealth[i][j] = HEALTH_BARRACKS_STABLES_ARMOURY;
            }
            else {
                buildingHealth[i][j] = 0;
            }
        }
    }

    printf("\n=== New Game Setup ===\nChoose the game mode:\n1. Single Player\n2. Two Players\nEnter your choice: ");

    if (scanf_s("%d", &gameMode) == 1) {
        if (gameMode == 2) {
            char playerChoice[20];
            printf("\nChoose your side:\n1. Gondor/Rivendell\n2. Mordor\nEnter your choice: ");
            if (scanf_s("%19s", playerChoice, (unsigned int)sizeof(playerChoice)) == 1) {
                if (playerChoice[0] == '2' || (playerChoice[0] == 'M' && playerChoice[1] == 'o' && playerChoice[2] == 'r' && playerChoice[3] == 'd' && playerChoice[4] == 'o' && playerChoice[5] == 'r')) {
                    printf("You have chosen to play as Mordor.\n");
                    turn = 2;
                }
                else {
                    printf("You have chosen to play as Gondor/Rivendell.\n");
                    turn = 1;
                }
                displayGameGrid(&player1Coins, &player2Coins, &turn, grid);
                printf("Starting the new game...\n");
            }
            else {
                printf("Error reading player choice.\n");
                exit(EXIT_FAILURE);
            }
        }
        else if (gameMode == 1) {
            printf("Single-player mode is currently under development.\n");
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

void saveGame(char grid[ROWS][COLS], int* player1Coins, int* player2Coins, int* currentTurn) {
    if (player1Coins == NULL || player2Coins == NULL || currentTurn == NULL) {
        printf("Error: Null pointer passed to saveGame.\n");
        return;
    }

    FILE* file = fopen("savegame.txt", "w");

    if (file == NULL) {
        printf("Error opening file for writing.\n");
        return;
    }

    for (int i = 0; i < ROWS; i++) {
        for (int j = 0; j < COLS; j++) {
            fprintf(file, "%c", (grid[i][j] == ' ') ? ' ' : grid[i][j]);
        }
        fprintf(file, "\n");
    }

    fprintf(file, "\n%d %d %d", *player1Coins, *player2Coins, *currentTurn);
    printf("Saved coins: Player 1: %d, Player 2: %d\n", *player1Coins, *player2Coins);

    fclose(file);
}

void loadGame(char grid[ROWS][COLS], int* player1Coins, int* player2Coins, int* turn) {
    FILE* file = fopen("savegame.txt", "r");

    if (file == NULL) {
        printf("No saved game found.\n");
        return;
    }

    char line[COLS + 2];
    for (int i = 0; i < ROWS; i++) {
        fgets(line, sizeof(line), file);
        for (int j = 0; j < COLS; j++) {
            grid[i][j] = line[j];
        }
    }

    fgetc(file);

    fscanf_s(file, "%d %d %d", player1Coins, player2Coins, turn);
    printf("Loaded coins: Player 1: %d, Player 2: %d\n", *player1Coins, *player2Coins);

    fclose(file);

    printf("Game loaded successfully!\n");
    displayGameGrid(player1Coins, player2Coins, turn, grid);
}

void settings() {  //UNDER DEVELOPMENT
    printf("\nSettings (Under Development)...\n");
}

int main() {
    int choice;

    do {
        displayMainMenu();
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
            settings();
            break;
        case 4:
            printf("Exiting the game. Goodbye!\n");
            break;
        default:
            printf("Invalid choice. Please try again.\n");
            break;
        }
    } while (choice != 4);

    return 0;
}