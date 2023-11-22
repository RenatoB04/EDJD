#include <stdio.h>
#include <stdlib.h>

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
const int STARTING_COINS = 100;
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

void displayMainMenu() {
    printf("=== Main Menu ===\n1. Start New Game\n2. Load Game\n3. Settings\n4. Exit\n");
}

void displayGameGrid() {
    printf("\n=== Middle-Earth Game Grid ===\n");

    printf("   ");
    for (char col = 'A'; col <= 'Z'; col++) {
        printf(" %c  ", col);
    }
    printf("\n");

    for (int row = 0; row <= 16; row++) {
        printf("%2d ", row);

        for (char col = 'A'; col <= 'Z'; col++) {
            printf("+---");
        }
        printf("+\n");

        printf("   ");
        for (char col = 'A'; col <= 'Z'; col++) {
            printf("|   ");
        }
        printf("|\n");
    }

    printf("   ");
    for (char col = 'A'; col <= 'Z'; col++) {
        printf("+---");
    }
    printf("+\n");

    printf("\nOptions:\n");
    printf("1. Place Buildings\n2. Place Unit\n3. Move Unit\n4. Attack with Unit\n5. Current Amount of Coins\n6. End Turn\n");

    int option;
    printf("Enter your choice: ");
    scanf_s("%d", &option);

    switch (option) {
    case 1:
        printf("Placing buildings...\n");
        break;
    case 2:
        printf("Placing a unit...\n");
        break;
    case 3:
        printf("Moving a unit...\n");
        break;
    case 4:
        printf("Attacking with a unit...\n");
        break;
    case 5:
        printf("Current Amount of Coins: \n");
        break;
    case 6:
        printf("Ending the turn...\n");
        break;
    default:
        printf("Invalid option. Please try again.\n");
        break;
    }
}

void startNewGame() {
    char playerChoice[20];
    printf("\n=== New Game Setup ===\nChoose your side:\n1. Gondor/Rivendell\n2. Mordor\nEnter your choice: ");
    scanf_s("%c", playerChoice);

    getchar();

    if (playerChoice[0] == '2' || (playerChoice[0] == 'M' && playerChoice[1] == 'o' && playerChoice[2] == 'r' && playerChoice[3] == 'd' && playerChoice[4] == 'o' && playerChoice[5] == 'r')) {
        printf("You have chosen to play as Mordor.\n");
    }
    else {
        printf("You have chosen to play as Gondor/Rivendell.\n");
    }

    displayGameGrid();

    printf("Starting the new game...\n");
}

void loadGame() {
    printf("\nLoading Game (Under Development)...\n");
}

void settings() {
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