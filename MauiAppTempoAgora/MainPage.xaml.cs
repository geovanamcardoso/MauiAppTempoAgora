using MauiAppTempoAgora.Models;
using MauiAppTempoAgora.Services;
using System;
using System.Net;
using System.Diagnostics;

namespace MauiAppTempoAgora
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
        }

        private async void Button_Clicked_Previsao(object sender, EventArgs e)
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

                    string mapa = $"https://embed.windy.com/embed.html?" +
                                      $"type=map&location=coordinates&metricRain=mm&metricTemp=°C" +
                                      $"&metricWind=km/h&zoom=5&overlay=wind&product=ecmwf&level=surface" +
                                      $"&lat={t.lat.ToString().Replace(",", ".")}&lon={t.lon.ToString().Replace(",", ".")}";

                    wv_mapa.Source = mapa;

                    Debug.WriteLine(mapa);
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

        private async void Button_Clicked_Localizacao(object sender, EventArgs e)
        {
            try
            {
                GeolocationRequest request =  new GeolocationRequest(GeolocationAccuracy.Medium, TimeSpan.FromSeconds(10));

                Location? local =  await Geolocation.Default.GetLocationAsync(request);   

                if(local != null)
                {
                    string local_disp = $"Latitude: {local.Latitude}\n" +
                                        $"Longitude: {local.Longitude}\n";

                    lbl_coords.Text = local_disp;
                    GetCidade(local.Latitude, local.Longitude);
                }
                else
                {
                    lbl_coords.Text = "Não foi possível obter a localização.";
                }



            }
            catch (FeatureNotSupportedException fnsEx)
            {
                await DisplayAlert("Erro: Dispositivo não suporta", fnsEx.Message, "OK");
            }
            catch (FeatureNotEnabledException fneEx)
            {
                await DisplayAlert("Erro: Localização desativada", fneEx.Message, "OK");
            }
            catch (PermissionException pEx)
            {
               await DisplayAlert("Erro: Permissão de localização negada", pEx.Message, "OK");
            }
            catch (Exception ex)
            {
                await DisplayAlert("Erro", $"Ocorreu um erro ao acessar a localização: {ex.Message}", "OK");
            }
        }

        private async void GetCidade(double lat, double lon) 
        {
            try
            {
                IEnumerable<Placemark> places = await Geocoding.Default.GetPlacemarksAsync(lat, lon);
                Placemark? place = places.FirstOrDefault();

                if (place != null)
                {
                    txt_cidade.Text = place.Locality;
                }
            }
            catch(Exception ex)
            {
                await DisplayAlert("Erro: Obtenção do nome da cidade", $"Ocorreu um erro ao obter o nome da cidade: {ex.Message}", "OK");
            }


        }
    }
}