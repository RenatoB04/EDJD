# Projeto Gestão de Clientes

Realizado por:
- Paulo Bastos 27945

Este projeto foi desenvolvido para UC de Sistemas de Armazenamento de Dados e inclui a introdução, atualização, pesquisa e eliminação de informações, com a utilização de **SQL Server** e **C#** utilizando a biblioteca **Windows Forms** para a interface gráfica. A aplicação permite manipular dados de clientes, contactos e matrículas.

---

## Funcionalidades Implementadas

1. **Introdução de Dados:**
   - Permite a inserção de um cliente e seus contactos (email e telefones).
   - Os dados são armazenados nas tabelas **cliente** e **contacto**.

2. **Alteração de Dados:**
   - Permite editar informações de um cliente, como nome, código postal, email e telefones.

3. **Eliminação de Dados:**
   - Permite eliminar um cliente e todos os seus dados relacionados (contactos e matrículas).

4. **Pesquisa de Clientes:**
   - Pesquisa de um cliente pelo NIF e exibição de todos os dados do cliente, incluindo telefones, email, código postal e matrículas associadas.

5. **Consulta com várias tabelas:**
   - Uma **Stored Procedure** é usada para realizar consultas complexas envolvendo múltiplas tabelas (cliente, contacto, stock).

6. **Stored Procedure:**
   - A Stored Procedure **PesquisarCliente** foi implementada para pesquisa de clientes, agregando vários dados da BD.

---

## Estrutura do Projeto

- **Windows Forms:** A aplicação foi construída utilizando a biblioteca **Windows Forms**, com uma interface gráfica simples e funcional.
- **Form1.cs:** Formulário principal da aplicação onde são exibidos os dados dos clientes, com a opção de adicionar, editar ou eliminar clientes.
- **FormInserir.cs:** Formulário para inserir um novo cliente e os seus contactos.
- **FormEditar.cs:** Formulário para editar os dados de um cliente já existente.
- **Stored Procedure:** Implementada para pesquisa de dados complexos e realização de consultas em várias tabelas.

---

## Como Usar

### 1. **Configuração da Base de Dados**

No ficheiro **BD.sql** está toda a estrutura da base de dados necessária para o funcionamento da aplicação. Para usá-la devemos:

1. Abrir o **SQL Server Management Studio (SSMS)**.
2. Criar um novo banco de dados ou usar um já existente.
3. Copiar o conteúdo do ficheiro **BD.sql** para a janela de consulta no SSMS ou abrir o mesmo.
4. Executar o script para criar as tabelas, todos os dados de teste e a Stored Procedure.

### 2. **Configuração da Aplicação**

1. Abrir o projeto no **Visual Studio** ou **VS Code**.
2. Certificar que a biblioteca **Microsoft.Data.SqlClient** está instalada. Caso não tenha, pode instalar com o comando:
```sql
dotnet add package Microsoft.Data.SqlClient
```
3. Atualizar a string de conexão no código para corresponder ao seu ambiente:
```
string connectionString = "Server=LEGION;Database=P01-SAD;Trusted_Connection=True;";
```
Altere o `Server` e `Database` conforme necessário.

### 3. **Usar a Aplicação**

1. Quando a aplicação for iniciada, o **Form1** exibirá os clientes existentes num `DataGridView`.
2. **Adicionar Cliente:** Clicar no botão "Novo Cliente" irá o formulário de inserção de novos clientes e os seus respetivos contactos.
3. **Editar Cliente:** Selecionar um cliente da tabela e clicar em "Editar Cliente" para irá modificar todas essas informações.
4. **Eliminar Cliente:** Selecionar um cliente e clicar em "Eliminar Cliente" irá remover o cliente e todos os seus dados associados.
5. **Pesquisar Cliente:** Inserir o NIF de um cliente na caixa de pesquisa e clicar em "Pesquisar Cliente" irá exibir os detalhes completos do cliente, incluindo os contactos e as matrículas.

---

## Considerações Finais

Este projeto é uma boa base para gestão de dados de clientes com **SQL Server** e **C#** utilizando **Windows Forms** para a interface gráfica. A aplicação foi projetada para ser simples e funcional, com a utilização de **Stored Procedures** para melhorar a performance e a segurança nas operações de consulta.