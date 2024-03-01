#include <stdio.h>
#include <stdlib.h>
#include <limits.h>

#define MAX_SIZE 10

void printMatrix(int matrix[MAX_SIZE][MAX_SIZE], int n) {
    printf("Matriz:\n");
    for (int i = 0; i < n; i++) {
        for (int j = 0; j < n; j++) {
            printf("%d ", matrix[i][j]);
        }
        printf("\n");
    }
    printf("\n");
}

int maxSum(int matrix[MAX_SIZE][MAX_SIZE], int n) {
    int sum = 0;
    int selectedRows[MAX_SIZE] = {0};
    int selectedCols[MAX_SIZE] = {0};

    printf("Numeros selecionados:\n");

    for (int i = 0; i < n; i++) {
        int max = 0;
        int maxIndex = -1;

        for (int j = 0; j < n; j++) {
            if (!selectedRows[i] && !selectedCols[j] && matrix[i][j] > max) {
                max = matrix[i][j];
                maxIndex = j;
            }
        }

        if (maxIndex != -1) {
            printf("%d ", max);
            sum += max;
            selectedRows[i] = 1;
            selectedCols[maxIndex] = 1;
        }
    }

    printf("\n");

    return sum;
}

int main() {
    FILE *file = fopen("matriz.txt", "r");
    if (file == NULL) {
        fprintf(stderr, "Erro ao abrir o arquivo.\n");
        return 1;
    }

    int matrix[MAX_SIZE][MAX_SIZE];
    int n = 0;

    while (fscanf(file, "%d", &matrix[n][0]) != EOF) {
        for (int j = 1; j < MAX_SIZE; j++) {
            fscanf(file, ";%d", &matrix[n][j]);
        }
        n++;
    }

    fclose(file);

    printMatrix(matrix, n);

    int result = maxSum(matrix, n);

    printf("\nSoma maxima possivel: %d\n", result);

    return 0;
}