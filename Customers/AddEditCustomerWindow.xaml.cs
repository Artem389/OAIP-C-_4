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

namespace WpfApp1.Customers
{
    /// <summary>
    /// Логика взаимодействия для AddEditCustomerWindow.xaml
    /// </summary>
    public partial class AddEditCustomerWindow : Window
    {
        private CustomersTableAdapter customersAdapter = new CustomersTableAdapter();

        private bool isEditMode = false;
        private DataRow customerRow;
        
        public AddEditCustomerWindow()
        {
            InitializeComponent();
        }

        public AddEditCustomerWindow(DataRow row) : this()
        {
            isEditMode = true;
            customerRow = row;
            Title = "Редактирование клиента";

            // Заполняем поля данными из выбранной строки
            FirstNameTextBox.Text = row["FirstName"].ToString();
            LastNameTextBox.Text = row["LastName"].ToString();
            EmailTextBox.Text = row["Email"].ToString();
            PhoneTextBox.Text = row["Phone"].ToString();
        }

        private string phoneFormat = "+7-___-___-__-__";
        private bool isFormatting = false; // Флаг для предотвращения рекурсии при форматировании
        private int[] separatorPositions = { 2, 6, 10, 13 }; // Позиции статичных тире

        private void PhoneTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            var textBox = sender as TextBox;
            int caretPos = textBox.CaretIndex;

            // Запрещаем ввод в позициях тире
            if (separatorPositions.Contains(caretPos))
            {
                textBox.CaretIndex = caretPos + 1; //перемещение курсора
                e.Handled = true; //Отмена ввода
                return;
            }

            // Разрешаем только цифры
            if (!char.IsDigit(e.Text, 0))
            {
                e.Handled = true;
                return;
            }

            // Получаем текущие цифры (без форматирования)
            string digits = GetDigitsFromPhone(textBox.Text);

            // Проверка на наличие 10 цифр (без +7)
            if (digits.Length >= 10 && string.IsNullOrEmpty(textBox.SelectedText))
            {
                e.Handled = true;
                return;
            }

            isFormatting = true; 

            // Вставляем новую цифру
            int digitPos = GetDigitPosition(caretPos);
            string newDigits = digits.Insert(digitPos, e.Text);
            textBox.Text = FormatPhoneNumber(newDigits);

            // Перемещаем курсор на следующую позицию
            textBox.CaretIndex = GetNextCursorPosition(caretPos);

            isFormatting = false;
            e.Handled = true; // Отменяем стандартную обработку
        }

        private void PhoneTextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            var textBox = sender as TextBox;
            int caretPos = textBox.CaretIndex;
            int selLength = textBox.SelectionLength;

            // Обработка Backspace
            if (e.Key == Key.Back)
            {
                if (selLength > 0)
                {
                    // Удаление выделенного текста
                    DeleteSelectedText(textBox);
                    e.Handled = true;
                }
                else if (caretPos > 3) // Пропускаем +7-
                {
                    // Если перед курсором тире - перепрыгиваем его
                    if (separatorPositions.Contains(caretPos - 1))
                    {
                        textBox.CaretIndex = caretPos - 1;
                        e.Handled = true;
                    }
                    else
                    {
                        // Удаляем цифру перед курсором
                        DeleteDigitAtPosition(textBox, caretPos - 1);
                        e.Handled = true;
                    }
                }
                else
                {
                    e.Handled = true;
                }
            }
            // Обработка Delete
            else if (e.Key == Key.Delete)
            {
                if (selLength > 0)
                {
                    // Удаление выделенного текста
                    DeleteSelectedText(textBox);
                    e.Handled = true;
                }
                else if (caretPos < textBox.Text.Length)
                {
                    // Если после курсора тире - перепрыгиваем его
                    if (separatorPositions.Contains(caretPos))
                    {
                        textBox.CaretIndex = caretPos + 1;
                        e.Handled = true;
                    }
                    else
                    {
                        // Удаляем цифру после курсора
                        DeleteDigitAtPosition(textBox, caretPos);
                        e.Handled = true;
                    }
                }
                else
                {
                    e.Handled = true;
                }
            }
        }

        private void DeleteSelectedText(TextBox textBox)
        {
            isFormatting = true;

            string digits = GetDigitsFromPhone(textBox.Text);
            int startPos = GetDigitPosition(textBox.SelectionStart);
            int length = GetDigitPosition(textBox.SelectionStart + textBox.SelectionLength) - startPos;

            digits = digits.Remove(startPos, length);
            textBox.Text = FormatPhoneNumber(digits);

            textBox.CaretIndex = textBox.SelectionStart;
            textBox.SelectionLength = 0;

            isFormatting = false;
        }

        private void DeleteDigitAtPosition(TextBox textBox, int position)
        {
            if (separatorPositions.Contains(position)) return;

            isFormatting = true;

            string digits = GetDigitsFromPhone(textBox.Text);
            int digitPos = GetDigitPosition(position);

            if (digitPos >= 0 && digitPos < digits.Length)
            {
                digits = digits.Remove(digitPos, 1); // Удаляем цифру
                textBox.Text = FormatPhoneNumber(digits);// Обновляем формат

                // После удаления оставляем курсор на той же позиции (если это возможно)
                if (position > 0 && position <= textBox.Text.Length)
                {
                    // Если на новой позиции оказалось тире - перепрыгиваем его
                    if (separatorPositions.Contains(position - 1))
                        textBox.CaretIndex = position - 1;
                    else
                        textBox.CaretIndex = position; // Возвращаем курсор на место
                }
            }

            isFormatting = false;
        }

        private int GetNextCursorPosition(int currentPos)
        {
            if (currentPos < 3) return 3; // После +7-

            // Пропускаем тире
            if (separatorPositions.Contains(currentPos + 1))
                return currentPos + 2;

            return currentPos + 1;
        }

        private int GetDigitPosition(int formattedPosition) // Получаем позицию цифры в неформатированной строке
        {
            if (formattedPosition <= 3) return 0; // Для +7-

            int digitPos = 0;
            for (int i = 3; i < formattedPosition; i++) // Начинаем после +7-
            {
                if (!separatorPositions.Contains(i))
                    digitPos++;
            }
            return digitPos;
        }

        private string GetDigitsFromPhone(string phone)
        {
            if (string.IsNullOrEmpty(phone)) return "";
            return new string(phone.Where(char.IsDigit).ToArray()).Substring(1); // Убираем первую 7
        }

        private string FormatPhoneNumber(string digits)
        {
            if (string.IsNullOrEmpty(digits)) return phoneFormat.Replace('_', ' ');

            // Ограничиваем 10 цифрами (без +7)
            digits = digits.Length > 10 ? digits.Substring(0, 10) : digits;

            var formatted = new StringBuilder("+7-");
            int digitIndex = 0;

            // Первая группа (3 цифры)
            for (int i = 0; i < 3 && digitIndex < digits.Length; i++)
                formatted.Append(digits[digitIndex++]);
            while (formatted.Length < 6) formatted.Append('_');

            formatted.Append('-');

            // Вторая группа (3 цифры)
            for (int i = 0; i < 3 && digitIndex < digits.Length; i++)
                formatted.Append(digits[digitIndex++]);
            while (formatted.Length < 10) formatted.Append('_');

            formatted.Append('-');

            // Третья группа (2 цифры)
            for (int i = 0; i < 2 && digitIndex < digits.Length; i++)
                formatted.Append(digits[digitIndex++]);
            while (formatted.Length < 13) formatted.Append('_');

            formatted.Append('-');

            // Четвертая группа (2 цифры)
            for (int i = 0; i < 2 && digitIndex < digits.Length; i++)
                formatted.Append(digits[digitIndex++]);
            while (formatted.Length < 16) formatted.Append('_');

            return formatted.ToString();
        }

        private void PhoneTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (isFormatting) return;

            var textBox = sender as TextBox;

            // Восстанавливаем формат если он был нарушен
            if (!IsValidPhoneFormat(textBox.Text))
            {
                var digits = GetDigitsFromPhone(textBox.Text);
                textBox.Text = FormatPhoneNumber(digits);
                textBox.CaretIndex = textBox.Text.Any(c => c == '_') ?
                    textBox.Text.IndexOf('_') : textBox.Text.Length;
            }
        }

        private bool IsValidPhoneFormat(string phone)
        {
            if (phone.Length != phoneFormat.Length) return false;
            if (!phone.StartsWith("+7-")) return false;

            for (int i = 0; i < phone.Length; i++)
            {
                if (separatorPositions.Contains(i))
                {
                    if (phone[i] != '-') return false;
                }
                else if (i > 2) // Пропускаем +7-
                {
                    if (!char.IsDigit(phone[i]) && phone[i] != '_') return false;
                }
            }

            return true;
        }

        private void PhoneTextBox_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            var textBox = sender as TextBox;

            // Разрешаем стандартную обработку клика
            e.Handled = false;

            // Всегда ставим курсор после "+7-" (позиция 3)
            textBox.CaretIndex = 3;

            // После стандартной обработки (которая установит курсор) проверим его позицию
            Dispatcher.BeginInvoke(new Action(() =>
            {
                // Если курсор оказался на тире или перед "+7-", перемещаем его
                if (separatorPositions.Contains(textBox.CaretIndex) || textBox.CaretIndex < 3)
                {
                    // Ищем ближайшую допустимую позицию справа
                    int newPos = textBox.CaretIndex;
                    while (newPos < textBox.Text.Length &&
                          (separatorPositions.Contains(newPos) || newPos < 3))
                    {
                        newPos++;
                    }

                    if (newPos < textBox.Text.Length)
                        textBox.CaretIndex = newPos;
                    else
                        textBox.CaretIndex = textBox.Text.Length;
                }
            }), System.Windows.Threading.DispatcherPriority.Input);

            // Фокусируем поле, если еще не в фокусе
            if (!textBox.IsFocused)
            {
                textBox.Focus();
            }

            e.Handled = true;
        }

        private void PhoneTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            var textBox = sender as TextBox;
            if (textBox.Text.Length <= 3 || !IsValidPhoneFormat(textBox.Text)) // Всегда ставим курсор после "+7-"
            {
                textBox.Text = phoneFormat.Replace('_', ' ');
            }

            // Устанавливаем курсор после "+7-" или на первую пустую позицию
            textBox.CaretIndex = 3;
        }

        private void PhoneTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            var textBox = sender as TextBox;
            if (textBox.Text == phoneFormat.Replace('_', ' '))
            {
                textBox.Text = phoneFormat.Replace('_', ' ');
            }
        }
        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidateInput())
                return;

            try
            {
                var phone = new string(PhoneTextBox.Text.Where(char.IsDigit).ToArray());
                if (phone.Length > 1) phone = phone.Substring(1); // Убираем 7, так как она уже есть в маске

                if (isEditMode)
                {
                    customersAdapter.UpdateCustomers(
                        FirstNameTextBox.Text,
                        LastNameTextBox.Text,
                        EmailTextBox.Text,
                        PhoneTextBox.Text,
                        (int)customerRow["CustomerID"]
                        //customerRow["FirstName"].ToString(),
                        //customerRow["LastName"].ToString(),
                        //customerRow["Email"] as string,
                        //customerRow["Phone"] as string
                    );
                }
                else
                {
                    customersAdapter.InsertCustomers(
                        FirstNameTextBox.Text,
                        LastNameTextBox.Text,
                        EmailTextBox.Text,
                        PhoneTextBox.Text
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
            if (string.IsNullOrWhiteSpace(FirstNameTextBox.Text))
            {
                MessageBox.Show("Введите имя клиента", "Ошибка",
                              MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (string.IsNullOrWhiteSpace(LastNameTextBox.Text))
            {
                MessageBox.Show("Введите фамилию клиента", "Ошибка",
                              MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (string.IsNullOrWhiteSpace(EmailTextBox.Text))
            {
                MessageBox.Show("Введите корректный email", "Ошибка",
                              MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            var phoneDigits = new string(PhoneTextBox.Text.Where(char.IsDigit).ToArray());
            if (phoneDigits.Length != 11)
            {
                MessageBox.Show("Введите корректный номер телефона (11 цифр)", "Ошибка",
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
