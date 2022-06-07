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
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Sqlite;

namespace _13357_programowanie_obiektowe
{
    // https://powietrze.gios.gov.pl/pjp/content/api

    /* PLAN:
     * 1 - program wyświetla wybrane stacje
     * 2 - uzytkownik podaje id stacji (jest obok nazwy)
     * 3 - program wyswietla mu id rzeczy ktore stacja mierzy (na podstawie stanowiska pomiarowego)
     * 4 - uzytkownik podaje id sensora
     * 5 - program wyswietla stan poszczegolnego skladnika powietrza (dane pomiarowe)
     * 6 - zapisywanie do bazy albo tuż po pobraniu danych i wtedy wyświetla wszystko na podstawie danych z bazy
     */


    /*
    stacje i ich id:
    [id w api] - [nazwa] - [id dla uzytkownika] 
    117 - wrocław - 1
    266 - lublin - 2
    285 - zamość - 3
    459 - zakopane - 4
    530 - warszawa - 5
    736 - gdańsk - 6
    944 - poznań - 7
    10058 - łódź - 8
    10121 - kraków - 9
    10125 - rzeszów - 10
    */

    // TODO:
    // 14.

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        AppContext PublicContext;

        public record ModelSensor
        {
            public int SensorId { get; set; }
            public int ParamId { get; set; }
            public int StationId { get; set; }
            public string ParamName { get; set; }
            public string ParamFormula { get; set; }
        }

        public record ModelDetail
        {
            public int DetailId { get; set; }
            public int SensorId { get; set; }
            public decimal? Value { get; set; }
        }

        class TableParams
        {
            [JsonPropertyName("id")]
            public int Id { get; set; }
            [JsonPropertyName("stationId")]
            public int StationId { get; set; }
            [JsonPropertyName("param")]
            public JsonParam Param { get; set; }
        }

        int[] stationIds = new int[10] { 117, 266, 285, 459, 530, 736, 944, 10058, 10121, 10125 };


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

        class TableDetails
        {
            [JsonPropertyName("key")]
            public string Key { get; set; }
            [JsonPropertyName("values")]
            public JsonValue[] Values { get; set; }
        }

        class JsonValue
        {
            [JsonPropertyName("date")]
            public string Date { get; set; }
            [JsonPropertyName("value")]
            public decimal? Value { get; set; }
        }

        public void DownloadDataJson(AppContext context)
        {

            WebClient client = new WebClient();
            client.Headers.Add("Accept", "application/json");
            List<TableParams> tableParams = new List<TableParams>();
            TableDetails tableDetails = new TableDetails();
            List<JsonValue> jsonValues = new List<JsonValue>();
            int index = 0;
            int indexDetail = 0;
            for (int i = 0; i < stationIds.Length; i++)
            {

                tableParams.Clear();
                string adress = "https://api.gios.gov.pl/pjp-api/rest/station/sensors/" + stationIds[i] + "";
                string json = client.DownloadString(adress);
                tableParams = JsonSerializer.Deserialize<List<TableParams>>(json);

                foreach (TableParams item in tableParams)
                {
                    index++;
                    indexDetail++;
                    int localParamId = item.Id;
                    string localParamName = item.Param.Name;
                    string localParamFormula = item.Param.Formula;

                    ModelSensor sensor = new ModelSensor() { SensorId = index, ParamId = localParamId, StationId = stationIds[i], ParamName = localParamName, ParamFormula = localParamFormula };
                    context.Sensors.Add(sensor);

                    string detailAdress = "https://api.gios.gov.pl/pjp-api/rest/data/getData/" + localParamId + "";
                    string detailJson = client.DownloadString(detailAdress);
                    tableDetails = JsonSerializer.Deserialize<TableDetails>(detailJson);

                    decimal? val = tableDetails.Values[1].Value;


                    ModelDetail detail = new ModelDetail() { DetailId = indexDetail, SensorId = localParamId, Value = val };
                    context.Details.Add(detail);

                    
                }
            }


            context.SaveChanges();
        }

        private void GetStation(object sender, RoutedEventArgs e)
        {

            int realId = 0;
            string stationIdInput = (string)StationInput.Text;
            if (int.TryParse(stationIdInput, out int id))
            {
                switch (id)
                {
                    case 1:
                        realId = 117;
                        break;
                    case 2:
                        realId = 266;
                        break;
                    case 3:
                        realId = 285;
                        break;
                    case 4:
                        realId = 459;
                        break;
                    case 5:
                        realId = 530;
                        break;
                    case 6:
                        realId = 736;
                        break;
                    case 7:
                        realId = 944;
                        break;
                    case 8:
                        realId = 10058;
                        break;
                    case 9:
                        realId = 10121;
                        break;
                    case 10:
                        realId = 10125;
                        break;
                    default:
                        realId = 0;
                        break;
                }

                if (realId != 0)
                {
                    var sensors = from sensor in PublicContext.Sensors
                                  where sensor.StationId == realId
                                  select sensor.SensorId + " " + sensor.ParamName + " (" + sensor.ParamFormula + ")";

                    SensorsList.Text = "Wybrane sensory na poszczególnej stacji: \n";
                    SensorsList.Text += string.Join("\n", sensors);
                }

            }

        }
        private void GetSensor(object sender, RoutedEventArgs e)
        {

            string sensorIdInput = (string)SensorInput.Text;
            if (int.TryParse(sensorIdInput, out int id))
            {
                var details = from detail in PublicContext.Details
                          where detail.DetailId == id
                          select detail.Value;

                
                ValueResult.Text = string.Join("\n", details);
            }


        }

        public class AppContext : DbContext
        {
            public DbSet<ModelSensor> Sensors { get; set; }
            public DbSet<ModelDetail> Details { get; set; }
            protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            {
                //   d:\\database\\base.db
                // DATASOURCE=D:/Users/tomasz.wiesek/database/base.db
                optionsBuilder.UseSqlite("DATASOURCE=C:/Users/tomas/Desktop/studia/4 semestr/programowanie obiektowe/laby/base.db");
            }

            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                
                modelBuilder.Entity<ModelSensor>()
                .ToTable("Sensors")
                .HasKey(s => s.SensorId);

                modelBuilder.Entity<ModelDetail>()
                .ToTable("Details")
                .HasKey(d => d.DetailId);
            }
        }

        public void SetPublicContext(AppContext context)
        {
            PublicContext = context;
        }


        public MainWindow()
        {
            AppContext context = new AppContext();
            SetPublicContext(context);
            if (context.Database.EnsureCreated())
            {
                context.Database.EnsureCreated();
                DownloadDataJson(PublicContext);
            }
            else
            {
                context.Database.EnsureDeleted();
                context.Database.EnsureCreated();
                DownloadDataJson(PublicContext);
            }
            
            InitializeComponent();
           

        }

    }
}
