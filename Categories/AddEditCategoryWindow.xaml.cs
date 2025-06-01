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

namespace WpfApp1.Categories
{
    /// <summary>
    /// Логика взаимодействия для AddEditCategoryWindow.xaml
    /// </summary>
    public partial class AddEditCategoryWindow : Window
    {
        private CategoriesTableAdapter categoriesAdapter = new CategoriesTableAdapter();

        private bool isEditMode = false;
        private DataRow categoryRow;

        public AddEditCategoryWindow()
        {
            InitializeComponent();
        }

        public AddEditCategoryWindow(DataRow row) : this()
        {
            isEditMode = true;
            categoryRow = row;
            Title = "Редактирование категории";

            // Заполняем поля данными из выбранной строки
            NameTextBox.Text = row["Name"].ToString();
            DescriptionTextBox.Text = row["Description"].ToString();
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidateInput())
                return;

            try
            {
                if (isEditMode)
                {
                    categoriesAdapter.UpdateCategories(
                        NameTextBox.Text,
                        DescriptionTextBox.Text,
                        (int)categoryRow["CategoryID"]
                        //categoryRow["Name"].ToString(),
                        //categoryRow["Description"] as string
                    );
                }
                else
                {
                    categoriesAdapter.InsertCategories(
                        NameTextBox.Text,
                        DescriptionTextBox.Text
                    );
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
                MessageBox.Show("Введите название категории", "Ошибка",
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
