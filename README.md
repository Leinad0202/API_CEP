#API_CEP
Introdução

API_CEP é uma API ASP.NET Core que permite consultar endereços brasileiros por CEP (Código de Endereçamento Postal). Ao receber uma requisição GET /api/cep/{cep}, a API consulta o serviço público ViaCEP para obter os dados do endereço correspondente. Em seguida, salva (ou atualiza) esses dados em um banco de dados local usando Entity Framework Core. Assim, consultas futuras ao mesmo CEP podem retornar o resultado do banco, melhorando a performance. A API também oferece endpoints para listar todos os CEPs cadastrados (GET /api/cep), cadastrar manualmente (POST /api/cep), atualizar (PUT /api/cep/{cep}) e excluir (DELETE /api/cep/{cep}) registros de endereço. Todo o processo utiliza o cache de memória do ASP.NET Core para acelerar respostas repetidas.

Tecnologias Utilizadas

.NET 8 (ASP.NET Core 8) – plataforma de desenvolvimento da API.

Entity Framework Core – ORM para acesso ao banco de dados.

xUnit – framework de testes unitários usado nos testes do projeto.

PostgreSQL via Npgsql – banco de dados relacional para persistência. (Por padrão o appsettings.json usa Postgres em localhost:5432, mas você pode trocar para outro banco alterando a string de conexão.)

Swagger – (opcional) para documentação dos endpoints (disponível ao rodar a API em ambiente de desenvolvimento).
