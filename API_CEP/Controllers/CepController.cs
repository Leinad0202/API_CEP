using API_CEP.Data;
using API_CEP.Models;
using API_CEP.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace API_CEP.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CepController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly ICepService _cepService;
        private readonly IMemoryCache _cache;

        public CepController(AppDbContext context, ICepService cepService, IMemoryCache memoryCache)
        {
            _context = context;
            _cepService = cepService;
            _cache = memoryCache;
        }


        // GET api/cep/{cep}
        [HttpGet("{cep}")]
        public async Task<ActionResult<CepEndereco>> GetByCep(string cep)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(cep) || cep.Length != 8 || !cep.All(char.IsDigit))
                    return BadRequest("CEP inválido. O CEP deve conter exatamente 8 dígitos numéricos.");

                string cacheKey = $"cep_{cep}";

                // 1️⃣ Tenta pegar do cache antes de qualquer coisa
                if (_cache.TryGetValue(cacheKey, out CepEndereco enderecoCache))
                    return Ok(enderecoCache);

                // 2️⃣ Busca normal via service (banco → ViaCEP → salva banco → retorna)
                var endereco = await _cepService.BuscarCepAsync(cep);

                // 3️⃣ Armazena no cache por 30 minutos
                _cache.Set(cacheKey, endereco, TimeSpan.FromMinutes(30));

                return Ok(endereco);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (KeyNotFoundException)
            {
                return NotFound("CEP não encontrado nem no banco local nem na API ViaCEP.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro interno ao consultar o CEP: {ex.Message}");
            }
        }


        // GET api/cep
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CepEndereco>>> GetAll()
        {
            var lista = await _context.CepEnderecos.ToListAsync();
            return Ok(lista);
        }

        // POST api/cep
        [HttpPost]
        public async Task<ActionResult<CepEndereco>> Create(CepEndereco endereco)
        {
            endereco.AtualizadoEm = DateTime.UtcNow;

            _context.CepEnderecos.Add(endereco);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetByCep), new { cep = endereco.Cep }, endereco);
        }

        // PUT api/cep/{cep}
        [HttpPut("{cep}")]
        public async Task<IActionResult> Update(string cep, CepEndereco endereco)
        {
            if (cep != endereco.Cep)
                return BadRequest("O CEP da URL não corresponde ao da entidade enviada.");

            endereco.AtualizadoEm = DateTime.UtcNow;

            _context.Entry(endereco).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.CepEnderecos.Any(e => e.Cep == cep))
                    return NotFound("CEP não encontrado.");

                throw;
            }

            return NoContent();
        }

        // DELETE api/cep/{cep}
        [HttpDelete("{cep}")]
        public async Task<IActionResult> Delete(string cep)
        {
            // validação básica
            if (string.IsNullOrWhiteSpace(cep) || cep.Length != 8 || !cep.All(char.IsDigit))
                return BadRequest("CEP inválido. Deve conter 8 dígitos.");

            var endereco = await _context.CepEnderecos.FindAsync(cep);

            if (endereco == null)
                return NotFound("CEP não encontrado no banco.");

            _context.CepEnderecos.Remove(endereco);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
