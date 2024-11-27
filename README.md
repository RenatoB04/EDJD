# P01-DJPM

# Gestor de Livros

## Descrição
O Gestor de Livros será uma aplicação que permitirá aos utilizadores organizar e acompanhar a sua biblioteca pessoal. A app incluirá funcionalidades para registar livros lidos, em leitura ou na lista de desejos, além de procurar informações automaticamente utilizando a Google Books API. Os dados dos utilizadores e dos livros serão armazenados na cloud utilizando o Firebase, permitindo sincronização entre dispositivos.

## Funcionalidades Planeadas
- **Login e Registo**: Sistema de autenticação integrado com Firebase Authentication para gerir bibliotecas pessoais.
- **Gestão de Livros**:
  - Adicionar livros manualmente ou pesquisando na Google Books API.
  - Categorizar livros como "Lido", "Em Leitura" ou "Lista de Desejos".
- **Base de Dados na Cloud**: Utilização do Firebase Firestore para armazenar e sincronizar dados.
- **Visualizações**: Exibição e organização dos livros por categorias.
- **Funcionalidades Opcionais**:
  - **Scanner de Código de Barras**: Permitir aos utilizadores adicionar livros através da digitalização do ISBN utilizando a câmara.
  - **Notificações**: Enviar lembretes para continuar leituras ou outros avisos relevantes.

## Tecnologias e Arquitetura
- **Linguagem**: Kotlin.
- **Arquitetura**: MVVM (Model-View-ViewModel).
- **Base de Dados**: Firebase Firestore.
- **Google Books API**: Para procurar informações de livros.
- **Firebase Authentication**: Para login e registo de utilizadores.
- **ML Kit Barcode Scanning (opcional)**: Para digitalizar códigos de barras.

## Objetivo
O objetivo do projeto será criar uma aplicação funcional e prática que permita aos utilizadores organizar e acompanhar os seus livros, enquanto demonstramos domínio de conceitos como manipulação de APIs, utilização de bases de dados na cloud e implementação da arquitetura MVVM.
