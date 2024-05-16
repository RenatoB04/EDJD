#include <stdio.h>
#include <stdlib.h>
#include <string.h>

void loadMatrixFromFile(const char *filename, int ***matrix, int *rows, int *cols) {
    FILE *file = fopen(filename, "r");
    if (file == NULL) {
        printf("Erro ao abrir o arquivo %s\n", filename);
        exit(EXIT_FAILURE);
    }
    char line[1000];
    int row = 0;
    int max_cols = 0;
    *matrix = malloc(sizeof(int*));
    while (fgets(line, sizeof(line), file) != NULL) {
        *matrix = realloc(*matrix, (row + 1) * sizeof(int*));
        (*matrix)[row] = malloc(sizeof(int) * 100);
        char *token = strtok(line, ";");
        int col = 0;
        while (token != NULL) {
            (*matrix)[row][col++] = atoi(token);
            token = strtok(NULL, ";");
        }
        if (col > max_cols)
            max_cols = col;
        row++;
    }
    *rows = row;
    *cols = max_cols;
    fclose(file);
}

void printMatrix(int **matrix, int rows, int cols) {
    printf("Matriz lida do arquivo:\n\n");
    for (int i = 0; i < rows; i++) {
        for (int j = 0; j < cols; j++) {
            printf("%4d ", matrix[i][j]);
        }
        printf("\n");
    }
}

int main() {
    const char *filename = "matriz.txt";
    int **matrix = NULL;
    int rows, cols;
    int choice;
    do {
        printf("\n1 - Imprimir matriz do arquivo\n");
        printf("0 - Sair\n");
        printf("Opcao: ");
        scanf("%d", &choice);
        system("cls");
        switch (choice) {
            case 1:
                loadMatrixFromFile(filename, &matrix, &rows, &cols);
                printMatrix(matrix, rows, cols);
                for (int i = 0; i < rows; i++) {
                    free(matrix[i]);
                }
                free(matrix);
                break;
            case 0:
                printf("A sair...\n");
                break;
            default:
                printf("Opcao inválida. Tente novamente.\n");
        }
    } while (choice != 0);
    return 0;
}