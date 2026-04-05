using MauiAppTempoAgora.Models;
using MauiAppTempoAgora.Services;
using System.Net;
using System.Net.Http; 
using System.Threading.Tasks;

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
                    Tempo? t = await DataService.GetPrevisao(txt_cidade.Text.Trim());

                    if(t != null)
                    {
                        string dados_previsao = " ";

                        dados_previsao = $"Cidade: {txt_cidade.Text}\n" +
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
                        lbl_res.Text = "Sem dados de previsão.";
                    }
                }
                else
                {
                    lbl_res.Text = "Preencha o nome da cidade";
                }
            }
            catch (HttpRequestException aex)
            {
                // Mensagem para cidade não encontrada
                if (aex.StatusCode == HttpStatusCode.NotFound)
                {
                    await DisplayAlert("Cidade não encontrada", $"A cidade '{txt_cidade.Text}' não foi encontrada. Verifique o nome e tente novamente.", "OK");
                    lbl_res.Text = string.Empty;
                }
                else if (aex.Message.Contains("Sem conexão"))
                {
                    // Alerta para sem conexão
                    await DisplayAlert("Sem conexão", "Sem conexão com a internet. Verifique sua conexão e tente novamente.", "OK");
                }
                else
                {
                    await DisplayAlert("Erro", aex.Message, "OK");
                }
            }
            catch(Exception ex)
            {
                await DisplayAlert("Ops", ex.Message, "OK");
            }
        }
    }
}
