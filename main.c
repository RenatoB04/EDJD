#include <stdio.h>
#include <stdlib.h>
#include <string.h>

typedef struct Node {
    int vertex;
    struct Node* next;
} Node;

typedef struct Graph {
    int numVertices;
    Node** adjLists;
} Graph;

typedef enum {
    HORIZONTAL,
    VERTICAL,
    HORIZONTAL_VERTICAL
} EdgeType;

EdgeType edgeType;

Node* createNode(int vertex) {
    Node* newNode = malloc(sizeof(Node));
    newNode->vertex = vertex;
    newNode->next = NULL;
    return newNode;
}

Graph* createGraph(int vertices) {
    Graph* graph = malloc(sizeof(Graph));
    graph->numVertices = vertices;
    graph->adjLists = malloc(vertices * sizeof(Node*));

    for (int i = 0; i < vertices; i++)
        graph->adjLists[i] = NULL;

    return graph;
}

void addEdge(Graph* graph, int src, int dest) {
    Node* newNode = createNode(dest);
    newNode->next = graph->adjLists[src];
    graph->adjLists[src] = newNode;
}

void freeGraph(Graph* graph) {
    for (int v = 0; v < graph->numVertices; v++) {
        Node* adjList = graph->adjLists[v];
        Node* tmp = NULL;
        while (adjList) {
            tmp = adjList;
            adjList = adjList->next;
            free(tmp);
        }
    }
    free(graph->adjLists);
    free(graph);
}

int** createAdjacencyMatrix(Graph* graph, int numVertices) {
    int** adjacencyMatrix = malloc(numVertices * sizeof(int*));
    for (int i = 0; i < numVertices; i++) {
        adjacencyMatrix[i] = malloc(numVertices * sizeof(int));
        for (int j = 0; j < numVertices; j++) {
            adjacencyMatrix[i][j] = 0;
        }
    }

    for (int v = 0; v < numVertices; v++) {
        Node* temp = graph->adjLists[v];
        while (temp) {
            adjacencyMatrix[v][temp->vertex] = 1;
            temp = temp->next;
        }
    }

    return adjacencyMatrix;
}

void printAdjacencyMatrix(int** adjacencyMatrix, int numVertices) {
    printf("\nMatriz de adjacencia:\n\n");

    printf("    ");
    for (int i = 1; i < numVertices; i++) {
        printf("%4d", i);
    }
    printf("\n");

    for (int i = 1; i < numVertices; i++) {
        printf("%4d", i);
        for (int j = 1; j < numVertices; j++) {
            printf("%4d", adjacencyMatrix[i][j]);
        }
        printf("\n");
    }
}

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
    printf("Matriz do arquivo:\n\n");
    for (int i = 0; i < rows; i++) {
        for (int j = 0; j < cols; j++) {
            printf("%4d ", matrix[i][j]);
        }
        printf("\n");
    }
}

void setEdgeType() {
    int choice;
    printf("\n1 - Arestas horizontais\n");
    printf("2 - Arestas verticais\n");
    printf("3 - Arestas horizontais e verticais\n");
    printf("Opcao: ");
    scanf("%d", &choice);
    switch (choice) {
        case 1:
            edgeType = HORIZONTAL;
            break;
        case 2:
            edgeType = VERTICAL;
            break;
        case 3:
            edgeType = HORIZONTAL_VERTICAL;
            break;
        default:
            printf("Opcao invalida. Tente novamente.\n");
            setEdgeType();
    }
}

Graph* convertMatrixToGraph(int **matrix, int rows, int cols) {
    int vertex = 1;
    Graph* graph = createGraph(rows * cols + 1);

    for (int i = 0; i < rows; i++) {
        for (int j = 0; j < cols; j++) {
            if (edgeType == HORIZONTAL || edgeType == HORIZONTAL_VERTICAL) {
                if (j > 0) addEdge(graph, vertex, vertex-1);
                if (j < cols-1) addEdge(graph, vertex, vertex+1);
            }
            if (edgeType == VERTICAL || edgeType == HORIZONTAL_VERTICAL) {
                if (i > 0) addEdge(graph, vertex, vertex-cols);
                if (i < rows-1) addEdge(graph, vertex, vertex+cols);
            }
            vertex++;
        }
    }
    return graph;
}

void printGraph(Graph* graph, int rows, int cols) {
    printf("Vertices do grafo:\n\n");
    for (int v = 1; v <= rows * cols; v++) {
        printf("%3d ", v);
        if (v % cols == 0) {
            printf("\n");
        }
    }
}

int main() {
    const char *filename = "matriz.txt";
    int **matrix = NULL;
    int rows, cols;
    Graph* graph = NULL;
    int choice;

    do {
        printf("\n1 - Imprimir matriz do arquivo\n");
        printf("2 - Imprimir vertices da matriz\n");
        printf("3 - Definir arestas\n");
        printf("4 - Construir grafo\n");
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
            case 2:
                loadMatrixFromFile(filename, &matrix, &rows, &cols);
                graph = convertMatrixToGraph(matrix, rows, cols);
                printGraph(graph, rows, cols);
                for (int i = 0; i < rows; i++) {
                    free(matrix[i]);
                }
                free(matrix);
                break;
            case 3:
                setEdgeType();
                break;
            case 4:
                loadMatrixFromFile(filename, &matrix, &rows, &cols);
                graph = convertMatrixToGraph(matrix, rows, cols);
                printGraph(graph, rows, cols);
                int** adjacencyMatrix = createAdjacencyMatrix(graph, rows * cols + 1);
                printAdjacencyMatrix(adjacencyMatrix, rows * cols + 1);
                for (int i = 0; i < rows; i++) {
                    free(matrix[i]);
                }
                free(matrix);
                for (int i = 0; i < rows * cols + 1; i++) {
                    free(adjacencyMatrix[i]);
                }
                free(adjacencyMatrix);
                break;
            case 0:
                if (graph) freeGraph(graph);
                break;
            default:
                printf("Opcao invalida. Tente novamente.\n");
        }
    } while (choice != 0);

    return 0;
}