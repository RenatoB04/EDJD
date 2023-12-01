#include <stdio.h>
#include <stdlib.h>
#include <stdbool.h>
#include <string.h>

#define STARTING_COINS 100

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
const char CAVALRY_SWAN_KNIGHTS[] = "SK";
const char ARTILLERY_TREBUCHETS = 'T';
const char INFANTRY_ORC_WARRIORS[] = "OW";
const char CAVALRY_WARGS = 'W';
const char ARTILLERY_SIEGE_TOWERS[] = "ST";

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

int getCurrentPlayer() {
    return (turn % 2 == 1) ? 1 : 2;
}

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
    // Check if the cells are within the grid boundaries
    if (row < 0 || row >= ROWS || col < 0 || col >= COLS || col + length > COLS) {
        return false;
    }

    // Check if the cells are empty
    for (int i = 0; i < length; i++) {
        if (grid[row][col + i] != ' ') {
            return false;
        }
    }

    return true;
}

bool placeBuilding(char grid[ROWS][COLS], const char building[], int row, int col) {
    if (building == NULL) {
        printf("Invalid building type.\n");
        return false;
    }

    int length = strlen(building);

    if (isCellEmpty(grid, row, col, length)) {
        for (int i = 0; i < length; i++) {
            grid[row][col + i] = building[i];
        }
        return true;  // Building placed successfully
    }
    else {
        printf("Invalid placement. The selected cell is not empty or out of bounds.\n");
        return false;  // Building placement failed
    }
}

void removeBuilding(char grid[ROWS][COLS], int row, int col) {
    if (row >= 0 && row < ROWS && col >= 0 && col < COLS && grid[row][col] != ' ') {
        // Remove the building
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
        // Player 1 (Gondor/Rivendell) building options
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
        // Player 2 (Mordor) building options
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
    col = colLetter - 'A'; // Convert letter to column index

    size_t length = strlen(building);
    if (isCellEmpty(grid, row, col, length)) {
        if (currentPlayer == 1 && *player1Coins >= buildingCost) {
            // Deduct the building cost from player 1's coins
            *player1Coins -= buildingCost;
        }
        else if (currentPlayer == 2 && *player2Coins >= buildingCost) {
            // Deduct the building cost from player 2's coins
            *player2Coins -= buildingCost;
        }
        else {
            printf("Not enough coins to build %s.\n", building);
            return;
        }

        // Place the building after checking for coins
        if (placeBuilding(grid, building, row - 1, col)) {
            printf("Building placed successfully!\n");
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
    col = colLetter - 'A'; // Convert letter to column index

    removeBuilding(grid, row - 1, col);
}

void displayGameGrid(int* player1Coins, int* player2Coins, int* currentPlayer, char grid[ROWS][COLS]) {
    int option = 0;

    do {
        displayGrid(grid);
        printf("\nOptions (Player %d - %d coins):\n", *currentPlayer, (*currentPlayer == 1) ? *player1Coins : *player2Coins);
        printf("1. Place Building\n2. Remove Building\n3. Place Unit\n4. Move Unit\n5. Attack with Unit\n6. End Turn\n");

        int option;
        printf("Enter your choice: ");
        scanf_s("%d", &option);

        switch (option) {
        case 1:
            playerPlaceBuilding(grid, *currentPlayer, player1Coins, player2Coins);
            break;
        case 2:
            playerRemoveBuilding(grid);
            break;
        case 3:  //UNDER DEVELOPMENT
            printf("Placing a unit...\n");
            break;
        case 4:  //UNDER DEVELOPMENT
            printf("Moving a unit...\n");
            break;
        case 5:  //UNDER DEVELOPMENT
            printf("Attacking with a unit...\n");
            break;
        case 6:
            printf("Ending the turn...\n");
            *currentPlayer = (*currentPlayer == 1) ? 2 : 1;
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
    // Initialize the grid with some default value
    for (int i = 0; i < ROWS; i++) {
        for (int j = 0; j < COLS; j++) {
            grid[i][j] = ' ';
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
                }
                else {
                    printf("You have chosen to play as Gondor/Rivendell.\n");
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

void loadGame() {  //UNDER DEVELOPMENT
    printf("\nLoading Game (Under Development)...\n");
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
            loadGame();
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