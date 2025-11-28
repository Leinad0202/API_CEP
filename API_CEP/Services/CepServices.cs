using API_CEP.Data;
using API_CEP.Models;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace API_CEP.Services
{
    public class CepService : ICepService
    {
        private readonly AppDbContext _context;
        private readonly HttpClient _httpClient;
        private readonly string _baseUrl;

        public CepService(AppDbContext context, HttpClient httpClient, IConfiguration config)
        {
            _context = context;
            _httpClient = httpClient;
            _baseUrl = config["ViaCep:BaseUrl"] ?? "https://viacep.com.br/ws/";
        }

        public async Task<CepEndereco> BuscarCepAsync(string cep)
        {
            // Normalização inicial
            if (cep == null)
                throw new ArgumentException("CEP inválido.");

            cep = cep.Replace("-", "").Trim();

            // Validação básica do CEP
            if (cep.Length != 8 || !cep.All(char.IsDigit))
                throw new ArgumentException("CEP inválido. Deve conter 8 dígitos numéricos.");

            // Evita CEPs como 00000000 ou 11111111
            if (cep.All(c => c == cep[0]))
                throw new ArgumentException("CEP inválido.");

            // Busca no banco
            var enderecoBanco = await _context.CepEnderecos.FindAsync(cep);
            if (enderecoBanco != null)
                return enderecoBanco;

            // Montagem da URL
            var url = $"{_baseUrl}{cep}/json/";

            ViaCepResponse viaCepResponse;

            try
            {
                using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));

                var httpResponse = await _httpClient.GetAsync(url, cts.Token);
                httpResponse.EnsureSuccessStatusCode();

                var json = await httpResponse.Content.ReadAsStringAsync(cts.Token);

                viaCepResponse = JsonSerializer.Deserialize<ViaCepResponse>(
                    json,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
                );
            }
            catch (TaskCanceledException)
            {
                throw new TimeoutException("Timeout ao consultar o ViaCEP.");
            }
            catch (HttpRequestException ex)
            {
                throw new HttpRequestException($"Erro ao acessar ViaCEP: {ex.Message}");
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Falha inesperada ao consultar ViaCEP: {ex.Message}");
            }

            // Validação da resposta
            if (viaCepResponse == null ||
                viaCepResponse.Erro == true ||
                string.IsNullOrWhiteSpace(viaCepResponse.Cep))
            {
                throw new KeyNotFoundException("CEP não encontrado no ViaCEP.");
            }

            // Preparar entidade para salvar
            var endereco = new CepEndereco
            {
                Cep = viaCepResponse.Cep.Replace("-", ""),
                Logradouro = viaCepResponse.Logradouro,
                Complemento = viaCepResponse.Complemento,
                Bairro = viaCepResponse.Bairro,
                Localidade = viaCepResponse.Localidade,
                Uf = viaCepResponse.Uf,
                Ibge = viaCepResponse.Ibge,
                Gia = viaCepResponse.Gia,
                Ddd = viaCepResponse.Ddd,
                Siafi = viaCepResponse.Siafi,
                AtualizadoEm = DateTime.UtcNow
            };

            // Salvar no banco
            _context.CepEnderecos.Add(endereco);
            await _context.SaveChangesAsync();

            return endereco;
        }

        // DTO ViaCEP
        private class ViaCepResponse
        {
            public string Cep { get; set; }
            public string Logradouro { get; set; }
            public string Complemento { get; set; }
            public string Bairro { get; set; }
            public string Localidade { get; set; }
            public string Uf { get; set; }
            public string Ibge { get; set; }
            public string Gia { get; set; }
            public string Ddd { get; set; }
            public string Siafi { get; set; }
            public bool? Erro { get; set; }
        }
    }
}
