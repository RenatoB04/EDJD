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
        printf("Erro ao alocar memória para o vértice\n");
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
        printf("Erro ao alocar memória para os vértices do grafo\n");
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
        printf("7. Calcular soma dos valores dos vertices num dado caminho\n");
        printf("8. Encontrar caminho com maior soma\n");
        printf("9. Sair\n");
        printf("Escolha uma opção: ");
        scanf("%d", &option);
        
        switch(option) {
            case 1:
                printf("\nMatriz do arquivo:\n\n");
                printMatrixFromFile(filename);
                printf("\n");
                break;
            case 2:
                // Implementação para adicionar vértice
                break;
            case 3:
                // Implementação para remover vértice
                break;
            case 4:
                // Implementação para adicionar aresta
                break;
            case 5:
                // Implementação para remover aresta
                break;
            case 6:
                // Implementação para procurar caminho
                break;
            case 7:
                // Implementação para calcular soma dos valores dos vértices em um dado caminho
                break;
            case 8:
                // Implementação para encontrar caminho com maior soma
                break;
            case 9:
                printf("\nEncerrando o programa.\n");
                break;
            default:
                printf("\nOpção inválida. Por favor, escolha uma opção válida.\n");
        }
    } while(option != 9);

    freeGraph(graph);
    return 0;
}