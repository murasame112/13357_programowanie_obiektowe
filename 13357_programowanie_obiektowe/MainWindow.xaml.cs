﻿using Microsoft.Win32;
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
     * 4 - uzytkownik podaje id stanowiska i sprawdza stan poszczegolnego skladnika powietrza (dane pomiarowe)
     * 5 - zapisywanie do bazy albo tuż po pobraniu danych i wtedy wyświetla wszystko na podstawie danych z bazy
     * 6 - na koniec może poszukać skali i np przeliczać, że np. wartość ozonu 77.1506 to Dobra, Bardzo Dobra, Zła, czy coś takiego?
     */



    // TODO:
    // 6. usunac dictionary param, bo chyba niepotrzebne skoro korzystam z bazy


    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
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
            public decimal Value { get; set; }
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

        int[] stationIds = new int[5] { 659, 459, 736, 10121, 9179};


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
            public JsonValue Values { get; set; }
            [JsonPropertyName("param")]
            public JsonParam Param { get; set; }
        }

        class JsonValue
        {
            [JsonPropertyName("date")]
            public string Date { get; set; }
            [JsonPropertyName("value")]
            public decimal Value { get; set; }
        }

        private void DownloadDataJson(AppContext context)
        {

            WebClient client = new WebClient();
            client.Headers.Add("Accept", "application/json");
            List<TableParams> tableParams = new List<TableParams>();
            List < TableDetails> tableDetails = new List<TableDetails>();
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
                    tableDetails = JsonSerializer.Deserialize<List<TableDetails>>(json);

                    ModelDetail detail = new ModelDetail() { DetailId = indexDetail, SensorId = localParamId, Value = tableDetails[0].Values.Value };
                    context.Details.Add(detail);
                    tableDetails.Clear();

                }
            }


            context.SaveChanges();
        }

        class AppContext : DbContext
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


        public MainWindow()
        {
            AppContext context = new AppContext();
            context.Database.EnsureCreated();
            InitializeComponent();
            DownloadDataJson(context);
        }
    }
}
