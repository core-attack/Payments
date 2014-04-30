using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Data;
using System.Text.RegularExpressions;
using System.IO;

namespace PaymentsWPF
{
    /// <summary>
    /// Логика взаимодействия для Admin.xaml
    /// </summary>
    public partial class Admin : Window
    {
        DBRequests DB;
        /// <summary>
        /// инициализировались ли все компоненты
        /// </summary>
        bool isInit = false;

        /// Лог отладки
        /// </summary>
        StreamWriter log_debug;
        /// <summary>
        /// Лог ошибок
        /// </summary>
        StreamWriter log_error;
        /// <summary>
        /// Лог запросов
        /// </summary>
        StreamWriter log_http;

        bool logging_http = false;
        bool logging_debug = false;

        Regex regex;

        public Admin(bool logging_http, bool logging_debug, StreamWriter error, StreamWriter http, StreamWriter debug)
        {
            InitializeComponent();

            this.logging_http = logging_http;
            this.logging_debug = logging_debug;
            log_debug = debug;
            log_error = error;
            log_http = http;
            isInit = true;
            DB = new DBRequests(logging_http, logging_debug, error, http, debug);
            //setAllComboBoxesAndListboxes();
            string pattern = @"^\d+(\,(\d+))?$";
            regex = new Regex(pattern);
        }

        /// <summary>
        /// Обрабатывает возникшую ошибку
        /// </summary>
        /// <param name="e"></param>
        void viewError(Exception e)
        {
            string error = string.Format("Что: {0}\nГде: {1}", e.Message, e.StackTrace);
            MessageBox.Show(error);
            log_error.WriteLine(error);
        }

        /// <summary>
        /// Записывает сообщение в лог
        /// </summary>
        /// <param name="message">Сообщение</param>
        void writeMessage(string message)
        {
            if (logging_debug)
                log_debug.WriteLine(message);
        }

        /// <summary>
        /// Записывает сообщение в лог
        /// </summary>
        /// <param name="sql">Запрос</param>
        /// <param name="result">Результат запроса</param>
        void writeMessage(string sql, string result)
        {
            if (logging_http)
                log_http.WriteLine("sql: {0} -> result: {1}", sql, result);
        }

        /// <summary>
        /// Записывает сообщение в лог
        /// </summary>
        /// <param name="sql">Запрос</param>
        /// <param name="result">Результат запроса</param>
        void writeMessage(string sql, string[] result)
        {
            if (logging_http)
            {
                log_http.WriteLine("sql: {0}", sql);
                log_http.Write("results: ");
                for (int i = 0; i < result.Length; i++)
                {
                    if (i != result.Length - 1)
                        log_http.Write("{0}, ", result[i]);
                    else
                        log_http.Write(result[i]);
                }
            }
        }

        /// <summary>
        /// Заполняет все комбобоксы
        /// </summary>
        void setAllComboBoxesAndListboxes()
        {
            comboBoxRoles.Items.Clear();
            comboBoxRoles.Items.Add("Администратор");
            comboBoxRoles.Items.Add("Пользователь");
            comboBoxRoles.SelectedIndex = 1;

            comboBoxStatus.Items.Clear();
            foreach (string status in DBRequests.STATUS_OF_USER)
                comboBoxStatus.Items.Add(status);
            comboBoxStatus.SelectedIndex = 0;

            comboBoxBankAccountStatus.Items.Clear();
            comboBoxUpdateCardStatus.Items.Clear();
            foreach (string status in DBRequests.STATUS_OF_CARD)
            {
                comboBoxBankAccountStatus.Items.Add(status);
                comboBoxUpdateCardStatus.Items.Add(status);
            }
            comboBoxBankAccountStatus.SelectedIndex = 0;
            comboBoxUpdateCardStatus.SelectedIndex = 0;
            
            loadUserNames();
            loadBankAccountUsers();
            loadAllCards();
            loadBankAccounts();
        }

        /// <summary>
        /// Загружает имена пользователей из БД
        /// </summary>
        void loadUserNames()
        {
            comboBoxUpdateCardUser.Items.Clear();
            comboBoxUsers.Items.Clear();
            listBoxAllUsers.Items.Clear();
            string[] names = DB.getAllUserNames();
            foreach (string s in names)
            {
                comboBoxUpdateCardUser.Items.Add(s);
                if (!s.Equals(labelAdminName.Content.ToString())) 
                    comboBoxUsers.Items.Add(s);
                listBoxAllUsers.Items.Add(s);
            }
            listBoxAllUsers.SelectedIndex = 0;
        }

        /// <summary>
        /// Загружает счета пользователей из БД
        /// </summary>
        void loadBankAccountUsers()
        {
            comboBoxBankAccountUsers.Items.Clear();
            string[] names = DB.getAllUserNames();
            foreach (string s in names)
            {
                comboBoxBankAccountUsers.Items.Add(s);
            }
        }

        private void buttonCreateUser_Click(object sender, RoutedEventArgs e)
        {
            writeMessage("Попытка создания пользователя...");
            if (passwordBoxFirst.Password.Equals(passwordBoxSecond.Password))
            {
                if (!DB.checkUser(textBoxName.Text))
                {
                    DB.createUser(textBoxName.Text, passwordBoxFirst.Password, comboBoxRoles.Text, comboBoxStatus.Text, Convert.ToInt32(labelAdminId.Content));
                    textBoxName.Text = "";
                    passwordBoxFirst.Password = passwordBoxSecond.Password = "";
                    buttonCreateUser.Content = "Пользователь создан";
                    buttonCreateUser.IsEnabled = false;
                    setAllComboBoxesAndListboxes();
                }
                else
                {
                    MessageBox.Show("Пользователь с таким именем уже существует!");
                    buttonCreateUser.Content = "Произошла ошибка";
                    buttonCreateUser.IsEnabled = false;
                    writeMessage("Запрет: пользователь с таким имененм уже существует!");
                }
            }
            else
            {
                MessageBox.Show("Пароли не совпадают!");
                writeMessage("Запрет: Пароли не совпадают!");
            }
        }

        /// <summary>
        /// Запрет создания записей пользователей с пустыми именем и (или) паролем
        /// </summary>
        void buttonCreateUserEnabled()
        {
            if (textBoxName.Text != "" && passwordBoxFirst.Password != "")
            {
                buttonCreateUser.IsEnabled = true;
                buttonCreateUser.Content = "Создать пользователя";
                writeMessage("Пользователь может быть создан.");
            }
            else
            {
                buttonCreateUser.IsEnabled = false;
                writeMessage("Запрет: создание пользователя с пустым именем и паролем недопустимо!");
            }
        }

        private void textBoxName_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (isInit)
                buttonCreateUserEnabled();
        }

        private void passwordBoxFirst_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (isInit)
                buttonCreateUserEnabled();
        }

        private void buttonDeleteUser_Click(object sender, RoutedEventArgs e)
        {
            writeMessage("Попытка удаления пользователя...");
            if (comboBoxUsers.SelectedIndex >= 0)
            {
                if (MessageBox.Show("Вы уверены?", "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Warning, MessageBoxResult.No) == MessageBoxResult.Yes)
                {
                    if (comboBoxUsers.Text.Equals("Пискарев Николай Сергеевич"))
                        labelMessage.Content = "Не стоит удалять разработчика приложения.";
                    else
                    {
                        DB.deleteUser(comboBoxUsers.Text, textBoxReason.Text, labelAdminName.Content.ToString());
                        textBoxReason.Text = "";
                        labelMessage.Content = "Пользователь успешно удален";
                        setAllComboBoxesAndListboxes();
                    }
                }
            }
            else
                MessageBox.Show("Выберите пользователя!");
        }

        private void buttonGenerateBankAccount_Click(object sender, RoutedEventArgs e)
        {
            generateBankAccount();
        }

        /// <summary>
        /// Создает счет
        /// </summary>
        public void generateBankAccount()
        {
            writeMessage("Генерация номера счета...");
            string bankAccount = "";
            do
            {
                bankAccount = "";
                Random r = new Random();
                for (int i = 0; i < 20; i++)
                    if (i == 0)
                        bankAccount += r.Next(1, 10).ToString();
                    else
                        bankAccount += r.Next(0, 10).ToString();
            }
            while (DB.checkBankAccount(bankAccount));
            writeMessage("Номер счета сгенерирован: " + bankAccount + ".");
            textBoxBankAccount.Text = bankAccount;
        }

        private void buttonCreateCard_Click(object sender, RoutedEventArgs e)
        {
            writeMessage("Попытка создания записи счета...");
            if (comboBoxBankAccountUsers.SelectedIndex >= 0)
            {
                if (DB.checkBankAccount(textBoxBankAccount.Text))
                    labelCreateBankAccountOK.Content = "Такой номер счета уже существует!";
                else if (textBoxBankAccount.Text == "")
                {
                    labelCreateBankAccountOK.Content = "Введите номер счета!";
                    textBoxBankAccount.Focus();
                }
                else
                {
                    if (comboBoxBankAccountStatus.SelectedIndex >= 0)
                    {
                        DB.createCard(textBoxBankAccount.Text, comboBoxBankAccountStatus.Text, DB.getUserId(comboBoxBankAccountUsers.Text), Convert.ToInt32(labelAdminId.Content));
                        labelCreateBankAccountOK.Content = "Запись счета успешно создана";
                        textBoxBankAccount.Text = "";
                        comboBoxBankAccountUsers.Text = "";
                        setAllComboBoxesAndListboxes();
                    }
                    else
                        MessageBox.Show("Выберите статус счета!");
                }
            }
            else
                MessageBox.Show("Выберите пользователя!");
        }

        /// <summary>
        /// Загружает все счета
        /// </summary>
        void loadAllCards()
        {
            try
            {
                listBoxCards.Items.Clear();
                DataSet cards = DB.getAllCards();
                if (cards != null)
                {
                    for (int i = 0; i < cards.Tables["cards"].Rows.Count; i++)
                    {
                        /*listBoxCards.Items.Add(string.Format("id: {0}, Счет: {1}, Остаток: {2}, Статус: {3}, Клиент: {4}, Создатель: {5}, Дата создания: {6}, Блокировал: {7}, Дата блокировки: {8}",
                            cards.Tables["cards"].Rows[i].ItemArray[0],
                            cards.Tables["cards"].Rows[i].ItemArray[1],
                            cards.Tables["cards"].Rows[i].ItemArray[2],
                            cards.Tables["cards"].Rows[i].ItemArray[3],
                            DB.getUserName(Convert.ToInt32(cards.Tables["cards"].Rows[i].ItemArray[4])),
                            DB.getUserName(Convert.ToInt32(cards.Tables["cards"].Rows[i].ItemArray[5])),
                            cards.Tables["cards"].Rows[i].ItemArray[6],
                            cards.Tables["cards"].Rows[i].ItemArray[7].GetType() == System.Type.GetType("DBNull") ? DB.getUserName(Convert.ToInt32(cards.Tables["cards"].Rows[i].ItemArray[7])) : "нет",
                            cards.Tables["cards"].Rows[i].ItemArray[8].GetType() == System.Type.GetType("DBNull") ? cards.Tables["cards"].Rows[i].ItemArray[8] : "нет"
                            ));
                         * */
                        listBoxCards.Items.Add(cards.Tables["cards"].Rows[i].ItemArray[1]);
                    }
                    listBoxCards.SelectedIndex = 0;
                }
            }
            catch(Exception e)
            {
                viewError(e);
            }
        }

        private void tabControl1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void listBoxAllUsers_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (listBoxAllUsers.SelectedIndex >= 0 && listBoxAllUsers.SelectedIndex < listBoxAllUsers.Items.Count)
                loadUserInfo(listBoxAllUsers.Items[listBoxAllUsers.SelectedIndex].ToString());
        }

        /// <summary>
        /// Загружает информацию о пользователе из БД
        /// </summary>
        /// <param name="name"></param>
        void loadUserInfo(string name)
        {
            try
            {
                DataSet user = DB.getUserInfo(name);
                int id = (int)user.Tables["users"].Rows[0].ItemArray[0];
                string crypted_password = user.Tables["users"].Rows[0].ItemArray[2].ToString();
                string role = user.Tables["users"].Rows[0].ItemArray[3].ToString();
                string status = user.Tables["users"].Rows[0].ItemArray[4].ToString();

                if (user.Tables["users"].Rows[0].ItemArray[5].GetType() != System.Type.GetType("System.DBNull"))
                {
                    DateTime block_date = (DateTime)user.Tables["users"].Rows[0].ItemArray[5];
                    labelSelectedUserBlockedDate.Content = "Дата блокировки: " + block_date.ToShortDateString() + " " + block_date.ToShortTimeString();
                }
                else
                    labelSelectedUserBlockedDate.Content = "";
                if (user.Tables["users"].Rows[0].ItemArray[6].GetType() != System.Type.GetType("System.DBNull"))
                {
                    int created_admin_id = (int)user.Tables["users"].Rows[0].ItemArray[6];
                    labelSelectedUserCreatedAdmin.Content = "Запись создал: " + DB.getUserName(created_admin_id);
                }
                else
                    labelSelectedUserCreatedAdmin.Content = "";
                if (user.Tables["users"].Rows[0].ItemArray[7].GetType() != System.Type.GetType("System.DBNull"))
                {
                    int blocked_admin_id = (int)user.Tables["users"].Rows[0].ItemArray[7];
                    labelSelectedUserBlockedAdmin.Content = "Заблокировал: " + DB.getUserName(blocked_admin_id);
                }
                else
                    labelSelectedUserBlockedAdmin.Content = "";

                labelSelectedUserName.Content = "ФИО: " + name;
                labelSelectedUserRole.Content = "Роль: " + role;
                labelSelectedUserStatus.Content = "Статус аккаунта: " + status;
                
                DataSet cards = DB.getAllCards(id);
                listBoxSelectedUserCards.Items.Clear();
                for (int i = 0; i < cards.Tables["cards"].Rows.Count; i++)
                    listBoxSelectedUserCards.Items.Add(string.Format("Счет №: {0} Остаток: {1} Статус: {2}", cards.Tables["cards"].Rows[i].ItemArray[1],
                                                                        cards.Tables["cards"].Rows[i].ItemArray[2],
                                                                            cards.Tables["cards"].Rows[i].ItemArray[3]));
                listBoxSelectedUserPayments.Items.Clear();
                DataSet payments = DB.getAllPaymentsByUser(id);
                for (int i = 0; i < payments.Tables["payments"].Rows.Count; i++)
                {
                    listBoxSelectedUserPayments.Items.Add(string.Format("Дата: {1} Платеж: {0} Статус: {2}",
                                                                        (double)payments.Tables["payments"].Rows[i].ItemArray[1],
                                                                        ((DateTime)payments.Tables["payments"].Rows[i].ItemArray[2]).ToString(),
                                                                        (bool)payments.Tables["payments"].Rows[i].ItemArray[3] == true ? "Совершен" : "Ожидание"));
                }
            }
            catch (Exception e)
            {
                viewError(e);
            }
        }

        private void listBoxCards_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (listBoxCards.SelectedIndex >= 0 && listBoxCards.SelectedIndex < listBoxCards.Items.Count)
                loadCardInfo(listBoxCards.Items[listBoxCards.SelectedIndex].ToString());
        }

        /// <summary>
        /// Загружает информацию о счете из БД
        /// </summary>
        void loadCardInfo(string bankAccount)
        {
            try
            {
                DataSet card = DB.getCardInfo(bankAccount);
                int id = (int)card.Tables["cards"].Rows[0].ItemArray[0];
                double rest = (double)card.Tables["cards"].Rows[0].ItemArray[2];
                string status = card.Tables["cards"].Rows[0].ItemArray[3].ToString();
                string user = DB.getUserName((int)card.Tables["cards"].Rows[0].ItemArray[4]);

                labelSelectedCardRest.Content = "Остаток на счете: " + rest.ToString();
                labelSelectedCardStatus.Content = "Статус счета: " + status;
                labelSelectedCardUser.Content = "Владелец счета: " + user;

                if (card.Tables["cards"].Rows[0].ItemArray[5].GetType() != System.Type.GetType("System.DBNull"))
                {
                    int created_admin_id = (int)card.Tables["cards"].Rows[0].ItemArray[5];
                    labelSelectedCardCreatedAdmin.Content = "Запись создал: " + DB.getUserName(created_admin_id);
                }
                else
                    labelSelectedCardCreatedAdmin.Content = "";

                if (card.Tables["cards"].Rows[0].ItemArray[6].GetType() != System.Type.GetType("System.DBNull"))
                {
                    DateTime openDate = (DateTime)card.Tables["cards"].Rows[0].ItemArray[6];
                    labelSelectedCardCreatedDate.Content = "Дата открытия: " + openDate.ToShortDateString() + " " + openDate.ToShortTimeString();
                }
                else
                    labelSelectedCardCreatedDate.Content = "";

                if (card.Tables["cards"].Rows[0].ItemArray[7].GetType() != System.Type.GetType("System.DBNull"))
                {
                    int blocked_admin_id = (int)card.Tables["cards"].Rows[0].ItemArray[7];
                    labelSelectedCardBlockedAdmin.Content = "Заблокировал: " + DB.getUserName(blocked_admin_id);
                }
                else
                    labelSelectedCardBlockedAdmin.Content = "";

                if (card.Tables["cards"].Rows[0].ItemArray[8].GetType() != System.Type.GetType("System.DBNull"))
                {
                    DateTime block_date = (DateTime)card.Tables["cards"].Rows[0].ItemArray[8];
                    labelSelectedCardBlockedDate.Content = "Дата блокировки: " + block_date.ToShortDateString() + " " + block_date.ToShortTimeString();
                }
                else
                    labelSelectedCardBlockedDate.Content = "";

            }
            catch (Exception e)
            {
                viewError(e);
            }
        }

        /// <summary>
        /// Загружает номера счетов в комбобокс из БД
        /// </summary>
        void loadBankAccounts()
        {
            comboBoxUpdateCard.Items.Clear();
            comboBoxDeleteCard.Items.Clear();
            string[] names = DB.getAllBankAccounts();
            foreach (string s in names)
            {
                comboBoxUpdateCard.Items.Add(s);
                comboBoxDeleteCard.Items.Add(s);
            }
        }

        /// <summary>
        /// Обновляет счет
        /// </summary>
        void updateCard(string bankAccount)
        {
            DB.updateCard(bankAccount, Convert.ToDouble(textBoxUpdateCardRest.Text), comboBoxUpdateCardStatus.Text, DB.getUserId(comboBoxUpdateCardUser.Text), labelAdminName.Content.ToString());
        }

        private void buttonUpdateCard_Click(object sender, RoutedEventArgs e)
        {
            writeMessage("Попытка обновления записи счета...");
            if (comboBoxUpdateCard.SelectedIndex >= 0)
            {
                updateCard(comboBoxUpdateCard.Text);
                labelUpdateCardMessage.Content = "Поля счета обновлены успешно";
            }
            else
                MessageBox.Show("Выберите номер счета!");
        }

        private void comboBoxUpdateCard_DropDownClosed(object sender, EventArgs e)
        {
            try
            {
                if (comboBoxUpdateCard.Text != "")
                {
                    DataSet card = DB.getCardInfo(comboBoxUpdateCard.Text);
                    int id = (int)card.Tables["cards"].Rows[0].ItemArray[0];
                    double rest = (double)card.Tables["cards"].Rows[0].ItemArray[2];
                    string status = card.Tables["cards"].Rows[0].ItemArray[3].ToString();
                    string user = DB.getUserName((int)card.Tables["cards"].Rows[0].ItemArray[4]);
                    textBoxUpdateCardRest.Text = rest.ToString();
                    comboBoxUpdateCardStatus.Text = status;
                    comboBoxUpdateCardUser.Text = user;
                }
            }
            catch (Exception ex)
            {
                viewError(ex);
            }
        }

        private void buttonDeleteCard_Click(object sender, RoutedEventArgs e)
        {
            writeMessage("Попытка удаления счета...");
            if (comboBoxDeleteCard.SelectedIndex >= 0)
            {
                if (MessageBox.Show("Вы уверены?", "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Warning, MessageBoxResult.No) == MessageBoxResult.Yes)
                {
                    DB.deleteCard(comboBoxDeleteCard.Text, textBoxDeleteCardReason.Text, labelAdminName.Content.ToString());
                    textBoxReason.Text = "";
                    labelDeleteCardMessage.Content = "Счет успешно удален";
                    setAllComboBoxesAndListboxes();
                }
            }
            else
                MessageBox.Show("Выберите номер счета!");
        }

        private void textBoxUpdateCardRest_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!regex.IsMatch(textBoxUpdateCardRest.Text))
            {
                textBoxUpdateCardRest.Foreground = Brushes.Red;
            }
            else
                textBoxUpdateCardRest.Foreground = Brushes.Black;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            setAllComboBoxesAndListboxes();
        }
    }
}
