#include <stdio.h>
#include <stdbool.h>
#include <stdlib.h>

typedef struct Node {
    int data;
    struct Node* next;
} Node;

typedef struct ED {
    Node* head;
    int length;
} ED;

int maxSum = 0;
int *combination;
int *maxCombination;

void findMaxSum(ED* matrix, int n, int m, int row, int col, int sum, bool *usedRows, bool *usedCols) {
    if (col >= m) {
        if (sum > maxSum) {
            maxSum = sum;
            for (int i = 0; i < m; i++) {
                maxCombination[i] = combination[i];
            }
        }
        return;
    }

    for (int i = 0; i < n; i++) {
        if (!usedRows[i] && !usedCols[col]) {
            usedRows[i] = true;
            usedCols[col] = true;
            Node* node = matrix[i].head;
            for (int j = 0; j < col; j++) {
                node = node->next;
            }
            combination[col] = node->data;
            findMaxSum(matrix, n, m, row, col + 1, sum + node->data, usedRows, usedCols);
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

    int n = 0, m = 0;
    char ch;
    while(!feof(file)) {
        ch = fgetc(file);
        if(ch == '\n' || ch == EOF) {
            n++;
        }
        if(ch == ';' && n == 1) {
            m++;
        }
    }
    m++;
    rewind(file);

    ED* matrix = malloc(n * sizeof(ED));
    for (int i = 0; i < n; i++) {
        matrix[i].head = NULL;
        Node** current = &(matrix[i].head);
        for (int j = 0; j < m; j++) {
            *current = malloc(sizeof(Node));
            if (fscanf(file, "%d;", &((*current)->data)) != 1) {
                fprintf(stderr, "Erro ao ler o arquivo.\n");
                fclose(file);
                return 1;
            }
            current = &((*current)->next);
        }
        *current = NULL;
    }

    combination = malloc(m * sizeof(int));
    maxCombination = malloc(m * sizeof(int));

    printf("Matriz:\n");
    for (int i = 0; i < n; i++) {
        Node* node = matrix[i].head;
        while (node != NULL) {
            printf("%d ", node->data);
            node = node->next;
        }
        printf("\n");
    }

    fclose(file);

    bool *usedRows = calloc(n, sizeof(bool));
    bool *usedCols = calloc(m, sizeof(bool));

    findMaxSum(matrix, n, m, 0, 0, 0, usedRows, usedCols);

    printf("\nElementos selecionados:\n");
    for (int i = 0; i < m; i++) {
        printf("%d ", maxCombination[i]);
    }
    printf("\n\nMaior soma possivel: %d\n", maxSum);

    for (int i = 0; i < n; i++) {
        Node* node = matrix[i].head;
        while (node != NULL) {
            Node* next = node->next;
            free(node);
            node = next;
        }
    }
    free(matrix);
    free(usedRows);
    free(usedCols);
    free(combination);
    free(maxCombination);

    return 0;
}