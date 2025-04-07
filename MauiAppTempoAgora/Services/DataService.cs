using MauiAppTempoAgora.Models;
using Newtonsoft.Json.Linq;
using System.Net;

namespace MauiAppTempoAgora.Services
{
    public class DataService
    {
        public static async Task<Tempo?> GetPrevisao(string cidade)
        {
            Tempo? t = null;

            string chave = "b59eb570193c5a3ca1e62bb5434b1188";

            string url = $"https://api.openweathermap.org/data/2.5/weather?" +
                         $"q={cidade}&units=metric&lang=pt_br&appid={chave}"; // <- Idioma adicionado aqui

            try
            {
                using (HttpClient client = new HttpClient())
                {
                    HttpResponseMessage resp = await client.GetAsync(url);

                    if (resp.IsSuccessStatusCode)
                    {
                        string json = await resp.Content.ReadAsStringAsync();

                        var rascunho = JObject.Parse(json);

                        DateTime sunrise = DateTimeOffset
                            .FromUnixTimeSeconds((long)rascunho["sys"]["sunrise"])
                            .ToLocalTime()
                            .DateTime;

                        DateTime sunset = DateTimeOffset
                            .FromUnixTimeSeconds((long)rascunho["sys"]["sunset"])
                            .ToLocalTime()
                            .DateTime;

                        t = new()
                        {
                            lat = (double)rascunho["coord"]["lat"],
                            lon = (double)rascunho["coord"]["lon"],
                            description = (string)rascunho["weather"][0]["description"],
                            main = (string)rascunho["weather"][0]["main"],
                            temp_min = (double)rascunho["main"]["temp_min"],
                            temp_max = (double)rascunho["main"]["temp_max"],
                            speed = (double)rascunho["wind"]["speed"],
                            visibility = (int)rascunho["visibility"],
                            sunrise = sunrise.ToString(),
                            sunset = sunset.ToString(),
                        };
                    }
                    else if (resp.StatusCode == HttpStatusCode.NotFound)
                    {
                        await Application.Current.MainPage.DisplayAlert("Cidade não encontrada", "Verifique o nome da cidade digitada e tente novamente.", "OK");
                    }
                    else
                    {
                        await Application.Current.MainPage.DisplayAlert("Erro", "Ocorreu um erro ao obter os dados do tempo. Tente novamente mais tarde.", "OK");
                    }
                }

            }
            catch (HttpRequestException)
            {
                await Application.Current.MainPage.DisplayAlert("Sem conexão", "Não foi possível conectar à internet. Verifique sua conexão e tente novamente.", "OK");
            }

            return t;
        }
    }
}
