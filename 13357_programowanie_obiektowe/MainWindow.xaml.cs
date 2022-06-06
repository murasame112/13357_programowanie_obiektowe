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
     * 1 - program wyświetla wybrane stacje lub uzytkownik je zna
     * 2 - uzytkownik podaje id stacji (jest obok nazwy jesli wyswietlone wybrane, lub po prostu je zna bo jest pracownikiem)
     * 3 - to wyswietla mu id rzeczy ktore stacja mierzy (na podstawie stanowiska pomiarowego)
     * 4 - uzytkownik podaje id stanowiska i sprawdza stan poszczegolnego skladnika powietrza (dane pomiarowe)
     * 5 - zapisywanie do bazy albo tuż po pobraniu danych i wtedy wyświetla wszystko na podstawie danych z bazy
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
