#include <stdbool.h>
#include <stdio.h>
#include <stdlib.h>
#include <string.h>

// Define a Node structure for linked list
typedef struct Node {
  int data;           // Data of the node
  struct Node* next;  // Pointer to the next node
} Node;

// Define a structure for Estrutura de Dados (ED)
typedef struct ED {
  Node* head;  // Head of the linked list
} ED;

int maxSum = 0;       // Maximum sum found so far
int* combination;     // Current combination of elements
int* maxCombination;  // Combination of elements with maximum sum
bool* usedIndices;    // Indices used in the max combination

// Recursive function to find the maximum sum of elements in the matrix
void findMaxSum(ED* matrix, int n, int m, int row, int col, int sum, bool* usedRows, bool* usedCols) {
  // If we have processed all rows
  if (row == n) {
    // If the current sum is greater than the maximum sum found so far
    if (sum > maxSum) {
      maxSum = sum;  // Update the maximum sum
      // Update the maximum combination and used indices
      for (int i = 0; i < n; i++) {
        if (usedRows[i]) {
          maxCombination[i] = combination[i];
          usedIndices[i] = true;
        } else {
          usedIndices[i] = false;
        }
      }
    }
    return;
  }

  // Traverse the linked list of the current row
  Node* node = matrix[row].head;
  for (int j = 0; j < m; j++) {
    // If the current column and row are not used
    if (!usedCols[j] && !usedRows[row]) {
      // Add the current element to the combination
      combination[row] = node->data;
      // Mark the current column and row as used
      usedCols[j] = true;
      usedRows[row] = true;
      // Recursively process the next row
      findMaxSum(matrix, n, m, row + 1, 0, sum + node->data, usedRows, usedCols);
      // Backtrack: Mark the current column and row as not used
      usedCols[j] = false;
      usedRows[row] = false;
    }
    // Move to the next node in the linked list
    node = node->next;
  }

  // If we have processed all nodes in the linked list
  if (node == NULL) {
    // Recursively process the next row without adding any element to the sum
    findMaxSum(matrix, n, m, row + 1, 0, sum, usedRows, usedCols);
  }
}

// Function to insert a node into the linked list of the matrix
void insertNode(ED* matrix, int data) {
  Node* newNode = malloc(sizeof(Node));  // Allocate memory for a new node
  newNode->data = data;                  // Set the data of the new node
  newNode->next = NULL;                  // Set the next pointer of the new node to NULL

  // If the head of the linked list is NULL
  if (matrix->head == NULL) {
    matrix->head = newNode;  // Set the head of the list to the new node
  } else {
    // If the list is not empty, traverse the list to the last node
    Node* temp = matrix->head;
    while (temp->next != NULL) {
      temp = temp->next;
    }
    // Insert the new node at the end of the list
    temp->next = newNode;
  }
}

// Function to insert a column into the matrix
void insertColumn(ED* matrix, int row, int data) {
  Node* newNode = malloc(sizeof(Node));  // Allocate memory for a new node
  newNode->data = data;                  // Set the data of the new node
  newNode->next = NULL;                  // Set the next pointer of the new node to NULL

  // If the head of the linked list in the specified row is NULL
  if (matrix[row].head == NULL) {
    matrix[row].head = newNode;  // Set the head of the list to the new node
  } else {
    // If the list is not empty, traverse the list to the last node
    Node* temp = matrix[row].head;
    while (temp->next != NULL) {
      temp = temp->next;
    }
    // Insert the new node at the end of the list
    temp->next = newNode;
  }
}

// Function to print the matrix
void printMatrix(ED* matrix, int n, int m) {
  system("cls");  // Clear the console
  printf("Matriz:\n");
  // Traverse each row of the matrix
  for (int i = 0; i < n; i++) {
    Node* node = matrix[i].head;  // Get the head of the linked list in the current row
    // Traverse the linked list and print each node's data
    while (node != NULL) {
      printf("%5d ", node->data);
      node = node->next;
    }
    printf("\n");  // Print a newline character after each row
  }
}

// Function to print the combination of elements with the maximum sum
void printMaxCombination(ED* matrix, int n, int m) {
  printf("\nElementos selecionados:\n");
  // Traverse the used indices
  for (int i = 0; i < n; i++) {
    // If the current index is used in the max combination
    if (usedIndices[i]) {
      printf("%d ", maxCombination[i]);  // Print the corresponding element
    }
  }
  printf("\n");  // Print a newline character at the end
}

// Function to remove a row from the matrix
void removeRow(ED** matrix, int* n, int row) {
  // Traverse the linked list of the row to be removed and free each node
  Node* node = (*matrix)[row].head;
  while (node != NULL) {
    Node* next = node->next;
    free(node);
    node = next;
  }

  // Shift each row after the removed row up by one position
  for (int i = row; i < *n - 1; i++) {
    (*matrix)[i] = (*matrix)[i + 1];
  }

  // Reallocate memory for the matrix to reduce its size by one row
  *matrix = realloc(*matrix, (*n - 1) * sizeof(ED));
  (*n)--;  // Decrease the number of rows by one
}

// Function to remove a column from the matrix
void removeColumn(ED* matrix, int n, int* m, int col) {
  // Traverse each row of the matrix
  for (int i = 0; i < n; i++) {
    Node* node = matrix[i].head;  // Get the head of the linked list in the current row
    Node* prev = NULL;            // Initialize the previous node to NULL

    // Traverse the linked list to the node at the position of the column to be removed
    for (int j = 0; j < col; j++) {
      prev = node;
      node = node->next;
    }

    // If the node to be removed is the first node in the list
    if (prev == NULL) {
      matrix[i].head = node->next;  // Set the head of the list to the next node
    } else {
      // If the node to be removed is not the first node in the list
      prev->next = node->next;  // Set the next pointer of the previous node to the next node
    }

    free(node);  // Free the node
  }

  (*m)--;  // Decrease the number of columns by one
}

// Function to free the memory allocated for the matrix
void freeMatrix(ED* matrix, int n) {
  // Traverse each row of the matrix
  for (int i = 0; i < n; i++) {
    Node* node = matrix[i].head;  // Get the head of the linked list in the current row
    // Traverse the linked list and free each node
    while (node != NULL) {
      Node* next = node->next;
      free(node);
      node = next;
    }
  }
  free(matrix);  // Free the memory allocated for the matrix
}

int main() {
  // Open the file "matriz.txt" for reading
  FILE* file = fopen("matriz.txt", "r");
  // If the file could not be opened, print an error message and return 1
  if (file == NULL) {
    fprintf(stderr, "Erro ao abrir o arquivo.\n");
    return 1;
  }

  // Initialize the number of rows and columns to 0
  int n = 0, m = 0;
  char ch;
  // Read the file character by character until the end of the file
  while (!feof(file)) {
    ch = fgetc(file);
    // If the current character is a newline character or the end of the file,
    // increment the number of rows
    if (ch == '\n' || ch == EOF) {
      n++;
    }
    // If the current character is a semicolon and we are in the first row,
    // increment the number of columns
    if (ch == ';' && n == 1) {
      m++;
    }
  }
  m++;           // Increment the number of columns one more time to account for the last column
  rewind(file);  // Move the file position indicator to the beginning of the file

  // Allocate memory for the matrix
  ED* matrix = malloc(n * sizeof(ED));
  // Initialize each row of the matrix
  for (int i = 0; i < n; i++) {
    matrix[i].head = NULL;               // Set the head of the linked list in the current row to NULL
    Node** current = &(matrix[i].head);  // Get a pointer to the head of the linked list
    // Initialize each column of the current row
    for (int j = 0; j < m; j++) {
      *current = malloc(sizeof(Node));  // Allocate memory for a new node
      // Read an integer from the file and set it as the data of the new node
      if (fscanf(file, "%d;", &((*current)->data)) != 1) {
        // If the integer could not be read, print an error message, close the file,
        // free the memory allocated for the matrix, and return 1
        fprintf(stderr, "Erro ao ler o arquivo.\n");
        fclose(file);
        freeMatrix(matrix, n);
        return 1;
      }
      current = &((*current)->next);  // Move to the next node in the linked list
    }
    *current = NULL;  // Set the next pointer of the last node in the linked list to NULL
  }

  fclose(file);  // Close the file

  // Allocate memory for the combination, max combination, and used indices
  combination = malloc(n * sizeof(int));
  maxCombination = malloc(n * sizeof(int));
  usedIndices = calloc(n, sizeof(bool));

  int option;
  // Start a loop that continues until the user chooses to exit
  do {
    // Print the menu options
    printf("\nOpcoes:\n");
    printf("1. Calcular matriz\n");
    printf("2. Adicionar linha/coluna\n");
    printf("3. Remover linha/coluna\n");
    printf("4. Modificar valor\n");
    printf("5. Sair\n");
    printf("Opcao: ");
    scanf("%d", &option);  // Read the user's choice
    getchar();             // Consume the newline character left in the input buffer

    // Perform an action based on the user's choice
    switch (option) {
      case 1:
        // Print the matrix, find the max sum, and print the max combination
        printMatrix(matrix, n, m);
        maxSum = 0;
        memset(maxCombination, 0, n * sizeof(int));
        bool* usedRows = calloc(n, sizeof(bool));
        bool* usedCols = calloc(m, sizeof(bool));
        findMaxSum(matrix, n, m, 0, 0, 0, usedRows, usedCols);
        printMaxCombination(matrix, n, m);
        printf("\nMaior soma possivel: %d\n", maxSum);
        free(usedRows);
        free(usedCols);
        break;
      case 2:
        // Add a row or a column to the matrix
        printf("\nAdicionar uma linha (1) ou uma coluna (2)?\n");
        int addOption;
        scanf("%d", &addOption);
        getchar();
        if (addOption == 1) {
          // Add a row to the matrix
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
        } else if (addOption == 2) {
          // Add a column to the matrix
          printf("\nInsira os numeros para a nova coluna no formato 1;2;3;n:\n");
          for (int i = 0; i < n; i++) {
            int data;
            scanf("%d;", &data);
            insertColumn(matrix, i, data);
          }
          m++;
        } else {
          printf("\nOpcao invalida.\n");
        }
        break;
      case 3:
        // Remove a row or a column from the matrix
        printf("\nRemover uma linha (1) ou uma coluna (2)?\n");
        int removeOption;
        scanf("%d", &removeOption);
        getchar();
        if (removeOption == 1) {
          // Remove a row from the matrix
          printf("\nMatriz atual:\n");
          printMatrix(matrix, n, m);
          printf("\nInsira o numero da linha para remover (1 a %d):\n", n);
          int row;
          scanf("%d", &row);
          getchar();
          if (row >= 1 && row <= n) {
            removeRow(&matrix, &n, row - 1);
          } else {
            printf("\nNumero de linha invalido.\n");
          }
        } else if (removeOption == 2) {
          // Remove a column from the matrix
          printf("\nMatriz atual:\n");
          printMatrix(matrix, n, m);
          printf("\nInsira o numero da coluna para remover (1 a %d):\n", m);
          int col;
          scanf("%d", &col);
          getchar();
          if (col >= 1 && col <= m) {
            removeColumn(matrix, n, &m, col - 1);
          } else {
            printf("\nNumero de coluna invalido.\n");
          }
        } else {
          printf("\nOpcao invalida.\n");
        }
        break;
      case 4:
        // Print the current matrix
        printMatrix(matrix, n, m);
        printf("\nInsira o numero da linha e da coluna no formato linha;coluna (1 a %d;1 a %d):\n", n, m);
        int row, col;
        scanf("%d;%d", &row, &col);
        getchar();
        printf("\nInsira o novo valor:\n");
        int newValue;
        scanf("%d", &newValue);
        getchar();
        // Check if the row and column numbers are valid
        if (row >= 1 && row <= n && col >= 1 && col <= m) {
          // Traverse the linked list of the specified row to the specified column
          Node* node = matrix[row - 1].head;
          for (int j = 1; j < col; j++) {
            node = node->next;
          }
          // Update the value of the node
          node->data = newValue;
          // Print the updated matrix
          printMatrix(matrix, n, m);
        } else {
          printf("\nNumero de linha ou coluna invalido.\n");
        }
        break;
      case 5:
        // Exit the program
        break;
      default:
        printf("\nOpcao invalida.\n");
    }
  } while (option != 5);

  // Free the memory allocated for the matrix and the combinations
  freeMatrix(matrix, n);
  free(combination);
  free(maxCombination);
  free(usedIndices);

  return 0;
}
