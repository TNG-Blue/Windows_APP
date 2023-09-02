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

namespace Blue
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            LoadBackground();
        }

        private async void LoadBackground()
        {
            // Hiển thị ảnh nền trong 3 giây
            var backgroundBrush = new System.Windows.Media.ImageBrush(new System.Windows.Media.Imaging.BitmapImage(new Uri("F:\\Source\\CShap\\Blue Test\\ROG-wallpaper-Gamer's-Room_1_3840x2160.jpg")));
            MainGrid.Background = backgroundBrush;

            await Task.Delay(3000); // Đợi 3 giây

            // Sau 3 giây, chuyển nền thành trắng và hiển thị giao diện đăng nhập
            MainGrid.Background = System.Windows.Media.Brushes.White;
            MainGrid.Children[0].Visibility = Visibility.Visible;
        }

        private void Login_Click(object sender, RoutedEventArgs e)
        {
            {
                string username = TxtUsername.Text;
                string password = TxtPassword.Password;

                // Kiểm tra tên người dùng và mật khẩu có hợp lệ không
                if (CheckLogin(username, password))
                {
                    MessageBox.Show("Đăng nhập thành công!");

                    // Navigate to the MenuPage
                    MenuPage menuPage = new MenuPage();
                    this.Content = menuPage;
                }
                else
                {
                    MessageBox.Show("Tên đăng nhập hoặc mật khẩu không đúng. Vui lòng thử lại.");
                }
            }
        }

        private void ForgotPassword_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Bạn đã chọn quên mật khẩu.");
            // Thực hiện hành động khi người dùng chọn "Quên mật khẩu"
        }

        private void LoginWithDifferentAccount_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Bạn đã chọn đăng nhập bằng tài khoản khác.");
            // Thực hiện hành động khi người dùng chọn "Đăng nhập bằng tài khoản khác"
        }

        private bool CheckLogin(string username, string password)
        {
            // Trong ví dụ này, sẽ kiểm tra mật khẩu đơn giản
            // Trong thực tế, cần kết hợp với cơ sở dữ liệu hoặc hệ thống xác thực
            return username == "blue" && password == "12345";
        }

        private void Click_ESC(object sender, RoutedEventArgs e)
        {
            
        }
    }
}
