# Documentação do Projeto - Aplicação de Gestão de Livros com Firebase e Google Books API

## Índice
1. [Estrutura do Projeto](#estrutura-do-projeto)
2. [Lista de Funcionalidades](#lista-de-funcionalidades)
3. [Desenhos, Esquemas e Protótipos](#desenhos-esquemas-e-protótipos)
4. [Modelo de Dados](#modelo-de-dados)
5. [Implementação do Projeto](#implementação-do-projeto)
6. [Tecnologias Utilizadas](#tecnologias-utilizadas)
7. [Dificuldades Encontradas](#dificuldades-encontradas)
8. [Conclusão](#conclusão)

---

## Estrutura do Projeto
A aplicação está organizada segundo o padrão de arquitetura MVVM (Model-View-ViewModel).  
Os ficheiros estão distribuídos em diferentes pacotes, refletindo a divisão por responsabilidades.

### Estrutura de Diretórios:
```
📁 P01-DJPM  
│   ├── 📁 activities
│   │   ├── AddBookActivity.kt          # Atividade para adicionar livros  
│   │   ├── BarcodeScannerActivity.kt   # Atividade para scanear códigos de barras  
│   │   ├── BookDetailsActivity.kt      # Atividade para exibir detalhes do livro  
│   │   ├── HomeActivity.kt             # Tela inicial da aplicação  
│   │   ├── LoginActivity.kt            # Atividade de login  
│   │   └── RegisterActivity.kt         # Atividade de registo de novos utilizadores  
│   ├── 📁 adapters 
│   │   └── BooksAdapter.kt             # Adaptador para a lista de livros  
│   ├── 📁 api  
│   │   ├── ApiClient.kt                # Cliente da API para comunicação com o servidor  
│   │   └── GoogleBooksApi.kt           # API para interação com o Google Books  
│   ├── 📁 models 
│   │   └── UserBookItem.kt             # Modelo de dados para livros do utilizador  
│   ├── 📁 resources 
│   │   ├── 📁 layout  
│   │   │   ├── activity_add_book.xml           # Layout para adicionar livro  
│   │   │   ├── activity_barcode_scanner.xml    # Layout para escaneamento de código de barras  
│   │   │   ├── activity_book_details.xml       # Layout para detalhes do livro  
│   │   │   ├── activity_home.xml              # Layout para a tela inicial  
│   │   │   ├── activity_login.xml             # Layout para login  
│   │   │   ├── activity_register.xml          # Layout para registo  
│   │   │   └── item_book.xml                  # Layout para item de livro na lista  
│   │   └── 📁 values
│   │       ├── colors.xml                    # Definição de cores  
│   │       ├── strings.xml                   # Definições de strings  
│   │       └── themes.xml                    # Definições de temas  
└── AndroidManifest.xml                       # Manifesto da aplicação Android  

```
---

## Lista de Funcionalidades
### Funcionalidades principais:
1. **Autenticação Firebase**  
   - Registo de novos utilizadores  
   - Login com validação de email e password  
   - Logout seguro  
   - Recuperação de password (através do Firebase)

2. **Gestão de Livros (Google Books API)**  
   - Pesquisa de livros pelo título ou autor  
   - Pesquisa de livros através de ISBN (scan de código de barras)  
   - Adicionar livros à coleção pessoal do utilizador  
   - Exibição de detalhes do livro, incluindo capa, descrição e autor  

3. **Persistência de Dados (Firebase Firestore)**  
   - Armazenamento de livros associados a cada utilizador  
   - Sincronização automática ao fazer login noutro dispositivo  
   - Exclusão e edição de livros guardados 

4. **Interface Gráfica Moderna e Minimalista**  
   - Design intuitivo com Material Design  
   - Layout responsivo para diferentes tamanhos de ecrã  

---

## Desenhos, Esquemas e Protótipos
A aplicação segue um design minimalista, com foco em usabilidade e simplicidade.  
Os layouts são desenhados utilizando ConstraintLayout, garantindo flexibilidade na disposição dos componentes.

Abaixo poderemos visualizar o fluxo de navegação na App

### 1. Abertura da Aplicação (Tela de Login)  
- O utilizador é recebido com a tela de **LoginActivity**.  
- Se já tiver uma conta:  
  - Insere email e password e clica em **Login**.  
  - Se as credenciais estiverem corretas, é redirecionado para a **HomeActivity**.  
- Se não tiver conta:  
  - Clica no link **"Não tem conta? Registe-se"**, que o leva para a **RegisterActivity**.  

---

### 2. Registo de Utilizador (Tela de Registo)  
- O utilizador preenche o formulário de registo (email e password) na **RegisterActivity**.  
- Ao clicar em **Registar**, o sistema cria a conta através do Firebase.  
- Após o registo bem-sucedido, o utilizador é redirecionado para a **HomeActivity** automaticamente.  

---

### 3. HomeActivity (Tela Principal)  
- A tela principal apresenta uma barra de pesquisa para encontrar livros manualmente ou por autor/título.  
- Duas opções principais:  
  - **Pesquisar manualmente**: O utilizador insere um título ou autor no campo de pesquisa.  
  - **Scanear código de barras (ISBN)**: Ao clicar no botão de scan, a aplicação abre a **BarcodeScannerActivity**.  

---

### 4. Escaneamento de Código de Barras  
- Na **BarcodeScannerActivity**, a câmara é ativada para ler códigos de barras de livros (ISBN).  
- Quando um ISBN é detetado:  
  - A aplicação pesquisa automaticamente o livro na Google Books API.  
  - Se o livro for encontrado, o utilizador é redirecionado para a **AddBookActivity** para visualizar o resultado.  

---

### 5. Adicionar Livro (AddBookActivity)  
- Os resultados da pesquisa são apresentados numa lista.  
- Ao clicar num livro, a aplicação abre a tela de detalhes (**BookDetailsActivity**).  
- O utilizador pode:  
  - **Adicionar o livro à biblioteca pessoal**.  
  - **Voltar para a HomeActivity** para continuar a pesquisa.  

---

### 6. Visualização de Detalhes (BookDetailsActivity)  
- O utilizador vê a capa, título, autor, descrição e ISBN do livro selecionado.  
- Pode escolher adicionar o livro à coleção, que é guardada na Firestore.  

---

### 7. Logout e Sincronização  
- Na **HomeActivity**, o utilizador pode fazer logout, voltando para a tela de login.  
- Ao fazer login noutro dispositivo, todos os livros previamente guardados aparecem sincronizados.  

```
[LoginActivity] 
      |
      |  (Se não tem conta)
      v
[RegisterActivity] -----> [HomeActivity]
      |                         |
      |                         |  
      |                         |---> [AddBookActivity] 
      |                         |         |
      |                         |         |----> [BookDetailsActivity]
      |                         |
      |                         |---> [BarcodeScannerActivity]
      |                                    |
      |                                    |----> [AddBookActivity]  
      |                                    |         |
      |                                    |         |----> [BookDetailsActivity]
      |                                    |
      |                                    |----> Voltar para HomeActivity
      |
      |  (Se já tem conta)
      v
[HomeActivity]
      |
      |---> Logout ----> [LoginActivity] 
```

---

## Modelo de Dados
Os dados são estruturados da seguinte forma:

### Firebase Firestore:
```json
{
  "users": {
    "user_id_1": {
      "books": [
        {
          "title": "Livro Exemplo",
          "author": "Autor Exemplo",
          "isbn": "9781234567897",
          "thumbnail": "url_da_imagem",
          "description": "Descrição do livro..."
        }
      ]
    }
  }
}
```

## Objetos no Código:

### UserBookItem.kt (Modelo de Livro)
```kotlin
data class UserBookItem(
    val title: String,
    val author: String,
    val isbn: String,
    val thumbnail: String,
    val description: String
)
```

## Implementação do Projeto

O projeto é implementado em Kotlin e segue uma estrutura modular, onde cada atividade tem uma responsabilidade específica.

### Principais Componentes:

- **LoginActivity**  
  Permite a autenticação do utilizador.  
  Se a autenticação for bem-sucedida, redireciona para a HomeActivity.

- **AddBookActivity**  
  Permite pesquisar livros manualmente ou através de código de barras (ISBN).

- **BarcodeScannerActivity**  
  Utiliza a câmara para ler códigos de barras (ISBN) de livros.

- **BookDetailsActivity**  
  Exibe informações detalhadas do livro selecionado.

---

## Tecnologias Utilizadas

- **Linguagem de Programação**:  
  Kotlin (Android SDK)

- **Frameworks e Bibliotecas**:  
  - Firebase (Auth, Firestore)  
  - Google Books API (Retrofit)  
  - ML Kit (Barcode Scanning)  
  - Glide (Carregamento de imagens)  
  - CameraX (Utilização da câmara)  
  - ConstraintLayout (UI flexível e responsiva)

- **Gerenciador de Dependências**:  
  Gradle (Kotlin DSL)

---

## Dificuldades Encontradas

- **Scanner de Código de Barras**  
  Inicialmente, o scanner de ISBN detetava múltiplas leituras, causando repetição de buscas.  
  A solução implementada foi evitar chamadas repetidas, fechando o ImageProxy assim que um ISBN válido fosse encontrado.

- **Pesquisa por ISBN**  
  A API do Google Books nem sempre retorna resultados ao usar "isbn:978...".  
  A correção passou por enviar diretamente o número do ISBN sem prefixos.

- **Interface e Design**  
  A implementação de um design moderno causou erros de compilação devido a incompatibilidade com componentes antigos.  
  Foi necessário garantir a compatibilidade com bibliotecas do Material Design 3.

---

## Conclusão

A aplicação de gestão de livros proporciona uma experiência intuitiva e moderna para utilizadores que pretendem guardar e consultar livros através do Google Books API.  
A integração com Firebase oferece uma forma segura de autenticação e armazenamento persistente.

O projeto reflete um equilíbrio entre simplicidade de uso e funcionalidades avançadas, com potencial para futuras melhorias, como integração com redes sociais para partilha de livros e recomendações personalizadas.
