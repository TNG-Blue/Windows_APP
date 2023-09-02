using System.Windows;
using System.Windows.Controls;
using Blue;

namespace Blue
{
    public partial class MenuPage : Page
    {
        public MenuPage()
        {
            InitializeComponent();
        }

        private void MenuOptions_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Kiểm tra xem có `ListViewItem` được chọn không
            if (MenuOptions.SelectedItem != null)
            {
                // Lấy nội dung của `ListViewItem` được chọn
                string selectedOption = ((ListViewItem)MenuOptions.SelectedItem).Content.ToString();

                // Chuyển hướng đến trang OptionPage tương ứng dựa vào lựa chọn
                switch (selectedOption)
                {
                    case "Option 1":
                        OpenNewWindow(new Option1Page());
                        break;
                    case "Option 2":
                        OpenNewWindow(new Option2Page());
                        break;
                    case "Option 3":
                        OpenNewWindow(new Option3Page());
                        break;
                    case "Option 4":
                        OpenNewWindow(new Option4Page());
                        break;
                    case "Option 5":
                        OpenNewWindow(new Option5Page());
                        break;
                    // Thêm các trường hợp khác tương ứng với các lựa chọn khác
                    default:
                        break;
                }

                // Đặt lại giá trị đã chọn để chuẩn bị cho lần chọn tiếp theo
                MenuOptions.SelectedItem = null;
            }
        }

        private void OpenNewWindow(Page page)
        {
            Window newWindow = new Window();
            newWindow.Content = page;
            newWindow.Show();
        }
    }
}