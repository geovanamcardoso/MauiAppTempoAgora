using MauiAppTempoAgora.Models;
using System.Net;
using System.Text.Json.Nodes;

namespace MauiAppTempoAgora.Services
{
    public class DataService
    {
        //É um método que pode retornar uma task nula (usamos '?'), caso o web service não responda
        public static async Task<Tempo?> GetPrevisao(string cidade) 
        {
            
            string chave = "f91d9f685fcf5b8e5fbb12306b66bb46";
            string url = $"https://api.openweathermap.org/data/2.5/weather?q={cidade}&appid={chave}&units=metric&lang=pt_br";

            using (HttpClient client = new HttpClient())
            {
                try
                {
                    HttpResponseMessage resp = await client.GetAsync(url);

                    if (!resp.IsSuccessStatusCode)
                    {
                        if (resp.StatusCode == HttpStatusCode.NotFound)
                        {
                            throw new HttpRequestException("Cidade não encontrada", null, HttpStatusCode.NotFound);
                        }
                        else
                        {
                            throw new HttpRequestException("Erro ao acessar a API", null, resp.StatusCode);
                        }
                    }

                    string json = await resp.Content.ReadAsStringAsync();

                    var rascunho = JsonObject.Parse(json);

                    //Ambos valores estão em segundos (conta a partir de uma data da computacao), então precisamos converter para o formato de data e hora
                    DateTime time = new();
                    DateTime sunrise = time.AddSeconds((double)rascunho["sys"]["sunrise"]).ToLocalTime();
                    DateTime sunset = time.AddSeconds((double)rascunho["sys"]["sunset"]).ToLocalTime();

                    Tempo t = new()
                    {
                        lat = (double)rascunho["coord"]["lat"],
                        lon = (double)rascunho["coord"]["lon"],
                        temp_min = (double)rascunho["main"]["temp_min"],
                        temp_max = (double)rascunho["main"]["temp_max"],
                        visibility = (int)rascunho["visibility"],
                        sunrise = sunrise.ToString(),
                        sunset = sunset.ToString(),
                        speed = (double)rascunho["wind"]["speed"],
                        description = (string)rascunho["weather"][0]["description"],
                        main = (string)rascunho["weather"][0]["main"]
                    };

                    return t;
                }
                catch (HttpRequestException ex)
                {
                    // Se já tem StatusCode (ex: 404), só repassa
                    if (ex.StatusCode != null)
                        throw;

                    // Se NÃO tem StatusCode 
                    throw new HttpRequestException("Sem conexão com a internet");
                }
            }
        }
    }
}