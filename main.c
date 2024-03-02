#include <stdio.h>
#include <stdbool.h>

#define MAX_SIZE 5

int maxSum = 0;
int combination[5];
int maxCombination[5];

void findMaxSum(int matrix[MAX_SIZE][MAX_SIZE], int n, int row, int col, int sum, bool usedRows[MAX_SIZE], bool usedCols[MAX_SIZE]) {
    if (col >= n) {
        if (sum > maxSum) {
            maxSum = sum;
            for (int i = 0; i < 5; i++) {
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

    int matrix[MAX_SIZE][MAX_SIZE];
    printf("Matriz:\n");
    for (int i = 0; i < MAX_SIZE; i++) {
        for (int j = 0; j < MAX_SIZE; j++) {
            if (fscanf(file, "%d", &matrix[i][j]) != 1) {
                fprintf(stderr, "Erro ao ler o arquivo.\n");
                fclose(file);
                return 1;
            }
            printf("%d ", matrix[i][j]);
            fgetc(file);
        }
        printf("\n");
    }

    fclose(file);

    bool usedRows[MAX_SIZE] = {false};
    bool usedCols[MAX_SIZE] = {false};

    findMaxSum(matrix, MAX_SIZE, 0, 0, 0, usedRows, usedCols);

    printf("\nElementos selecionados:\n");
    for (int i = 0; i < 5; i++) {
        printf("%d ", maxCombination[i]);
    }
    printf("\n\nMaior soma possivel: %d\n\n", maxSum);

    return 0;
}