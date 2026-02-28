# 💰 ControleFinanceiro REST API

Sistema de gestão financeira desenvolvido como desafio técnico.

A aplicação permite controle de receitas e despesas por usuário, categorização, vinculação a pessoas, geração de dashboard com métricas consolidadas e autenticação segura via JWT.

---

# 🏗 Arquitetura

O projeto foi desenvolvido utilizando **arquitetura em camadas**, separando responsabilidades de forma clara e escalável.

API (Controllers)  
↓  
BLL (Regras de Negócio)  
↓  
DAL (Acesso a Dados)  
↓  
DAO (Entidades / EF Core)  
↓  
PostgreSQL  

## Camadas

**API**  
Responsável por expor endpoints REST e controlar autenticação/autorização.

**BLL (Business Logic Layer)**  
Contém todas as regras de negócio, validações e controle de domínio.

**DAL (Data Access Layer)**  
Abstrai o acesso ao banco de dados e centraliza operações CRUD.

**DAO**  
Entidades mapeadas pelo Entity Framework Core.

**DTO**  
Objetos de transferência de dados, evitando exposição direta das entidades.

**Utils**  
Componentes de autenticação, criptografia e contexto de usuário.

Essa separação garante:

- Baixo acoplamento
- Alta coesão
- Facilidade de manutenção
- Escalabilidade futura
- Clareza arquitetural

---

# 🛠 Tecnologias Utilizadas

- .NET 8
- ASP.NET Core Web API
- Entity Framework Core
- PostgreSQL
- ASP.NET Identity
- JWT (JSON Web Token)
- AutoMapper
- Swagger / OpenAPI

---

# 🔐 Autenticação e Segurança

A autenticação é feita utilizando **ASP.NET Identity + JWT**.

## Fluxo de autenticação

1. Usuário realiza login.
2. Credenciais são validadas pelo Identity.
3. Um token JWT é gerado contendo:
   - Id do usuário
   - Nome
   - Role
4. O token deve ser enviado no header das requisições:


Authorization: Bearer {token}


## Segurança implementada

- Isolamento total de dados por usuário
- Controle de acesso por role (Administrador x Usuário)
- Validação completa do token (issuer, audience, assinatura e expiração)
- Reset de senha via token seguro
- Separação entre base de autenticação (Identity) e base de domínio (Usuario)

---

# 📊 Funcionalidades

## 👤 Usuários

- Cadastro de usuário
- Atualização de dados
- Reset de senha
- Exclusão
- Paginação com filtro
- Controle de visualização:
  - Administrador visualiza todos
  - Usuário comum visualiza apenas o próprio registro

---

## 👥 Pessoas

- Cadastro de pessoas vinculadas ao usuário
- Atualização
- Exclusão
- Paginação
- Isolamento por usuário

### Regra de negócio

- Menores de idade não podem registrar receitas.

---

## 🗂 Categorias

- Cadastro de categorias
- Definição de finalidade (Receita ou Despesa)
- Não permite exclusão se houver transações vinculadas
- Isolamento por usuário

---

## 💳 Transações

- Cadastro de receita ou despesa
- Vinculação obrigatória a:
  - Pessoa
  - Categoria
- Validações:
  - Valor deve ser maior que zero
  - Pessoa deve pertencer ao usuário
  - Categoria deve pertencer ao usuário
  - Data não pode ser futura
  - Tipo da transação é definido pela categoria
- Paginação
- Atualização
- Exclusão

---

## 📈 Dashboard

### Resumo

- Total de receitas
- Total de despesas
- Saldo consolidado

### Gastos por dia

- Agrupamento por data
- Consulta por período

### Gastos por pessoa

- Agrupamento por pessoa
- Ordenado por maior valor

### Totais por Pessoa

- Receita total
- Despesa total
- Saldo individual
- Total geral consolidado

### Totais por Categoria

- Receita total
- Despesa total
- Saldo por categoria
- Total geral consolidado

---

# 📂 Estrutura do Projeto


ControleFinanceiro_REST.API
ControleFinanceiro_REST.BLL
ControleFinanceiro_REST.DAL
ControleFinanceiro_REST.DAO
ControleFinanceiro_REST.DTO


---

# ▶️ Como Executar o Projeto

## 1️⃣ Clonar repositório


git clone https://github.com/seu-repositorio.git


## 2️⃣ Configurar banco de dados

Editar o `appsettings.json`:

```json
"ConnectionStrings": {
  "DefaultConnection": "Host=localhost;Port=5432;Database=ControleFinanceiro;Username=postgres;Password=senha"
}
3️⃣ Aplicar migrations
dotnet ef database update
4️⃣ Executar aplicação
dotnet run

Swagger estará disponível em:

https://localhost:{porta}/swagger
📡 Principais Endpoints
Autenticação

POST /api/Autenticacao/Login

Usuários

GET /api/Usuarios/Get
POST /api/Usuarios/Cadastrar
PUT /api/Usuarios/Editar/{id}
DELETE /api/Usuarios/Deletar/{id}

Pessoas

GET /api/Pessoas/Get
POST /api/Pessoas/Cadastrar

Categorias

GET /api/Categorias/Get
POST /api/Categorias/Cadastrar

Transações

GET /api/Transacoes/Get
POST /api/Transacoes/Cadastrar

Dashboard

GET /api/Dashboard/Resumo
GET /api/Dashboard/GastosPorDia
GET /api/Dashboard/GastosPorPessoa
GET /api/Dashboard/TotaisPorPessoa
GET /api/Dashboard/TotaisPorCategoria

💡 Decisões Arquiteturais

Separação clara entre autenticação (Identity) e domínio (Usuario)

Paginação eficiente via LINQ + ProjectTo

Validações de negócio centralizadas na BLL

DAL genérico reutilizável

DTO para evitar exposição direta de entidades

Claims-based authorization

Isolamento total por usuário autenticado

🚀 Diferenciais Técnicos

Controle completo de acesso por role

Integração estruturada com ASP.NET Identity

JWT com validação completa

Regras de domínio reais

Dashboard com agregações performáticas no banco

Código organizado e preparado para crescimento

📌 Considerações Finais

O sistema foi desenvolvido com foco em:

Clareza arquitetural

Segurança

Boas práticas

Separação de responsabilidades

Escalabilidade futura