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

namespace WpfApp1.Products
{
    /// <summary>
    /// Логика взаимодействия для ProductsPage.xaml
    /// </summary>
    public partial class ProductsPage : Page
    {
        private ProductsTableAdapter productsAdapter = new ProductsTableAdapter();
       
        public ProductsPage()
        {
            InitializeComponent();
            LoadData();
        }

        private void LoadData()
        {
            var productsData = productsAdapter.GetData();
            ProductsDataGrid.ItemsSource = productsData;
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            var addEditWindow = new AddEditProductWindow();
            if (addEditWindow.ShowDialog() == true)
            {
                LoadData();
            }
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            if (ProductsDataGrid.SelectedItem == null)
            {
                MessageBox.Show("Выберите товар для редактирования", "Ошибка",
                               MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var selectedProduct = (ProductsDataGrid.SelectedItem as DataRowView).Row as ElectronicsStoreDataSet.ProductsRow;
            var addEditWindow = new AddEditProductWindow(selectedProduct);

            if (addEditWindow.ShowDialog() == true)
            {
                LoadData();
            }
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (ProductsDataGrid.SelectedItem == null)
            {
                MessageBox.Show("Выберите товар для удаления", "Ошибка",
                               MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var row = (ProductsDataGrid.SelectedItem as DataRowView).Row as ElectronicsStoreDataSet.ProductsRow;

            try
            {
                productsAdapter.DeleteProducts(row.ProductID);
                LoadData();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Не удалось удалить товар: {ex.Message}\n\n" +
                               "Возможно, этот товар используется в других таблицах.",
                               "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            if (this.Parent is Frame frame)
            {
                frame.GoBack();
            }
        }

        private void ProductsDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            EditButton.IsEnabled = DeleteButton.IsEnabled = ProductsDataGrid.SelectedItem != null;
        }
    }
}
