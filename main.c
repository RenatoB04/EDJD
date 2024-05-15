#include <stdio.h>
#include <stdlib.h>
#include <string.h>

typedef struct Vertex {
    int value;
    struct Edge *edges;
} Vertex;

typedef struct Edge {
    struct Vertex *destination;
    struct Edge *next;
} Edge;

typedef struct Graph {
    int numVertices;
    struct Vertex **vertices;
} Graph;

Vertex* createVertex(int value) {
    Vertex *vertex = (Vertex*)malloc(sizeof(Vertex));
    if (vertex == NULL) {
        printf("Erro ao alocar memória para o vertice\n");
        exit(EXIT_FAILURE);
    }
    vertex->value = value;
    vertex->edges = NULL;
    return vertex;
}

Graph* createGraph(int numVertices) {
    Graph *graph = (Graph*)malloc(sizeof(Graph));
    if (graph == NULL) {
        printf("Erro ao alocar memória para o grafo\n");
        exit(EXIT_FAILURE);
    }
    graph->numVertices = numVertices;
    graph->vertices = (Vertex**)malloc(numVertices * sizeof(Vertex*));
    if (graph->vertices == NULL) {
        printf("Erro ao alocar memória para os vertices do grafo\n");
        exit(EXIT_FAILURE);
    }
    for (int i = 0; i < numVertices; i++) {
        graph->vertices[i] = createVertex(i);
    }
    return graph;
}

void addEdge(Vertex *src, Vertex *dest) {
    Edge *newEdge = (Edge*)malloc(sizeof(Edge));
    if (newEdge == NULL) {
        printf("Erro ao alocar memória para a aresta\n");
        exit(EXIT_FAILURE);
    }
    newEdge->destination = dest;
    newEdge->next = src->edges;
    src->edges = newEdge;
}

void removeEdge(Vertex *src, Vertex *dest) {
    Edge *prev = NULL;
    Edge *current = src->edges;
    while (current != NULL && current->destination != dest) {
        prev = current;
        current = current->next;
    }
    if (current != NULL) {
        if (prev != NULL) {
            prev->next = current->next;
        } else {
            src->edges = current->next;
        }
        free(current);
    }
}

void freeGraph(Graph *graph) {
    for (int i = 0; i < graph->numVertices; i++) {
        Edge *edge = graph->vertices[i]->edges;
        while (edge != NULL) {
            Edge *temp = edge;
            edge = edge->next;
            free(temp);
        }
        free(graph->vertices[i]);
    }
    free(graph->vertices);
    free(graph);
}

void loadMatrixFromFile(const char *filename, Graph *graph) {
    FILE *file = fopen(filename, "r");
    if (file == NULL) {
        printf("Erro ao abrir o arquivo %s\n", filename);
        exit(EXIT_FAILURE);
    }
    char line[1000];
    int row = 0;
    while (fgets(line, sizeof(line), file) != NULL) {
        char *token = strtok(line, ";");
        int col = 0;
        while (token != NULL) {
            int value = atoi(token);
            if (value != 0) {
                addEdge(graph->vertices[row], graph->vertices[col]);
            }
            col++;
            token = strtok(NULL, ";");
        }
        row++;
    }
    fclose(file);
}

void printMatrixFromFile(const char *filename) {
    FILE *file = fopen(filename, "r");
    if (file == NULL) {
        printf("Erro ao abrir o arquivo %s\n", filename);
        exit(EXIT_FAILURE);
    }
    char line[1000];
    while (fgets(line, sizeof(line), file) != NULL) {
        printf("%s", line);
    }
    fclose(file);
}

void addVertex(Graph *graph) {
    graph->numVertices++;
    graph->vertices = realloc(graph->vertices, graph->numVertices * sizeof(Vertex*));
    if (graph->vertices == NULL) {
        printf("Erro ao realocar memória para os vertices do grafo\n");
        exit(EXIT_FAILURE);
    }
    graph->vertices[graph->numVertices - 1] = createVertex(graph->numVertices - 1);
}

void removeVertex(Graph *graph, int vertexIndex) {
    if (vertexIndex < 0 || vertexIndex >= graph->numVertices) {
        printf("indice de vertice inválido\n");
        return;
    }
    for (int i = 0; i < graph->numVertices; i++) {
        if (i != vertexIndex) {
            Edge *prev = NULL;
            Edge *current = graph->vertices[i]->edges;
            while (current != NULL) {
                if (current->destination == graph->vertices[vertexIndex]) {
                    if (prev != NULL) {
                        prev->next = current->next;
                    } else {
                        graph->vertices[i]->edges = current->next;
                    }
                    free(current);
                    break;
                }
                prev = current;
                current = current->next;
            }
        }
    }
    free(graph->vertices[vertexIndex]);
    for (int i = vertexIndex; i < graph->numVertices - 1; i++) {
        graph->vertices[i] = graph->vertices[i + 1];
    }
    graph->numVertices--;
    graph->vertices = realloc(graph->vertices, graph->numVertices * sizeof(Vertex*));
    if (graph->vertices == NULL && graph->numVertices > 0) {
        printf("Erro ao realocar memória para os vertices do grafo\n");
        exit(EXIT_FAILURE);
    }
}

void addEdgeBetweenVertices(Graph *graph, int srcIndex, int destIndex) {
    if (srcIndex < 0 || srcIndex >= graph->numVertices || destIndex < 0 || destIndex >= graph->numVertices) {
        printf("indices de vertice inválidos\n");
        return;
    }
    addEdge(graph->vertices[srcIndex], graph->vertices[destIndex]);
}

void removeEdgeBetweenVertices(Graph *graph, int srcIndex, int destIndex) {
    if (srcIndex < 0 || srcIndex >= graph->numVertices || destIndex < 0 || destIndex >= graph->numVertices) {
        printf("indices de vertice inválidos\n");
        return;
    }
    removeEdge(graph->vertices[srcIndex], graph->vertices[destIndex]);
}

// Implementação para as opções 6, 7 e 8

int main() {
    const char *filename = "matriz.txt";
    int numVertices = 5;
    Graph *graph = createGraph(numVertices);
    loadMatrixFromFile(filename, graph);
    
    int option;
    do {
        printf("\nMenu:\n");
        printf("1. Imprimir matriz do arquivo\n");
        printf("2. Adicionar vertice\n");
        printf("3. Remover vertice\n");
        printf("4. Adicionar aresta\n");
        printf("5. Remover aresta\n");
        printf("6. Procurar caminho\n");
        printf("7. Calcular soma dos valores dos vertices em um dado caminho\n");
        printf("8. Encontrar caminho com maior soma\n");
        printf("9. Sair\n");
        printf("Escolha uma opcao: ");
        scanf("%d", &option);
        
        switch(option) {
            case 1:
                printf("\nMatriz do arquivo:\n\n");
                printMatrixFromFile(filename);
                break;
            case 2:
                addVertex(graph);
                break;
            case 3:
                printf("Escreva o indice do vertice a ser removido: ");
                int vertexIndexToRemove;
                scanf("%d", &vertexIndexToRemove);
                removeVertex(graph, vertexIndexToRemove);
                break;
            case 4:
                printf("Escreva o indice do vertice de origem: ");
                int srcIndexToAddEdge;
                scanf("%d", &srcIndexToAddEdge);
                printf("Escreva o indice do vertice de destino: ");
                int destIndexToAddEdge;
                scanf("%d", &destIndexToAddEdge);
                addEdgeBetweenVertices(graph, srcIndexToAddEdge, destIndexToAddEdge);
                break;
            case 5:
                printf("Escreva o indice do vertice de origem: ");
                int srcIndexToRemoveEdge;
                scanf("%d", &srcIndexToRemoveEdge);
                printf("Escreva o indice do vertice de destino: ");
                int destIndexToRemoveEdge;
                scanf("%d", &destIndexToRemoveEdge);
                removeEdgeBetweenVertices(graph, srcIndexToRemoveEdge, destIndexToRemoveEdge);
                break;
            case 6:
                // Implementação para procurar caminho
                break;
            case 7:
                // Implementação para calcular soma dos valores dos vertices em um dado caminho
                break;
            case 8:
                // Implementação para encontrar caminho com maior soma
                break;
            case 9:
                printf("\nEncerrando o programa.\n");
                break;
            default:
                printf("\nOpção invalida. Por favor, escolha uma opcao valida.\n");
        }

        int c;
        while ((c = getchar()) != '\n' && c != EOF);

    } while(option != 9);

    freeGraph(graph);
    return 0;
}