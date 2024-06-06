using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

public class ApiCEP
{
    private readonly HttpClient _client;

    public ApiCEP(HttpClient client)
    {
        _client = client;
    }

    public async Task<Endereco> GetAddressAsync(string cep)
    {
        string url = $"https://viacep.com.br/ws/{cep}/json/";

        try
        {
            HttpResponseMessage response = await _client.GetAsync(url);
            response.EnsureSuccessStatusCode();

            string responseBody = await response.Content.ReadAsStringAsync();
            var endereco = JsonConvert.DeserializeObject<Endereco>(responseBody);

            return endereco;
        }
        catch (HttpRequestException)
        {
            return null;
        }
    }
}

public class Endereco
{
    public string Cep { get; set; }
    public string Logradouro { get; set; }
    public string Bairro { get; set; }
    public string Localidade { get; set; }
    public string Uf { get; set; }
    // Adicione outras propriedades conforme necessário
}
