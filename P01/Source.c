#include <stdio.h>
#include <stdlib.h>

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

int getCurrentPlayer() {
    return (turn % 2 == 1) ? 1 : 2;
}

void displayMainMenu() {
    printf("=== Main Menu ===\n1. Start New Game\n2. Load Game\n3. Settings\n4. Exit\n");
}

void displayGrid() { //TO BE CHANGED
    printf("\n=== Middle-Earth Game Grid ===\n");
    printf("   A B C D E F G H I J K L M N O P Q R J T U V W X Y Z\n");
    printf("  %c", 218);
    for (int i = 0; i < 25; i++)
    {
        printf("%c%c", 196, 194);
    }
    printf("%c%c\n", 196, 191);
    for (int ii = 0; ii < 10; ii++)
    {
        printf("%d ", ii);
        for (int i = 0; i < 27; i++)
        {
            printf("%c ", 179);
        }
        printf("\n  %c", 195);
        for (int i = 0; i < 25; i++)
        {
            printf("%c%c", 196, 197);
        }
        printf("%c%c\n", 196, 180);
    }
    for (int ii = 10; ii < 17; ii++)
    {
        printf("%d", ii);
        for (int i = 0; i < 27; i++)
        {
            printf("%c ", 179);
        }
        printf("\n  %c", 195);
        for (int i = 0; i < 25; i++)
        {
            printf("%c%c", 196, 197);
        }
        printf("%c%c\n", 196, 180);
    }
    printf("17");
    for (int i = 0; i < 27; i++)
    {
        printf("%c ", 179);
    }
    printf("\n  %c", 192);
    for (int i = 0; i < 25; i++)
    {
        printf("%c%c", 196, 193);
    }
    printf("%c%c", 196, 217);
}

void displayGameGrid(int* player1Coins, int* player2Coins, int* currentPlayer) {
    int option = 0;

    do {
        displayGrid();
        printf("\nOptions (Player %d):\n", *currentPlayer);
        printf("1. Place Buildings\n2. Place Unit\n3. Move Unit\n4. Attack with Unit\n5. Current Amount of Coins\n6. End Turn\n");

        int option;
        printf("Enter your choice: ");
        scanf_s("%d", &option);

        switch (option) {
        case 1:  //UNDER DEVELOPMENT
            printf("Placing buildings...\n");
            break;
        case 2:  //UNDER DEVELOPMENT
            printf("Placing a unit...\n");
            break;
        case 3:  //UNDER DEVELOPMENT
            printf("Moving a unit...\n");
            break;
        case 4:  //UNDER DEVELOPMENT
            printf("Attacking with a unit...\n");
            break;
        case 5:
            if (*currentPlayer == 1) {
                printf("Current Amount of Coins: %d\n", *player1Coins);
            }
            else {
                printf("Current Amount of Coins: %d\n", *player2Coins);
            }
            break;
        case 6:
            printf("Ending the turn...\n");
            *currentPlayer = (*currentPlayer == 1) ? 2 : 1;
            break;
        default:
            printf("Invalid option. Please try again.\n");
            break;
        }
    } while (option != 6);
}

void startNewGame() {
    int gameMode;
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

                displayGameGrid(&player1Coins, &player2Coins, &turn);
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