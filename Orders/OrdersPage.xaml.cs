using System;
using System.Collections.Generic;
using System.Data;
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
using WpfApp1.ElectronicsStoreDataSetTableAdapters;

namespace WpfApp1.Orders
{
    /// <summary>
    /// Логика взаимодействия для OrdersPage.xaml
    /// </summary>
    public partial class OrdersPage : Page
    {
        private OrdersTableAdapter ordersAdapter = new OrdersTableAdapter();
        private CustomersTableAdapter customersAdapter = new CustomersTableAdapter();
        private ProductsTableAdapter productsAdapter = new ProductsTableAdapter();
        public OrdersPage()
        {
            InitializeComponent();
            LoadData();
        }

        private void LoadData()
        {
            var ordersData = ordersAdapter.GetData();
            OrdersDataGrid.ItemsSource = ordersData;
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            var addEditWindow = new AddEditOrderWindow(customersAdapter, productsAdapter);
            if (addEditWindow.ShowDialog() == true)
            {
                LoadData();
            }
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            if (OrdersDataGrid.SelectedItem == null)
            {
                MessageBox.Show("Выберите заказ для редактирования", "Ошибка",
                               MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var selectedOrder = (OrdersDataGrid.SelectedItem as DataRowView).Row
                as ElectronicsStoreDataSet.OrdersRow;

            var addEditWindow = new AddEditOrderWindow(
                customersAdapter,
                productsAdapter,
                selectedOrder);

            if (addEditWindow.ShowDialog() == true)
            {
                LoadData();
            }
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (OrdersDataGrid.SelectedItem == null)
            {
                MessageBox.Show("Выберите заказ для удаления", "Ошибка",
                               MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var selectedOrder = (OrdersDataGrid.SelectedItem as DataRowView).Row
                as ElectronicsStoreDataSet.OrdersRow;

            try
            {
                ordersAdapter.DeleteOrders(selectedOrder.OrderID);
                LoadData();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Не удалось удалить заказ: {ex.Message}",
                               "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            if (this.NavigationService.CanGoBack)
            {
                this.NavigationService.GoBack();
            }
            else
            {
                new MainWindow().Show();
                Window.GetWindow(this)?.Close();
            }
        }

        private void OrdersDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            EditButton.IsEnabled = DeleteButton.IsEnabled = OrdersDataGrid.SelectedItem != null;
        }
    }
}
