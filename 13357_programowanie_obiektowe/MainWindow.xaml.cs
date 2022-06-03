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
     * 1 - program wyświetla pierwszy interfejs, w którym można wybrać "wyświetl wybrane stacje"
     * 2 - program pobiera (z bazy lub api, punkt 4) i wyświetla kilka wybranych stacji (w formie przycisków, lub przycisk obok)
     * 3 - po kliknięciu na daną stację, program pokazuje stan powietrza (lub poszczególnego jego składnika) w tym miejscu  
     * 4 - zapisywanie do bazy albo tuż po pobraniu danych i wtedy wyświetla wszystko na podstawie danych z bazy (można będzie dodać przycisk "aktualizuj"),
     * albo dopiero gdy użytkownik zdecyduje się na zapisanie do niej konkretnych danych
     */



    // TODO:
    // 3. 

    // 

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }
    }
}
