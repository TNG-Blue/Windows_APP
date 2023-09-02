using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Windows;
using System.Windows.Controls;
using Newtonsoft.Json;

namespace Blue
{
    public partial class Option2Page : Page
    {
        private List<SensorData> sensorDataList;

        public Option2Page()
        {
            InitializeComponent();
        }

        private async void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            string apiUrl = "http://localhost:5000/get_sensor_data";

            try
            {
                using (HttpClient client = new HttpClient())
                {
                    HttpResponseMessage response = await client.GetAsync(apiUrl);

                    if (response.IsSuccessStatusCode)
                    {
                        string json = await response.Content.ReadAsStringAsync();
                        sensorDataList = JsonConvert.DeserializeObject<List<SensorData>>(json);

                        // Gán danh sách dữ liệu cho DataGrid để hiển thị
                        MyDataGrid.ItemsSource = sensorDataList;
                    }
                    else
                    {
                        MessageBox.Show("Failed to fetch data from the API.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }

    public class SensorData
    {
        public int ID { get; set; }
        public int DeviceID { get; set; }
        public double LightIntensity { get; set; }
        public double Temperature { get; set; }
        public double AirHumidity { get; set; }
        public double SoilHumidity { get; set; }
        public string Prediction { get; set; }
        public string Timestamp { get; set; }
        public string Note { get; set; }
    }
}
