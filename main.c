#include <stdio.h>
#include <stdlib.h>
#include <string.h>
#include <limits.h>

typedef struct Node {
    int vertex;
    int weight;
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

Node* createNode(int vertex, int weight) {
    Node* newNode = malloc(sizeof(Node));
    newNode->vertex = vertex;
    newNode->weight = weight;
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

void addEdge(Graph* graph, int src, int dest, int weight) {
    Node* newNode = createNode(dest, weight);
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
            adjacencyMatrix[v][temp->vertex] = temp->weight;
            temp = temp->next;
        }
    }

    return adjacencyMatrix;
}

void printAdjacencyMatrix(int** adjacencyMatrix, int numVertices) {
    printf("\nMatriz de adjacencia:\n\n");

    printf("    ");
    for (int i = 1; i < numVertices; i++) {
        printf("%6d", i);
    }
    printf("\n");

    for (int i = 1; i < numVertices; i++) {
        printf("%6d", i);
        for (int j = 1; j < numVertices; j++) {
            printf("%6d", adjacencyMatrix[i][j]);
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
    Graph* graph = createGraph(rows * cols + 1);

    for (int i = 0; i < rows; i++) {
        for (int j = 0; j < cols; j++) {
            int vertex = i * cols + j + 1;
            if (edgeType == HORIZONTAL || edgeType == HORIZONTAL_VERTICAL) {
                if (j < cols-1) {
                    addEdge(graph, vertex, vertex + 1, matrix[i][j+1]);
                }
                if (j > 0) {
                    addEdge(graph, vertex, vertex - 1, matrix[i][j-1]);
                }
            }
            if (edgeType == VERTICAL || edgeType == HORIZONTAL_VERTICAL) {
                if (i < rows-1) {
                    addEdge(graph, vertex, vertex + cols, matrix[i+1][j]);
                }
                if (i > 0) {
                    addEdge(graph, vertex, vertex - cols, matrix[i-1][j]);
                }
            }
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

void calculateMaxSum(int **matrix, int rows, int cols) {
    int maxSum = 0;

    printf("Somas das linhas e colunas:\n\n");

    if (edgeType == HORIZONTAL || edgeType == HORIZONTAL_VERTICAL) {
        for (int i = 0; i < rows; i++) {
            int rowSum = 0;
            for (int j = 0; j < cols; j++) {
                rowSum += matrix[i][j];
            }
            printf("Soma da linha %d: %d\n", i + 1, rowSum);
            if (rowSum > maxSum) {
                maxSum = rowSum;
            }
        }
    }

    if (edgeType == VERTICAL || edgeType == HORIZONTAL_VERTICAL) {
        for (int j = 0; j < cols; j++) {
            int colSum = 0;
            for (int i = 0; i < rows; i++) {
                colSum += matrix[i][j];
            }
            printf("Soma da coluna %d: %d\n", j + 1, colSum);
            if (colSum > maxSum) {
                maxSum = colSum;
            }
        }
    }

    printf("\nMaior soma possivel: %d\n", maxSum);
}

int minDistance(int dist[], int sptSet[], int numVertices) {
    int min = INT_MAX, min_index;
    for (int v = 0; v < numVertices; v++)
        if (sptSet[v] == 0 && dist[v] <= min)
            min = dist[v], min_index = v;
    return min_index;
}

void dijkstra(Graph* graph, int src, int dest) {
    int numVertices = graph->numVertices;
    int *dist = malloc(numVertices * sizeof(int));
    int *sptSet = malloc(numVertices * sizeof(int));
    int *parent = malloc(numVertices * sizeof(int));

    for (int i = 0; i < numVertices; i++) {
        dist[i] = INT_MAX;
        sptSet[i] = 0;
        parent[i] = -1;
    }
    dist[src] = 0;
    for (int count = 0; count < numVertices - 1; count++) {
        int u = minDistance(dist, sptSet, numVertices);
        sptSet[u] = 1;
        Node* temp = graph->adjLists[u];
        while (temp != NULL) {
            int v = temp->vertex;
            int weight = temp->weight;
            if (!sptSet[v] && dist[u] != INT_MAX && dist[u] + weight < dist[v]) {
                dist[v] = dist[u] + weight;
                parent[v] = u;
            }
            temp = temp->next;
        }
    }
    if (dist[dest] == INT_MAX) {
        printf("Nao existe caminho do vertice %d para o vertice %d\n", src, dest);
    } else {
        printf("Menor distancia do vertice %d para o vertice %d: %d\n", src, dest, dist[dest]);
        printf("Caminho: ");
        int *path = malloc(numVertices * sizeof(int));
        int j = 0;
        for (int i = dest; i != -1; i = parent[i]) {
            path[j++] = i;
        }
        for (int i = j - 1; i >= 0; i--) {
            printf("%d ", path[i]);
        }
        printf("\n");
        free(path);
    }

    free(dist);
    free(sptSet);
    free(parent);
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
        printf("5 - Calcular maior soma\n");
        printf("6 - Calcular menor caminho\n");
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
            case 5:
                loadMatrixFromFile(filename, &matrix, &rows, &cols);
                printMatrix(matrix, rows, cols);
                printf("\n");
                graph = convertMatrixToGraph(matrix, rows, cols);
                printGraph(graph, rows, cols);
                printf("\n");
                calculateMaxSum(matrix, rows, cols);
                for (int i = 0; i < rows; i++) {
                    free(matrix[i]);
                }
                free(matrix);
                break;
            case 6:
                loadMatrixFromFile(filename, &matrix, &rows, &cols);
                graph = convertMatrixToGraph(matrix, rows, cols);
                printMatrix(matrix, rows, cols);
                printf("\n");
                printGraph(graph, rows, cols);
                int src, dest;
                printf("\nVertice de origem: ");
                scanf("%d", &src);
                printf("Vertice de destino: ");
                scanf("%d", &dest);
                dijkstra(graph, src, dest);
                for (int i = 0; i < rows; i++) {
                    free(matrix[i]);
                }
                free(matrix);
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