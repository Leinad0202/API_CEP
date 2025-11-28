using API_CEP.Controllers;
using API_CEP.Data;
using API_CEP.Models;
using API_CEP.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Moq;
using Xunit;

namespace API_CEP.Tests.Tests
{
    public class CepControllerTests
    {
        private AppDbContext BuildContext()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase("controller_test_db_" + Guid.NewGuid())
                .Options;
            return new AppDbContext(options);
        }

        [Fact]
        public async Task GetByCep_RetornaOk_QuandoExisteNoCache()
        {
            var ctx = BuildContext();
            var cache = new MemoryCache(new MemoryCacheOptions());
            var cep = new CepEndereco { Cep = "01001000", Logradouro = "Rua A", AtualizadoEm = DateTime.UtcNow };
            cache.Set("cep_01001000", cep);

            var mockService = new Mock<ICepService>();
            var controller = new CepController(ctx, mockService.Object, cache);

            var response = await controller.GetByCep("01001000");
            var ok = Assert.IsType<OkObjectResult>(response.Result);
            var value = Assert.IsType<CepEndereco>(ok.Value);
            Assert.Equal("01001000", value.Cep);
        }

        [Fact]
        public async Task GetByCep_RetornaBadRequest_QuandoCepInvalido()
        {
            var ctx = BuildContext();
            var cache = new MemoryCache(new MemoryCacheOptions());
            var mockService = new Mock<ICepService>();
            var controller = new CepController(ctx, mockService.Object, cache);

            var response = await controller.GetByCep("123"); // inválido
            Assert.IsType<BadRequestObjectResult>(response.Result);
        }

        [Fact]
        public async Task GetAll_RetornaLista()
        {
            var ctx = BuildContext();

            // --- INÍCIO DA CORREÇÃO ---
            var cep1 = new CepEndereco
            {
                Cep = "01001000",
                Logradouro = "Rua A",
                AtualizadoEm = DateTime.UtcNow,
                Bairro = "Bairro A",
                Localidade = "Cidade A",
                Uf = "SA",
                // Valores de teste para as propriedades obrigatórias:
                Complemento = string.Empty,
                Ibge = "1111111",
                Gia = "1111",
                Ddd = "11",
                Siafi = "1111"
            };

            var cep2 = new CepEndereco
            {
                Cep = "02002000",
                Logradouro = "Rua B",
                AtualizadoEm = DateTime.UtcNow,
                Bairro = "Bairro B",
                Localidade = "Cidade B",
                Uf = "SB",
                // Valores de teste para as propriedades obrigatórias:
                Complemento = "apto 101",
                Ibge = "2222222",
                Gia = "2222",
                Ddd = "22",
                Siafi = "2222"
            };

            ctx.CepEnderecos.Add(cep1);
            ctx.CepEnderecos.Add(cep2);
            await ctx.SaveChangesAsync();
            // --- FIM DA CORREÇÃO ---

            var cache = new MemoryCache(new MemoryCacheOptions());
            var mockService = new Mock<ICepService>();
            var controller = new CepController(ctx, mockService.Object, cache);

            var resp = await controller.GetAll();
            var ok = Assert.IsType<OkObjectResult>(resp.Result);
            var list = Assert.IsAssignableFrom<IEnumerable<CepEndereco>>(ok.Value);
            Assert.True(list.Count() >= 2);
        }
    }
}