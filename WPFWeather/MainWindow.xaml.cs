using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
using System.Net;
using Newtonsoft.Json;
using System.IO;
using System.Drawing;


namespace WPFWeather
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public Dictionary<string, string> cities_data = new Dictionary<string, string>();
        public string apiKey = "apikey";
        public string language = "uk";
        const string fileSettings = "FileSettings.txt";


        public MainWindow()
        {
            InitializeComponent();
            
            string storageFolder = Directory.GetCurrentDirectory();
            

            bool fileExists = File.Exists($"{storageFolder}\\{fileSettings}");
            if (!fileExists)
            {
                CreateFileWithData();
            }

            HandleSettingFileData();
            
                       

            //cities_data.TryAdd("Измаил", "325086");
            //cities_data.TryAdd("Одеса", "325343");
            foreach (string city in cities_data.Keys)
            {
                cities.Items.Add(city);
            }
            
        }

        public void HandleSettingFileData()
        {
            char[] delimiters = { '-', ',', ' ' };


            StreamReader sR = File.OpenText(fileSettings);

            string[] lines = sR.ReadToEnd().Split('\n', StringSplitOptions.RemoveEmptyEntries);
            foreach (string line in lines)
            {
                string[] cityInfo = line.Split(delimiters, StringSplitOptions.RemoveEmptyEntries);
                cities_data.TryAdd(cityInfo[0], cityInfo[1]);  
            }                       
            
        }
        public static void CreateFileWithData()
        {
            string defaultFileText = "Одеса - 325343,\n" +
                   "Ізмаїл - 325086,\n" +
                   "Київ - 324505,\n" +
                   "Львів - 324561,\n" +
                   "Харків - 323903,";
            File.WriteAllText(fileSettings, defaultFileText);
        }

        private  void mainButton1_Click(object sender, RoutedEventArgs e)
        {
            WebClient client = new WebClient();

            string cityKey;                
            cities_data.TryGetValue(cities.Text, out cityKey) ;
            string url = $"http://dataservice.accuweather.com/forecasts/v1/daily/1day/{cityKey}?apikey={apiKey}&language={language}&metric=true";
            string response;
            
            response = client.DownloadString(url);

            
            RootWeather weatherData = JsonConvert.DeserializeObject<RootWeather>(response);
            
            mainImage.Source = BitmapFrame.Create(
                new Uri(@$"D:\DD\project\WPFWeather\WPFWeather\Icons\{weatherData.DailyForecasts[0].Day.Icon}.png"
                ));
            mainImage2.Source = BitmapFrame.Create(
                new Uri(@$"D:\DD\project\WPFWeather\WPFWeather\Icons\{weatherData.DailyForecasts[0].Night.Icon}.png"
                ));

            dayLabelPharse.Content = weatherData.DailyForecasts[0].Day.IconPhrase;
            nightLabelPharse.Content = weatherData.DailyForecasts[0].Night.IconPhrase;
            mainMinTempLabel.Content = weatherData.DailyForecasts[0].Temperature.Minimum.Value.ToString();
            mainMaxTempLabel.Content = weatherData.DailyForecasts[0].Temperature.Maximum.Value.ToString();

            

            //mainRTB.AppendText(weatherData.DailyForecasts[0].Temperature.Minimum.Value.ToString());
            
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}
