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
        graph->vertices[i] = NULL;
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

int main() {
    const char *filename = "matriz.txt";
    int **matrix = NULL;
    int rows, cols;
    loadMatrixFromFile(filename, &matrix, &rows, &cols);
    printf("\nMatriz lida do arquivo:\n\n");
    for (int i = 0; i < rows; i++) {
        for (int j = 0; j < cols; j++) {
            printf("%d ", matrix[i][j]);
        }
        printf("\n");
    }
    for (int i = 0; i < rows; i++) {
        free(matrix[i]);
    }
    printf("\n");
    free(matrix);
    return 0;
}