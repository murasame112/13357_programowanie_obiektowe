using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml.Linq;

namespace _13357_programowanie_obiektowe
{
    // https://powietrze.gios.gov.pl/pjp/content/api

    /* PLAN:
     * 1 - program wyświetla wybrane stacje
     * 2 - uzytkownik podaje id stacji (jest obok nazwy)
     * 3 - program wyswietla mu id rzeczy ktore stacja mierzy (na podstawie stanowiska pomiarowego)
     * 4 - uzytkownik podaje id stanowiska i sprawdza stan poszczegolnego skladnika powietrza (dane pomiarowe)
     * 5 - zapisywanie do bazy albo tuż po pobraniu danych i wtedy wyświetla wszystko na podstawie danych z bazy
     * 6 - na koniec może poszukać skali i np przeliczać, że np. wartość ozonu 77.1506 to Dobra, Bardzo Dobra, Zła, czy coś takiego?
     */



    // TODO:
    // 4. przetworzenie danych
    // 5. dodawanie do bazy


    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        class TableStations
        {
            public List<TableParams> tableParameters { get; set; }
        }
        class TableParams
        {
            [JsonPropertyName("id")]
            public int Id { get; set; }
            [JsonPropertyName("stationId")]
            public int StationId { get; set; }
            [JsonPropertyName("param")]
            public List<JsonParam> Params { get; set; }
        }

        record Param(string Name, string Formula, string Code, int IdParam);
        int[] stationIds = new int[] { 659, 459, 736, 10121, 9179};


        record JsonParam
        {

            [JsonPropertyName("paramName")]
            public string Name { get; set; }
            [JsonPropertyName("paramFormula")]
            public string Formula { get; set; }
            [JsonPropertyName("paramCode")]
            public string Code { get; set; }
            [JsonPropertyName("idParam")]
            public int IdParam { get; set; }
        }

        Dictionary<string, Param> Params = new Dictionary<string, Param>();
        private void DownloadDataJson()
        {
            WebClient client = new WebClient();
            client.Headers.Add("Accept", "application/json");

            List<TableStations> tableStations = new List<TableStations>();
            for (int i = 0; i < stationIds.Length; i++)
            {
                string adress = "https://api.gios.gov.pl/pjp-api/rest/station/sensors/" + stationIds[i] + "";
                string json = client.DownloadString(adress);
                List<TableParams> tableParams = JsonSerializer.Deserialize<List<TableParams>>(json);
                TableStations tableStation = new TableStations();
                tableStation.tableParameters = tableParams;
                tableStations.Add(tableStation);
            }
        }


        public MainWindow()
        {
            InitializeComponent();
            DownloadDataJson();
        }
    }
}
