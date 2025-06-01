using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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

namespace WpfApp1.Manufacturers
{
    /// <summary>
    /// Логика взаимодействия для AddEditManufacturerWindow.xaml
    /// </summary>
    public partial class AddEditManufacturerWindow : Window
    {
        private ManufacturersTableAdapter manufacturersAdapter = new ManufacturersTableAdapter();
        private bool isEditMode = false;
        private DataRow manufacturerRow;
        public AddEditManufacturerWindow()
        {
            InitializeComponent();
        }

        public AddEditManufacturerWindow(DataRow row) : this()
        {
            isEditMode = true;
            manufacturerRow = row;
            Title = "Редактирование производителя";

            // Заполняем поля данными из выбранной строки
            NameTextBox.Text = row["Name"].ToString();
            CountryTextBox.Text = row["Country"].ToString();
            WebsiteTextBox.Text = row["Website"].ToString();
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidateInput())
                return;

            try
            {
                if (isEditMode)
                {
                    manufacturersAdapter.UpdateManufacturers(
                        NameTextBox.Text,
                        CountryTextBox.Text,
                        WebsiteTextBox.Text,
                        (int)manufacturerRow["ManufacturerID"]
                        //manufacturerRow["Name"].ToString()
                        //manufacturerRow["Country"] as string
                        //manufacturerRow["Website"] as string
                    );
                }
                else
                {
                    manufacturersAdapter.InsertManufacturers(
                        NameTextBox.Text,
                        CountryTextBox.Text,
                        WebsiteTextBox.Text
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
                MessageBox.Show("Введите название производителя", "Ошибка",
                              MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (string.IsNullOrWhiteSpace(WebsiteTextBox.Text))
            {
                MessageBox.Show("Введите URL веб-сайта", "Ошибка",
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
