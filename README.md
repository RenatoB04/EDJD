# Estruturas de Dados Avançadas

Este projeto foi realizado por:
- Paulo Bastos / 27945
- Bruno Mesquita / 27947
- Bento Simões / 27914
- Ricardo Miranda / 27927

# Introdução ao Projeto de Grafos

Este projeto tem como objetivo a implementação e manipulação de grafos utilizando a linguagem de programação C. Grafos são estruturas de dados poderosas que modelam relações entre objetos. Eles são amplamente utilizados em diversas áreas, incluindo ciência da computação, matemática, engenharia e redes sociais, para representar e resolver problemas complexos.

## Estrutura do Projeto

O projeto está estruturado em torno de várias funções essenciais que permitem criar, manipular e analisar grafos. Utilizamos uma representação de listas de adjacência para armazenar os grafos, o que proporciona eficiência em termos de espaço e flexibilidade na manipulação de arestas e vértices.

### Objetivos Específicos

1. **Representação de Grafos:** Implementar uma estrutura de dados que utilize listas de adjacência para representar grafos.
2. **Manipulação de Grafos:** Desenvolver funções para adicionar arestas, criar nós e liberar memória alocada para grafos.
3. **Conversão e Impressão:** Implementar funções para converter matrizes de adjacência em grafos e imprimir suas representações de forma clara e legível.
4. **Algoritmos de Grafos:** Implementar o algoritmo de Dijkstra para encontrar o caminho mais curto entre dois vértices.
5. **Interface Interativa:** Criar um menu interativo que permita ao utilizador realizar várias operações relacionadas com grafos e matrizes.

## Motivação

A manipulação e análise de grafos são fundamentais para resolver uma vasta gama de problemas práticos. Ao implementar este projeto, adquirimos uma compreensão mais profunda dos conceitos teóricos de grafos e desenvolvemos habilidades práticas na implementação de algoritmos e estruturas de dados eficientes.

## Estrutura do Documento

1. [Introdução](#introdução-ao-projeto-de-grafos)
2. [Funções](#principais-funções)
3. [Funcionamento do Programa](#funcionamento-do-programa)
4. [Possíveis Melhorias Futuras](#possíveis-melhorias-futuras)
5. [Conclusão](#conclusão)

Esperamos que esta documentação sirva como uma referência útil para aqueles que desejam entender e expandir o trabalho realizado neste projeto.

# Principais Funções

## struct Node

Neste projeto, utilizamos uma estrutura de dados para representar grafos utilizando listas de adjacência. Cada nó do grafo é representado pela estrutura `Node`, que contém o índice do vértice, o peso da aresta e um ponteiro para o próximo nó na lista de adjacência. A estrutura `Graph` armazena o número de vértices e um array de ponteiros para as listas de adjacência de cada vértice.

## struct Graph

Neste projeto, utilizamos uma estrutura de dados para representar grafos utilizando listas de adjacência. A estrutura `Graph` é usada para armazenar o número de vértices e um array de ponteiros para as listas de adjacência de cada vértice. Cada lista de adjacência é composta por nós representados pela estrutura `Node`, que contém o índice do vértice, o peso da aresta e um ponteiro para o próximo nó na lista `Node`, que contém o índice do vértice, o peso da aresta e um ponteiro para o próximo nó na lista.

## EdgeType

Neste projeto, utilizamos um tipo enumerado para representar os diferentes tipos de arestas que podem existir num grafo. O tipo enumerado `EdgeType` define três tipos de arestas: horizontal, vertical e horizontal-vertical. Esta categorização é utilizada onde a direção das arestas é importante no caso do trabalho proposto em grafos em que utilizamos as arestas de diferentes maneiras para realizar cálculos etc.

## createNode

A função `createNode` é responsável por criar e inicializar um novo nó na lista de adjacência de um grafo. Ela aloca memória para um nó, inicializa os campos `vertex` e `weight` com os valores fornecidos e define o ponteiro `next` como `NULL`. Esta função é essencial para a construção e manipulação de grafos, sendo utilizada na adição de arestas entre vértices.

## createGraph

A função `createGraph` é responsável por criar e inicializar um grafo com um número especificado de vértices. Ela aloca memória para a estrutura `Graph`, define o número de vértices, aloca memória para o array de listas de adjacência e inicializa cada lista como vazia (`NULL`). Esta função é essencial para preparar a estrutura do grafo antes da adição de arestas e manipulação subsequente dos vértices.

## addEdge

A função `addEdge` é responsável por adicionar uma aresta a um grafo. Ela cria um nó representando a aresta com o vértice de destino e o peso especificado, e insere este nó no início da lista de adjacência do vértice de origem. Esta função permite a construção dinâmica de grafos, adicionando conexões entre vértices de forma eficiente.

## freeGraph

A função `freeGraph` é responsável por libertar a memória alocada para um grafo. A função percorre cada lista de adjacência de todos os vértices, libertando a memória de cada nó individualmente. Após libertar todas as listas de adjacência, a função liberta a memória alocada para o array de listas de adjacência e, finalmente, liberta a estrutura do grafo. Esta função é essencial para evitar fugas de memória em programas que utilizam grafos.

## createAdjacencyMatrix

A função `createAdjacencyMatrix` é responsável por criar uma matriz de adjacência a partir de um grafo representado por listas de adjacência. A função aloca memória para a matriz, inicializa todas as entradas com zero e preenche a matriz com os pesos das arestas existentes no grafo. Esta função permite uma representação alternativa do grafo, utilizada no algoritmo de grafos que operam mais eficientemente nas matrizes de adjacência.

## printAdjacencyMatrix

A função `printAdjacencyMatrix` é responsável por imprimir a matriz de adjacência de um grafo de forma tabular. Ela exibe os índices dos vértices e os pesos das arestas, facilitando a visualização das conexões no grafo. A função percorre a matriz de adjacência e formata a saída para melhor legibilidade, mostrando claramente as relações entre os vértices. Esta função é útil para a análise visual e verificação da estrutura do grafo.

## loadMatrixFromFile

A função `loadMatrixFromFile` é responsável por carregar a matriz a partir do ficheiro. Ela lê o conteúdo do ficheiro linha por linha, divide cada linha em tokens separados por ";", converte esses tokens para inteiros e armazena-os numa matriz dinâmica. A função também define o número de linhas e colunas da matriz. Esta função é essencial para carregar dados estruturados do ficheiro para uso no programa.

## printMatrix

A função `printMatrix` é responsável por imprimir a matriz de inteiros de forma tabular. Ela percorre a matriz linha por linha e coluna por coluna, exibindo cada elemento com uma formatação adequada para garantir um alinhamento claro e legível. Esta função é útil para a visualização e verificação dos dados armazenados na matriz, especialmente após o carregamento de dados a partir de um ficheiro ou durante a depuração do programa.

## setEdgeType

A função `setEdgeType` é responsável por definir o tipo de aresta com base na escolha do utilizador. Ela apresenta um menu com três opções: arestas horizontais, verticais e horizontais e verticais. A função lê a escolha do utilizador e configura a variável `edgeType` de acordo com a opção selecionada. Se a escolha for inválida, a função exibe uma mensagem de erro e solicita ao utilizador que tente novamente. Esta função é útil para personalizar o comportamento do grafo com base nas preferências do utilizador.

## convertMatrixToGraph

A função `convertMatrixToGraph` é responsável por converter uma matriz de adjacência numa estrutura de grafo. Ela cria um grafo com vértices correspondentes aos elementos da matriz e adiciona arestas com base nos valores da matriz e no tipo de aresta especificado (horizontal, vertical ou ambos). Esta função permite a criação dinâmica de grafos a partir de matrizes, facilitando a manipulação e análise dos dados representados na matriz.

## printGraph

A função `printGraph` é responsável por imprimir os vértices de um grafo de forma organizada, respeitando a disposição original da matriz de onde o grafo foi derivado. A função percorre todos os vértices do grafo e os imprime em linhas e colunas, facilitando a visualização e a verificação da estrutura do grafo. Esta função é útil para a análise visual da disposição dos vértices em grafos derivado da matriz.

## maxSum

A variável `maxSum` é inicializada para armazenar a maior soma encontrada entre as somas das linhas e colunas. A função imprime um cabeçalho para indicar que a seguir serão exibidas as somas das linhas e colunas:

- Se o tipo de aresta for HORIZONTAL ou HORIZONTAL_VERTICAL, a função calcula a soma de cada linha.
    - Para cada linha, a soma dos elementos é calculada e armazenada em `rowSum`.
    - A soma da linha é impressa.
    - Se a soma da linha for maior que `maxSum`, `maxSum` é atualizado.

- Se o tipo de aresta for VERTICAL ou HORIZONTAL_VERTICAL, a função calcula a soma de cada coluna.
    - Para cada coluna, a soma dos elementos é calculada e armazenada em `colSum`.
    - A soma da coluna é impressa.
    - Se a soma da coluna for maior que `maxSum`, `maxSum` é atualizado.

A função `calculateMaxSum` é responsável por calcular e imprimir as somas das linhas e colunas de uma matriz, considerando o tipo de aresta especificado (horizontal, vertical ou ambos). A função percorre a matriz para calcular as somas das linhas e/ou colunas e imprime essas somas, além de determinar e imprimir a maior soma encontrada. Esta função é útil para analisar a distribuição dos valores na matriz e identificar a linha ou coluna com a maior soma.

## minDistance

A função `minDistance` é utilizada para encontrar o vértice com a menor distância que ainda não foi incluído no conjunto de vértices processados (`sptSet`).

1. `min` é inicializado com `INT_MAX`, que representa o valor máximo de um inteiro. Este valor é utilizado para encontrar a menor distância.
    - `min_index` será utilizado para armazenar o índice do vértice com a menor distância.
2. O loop percorre todos os vértices (v de 0 até `numVertices - 1`).
    - `if (sptSet[v] == 0 && dist[v] <= min)`: Se o vértice v não foi processado (`sptSet[v] == 0`) e a distância `dist[v]` é menor ou igual a `min`, então:
        - `min = dist[v]`: Atualiza `min` com a nova menor distância.
        - `min_index = v`: Atualiza `min_index` com o índice do vértice com a menor distância.

Em suma, a função `minDistance` é responsável por encontrar o vértice com a menor distância que ainda não foi processado no conjunto de vértices do grafo. Ela percorre todos os vértices

, verifica se cada vértice não foi processado e se a sua distância é a menor encontrada até o momento. Esta função é essencial no algoritmo de Dijkstra, permitindo a seleção do próximo vértice a ser processado na busca pelo caminho mais curto.

## dijkstra

A função `dijkstra` é utilizada para encontrar o caminho mais curto entre dois vértices num grafo, utilizando o algoritmo de Dijkstra. Esta função calcula a menor distância entre o vértice de origem (`src`) e o vértice de destino (`dest`), e também imprime o caminho encontrado.

1. **Inicialização de Variáveis e Alocação de Memória:**
    - `numVertices` armazena o número de vértices no grafo.
    - `dist` armazena a distância mínima encontrada de `src` para cada vértice.
    - `sptSet` rastreia os vértices incluídos no conjunto de caminho mais curto.
    - `parent` armazena o predecessor de cada vértice no caminho mais curto.

2. **Inicialização das Estruturas de Dados:**
    - Todas as distâncias são inicializadas como `INT_MAX`.
    - `sptSet` é inicializado com 0 (todos os vértices não processados).
    - `parent` é inicializado com -1 (nenhum predecessor).
    - A distância para o vértice de origem (`src`) é inicializada como 0.

3. **Cálculo das Menores Distâncias:**
    - Para cada vértice, encontra o vértice `u` com a menor distância que ainda não foi processado utilizando a função `minDistance`.
    - Marca o vértice `u` como processado no `sptSet`.
    - Para cada vértice adjacente `v` de `u`, atualiza a distância se for menor que a distância atual.

4. **Verificação e Impressão do Resultado:**
    - Se a distância para o destino `dest` for `INT_MAX`, não existe caminho entre `src` e `dest`.
    - Caso contrário, imprime a menor distância e o caminho encontrado, percorrendo o array `parent` de `dest` para `src`.

Em resumo, a função `dijkstra` implementa o algoritmo de Dijkstra para encontrar o caminho mais curto entre dois vértices num grafo. A função inicializa as distâncias, processa os vértices para encontrar as menores distâncias e, finalmente, imprime a menor distância e o caminho encontrado. A função utiliza estruturas auxiliares para rastrear as distâncias, os vértices processados e os predecessores no caminho mais curto, garantindo uma implementação eficiente e correta do algoritmo de Dijkstra.

## main

A função `main` implementa um menu interativo que permite ao utilizador realizar várias operações relacionadas com grafos e matrizes, como carregar uma matriz a partir de um ficheiro, imprimir a matriz, definir arestas, construir um grafo, calcular a maior soma de elementos em linhas ou colunas, e calcular o menor caminho entre dois vértices. O programa continua a executar até que o utilizador escolha a opção de sair, garantindo a libertação adequada da memória alocada em cada operação.

## Funcionamento do Programa
https://github.com/RenatoB04/P02-EDA/assets/47258416/038117cf-8a30-408d-9219-110c7283a347

### Possíveis Melhorias Futuras

- Otimizar as funções de manipulação de grafos para melhorar o desempenho, especialmente ao lidar com grafos grandes.
- Implementar a capacidade de guardar o estado atual do grafo e da matriz em ficheiros, permitindo ao utilizador retomar o trabalho em sessões futuras.
- Adicionar suporte para grafos direcionados, permitindo a definição de arestas com direções específicas e o cálculo de caminhos direcionados.

### Conclusão

A realização deste trabalho, em C, proporcionou-nos uma oportunidade valiosa para aprofundar os conhecimentos sobre a manipulação e análise de grafos e matrizes. A implementação de diversas operações sobre grafos, desde a leitura de matrizes a partir de ficheiros até a aplicação de algoritmos de caminhos mínimos, e ainda ficamos a compreender um pouco mais a cerca da complexidade e a utilidade dos grafos. As habilidades desenvolvidas com a realização do trabalho serão muito úteis em projetos futuros e aplicações práticas que envolvam análise de dados.
