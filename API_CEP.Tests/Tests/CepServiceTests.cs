using API_CEP.Data;
using API_CEP.Models;
using API_CEP.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace API_CEP.Tests.Tests
{
    public class CepServiceTests
    {
        // fake handler simples para retornar JSON controlado
        private class FakeHttpHandler : HttpMessageHandler
        {
            public string ResponseJson { get; set; } = "{}";
            protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            {
                var resp = new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent(ResponseJson)
                };
                return Task.FromResult(resp);
            }
        }

        private (AppDbContext context, CepService service, FakeHttpHandler handler) BuildService(string responseJson)
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: "cep_test_db_" + Guid.NewGuid())
                .Options;
            var context = new AppDbContext(options);

            var handler = new FakeHttpHandler { ResponseJson = responseJson };
            var httpClient = new HttpClient(handler);

            var config = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string?>
                {
                    { "ViaCep:BaseUrl", "https://viacep.com.br/ws/" }
                })
                .Build();

            var service = new CepService(context, httpClient, config);

            return (context, service, handler);
        }

        [Fact]
        public async Task BuscarCepAsync_QuandoNaoExisteNoBanco_DeveConsultarViaCepESalvar()
        {
            // arrange: resposta mockada do ViaCEP
            var viaCepJson = """
            {
              "cep":"01001-000",
              "logradouro":"Praça da Sé",
              "complemento":"",
              "bairro":"Sé",
              "localidade":"São Paulo",
              "uf":"SP",
              "ibge":"3550308",
              "gia":"",
              "ddd":"11",
              "siafi":"7107"
            }
            """;

            var (context, service, _) = BuildService(viaCepJson);

            // act
            var result = await service.BuscarCepAsync("01001000");

            // assert
            Assert.NotNull(result);
            Assert.Equal("01001000", result.Cep.Replace("-", ""));
            Assert.Equal("Praça da Sé", result.Logradouro);

            // confere que foi salvo no DB
            var saved = await context.CepEnderecos.FindAsync("01001000");
            Assert.NotNull(saved);
            Assert.Equal("Praça da Sé", saved.Logradouro);
        }

        [Fact]
        public async Task BuscarCepAsync_CepInvalido_DeveLancarArgumentException()
        {
            var (_, service, _) = BuildService("{}");
            await Assert.ThrowsAsync<ArgumentException>(() => service.BuscarCepAsync("123"));
        }

        [Fact]
        public async Task BuscarCepAsync_ViaCepRetornaErro_DeveLancarKeyNotFoundException()
        {
            // CEP válido, mas o ViaCEP retorna {"erro": true} simulando "não encontrado"
            var viaCepJson = "{\"erro\": true}";

            // Construindo service com o handler fake
            var (_, service, _) = BuildService(viaCepJson);

            // O CEP precisa ser exatamente 8 dígitos numéricos
            var cepInexistente = "87654321";

            // Espera KeyNotFoundException
            var exception = await Assert.ThrowsAsync<KeyNotFoundException>(
                () => service.BuscarCepAsync(cepInexistente)
            );

            // Opcional: checar a mensagem
            Assert.Equal("CEP não encontrado no ViaCEP.", exception.Message);
        }


    }
}
