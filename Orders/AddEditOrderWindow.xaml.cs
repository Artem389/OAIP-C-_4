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

namespace WpfApp1.Orders
{
    /// <summary>
    /// Логика взаимодействия для AddEditOrderWindow.xaml
    /// </summary>
    public partial class AddEditOrderWindow : Window
    {
        private OrdersTableAdapter ordersAdapter = new OrdersTableAdapter();
        private CustomersTableAdapter customersAdapter;
        private ProductsTableAdapter productsAdapter;

        private bool isEditMode = false;
        private DataRow orderRow;
        public AddEditOrderWindow(CustomersTableAdapter custAdapter, ProductsTableAdapter prodAdapter)
        {
            InitializeComponent();
            customersAdapter = custAdapter;
            productsAdapter = prodAdapter;
            LoadComboBoxes();
        }

        public AddEditOrderWindow(CustomersTableAdapter custAdapter, ProductsTableAdapter prodAdapter, DataRow row)
           : this(custAdapter, prodAdapter)
        {
            isEditMode = true;
            orderRow = row;
            Title = "Редактирование заказа";

            // Заполняем поля данными из выбранной строки
            CustomerComboBox.SelectedValue = row["CustomerID"];
            ProductComboBox.SelectedValue = row["ProductID"];
            QuantityTextBox.Text = row["Quantity"].ToString();

            // Устанавливаем статус
            foreach (ComboBoxItem item in StatusComboBox.Items)
            {
                if (item.Content.ToString() == row["Status"].ToString())
                {
                    StatusComboBox.SelectedItem = item;
                    break;
                }
            }
        }

        private void LoadComboBoxes()
        {
            // Загрузка клиентов с объединением имени и фамилии
            var customers = customersAdapter.GetData()
                .Select(c => new
                {
                    CustomerID = c.CustomerID,
                    FullName = c.FirstName + " " + c.LastName
                }).ToList();

            CustomerComboBox.ItemsSource = customers;
            CustomerComboBox.SelectedValuePath = "CustomerID";

            // Загрузка товаров
            ProductComboBox.ItemsSource = productsAdapter.GetData();
            ProductComboBox.SelectedValuePath = "ProductID";

            // Установка статуса по умолчанию
            StatusComboBox.SelectedIndex = 0;
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidateInput())
                return;

            try
            {
                int customerId = (int)CustomerComboBox.SelectedValue;
                int productId = (int)ProductComboBox.SelectedValue;
                int quantity = int.Parse(QuantityTextBox.Text);
                string status = (StatusComboBox.SelectedItem as ComboBoxItem).Content.ToString();

                // Получаем цену товара для расчета суммы
                var product = productsAdapter.GetData()
                    .FirstOrDefault(p => p.ProductID == productId);
                decimal totalAmount = product.Price * quantity;

                if (isEditMode)
                {
                    ordersAdapter.UpdateOrders(
                        customerId,
                        productId,
                        DateTime.Now.ToString("yyyy-MM-dd"),
                        quantity,
                        totalAmount,
                        status,
                        (int)orderRow["OrderID"]
                        //(int)orderRow["CustomerID"],
                        //(int)orderRow["ProductID"],
                        //orderRow["OrderDate"].ToString(),
                        //(int)orderRow["Quantity"],
                        //(decimal)orderRow["TotalAmount"],
                        //orderRow["Status"].ToString()
                    );
                }
                else
                {
                    ordersAdapter.InsertOrders(
                        customerId,
                        productId,
                        DateTime.Now.ToString("yyyy-MM-dd"),
                        quantity,
                        totalAmount,
                        status
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
            if (CustomerComboBox.SelectedItem == null)
            {
                MessageBox.Show("Выберите клиента", "Ошибка",
                              MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (ProductComboBox.SelectedItem == null)
            {
                MessageBox.Show("Выберите товар", "Ошибка",
                              MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (!int.TryParse(QuantityTextBox.Text, out int quantity) || quantity <= 0)
            {
                MessageBox.Show("Введите корректное количество (целое положительное число)", "Ошибка",
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
