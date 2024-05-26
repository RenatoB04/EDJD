#include <stdio.h>
#include <stdlib.h>
#include <string.h>
#include <limits.h>

// Estrutura de um nó na lista de adjacências
typedef struct Node {
    int vertex;           // Vértice de destino da aresta
    int weight;           // Peso da aresta
    struct Node* next;    // Próximo nó na lista de adjacências
} Node;

// Estrutura de um grafo
typedef struct Graph {
    int numVertices;      // Número de vértices no grafo
    Node** adjLists;      // Lista de adjacências (array de listas ligadas)
} Graph;

// Tipos de arestas possíveis
typedef enum {
    HORIZONTAL,           // Arestas horizontais
    VERTICAL,             // Arestas verticais
    HORIZONTAL_VERTICAL   // Arestas horizontais e verticais
} EdgeType;

EdgeType edgeType;        // Variável global para o tipo de aresta

// Função para criar um novo nó na lista de adjacências
Node* createNode(int vertex, int weight) {
    Node* newNode = malloc(sizeof(Node));    // Aloca memória para o novo nó
    newNode->vertex = vertex;                // Define o vértice de destino
    newNode->weight = weight;                // Define o peso da aresta
    newNode->next = NULL;                    // Inicializa o próximo nó como NULL
    return newNode;                          // Retorna o novo nó
}

// Função para criar um grafo com um número específico de vértices
Graph* createGraph(int vertices) {
    Graph* graph = malloc(sizeof(Graph));    // Aloca memória para o grafo
    graph->numVertices = vertices;           // Define o número de vértices
    graph->adjLists = malloc(vertices * sizeof(Node*));  // Aloca memória para as listas de adjacências

    // Inicializa cada lista de adjacências como NULL
    for (int i = 0; i < vertices; i++)
        graph->adjLists[i] = NULL;

    return graph;    // Retorna o grafo criado
}

// Função para adicionar uma aresta ao grafo
void addEdge(Graph* graph, int src, int dest, int weight) {
    Node* newNode = createNode(dest, weight);  // Cria um novo nó
    newNode->next = graph->adjLists[src];      // Aponta o novo nó para o início da lista de adjacências
    graph->adjLists[src] = newNode;            // Define o novo nó como o primeiro da lista
}

// Função para libertar a memória utilizada pelo grafo
void freeGraph(Graph* graph) {
    // Percorre todas as listas de adjacências
    for (int v = 0; v < graph->numVertices; v++) {
        Node* adjList = graph->adjLists[v];  // Obtém a lista de adjacências do vértice v
        Node* tmp;
        // liberta cada nó da lista de adjacências
        while (adjList) {
            tmp = adjList;
            adjList = adjList->next;
            free(tmp);
        }
    }
    // liberta a memória das listas de adjacências e do grafo
    free(graph->adjLists);
    free(graph);
}

// Função para criar uma matriz de adjacência a partir do grafo
int** createAdjacencyMatrix(Graph* graph, int numVertices) {
    int** adjacencyMatrix = malloc(numVertices * sizeof(int*));  // Aloca memória para a matriz
    // Inicializa a matriz com zeros
    for (int i = 0; i < numVertices; i++) {
        adjacencyMatrix[i] = malloc(numVertices * sizeof(int));
        for (int j = 0; j < numVertices; j++) {
            adjacencyMatrix[i][j] = 0;
        }
    }

    // Preenche a matriz com os pesos das arestas
    for (int v = 0; v < numVertices; v++) {
        Node* temp = graph->adjLists[v];
        while (temp) {
            adjacencyMatrix[v][temp->vertex] = temp->weight;
            temp = temp->next;
        }
    }

    return adjacencyMatrix;  // Retorna a matriz de adjacência
}

// Função para imprimir a matriz de adjacência
void printAdjacencyMatrix(int** adjacencyMatrix, int numVertices) {
    printf("\nMatriz de adjacência:\n\n");

    // Imprime os cabeçalhos das colunas
    printf("    ");
    for (int i = 1; i < numVertices; i++) {
        printf("%6d", i);
    }
    printf("\n");

    // Imprime as linhas da matriz
    for (int i = 1; i < numVertices; i++) {
        printf("%6d", i);
        for (int j = 1; j < numVertices; j++) {
            printf("%6d", adjacencyMatrix[i][j]);
        }
        printf("\n");
    }
}

// Função para carregar uma matriz a partir de um ficheiro
void loadMatrixFromFile(const char *filename, int ***matrix, int *rows, int *cols) {
    FILE *file = fopen(filename, "r");
    if (file == NULL) {
        printf("Erro ao abrir o ficheiro %s\n", filename);
        exit(EXIT_FAILURE);
    }

    char line[1000];
    int row = 0;
    int max_cols = 0;
    *matrix = malloc(sizeof(int*));

    // Lê cada linha do ficheiro
    while (fgets(line, sizeof(line), file) != NULL) {
        *matrix = realloc(*matrix, (row + 1) * sizeof(int*));
        (*matrix)[row] = malloc(sizeof(int) * 100);
        char *token = strtok(line, ";");
        int col = 0;
        // Separa os valores da linha
        while (token != NULL) {
            (*matrix)[row][col++] = atoi(token);
            token = strtok(NULL, ";");
        }
        if (col > max_cols)
            max_cols = col;
        row++;
    }
    *rows = row;       // Define o número de linhas
    *cols = max_cols;  // Define o número de colunas
    fclose(file);      // Fecha o ficheiro
}

// Função para imprimir a matriz carregada do ficheiro
void printMatrix(int **matrix, int rows, int cols) {
    printf("Matriz do ficheiro:\n\n");
    // Imprime cada elemento da matriz
    for (int i = 0; i < rows; i++) {
        for (int j = 0; j < cols; j++) {
            printf("%4d ", matrix[i][j]);
        }
        printf("\n");
    }
}

// Função para definir o tipo de aresta com base na escolha do utilizador
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

// Função para converter uma matriz para um grafo
Graph* convertMatrixToGraph(int **matrix, int rows, int cols) {
    Graph* graph = createGraph(rows * cols + 1);  // Cria um grafo com o número necessário de vértices

    // Adiciona arestas ao grafo com base no tipo de aresta selecionado
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
    return graph;  // Retorna o grafo criado
}

// Função para imprimir os vértices do grafo
void printGraph(Graph* graph, int rows, int cols) {
    printf("Vertices do grafo:\n\n");
    // Imprime os vértices em formato de matriz
    for (int v = 1; v <= rows * cols; v++) {
        printf("%3d ", v);
        if (v % cols == 0) {
            printf("\n");
        }
    }
}

// Função para calcular a maior soma possível das linhas e colunas da matriz
void calculateMaxSum(int **matrix, int rows, int cols) {
    int maxSum = 0;

    printf("Somas das linhas e colunas:\n\n");

    // Calcula a soma das linhas
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

    // Calcula a soma das colunas
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

    printf("\nMaior soma possivel: %d\n", maxSum);  // Imprime a maior soma encontrada
}

// Função auxiliar para encontrar o vértice com a menor distância que ainda não foi processado
int minDistance(int dist[], int sptSet[], int numVertices) {
    int min = INT_MAX, min_index;
    for (int v = 0; v < numVertices; v++)
        if (sptSet[v] == 0 && dist[v] <= min)
            min = dist[v], min_index = v;
    return min_index;
}

// Implementa o algoritmo de Dijkstra para encontrar o menor caminho
void dijkstra(Graph* graph, int src, int dest) {
    int numVertices = graph->numVertices;
    int *dist = malloc(numVertices * sizeof(int));
    int *sptSet = malloc(numVertices * sizeof(int));
    int *parent = malloc(numVertices * sizeof(int));

    // Inicializa as distâncias e o conjunto de vértices processados
    for (int i = 0; i < numVertices; i++) {
        dist[i] = INT_MAX;
        sptSet[i] = 0;
        parent[i] = -1;
    }
    dist[src] = 0;  // Define a distância da origem como 0

    // Encontra o caminho mais curto para todos os vértices
    for (int count = 0; count < numVertices - 1; count++) {
        int u = minDistance(dist, sptSet, numVertices);  // Obtém o vértice com a menor distância
        sptSet[u] = 1;  // Marca o vértice como processado
        Node* temp = graph->adjLists[u];
        while (temp != NULL) {
            int v = temp->vertex;
            int weight = temp->weight;
            // Atualiza a distância do vértice v
            if (!sptSet[v] && dist[u] != INT_MAX && dist[u] + weight < dist[v]) {
                dist[v] = dist[u] + weight;
                parent[v] = u;
            }
            temp = temp->next;
        }
    }

    // Verifica se existe um caminho do vértice origem ao destino
    if (dist[dest] == INT_MAX) {
        printf("Nao existe caminho do vertice %d para o vertice %d\n", src, dest);
    } else {
        printf("Menor distancia do vertice %d para o vertice %d: %d\n", src, dest, dist[dest]);
        printf("Caminho: ");
        int *path = malloc(numVertices * sizeof(int));
        int j = 0;
        // Reconstrói o caminho
        for (int i = dest; i != -1; i = parent[i]) {
            path[j++] = i;
        }
        for (int i = j - 1; i >= 0; i--) {
            printf("%d ", path[i]);
        }
        printf("\n");
        free(path);
    }

    free(dist);     // Liberta a memória alocada
    free(sptSet);
    free(parent);
}

// Função principal
int main() {
    const char *filename = "matriz.txt";
    int **matrix = NULL;
    int rows, cols;
    Graph* graph = NULL;
    int choice;

    do {
        // Menu de opções
        printf("\n1 - Imprimir matriz do ficheiro\n");
        printf("2 - Imprimir vertices da matriz\n");
        printf("3 - Definir arestas\n");
        printf("4 - Construir grafo\n");
        printf("5 - Calcular maior soma\n");
        printf("6 - Calcular menor caminho\n");
        printf("0 - Sair\n");
        printf("Opcao: ");
        scanf("%d", &choice);
        system("cls");  // Limpa o ecrã

        switch (choice) {
            case 1:
                loadMatrixFromFile(filename, &matrix, &rows, &cols);  // Carrega a matriz do ficheiro
                printMatrix(matrix, rows, cols);  // Imprime a matriz
                for (int i = 0; i < rows; i++) {
                    free(matrix[i]);  // Liberta a memória da matriz
                }
                free(matrix);
                break;
            case 2:
                loadMatrixFromFile(filename, &matrix, &rows, &cols);  // Carrega a matriz do ficheiro
                graph = convertMatrixToGraph(matrix, rows, cols);  // Converte a matriz para um grafo
                printGraph(graph, rows, cols);  // Imprime os vértices do grafo
                for (int i = 0; i < rows; i++) {
                    free(matrix[i]);  // Liberta a memória da matriz
                }
                free(matrix);
                break;
            case 3:
                setEdgeType();  // Define o tipo de aresta
                break;
            case 4:
                loadMatrixFromFile(filename, &matrix, &rows, &cols);  // Carrega a matriz do ficheiro
                graph = convertMatrixToGraph(matrix, rows, cols);  // Converte a matriz para um grafo
                printGraph(graph, rows, cols);  // Imprime os vértices do grafo
                int** adjacencyMatrix = createAdjacencyMatrix(graph, rows * cols + 1);  // Cria a matriz de adjacência
                printAdjacencyMatrix(adjacencyMatrix, rows * cols + 1);  // Imprime a matriz de adjacência
                for (int i = 0; i < rows; i++) {
                    free(matrix[i]);  // Liberta a memória da matriz
                }
                free(matrix);
                for (int i = 0; i < rows * cols + 1; i++) {
                    free(adjacencyMatrix[i]);  // Liberta a memória da matriz de adjacência
                }
                free(adjacencyMatrix);
                break;
            case 5:
                loadMatrixFromFile(filename, &matrix, &rows, &cols);  // Carrega a matriz do ficheiro
                printMatrix(matrix, rows, cols);  // Imprime a matriz
                printf("\n");
                graph = convertMatrixToGraph(matrix, rows, cols);  // Converte a matriz para um grafo
                printGraph(graph, rows, cols);  // Imprime os vértices do grafo
                printf("\n");
                calculateMaxSum(matrix, rows, cols);  // Calcula a maior soma possível
                for (int i = 0; i < rows; i++) {
                    free(matrix[i]);  // Liberta a memória da matriz
                }
                free(matrix);
                break;
            case 6:
                loadMatrixFromFile(filename, &matrix, &rows, &cols);  // Carrega a matriz do ficheiro
                graph = convertMatrixToGraph(matrix, rows, cols);  // Converte a matriz para um grafo
                printMatrix(matrix, rows, cols);  // Imprime a matriz
                printf("\n");
                printGraph(graph, rows, cols);  // Imprime os vértices do grafo
                int src, dest;
                printf("\nVertice de origem: ");
                scanf("%d", &src);
                printf("Vertice de destino: ");
                scanf("%d", &dest);
                dijkstra(graph, src, dest);  // Calcula o menor caminho usando o algoritmo de Dijkstra
                for (int i = 0; i < rows; i++) {
                    free(matrix[i]);  // Liberta a memória da matriz
                }
                free(matrix);
                break;
            case 0:
                if (graph) freeGraph(graph);  // Liberta a memória do grafo
                break;
            default:
                printf("Opcao invalida. Tente novamente.\n");
        }
    } while (choice != 0);

    return 0;
}