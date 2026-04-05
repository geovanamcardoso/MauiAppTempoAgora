using MauiAppTempoAgora.Models;
using MauiAppTempoAgora.Services;
using System.Net;

namespace MauiAppTempoAgora
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
        }

        private async void Button_Clicked(object sender, EventArgs e)
        {
            try
            {
                if (!string.IsNullOrEmpty(txt_cidade.Text))
                {
                    Tempo t = await DataService.GetPrevisao(txt_cidade.Text.Trim());

                    string dados_previsao =
                        $"Cidade: {txt_cidade.Text}\n" +
                        $"Latitude: {t.lat}\n" +
                        $"Longitude: {t.lon}\n" +
                        $"Temperatura: {t.temp_min}°C a {t.temp_max}°C\n" +
                        $"Descrição: {t.main} - {t.description}\n" +
                        $"Visibilidade: {t.visibility} metros\n" +
                        $"Vento: {t.speed} m/s\n" +
                        $"Nascer do Sol: {t.sunrise}\n" +
                        $"Pôr do Sol: {t.sunset}";

                    lbl_res.Text = dados_previsao;
                }
                else
                {
                    lbl_res.Text = "Preencha o nome da cidade";
                }
            }
            catch (HttpRequestException aex)
            {
                if (aex.StatusCode == HttpStatusCode.NotFound)
                {
                    await DisplayAlert("Cidade não encontrada",
                        $"A cidade '{txt_cidade.Text}' não foi encontrada. Verifique o nome e tente novamente.",
                        "OK");

                    lbl_res.Text = string.Empty;
                }
                else if (aex.StatusCode == null)
                {
                    await DisplayAlert("Sem conexão",
                        "Sem conexão com a internet. Verifique sua conexão e tente novamente.",
                        "OK");
                }
                else
                {
                    await DisplayAlert("Erro",
                        "Erro ao buscar dados da API.",
                        "OK");
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Ops", ex.Message, "OK");
            }
        }
    }
}