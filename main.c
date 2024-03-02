#include <stdio.h>
#include <stdbool.h>
#include <stdlib.h>

int maxSum = 0;
int *combination;
int *maxCombination;

void findMaxSum(int **matrix, int n, int row, int col, int sum, bool *usedRows, bool *usedCols) {
    if (col >= n) {
        if (sum > maxSum) {
            maxSum = sum;
            for (int i = 0; i < n; i++) {
                maxCombination[i] = combination[i];
            }
        }
        return;
    }

    for (int i = 0; i < n; i++) {
        if (!usedRows[i] && !usedCols[col]) {
            usedRows[i] = true;
            usedCols[col] = true;
            combination[col] = matrix[i][col];
            findMaxSum(matrix, n, row, col + 1, sum + matrix[i][col], usedRows, usedCols);
            usedRows[i] = false;
            usedCols[col] = false;
        }
    }
}

int main() {
    FILE *file = fopen("matriz.txt", "r");
    if (file == NULL) {
        fprintf(stderr, "Erro ao abrir o arquivo.\n");
        return 1;
    }

    int n = 0;
    char ch;
    while(!feof(file)) {
        ch = fgetc(file);
        if(ch == '\n' || ch == EOF) {
            n++;
        }
    }
    rewind(file);

    int **matrix = malloc(n * sizeof(int *));
    for (int i = 0; i < n; i++) {
        matrix[i] = malloc(n * sizeof(int));
    }

    combination = malloc(n * sizeof(int));
    maxCombination = malloc(n * sizeof(int));

    printf("Matriz:\n");
    for (int i = 0; i < n; i++) {
        for (int j = 0; j < n; j++) {
            if (fscanf(file, "%d;", &matrix[i][j]) != 1) {
                fprintf(stderr, "Erro ao ler o arquivo.\n");
                fclose(file);
                return 1;
            }
            printf("%d ", matrix[i][j]);
        }
        printf("\n");
    }

    fclose(file);

    bool *usedRows = calloc(n, sizeof(bool));
    bool *usedCols = calloc(n, sizeof(bool));

    findMaxSum(matrix, n, 0, 0, 0, usedRows, usedCols);

    printf("\nElementos selecionados:\n");
    for (int i = 0; i < n; i++) {
        printf("%d ", maxCombination[i]);
    }
    printf("\n\nMaior soma possivel: %d\n", maxSum);

    // Free dynamically allocated memory
    for (int i = 0; i < n; i++) {
        free(matrix[i]);
    }
    free(matrix);
    free(usedRows);
    free(usedCols);
    free(combination);
    free(maxCombination);

    return 0;
}