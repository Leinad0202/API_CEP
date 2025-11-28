namespace API_CEP.Services
{
    public interface ICepService
    {
        Task<API_CEP.Models.CepEndereco> BuscarCepAsync(string cep);
    }
}
