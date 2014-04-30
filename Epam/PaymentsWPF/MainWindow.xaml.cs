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
using System.Windows.Navigation;
using System.Windows.Shapes;
using MySql.Data.MySqlClient;
using System.IO;
using System.Data;

namespace PaymentsWPF
{
    #region[Условия задачи]
    /*
     * 2.	Система Платежи. Клиент имеет одну или несколько Кредитных Карт, 
     * каждая из которых соответствует некоторому Счету в системе платежей. 
     * Клиент может при помощи Счета сделать Платеж, заблокировать Счет и пополнить Счет. 
     * Администратор снимает блокировку.
     */
    #endregion

    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        DBRequests DB;
        public const string debug_directory = "logs\\debug\\";
        public const string error_directory = "logs\\errors\\";
        public const string http_directory = "logs\\http\\";
        public StreamWriter log_debug;
        public StreamWriter log_error;
        public StreamWriter log_http;
        bool logging_debug = false;
        bool logging_http = false;
        /// <summary>
        /// Прошла ли инициализация
        /// </summary>
        bool isInit = false;

        public MainWindow()
        {
            InitializeComponent();
            isInit = true;
            initLogs();
            DB = new DBRequests(logging_http, logging_debug, log_error, log_http, log_debug);
            loadAllUsersToCombobox();
        }


        /// <summary>
        /// Создание логов
        /// </summary>
        void initLogs()
        {
            DateTime now = DateTime.Now;
            string dir = System.AppDomain.CurrentDomain.BaseDirectory + debug_directory;
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);
            string path_debug = string.Format(dir + "debug at {4}-{5}-{6} {0}h {1}m {2}s {3}ms.txt", now.Hour.ToString(), now.Minute.ToString(), now.Second.ToString(), now.Millisecond.ToString(), now.Day.ToString(), now.Month.ToString(), now.Year.ToString());
            log_debug = new StreamWriter(path_debug, false, Encoding.UTF8, 10000);
            dir = System.AppDomain.CurrentDomain.BaseDirectory + error_directory;
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);
            string path_error = string.Format(dir + "error at {4}-{5}-{6} {0}h {1}m {2}s {3}ms.txt", now.Hour.ToString(), now.Minute.ToString(), now.Second.ToString(), now.Millisecond.ToString(), now.Day.ToString(), now.Month.ToString(), now.Year.ToString());
            log_error = new StreamWriter(path_error, false, Encoding.UTF8, 10000);
            dir = System.AppDomain.CurrentDomain.BaseDirectory + http_directory;
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);
            string path_http = string.Format(dir + "http at {4}-{5}-{6} {0}h {1}m {2}s {3}ms.txt", now.Hour.ToString(), now.Minute.ToString(), now.Second.ToString(), now.Millisecond.ToString(), now.Day.ToString(), now.Month.ToString(), now.Year.ToString());
            log_http = new StreamWriter(path_http, false, Encoding.UTF8, 10000);
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
        /// Проверяет подключение к БД
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click(object sender, RoutedEventArgs e)
        {
            DB.checkConnection();
        }

        /// <summary>
        /// Загружает имена пользователей в combobox
        /// </summary>
        void loadAllUsersToCombobox()
        {
            try
            {
                comboBoxUsers.Items.Clear();
                string[] names = DB.getAllUserNames();
                foreach (string s in names)
                {
                    comboBoxUsers.Items.Add(s);
                }
                
            }
            catch (Exception ex)
            {
                viewError(ex);
            }
        }

        private void buttonSignUp_Click(object sender, RoutedEventArgs e)
        {
            writeMessage(string.Format("Попытка авторизации (логин: {0}, пароль: {1})...", comboBoxUsers.Text, passwordBox1.Password));
            string pass = DB.getMD5(passwordBox1.Password);
            if (DB.checkUser(comboBoxUsers.Text, pass))
            {
                DataSet user = DB.getUserInfo(comboBoxUsers.Text, pass);
                string id = user.Tables["users"].Rows[0].ItemArray[0].ToString();
                string name = user.Tables["users"].Rows[0].ItemArray[1].ToString();
                string crypted_password = user.Tables["users"].Rows[0].ItemArray[2].ToString();
                string role = user.Tables["users"].Rows[0].ItemArray[3].ToString();
                string status = user.Tables["users"].Rows[0].ItemArray[4].ToString();
                if (status == "Активен")
                {
                    switch (role)
                    {
                        case "Администратор":
                            {
                                writeMessage("Идентифицирована администраторская роль.");
                                Admin adminWindow = new Admin(logging_http, logging_debug, log_error, log_http, log_debug);
                                adminWindow.labelAdminId.Content = id;
                                adminWindow.labelAdminName.Content = name;
                                adminWindow.Show();
                            }
                            break;
                        case "Пользователь":
                            {
                                writeMessage("Идентифицирвана пользовательская роль.");
                                Client clientWindow = new Client(Convert.ToInt32(id), logging_http, logging_debug, log_error, log_http, log_debug);
                                clientWindow.labelClientId.Content = id;
                                clientWindow.labelClientName.Content = name;
                                clientWindow.Show();
                            }
                            break;
                    }
                }
            }
            else
            {
                MessageBox.Show("Пользователя с таким именем или паролем не существует");

            }
        }

        private void comboBoxUsers_GotFocus(object sender, RoutedEventArgs e)
        {
        }

        private void Window_GotFocus(object sender, RoutedEventArgs e)
        {
        }

        private void comboBoxUsers_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (comboBoxUsers.SelectedIndex == 0)
                passwordBox1.Password = "admin_P@ssw0rd";
            else if (comboBoxUsers.SelectedIndex == 1)
                passwordBox1.Password = "qwerty";
            else
                passwordBox1.Password = "";
        }

        private void comboBox1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            switch (comboBoxLogging.SelectedIndex)
            {
                case 0: {
                    logging_http = false;
                    logging_debug = false;
                    if (DB != null)
                    {
                        DB.logging_http = false;
                        DB.logging_debug = false;
                    }
                    if (isInit)
                        labelMessage.Content = "Логируются только ошибки";
                } break;
                case 1: {
                    logging_http = true;
                    logging_debug = false;
                    if (DB != null)
                    {
                        DB.logging_http = true;
                        DB.logging_debug = false;
                    }
                    if (isInit)
                        labelMessage.Content = "Логируются ошибки и http-запросы";
                } break;
                case 2: {
                    logging_http = true;
                    logging_debug = true;
                    if (DB != null)
                    {
                        DB.logging_http = true;
                        DB.logging_debug = true;
                    }
                    if (isInit)
                        labelMessage.Content = "Логируются ошибки, http-запросы и пользовательские действия";
                } break;
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            log_debug.Close();
            log_error.Close();
            log_http.Close();
        }

        private void comboBoxUsers_DropDownOpened(object sender, EventArgs e)
        {
            loadAllUsersToCombobox();
        }

        
    }
}
