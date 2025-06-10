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

namespace WpfApp1.Manufacturers
{
    /// <summary>
    /// Логика взаимодействия для ManufacturersPage.xaml
    /// </summary>
    public partial class ManufacturersPage : Page
    {
        private ManufacturersTableAdapter manufacturersAdapter = new ManufacturersTableAdapter();
        public ManufacturersPage()
        {
            InitializeComponent();
            LoadData();
        }

        private void LoadData()
        {
            var manufacturersData = manufacturersAdapter.GetData();
            ManufacturersDataGrid.ItemsSource = manufacturersData;
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            var addEditWindow = new AddEditManufacturerWindow();
            if (addEditWindow.ShowDialog() == true)
            {
                LoadData();
            }
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            if (ManufacturersDataGrid.SelectedItem == null)
            {
                MessageBox.Show("Выберите производителя для редактирования", "Ошибка",
                               MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var selectedManufacturer = (ManufacturersDataGrid.SelectedItem as DataRowView).Row
                as ElectronicsStoreDataSet.ManufacturersRow;
            var addEditWindow = new AddEditManufacturerWindow(selectedManufacturer);

            if (addEditWindow.ShowDialog() == true)
            {
                LoadData();
            }
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (ManufacturersDataGrid.SelectedItem == null)
            {
                MessageBox.Show("Выберите производителя для удаления", "Ошибка",
                               MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var selectedManufacturer = (ManufacturersDataGrid.SelectedItem as DataRowView).Row
                as ElectronicsStoreDataSet.ManufacturersRow;

            try
            {
                manufacturersAdapter.DeleteManufacturers(selectedManufacturer.ManufacturerID);
                LoadData();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Не удалось удалить производителя: {ex.Message}\n\n" +
                               "Возможно, этот производитель используется в таблице товаров.",
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

        private void ManufacturersDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            EditButton.IsEnabled = DeleteButton.IsEnabled = ManufacturersDataGrid.SelectedItem != null;
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            EditButton.IsEnabled = DeleteButton.IsEnabled = ManufacturersDataGrid.SelectedItem != null;
        }
    }
}
