# API_CEP
## Introdução

API_CEP é uma API ASP.NET Core que permite consultar endereços brasileiros por CEP (Código de Endereçamento Postal). Ao receber uma requisição GET /api/cep/{cep}, a API consulta o serviço público ViaCEP para obter os dados do endereço correspondente. Em seguida, salva (ou atualiza) esses dados em um banco de dados local usando Entity Framework Core. Assim, consultas futuras ao mesmo CEP podem retornar o resultado do banco, melhorando a performance. A API também oferece endpoints para listar todos os CEPs cadastrados (GET /api/cep), cadastrar manualmente (POST /api/cep), atualizar (PUT /api/cep/{cep}) e excluir (DELETE /api/cep/{cep}) registros de endereço. Todo o processo utiliza o cache de memória do ASP.NET Core para acelerar respostas repetidas.

## Tecnologias Utilizadas

.NET 8 (ASP.NET Core 8) – plataforma de desenvolvimento da API.

Entity Framework Core – ORM para acesso ao banco de dados.

xUnit – framework de testes unitários usado nos testes do projeto.

PostgreSQL via Npgsql – banco de dados relacional para persistência. (Por padrão o appsettings.json usa Postgres em localhost:5432, mas você pode trocar para outro banco alterando a string de conexão.)

Swagger – (opcional) para documentação dos endpoints (disponível ao rodar a API em ambiente de desenvolvimento).

## Como Rodar o Projeto Localmente com Docker

1. No terminal vá em API_CEP/API_CEP e então escreva:
```bash
docker compose up --build
```

2. Abra o navegador e cole:
```bash
http://localhost:8080/swagger
```

Caso decida sem docker!

1.Restaurar pacotes: No terminal, navegue até a pasta do projeto (onde está o .sln) e execute ```bash dotnet restore```. O comando restaura todas as dependências necessárias.

2.E então faça:

```bash
dotnet run
```

3. Abra a API aqui

```bash
http://localhost:5115/swagger
```

### Obs
Pré-requisitos: Instale o .NET 8 SDK, o Git e uma IDE como Visual Studio 2022 ou Visual Studio Code.

Clonar o repositório: Abra um terminal e execute git clone <URL-do-repositório> para copiar o projeto.

Configurar a string de conexão: (Opcional) Se necessário, ajuste a string de conexão no arquivo appsettings.json para apontar para seu banco de dados PostgreSQL ou outro. Por exemplo: "Host=localhost;Port=5432;Database=cepdb;Username=postgres;Password=senha".

## Testes

Este projeto inclui testes unitários escritos com xUnit. Para executá-los, siga:

No terminal, navegue até a pasta CEP_API/CEP_API.Tests.

Rode o comando:
```bash
dotnet test
```

Esse comando compila o projeto de testes e executa todos os testes, mostrando o resultado no console
learn.microsoft.com


Os testes cobrem os comportamentos principais da aplicação, como:

1. A busca de CEP no serviço ViaCEP e inserção no banco.

2. Tratamento de CEP inválido (retornando BadRequest).

3. Regras de cache do controlador (ex.: resposta Ok quando já existe em cache).

4. Os endpoints (controller) GetByCep, GetAll, verificando retornos 200, 400, 404 conforme cada cenário.

