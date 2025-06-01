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
using System.Windows.Shapes;
using WpfApp1.ElectronicsStoreDataSetTableAdapters;

namespace WpfApp1.Products
{
    /// <summary>
    /// Логика взаимодействия для AddEditProductWindow.xaml
    /// </summary>
    public partial class AddEditProductWindow : Window
    {
        private ProductsTableAdapter productsAdapter = new ProductsTableAdapter();
        private ManufacturersTableAdapter manufacturersAdapter = new ManufacturersTableAdapter();
        private CategoriesTableAdapter categoriesAdapter = new CategoriesTableAdapter();

        private bool isEditMode = false;
        private DataRow productRow;

        public AddEditProductWindow()
        {
            InitializeComponent();
            LoadComboBoxes();
        }

        public AddEditProductWindow(DataRow row) : this()
        {
            isEditMode = true;
            productRow = row;
            Title = "Редактирование товара";

            // Заполняем поля данными из выбранной строки
            NameTextBox.Text = row["Name"].ToString();
            DescriptionTextBox.Text = row["Description"].ToString();
            PriceTextBox.Text = row["Price"].ToString();
            StockQuantityTextBox.Text = row["StockQuantity"].ToString();

            // Устанавливаем выбранные значения в ComboBox
            ManufacturerComboBox.SelectedValue = row["ManufacturerID"];
            CategoryComboBox.SelectedValue = row["CategoryID"];
        }

        private void LoadComboBoxes()
        {
            ManufacturerComboBox.ItemsSource = manufacturersAdapter.GetData();
            ManufacturerComboBox.SelectedValuePath = "ManufacturerID";

            CategoryComboBox.ItemsSource = categoriesAdapter.GetData();
            CategoryComboBox.SelectedValuePath = "CategoryID";
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidateInput())
                return;

            try
            {
                if (isEditMode)
                {
                    productsAdapter.UpdateProducts(
                        NameTextBox.Text,
                        DescriptionTextBox.Text,
                        decimal.Parse(PriceTextBox.Text),
                        int.Parse(StockQuantityTextBox.Text),
                        (int)ManufacturerComboBox.SelectedValue,
                        (int)CategoryComboBox.SelectedValue,
                        //productRow["Name"].ToString(),
                        //productRow["Description"] as string,
                        //(decimal)productRow["Price"],
                        //(int)productRow["StockQuantity"],
                        //(int)productRow["ManufacturerID"],
                        //(int)productRow["CategoryID"],
                        (int)productRow["ProductID"]
                    );
                }
                else
                {
                    productsAdapter.InsertProducts(
                        NameTextBox.Text,
                        DescriptionTextBox.Text,
                        decimal.Parse(PriceTextBox.Text),
                        int.Parse(StockQuantityTextBox.Text),
                        (int)ManufacturerComboBox.SelectedValue,
                        (int)CategoryComboBox.SelectedValue);
                }

                DialogResult = true;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при сохранении: {ex.Message}", "Ошибка",
                              MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private bool ValidateInput()
        {
            if (string.IsNullOrWhiteSpace(NameTextBox.Text))
            {
                MessageBox.Show("Введите название товара", "Ошибка",
                              MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (!decimal.TryParse(PriceTextBox.Text, out decimal price) || price <= 0)
            {
                MessageBox.Show("Введите корректную цену (положительное число)", "Ошибка",
                              MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (!int.TryParse(StockQuantityTextBox.Text, out int quantity) || quantity < 0)
            {
                MessageBox.Show("Введите корректное количество (целое неотрицательное число)", "Ошибка",
                              MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (ManufacturerComboBox.SelectedItem == null)
            {
                MessageBox.Show("Выберите производителя", "Ошибка",
                              MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (CategoryComboBox.SelectedItem == null)
            {
                MessageBox.Show("Выберите категорию", "Ошибка",
                              MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            return true;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
