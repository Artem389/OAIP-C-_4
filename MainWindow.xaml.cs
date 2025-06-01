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
using WpfApp1.Products;
using WpfApp1.Categories;
using WpfApp1.Manufacturers;
using WpfApp1.Orders;
using WpfApp1.Customers;

namespace WpfApp1
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void ManufacturersMenuItem_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Content = new ManufacturersPage();
        }

        private void CategoriesMenuItem_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Content = new CategoriesPage();
        }

        private void ProductsMenuItem_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Content = new ProductsPage();
        }

        private void CustomersMenuItem_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Content = new CustomersPage();
        }

        private void OrdersMenuItem_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Content = new OrdersPage();
        }
    }
}
