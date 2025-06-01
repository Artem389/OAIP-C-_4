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

namespace WpfApp1.Categories
{
    /// <summary>
    /// Логика взаимодействия для CategoriesPage.xaml
    /// </summary>
    public partial class CategoriesPage : Page
    {
        private CategoriesTableAdapter categoriesAdapter = new CategoriesTableAdapter();
        public CategoriesPage()
        {
            InitializeComponent();
            LoadData();
        }

        private void LoadData()
        {
            var categoriesData = categoriesAdapter.GetData();
            CategoriesDataGrid.ItemsSource = categoriesData;
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            var addEditWindow = new AddEditCategoryWindow();
            if (addEditWindow.ShowDialog() == true)
            {
                LoadData();
            }
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            if (CategoriesDataGrid.SelectedItem == null)
            {
                MessageBox.Show("Выберите категорию для редактирования", "Ошибка",
                               MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var selectedCategory = (CategoriesDataGrid.SelectedItem as DataRowView).Row as ElectronicsStoreDataSet.CategoriesRow;
            var addEditWindow = new AddEditCategoryWindow(selectedCategory);

            if (addEditWindow.ShowDialog() == true)
            {
                LoadData();
            }
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (CategoriesDataGrid.SelectedItem == null)
            {
                MessageBox.Show("Выберите категорию для удаления", "Ошибка",
                               MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var selectedCategory = (CategoriesDataGrid.SelectedItem as DataRowView).Row as ElectronicsStoreDataSet.CategoriesRow;

            try
            {
                categoriesAdapter.DeleteCategories(selectedCategory.CategoryID);
                LoadData();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Не удалось удалить категорию: {ex.Message}\n\n" +
                               "Возможно, эта категория используется в таблице товаров.",
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

        private void CategoriesDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            EditButton.IsEnabled = DeleteButton.IsEnabled = CategoriesDataGrid.SelectedItem != null;
        }
    }
}
