using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Windows;
using System.Windows.Controls;
using Newtonsoft.Json;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using System.Windows.Threading;

namespace Blue
{
    public partial class Option2Page : Page
    {
        private List<SensorData> sensorDataList;
        private DispatcherTimer dataRefreshTimer;

        public Option2Page()
        {
            InitializeComponent();

            // Khởi tạo và cấu hình DispatcherTimer
            dataRefreshTimer = new DispatcherTimer();
            dataRefreshTimer.Interval = TimeSpan.FromSeconds(30); // Cập nhật sau mỗi 30 giây
            dataRefreshTimer.Tick += DataRefreshTimer_Tick;

            // Bắt đầu cập nhật dữ liệu khi trang được tạo
            RefreshDataAndChart();
            dataRefreshTimer.Start();
        }

        private async void DataRefreshTimer_Tick(object sender, EventArgs e)
        {
            // Gọi lại phương thức RefreshDataAndChart sau mỗi đợt cập nhật
            RefreshDataAndChart();
        }

        private async void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            // Gọi lại phương thức RefreshDataAndChart khi nhấn nút "Refresh"
            RefreshDataAndChart();
        }

        // Tạo và làm mới biểu đồ
        private void CreateAndRefreshChart()
        {
            if (sensorDataList != null && sensorDataList.Count > 0)
            {
                // Create and configure the PlotModel
                var model = new PlotModel();
                var dateAxis = new DateTimeAxis { Title = "Timestamp" };
                var valueAxis = new LinearAxis { Title = "Value" };

                model.Axes.Add(dateAxis);
                model.Axes.Add(valueAxis);

                // Create LineSeries for each sensor
                var lightIntensitySeries = new LineSeries { Title = "Light Intensity" };
                var temperatureSeries = new LineSeries { Title = "Temperature" };
                var airHumiditySeries = new LineSeries { Title = "Air Humidity" };
                var soilHumiditySeries = new LineSeries { Title = "Soil Humidity" };

                // Populate the series with data
                foreach (var data in sensorDataList)
                {
                    DateTime timestamp = DateTime.Parse(data.timestamp);
                    lightIntensitySeries.Points.Add(new DataPoint(DateTimeAxis.ToDouble(timestamp),
                        data.light_intensity));
                    temperatureSeries.Points.Add(new DataPoint(DateTimeAxis.ToDouble(timestamp), data.temperature));
                    airHumiditySeries.Points.Add(new DataPoint(DateTimeAxis.ToDouble(timestamp), data.air_humidity));
                    soilHumiditySeries.Points.Add(new DataPoint(DateTimeAxis.ToDouble(timestamp), data.soil_humidity));
                }

                model.Series.Add(lightIntensitySeries);
                model.Series.Add(temperatureSeries);
                model.Series.Add(airHumiditySeries);
                model.Series.Add(soilHumiditySeries);

                SensorDataPlot.Model = model;
            }
        }

        // Tạo phương thức để làm mới dữ liệu từ API
        private async void RefreshDataAndChart()
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

                        // Làm mới bảng
                        MyDataGrid.ItemsSource = sensorDataList;

                        // Làm mới biểu đồ
                        CreateAndRefreshChart();
                    }
                    else
                    {
                        MessageBox.Show("Failed to fetch data from the API.", "Error", MessageBoxButton.OK,
                            MessageBoxImage.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred: " + ex.Message, "Error", MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }
    }

    public class SensorData
    {
        public int id { get; set; }
        public string device_id { get; set; }
        public double light_intensity { get; set; }
        public double temperature { get; set; }
        public double air_humidity { get; set; }
        public double soil_humidity { get; set; }
        public string prediction { get; set; }
        public string timestamp { get; set; }
        public string note { get; set; }
    }
}
