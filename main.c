#include <stdio.h>
#include <stdbool.h>
#include <stdlib.h>
#include <string.h>

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
    if (row == n) {
        if (sum > maxSum) {
            maxSum = sum;
            for (int i = 0; i < n; i++) {
                maxCombination[i] = combination[i];
            }
        }
        return;
    }

    Node* node = matrix[row].head;
    for (int j = 0; j < m; j++) {
        if (!usedCols[j] && !usedRows[row]) {
            combination[row] = node->data;
            usedCols[j] = true;
            usedRows[row] = true;
            findMaxSum(matrix, n, m, row + 1, 0, sum + node->data, usedRows, usedCols);
            usedCols[j] = false;
            usedRows[row] = false;
        }
        node = node->next;
    }

    if (node == NULL) {
        findMaxSum(matrix, n, m, row + 1, 0, sum, usedRows, usedCols);
    }
}

void insertNode(ED* matrix, int data) {
    Node* newNode = malloc(sizeof(Node));
    newNode->data = data;
    newNode->next = NULL;

    if (matrix->head == NULL) {
        matrix->head = newNode;
    } else {
        Node* temp = matrix->head;
        while (temp->next != NULL) {
            temp = temp->next;
        }
        temp->next = newNode;
    }
}

void insertColumn(ED* matrix, int row, int data) {
    Node* newNode = malloc(sizeof(Node));
    newNode->data = data;
    newNode->next = NULL;

    if (matrix[row].head == NULL) {
        matrix[row].head = newNode;
    } else {
        Node* temp = matrix[row].head;
        while (temp->next != NULL) {
            temp = temp->next;
        }
        temp->next = newNode;
    }
}

void printMatrix(ED* matrix, int n, int m) {
    for (int i = 0; i < n; i++) {
        Node* node = matrix[i].head;
        while (node != NULL) {
            printf("%d ", node->data);
            node = node->next;
        }
        printf("\n");
    }
}

void freeMatrix(ED* matrix, int n) {
    for (int i = 0; i < n; i++) {
        Node* node = matrix[i].head;
        while (node != NULL) {
            Node* next = node->next;
            free(node);
            node = next;
        }
    }
    free(matrix);
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
                freeMatrix(matrix, n);
                return 1;
            }
            current = &((*current)->next);
        }
        *current = NULL;
    }

    fclose(file);

    combination = malloc(n * sizeof(int));
    maxCombination = malloc(n * sizeof(int));

    int option;
    do {
        printf("\nOpcoes:\n");
        printf("1. Calcular matriz atual\n");
        printf("2. Adicionar linha\n");
        printf("3. Adicionar coluna\n");
        printf("4. Calcular matriz modificada\n");
        printf("5. Sair\n");
        printf("Escolha uma opcao: ");
        scanf("%d", &option);
        getchar();

        switch(option) {
            case 1:
                printf("\nMatriz Atual:\n");
                printMatrix(matrix, n, m);
                bool *usedRows = calloc(n, sizeof(bool));
                bool *usedCols = calloc(m, sizeof(bool));
                findMaxSum(matrix, n, m, 0, 0, 0, usedRows, usedCols);
                printf("\nElementos selecionados:\n");
                for (int i = 0; i < n; i++) {
                    printf("%d ", maxCombination[i]);
                }
                printf("\n\nMaior soma possivel: %d\n", maxSum);
                free(usedRows);
                free(usedCols);
                break;
            case 2:
                printf("\nInsira os numeros para a nova linha no formato 1;2;3;n:\n");
                ED* newLineMatrix = malloc(sizeof(ED));
                newLineMatrix->head = NULL;
                for (int j = 0; j < m; j++) {
                    int data;
                    scanf("%d;", &data);
                    insertNode(newLineMatrix, data);
                }
                matrix = realloc(matrix, (n + 1) * sizeof(ED));
                matrix[n] = *newLineMatrix;
                n++;
                free(newLineMatrix);
                break;
            case 3:
                printf("\nInsira os numeros para a nova coluna no formato 1;2;3;n:\n");
                for (int i = 0; i < n; i++) {
                    int data;
                    scanf("%d;", &data);
                    insertColumn(matrix, i, data);
                }
                m++;
                break;
            case 4:
                printf("\nMatriz Modificada:\n");
                printMatrix(matrix, n, m);
                maxSum = 0;
                memset(maxCombination, 0, n * sizeof(int));
                bool *usedRowsMod = calloc(n, sizeof(bool));
                bool *usedColsMod = calloc(m, sizeof(bool));
                findMaxSum(matrix, n, m, 0, 0, 0, usedRowsMod, usedColsMod);
                printf("\nElementos selecionados:\n");
                for (int i = 0; i < n; i++) {
                    printf("%d ", maxCombination[i]);
                }
                printf("\n\nMaior soma possivel: %d\n", maxSum);
                free(usedRowsMod);
                free(usedColsMod);
                break;
            case 5:
                printf("\nA sair...\n");
                break;
            default:
                printf("\nOpcao invalida. Tente novamente.\n");
        }
    } while (option != 5);

    freeMatrix(matrix, n);
    free(combination);
    free(maxCombination);

    return 0;
}