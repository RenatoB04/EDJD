void loadGame(char grid[ROWS][COLS], int* player1Coins, int* player2Coins, int* turn) {
    FILE* file = fopen("savegame.txt", "r");

    if (file == NULL) {
        printf("No saved game found.\n");
        return;
    }

    char line[COLS + 2];  // +2 for the newline character and null terminator
    for (int i = 0; i < ROWS; i++) {
        fgets(line, sizeof(line), file);
        for (int j = 0; j < COLS; j++) {
            grid[i][j] = line[j];
        }
    }

    fscanf_s(file, "%d %d %d", player1Coins, player2Coins, turn);

    fclose(file);

    printf("Game loaded successfully!\n");
    displayGameGrid(player1Coins, player2Coins, turn, grid);
}