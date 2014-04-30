using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data.MySqlClient;
using System.IO;
using System.Windows;
using System.Linq;
using System.Linq.Expressions;
using System.Data.Common;
using System.Data;

namespace PaymentsWPF
{
    /// <summary>
    /// Логика запросов к базе данных
    /// </summary>
    class DBRequests
    {
        /// <summary>
        /// Строка соединения с базой данных
        /// </summary>
        const string connectionString = "Server=127.0.0.1;Database=core-attack;Uid=core-attack;Pwd=P@ssw0rd;Charset=utf8;";///"Database=core-attack;Data Source=127.0.0.1;User Id=core-attack;Password=P@ssw0rd";
        /// <summary>
        /// Директория сохранения логов с запросами
        /// </summary>
        const string sql_directory = "logs\\sql\\";
        /// <summary>
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

        public bool logging_http = false;
        public bool logging_debug = false;


        /// <summary>
        /// Статусы счетов
        /// </summary>
        public static string[] STATUS_OF_CARD = { "Не заблокирован", "Заблокирован"};
        /// <summary>
        /// Статусы аккаунтов
        /// </summary>
        public static string[] STATUS_OF_USER = { "Активен", "Только чтение", "Заблокирован" };
        /// <summary>
        /// Типы операция по счету
        /// </summary>
        public static string[] TYPE_OF_OPERATION = { "Снять", "Положить" };

        /// <summary>
        /// Конструктор 
        /// </summary>
        /// <param name="logging_http">Журналируются ли http-запросы</param>
        /// <param name="logging_debug">Журналируется ли отладочная информация</param>
        /// <param name="error"></param>
        /// <param name="http"></param>
        /// <param name="debug"></param>
        public DBRequests(bool logging_http, bool logging_debug, StreamWriter error, StreamWriter http, StreamWriter debug)
        {
            try
            {
                writeMessage("Запуск конструктора DSRequests...");
                this.logging_http = logging_http;
                this.logging_debug = logging_debug;
                log_debug = debug;
                log_error = error;
                log_http = http;
                writeMessage("Инициализация прошла успешно.");
            }
            catch (Exception ex)
            {
                viewError(ex);
            }
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
        /// Проверяет наличие подключения к базе данных
        /// </summary>
        public void checkConnection()
        {
            try
            {
                writeMessage("Проверка соединения с базой данных...");
                using (MySqlConnection myConnection = new MySqlConnection(connectionString))
                {
                    using (MySqlCommand myCommand = new MySqlCommand("select * from users", myConnection))
                    {
                        myConnection.Open();
                        myConnection.Close();
                        MessageBox.Show("Подключение к базе данных прошло успешно!");
                        writeMessage("Успешно.");
                    }
                }
            }
            catch (Exception ex)
            {
                viewError(ex);
            }
        }

        /// <summary>
        /// Шифрует строку MD5
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public string getMD5(string message)
        {
            try
            {
                writeMessage("Получение зашифрованного сообщения...");
                string sql = "select md5(@message) from dual";
                using (MySqlConnection myConnection = new MySqlConnection(connectionString))
                {
                    using (MySqlCommand myCommand = new MySqlCommand(sql, myConnection))
                    {
                        myCommand.Parameters.Add("@message", MySqlDbType.VarChar).Value = message;
                        myConnection.Open();
                        MySqlDataReader MyDataReader = myCommand.ExecuteReader();
                        string result = "";
                        while (MyDataReader.Read())
                        {
                            result = MyDataReader.GetString(0);
                            writeMessage(sql, result);
                        }
                        myConnection.Close();
                        writeMessage("Успех: " + result + ".");
                        return result;
                    }
                }
            }
            catch (Exception e)
            {
                viewError(e);
                return "";
            }
        }

        /// <summary>
        /// Возвращает все имена пользователей из БД
        /// </summary>
        /// <returns></returns>
        public string[] getAllUserNames()
        {
            try
            {
                writeMessage("Запрос имен пользователей...");
                string sql = "select name from users";
                using (MySqlConnection myConnection = new MySqlConnection(connectionString))
                {
                    using (MySqlCommand myCommand = new MySqlCommand(sql, myConnection))
                    {
                        myConnection.Open();
                        MySqlDataReader MyDataReader = myCommand.ExecuteReader();
                        List<string> names = new List<string>();
                        while (MyDataReader.Read())
                        {
                            names.Add(MyDataReader.GetString(0));
                        }
                        myConnection.Close();
                        writeMessage(sql, names.ToArray());
                        writeMessage("Успешно.");
                        return names.ToArray();
                    }
                }
            }
            catch (Exception e)
            {
                viewError(e);
                return null;
            }
        }

        /// <summary>
        /// Проверка на существование пользователя в БД
        /// </summary>
        /// <param name="name">ФИО</param>
        /// <param name="password">Пароль</param>
        /// <returns></returns>
        public bool checkUser(string name, string password)
        {
            try
            {
                writeMessage(string.Format("Проверка существования записи пользователя (логин: {0}, пароль: {1}) в базе данных...", name, password));
                string sql = "select count(*) from users where name = @name and crypted_password = @password";
                using (MySqlConnection myConnection = new MySqlConnection(connectionString))
                {
                    using (MySqlCommand myCommand = new MySqlCommand(sql, myConnection))
                    {
                        myCommand.Parameters.Add("@name", MySqlDbType.VarChar).Value = name;
                        myCommand.Parameters.Add("@password", MySqlDbType.VarChar).Value = password;
                        myConnection.Open();
                        MySqlDataReader MyDataReader = myCommand.ExecuteReader();
                        int result = 0;
                        while (MyDataReader.Read())
                        {
                            result = MyDataReader.GetInt32(0);
                            writeMessage(sql, result.ToString());
                        }
                        myConnection.Close();
                        if (result == 0)
                        {
                            writeMessage("Записи не существует.");
                            return false;
                        }
                        else
                        {
                            writeMessage("Запись существует.");
                            return true;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                viewError(e);
                return false;
            }
        }

        /// <summary>
        /// Проверка на существование пользователя в БД
        /// </summary>
        /// <param name="name">ФИО</param>
        /// <returns></returns>
        public bool checkUser(string name)
        {
            try
            {
                writeMessage(string.Format("Проверка существования записи пользователя (логин: {0}) в базе данных...", name));
                string sql = "select count(*) from users where name = @name";
                using (MySqlConnection myConnection = new MySqlConnection(connectionString))
                {
                    using (MySqlCommand myCommand = new MySqlCommand(sql, myConnection))
                    {
                        myCommand.Parameters.Add("@name", MySqlDbType.VarChar).Value = name;
                        myConnection.Open();
                        MySqlDataReader MyDataReader = myCommand.ExecuteReader();
                        int result = 0;
                        while (MyDataReader.Read())
                        {
                            result = MyDataReader.GetInt32(0);
                            writeMessage(sql, result.ToString());
                        }
                        myConnection.Close();
                        if (result == 0)
                        {
                            writeMessage("Запись не существует.");
                            return false;
                        }
                        else
                        {
                            writeMessage("Запись существует.");
                            return true;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                viewError(e);
                return false;
            }
        }

        /// <summary>
        /// Получает id пользователя
        /// </summary>
        /// <param name="name">ФИО</param>
        /// <returns></returns>
        public int getUserId(string name)
        {
            try
            {
                writeMessage(string.Format("Запрос id пользователя {0}", name));
                string sql = "select id from users where name = @name";
                using (MySqlConnection myConnection = new MySqlConnection(connectionString))
                {
                    using (MySqlCommand myCommand = new MySqlCommand(sql, myConnection))
                    {
                        myCommand.Parameters.Add("@name", MySqlDbType.VarChar).Value = name;
                        myConnection.Open();
                        MySqlDataReader MyDataReader = myCommand.ExecuteReader();
                        int id = -1;
                        while (MyDataReader.Read())
                        {
                            id = MyDataReader.GetInt32(0);
                            writeMessage(sql, id.ToString());
                        }
                        myConnection.Close();
                        writeMessage(string.Format("id: {0}", id));
                        return id;
                    }
                }
            }
            catch (Exception e)
            {
                viewError(e);
                return -1;
            }
        }

        /// <summary>
        /// Получает имя пользователя
        /// </summary>
        /// <param name="id">id</param>
        /// <returns></returns>
        public string getUserName(int id)
        {
            try
            {
                writeMessage(string.Format("Запрос имени пользователя (id: {0})", id));
                string sql = "select name from users where id = @id";
                using (MySqlConnection myConnection = new MySqlConnection(connectionString))
                {
                    using (MySqlCommand myCommand = new MySqlCommand(sql, myConnection))
                    {
                        myCommand.Parameters.Add("@id", MySqlDbType.Int32).Value = id;
                        myConnection.Open();
                        MySqlDataReader MyDataReader = myCommand.ExecuteReader();
                        string name = "";
                        while (MyDataReader.Read())
                        {
                            name = MyDataReader.GetString(0);
                            writeMessage(sql, id.ToString());
                        }
                        myConnection.Close();
                        writeMessage(string.Format("Имя: {0}", name));
                        return name;
                    }
                }
            }
            catch (Exception e)
            {
                viewError(e);
                return "";
            }
        }

        /// <summary>
        /// Получает информацию о пользователе 
        /// </summary>
        /// <param name="name">имя</param>
        /// <param name="password">пароль</param>
        /// <returns></returns>
        public DataSet getUserInfo(string name, string password)
        {
            try
            {
                writeMessage(string.Format("Запрос информации о пользователе {0}", name));
                string sql = "select id, name, crypted_password, role, status from users where name = @name and crypted_password = @password";
                using (MySqlConnection myConnection = new MySqlConnection(connectionString))
                {
                    using (MySqlCommand myCommand = new MySqlCommand(sql, myConnection))
                    {
                        myCommand.Parameters.Add("@name", MySqlDbType.VarChar).Value = name;
                        myCommand.Parameters.Add("@password", MySqlDbType.VarChar).Value = password;
                        myConnection.Open();
                        MySqlDataAdapter adapter = new MySqlDataAdapter(sql, myConnection);
                        adapter.InsertCommand = myCommand;
                        DataSet ds_user = new DataSet();
                        //adapter.Fill(ds_user, "users");
                        DataTable users = ds_user.Tables.Add("users");
                        users.Columns.Add(new DataColumn("id", System.Type.GetType("System.Int32")));
                        users.Columns.Add(new DataColumn("name", System.Type.GetType("System.String")));
                        users.Columns.Add(new DataColumn("crypted_password", System.Type.GetType("System.String")));
                        users.Columns.Add(new DataColumn("role", System.Type.GetType("System.String")));
                        users.Columns.Add(new DataColumn("status", System.Type.GetType("System.String")));

                        MySqlDataReader MyDataReader = myCommand.ExecuteReader();
                        while (MyDataReader.Read())
                        {
                            users.Rows.Add(MyDataReader.GetValue(0), MyDataReader.GetValue(1),
                                MyDataReader.GetValue(2), MyDataReader.GetValue(3), MyDataReader.GetValue(4));

                        }
                        myConnection.Close();
                        writeMessage(string.Format("Информация получена."));
                        return ds_user;
                    }
                }
            }
            catch (Exception e)
            {
                viewError(e);
                return null;
            }
        }

        /// <summary>
        /// Получает информацию о пользователе 
        /// </summary>
        /// <param name="name">имя</param>
        public DataSet getUserInfo(string name)
        {
            try
            {
                writeMessage(string.Format("Запрос информации о пользователе {0}", name));
                string sql = "select id, name, crypted_password, role, status, block_date, created_admin_id, blocked_admin_id from users where name = @name";
                using (MySqlConnection myConnection = new MySqlConnection(connectionString))
                {
                    using (MySqlCommand myCommand = new MySqlCommand(sql, myConnection))
                    {
                        myCommand.Parameters.Add("@name", MySqlDbType.VarChar).Value = name;
                        myConnection.Open();
                        MySqlDataAdapter adapter = new MySqlDataAdapter(sql, myConnection);
                        adapter.InsertCommand = myCommand;
                        DataSet ds_user = new DataSet();
                        //adapter.Fill(ds_user, "users");
                        DataTable users = ds_user.Tables.Add("users");
                        users.Columns.Add(new DataColumn("id", System.Type.GetType("System.Int32")));
                        users.Columns.Add(new DataColumn("name", System.Type.GetType("System.String")));
                        users.Columns.Add(new DataColumn("crypted_password", System.Type.GetType("System.String")));
                        users.Columns.Add(new DataColumn("role", System.Type.GetType("System.String")));
                        users.Columns.Add(new DataColumn("status", System.Type.GetType("System.String")));
                        users.Columns.Add(new DataColumn("block_date", System.Type.GetType("System.DateTime")));
                        users.Columns.Add(new DataColumn("created_admin_id", System.Type.GetType("System.Int32")));
                        users.Columns.Add(new DataColumn("blocked_admin_id", System.Type.GetType("System.Int32")));

                        using (MySqlDataReader MyDataReader = myCommand.ExecuteReader())
                        {
                            while (MyDataReader.Read())
                            {
                                users.Rows.Add(MyDataReader.GetValue(0), MyDataReader.GetValue(1),
                                    MyDataReader.GetValue(2), MyDataReader.GetValue(3), MyDataReader.GetValue(4),
                                    MyDataReader.GetValue(5), MyDataReader.GetValue(6), MyDataReader.GetValue(7));

                            }
                        }
                        myConnection.Close();
                        writeMessage(string.Format("Информация получена."));
                        return ds_user;
                    }
                }
            }
            catch (Exception e)
            {
                viewError(e);
                return null;
            }
        }

        /// <summary>
        /// Создает запись пользователя в БД
        /// </summary>
        /// <param name="name">ФИО</param>
        /// <param name="password">Пароль</param>
        /// <param name="role">Роль</param>
        /// <param name="status">Статус</param>
        /// <param name="admin_id">id админа-создателя</param>
        public void createUser(string name, string password, string role, string status, int admin_id)
        {
            try
            {
                writeMessage(string.Format("Создание записи пользователя..."));
                string sql = "INSERT INTO `users`(`name`, `crypted_password`, `role`, `status`, `block_date`, `created_admin_id`, `blocked_admin_id`) VALUES (@name, @password, @role, @status, @block_date, @created_admin_id, @blocked_admin_id)";
                using (MySqlConnection myConnection = new MySqlConnection(connectionString))
                {
                    using (MySqlCommand myCommand = new MySqlCommand(sql, myConnection))
                    {
                        myCommand.Parameters.Add("@name", MySqlDbType.VarChar).Value = name;
                        myCommand.Parameters.Add("@password", MySqlDbType.VarChar).Value = getMD5(password);
                        myCommand.Parameters.Add("@role", MySqlDbType.VarChar).Value = role;
                        myCommand.Parameters.Add("@status", MySqlDbType.VarChar).Value = status;
                        if (status != "Активен")
                        {
                            myCommand.Parameters.Add("@block_date", MySqlDbType.DateTime).Value = DateTime.Now;
                            myCommand.Parameters.Add("@blocked_admin_id", MySqlDbType.Int32).Value = admin_id;
                        }
                        else
                        {
                            myCommand.Parameters.Add("@block_date", MySqlDbType.DateTime).Value = null;
                            myCommand.Parameters.Add("@blocked_admin_id", MySqlDbType.Int32).Value = null;
                        }
                        myCommand.Parameters.Add("@created_admin_id", MySqlDbType.Int32).Value = admin_id;
                        myConnection.Open();
                        myCommand.ExecuteNonQuery();
                        myConnection.Close();
                        writeMessage(sql, "Пользователь успешно добавлен");
                        writeMessage(string.Format("Пользователь {0} успешно добавлен.", name));
                    }
                }
            }
            catch (Exception e)
            {
                viewError(e);
            }
        }

        /// <summary>
        /// Удаление пользователя
        /// </summary>
        /// <param name="name">Кого удаляем</param>
        /// <param name="reason">Основание</param>
        /// <param name="admin_name">Кто удалил</param>
        public void deleteUser(string name, string reason, string admin_name)
        {
            try
            {
                int id = getUserId(name);
                if (id != 1)
                {
                    writeMessage(string.Format("Удаление пользователя {0}...", name));
                    string sql = "DELETE FROM `users` WHERE name = @name";
                    using (MySqlConnection myConnection = new MySqlConnection(connectionString))
                    {
                        using (MySqlCommand myCommand = new MySqlCommand(sql, myConnection))
                        {
                            myCommand.Parameters.Add("@name", MySqlDbType.VarChar).Value = name;
                            myConnection.Open();
                            myCommand.ExecuteNonQuery();
                            myConnection.Close();
                            writeMessage(sql, string.Format("Основание: {0}\nИнициатор: {1}\nПользователь удален", reason, admin_name));
                            writeMessage(string.Format("Пользователь удален успешно."));
                            //а теперь нужно удалить все счета и платежи этого пользователя из базы
                            writeMessage(string.Format("Удаление счетов пользователя {0}...", name));
                            sql = "DELETE FROM `cards` WHERE user_id = @id";
                            using (MySqlConnection myConnection1 = new MySqlConnection(connectionString))
                            {
                                using (MySqlCommand myCommand1 = new MySqlCommand(sql, myConnection))
                                {
                                    myCommand1.Parameters.Add("@id", MySqlDbType.VarChar).Value = id;
                                    myConnection1.Open();
                                    myCommand1.ExecuteNonQuery();
                                    myConnection1.Close();
                                    writeMessage(sql, "Счета пользователя удалены");
                                    writeMessage(string.Format("Счета пользователя удалены успешно."));
                                }
                            }
                        }
                    }
                }
                else
                    writeMessage(string.Format("Попытка удаления записи разработчика приложения. Отказано."));

            }
            catch (Exception e)
            {
                viewError(e);
            }
        }

        /// <summary>
        /// Проверяет существование счета
        /// </summary>
        /// <param name="bankAccount">номер счета</param>
        /// <returns></returns>
        public bool checkBankAccount(string bankAccount)
        {
            try
            {
                writeMessage(string.Format("Проверка существования записи счета {0}...", bankAccount));
                string sql = "select count(*) from cards where bank_account = @bankAccount";
                using (MySqlConnection myConnection = new MySqlConnection(connectionString))
                {
                    using (MySqlCommand myCommand = new MySqlCommand(sql, myConnection))
                    {
                        myCommand.Parameters.Add("@bankAccount", MySqlDbType.VarChar).Value = bankAccount;
                        myConnection.Open();
                        MySqlDataReader MyDataReader = myCommand.ExecuteReader();
                        int result = 0;
                        while (MyDataReader.Read())
                        {
                            result = MyDataReader.GetInt32(0);
                            writeMessage(sql, result.ToString());
                        }
                        myConnection.Close();
                        if (result == 0)
                        {
                            writeMessage(string.Format("Записи не существует."));
                            return false;
                        }
                        else
                        {
                            writeMessage(string.Format("Запись существует."));
                            return true;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                viewError(e);
                return false;
            }
        }

        /// <summary>
        /// Создает счет для пользователя
        /// </summary>
        /// <param name="bankAccount">номер счета</param>
        /// <param name="status">статус</param>
        /// <param name="user_id">id пользователя</param>
        public void createCard(string bankAccount, string status, int userId, int createdAdminId)
        {
            try
            {
                writeMessage(string.Format("Создание записи счета {0}...", bankAccount));
                string sql = "INSERT INTO `cards`(`bank_account`, `rest`, `status`, `user_id`, `created_admin_id`, `created_date`, `blocked_date`, `blocked_id`) VALUES (@bankAccount, @rest, @status, @userId, @createdAdminId, @createdDate, @blockedDate, @blockedId)";
                using (MySqlConnection myConnection = new MySqlConnection(connectionString))
                {
                    using (MySqlCommand myCommand = new MySqlCommand(sql, myConnection))
                    {
                        myCommand.Parameters.Add("@bankAccount", MySqlDbType.VarChar).Value = bankAccount;
                        myCommand.Parameters.Add("@rest", MySqlDbType.Double).Value = 0.0;
                        myCommand.Parameters.Add("@status", MySqlDbType.VarChar).Value = status;
                        myCommand.Parameters.Add("@userId", MySqlDbType.Int32).Value = userId;
                        myCommand.Parameters.Add("@createdAdminId", MySqlDbType.Int32).Value = createdAdminId;
                        myCommand.Parameters.Add("@createdDate", MySqlDbType.DateTime).Value = DateTime.Now;
                        myCommand.Parameters.Add("@blockedDate", MySqlDbType.DateTime).Value = DateTime.Now;
                        myCommand.Parameters.Add("@blockedId", MySqlDbType.Int32).Value = createdAdminId;
                        myConnection.Open();
                        myCommand.ExecuteNonQuery();
                        myConnection.Close();
                        writeMessage(sql, "Счет успешно добавлен");
                        writeMessage(string.Format("Запись счета создана успешно."));
                    }
                }
            }
            catch (Exception e)
            {
                viewError(e);
            }
        }

        /// <summary>
        /// Возвращает все счета из БД
        /// </summary>
        /// <returns></returns>
        public DataSet getAllCards()
        {
            try
            {
                writeMessage(string.Format("Запрос всех записей счетов..."));
                string sql = "select id, bank_account, rest, status, user_id, created_admin_id, created_date, blocked_id, blocked_date from cards";
                using (MySqlConnection myConnection = new MySqlConnection(connectionString))
                {
                    using (MySqlCommand myCommand = new MySqlCommand(sql, myConnection))
                    {
                        myConnection.Open();
                        MySqlDataAdapter adapter = new MySqlDataAdapter(sql, myConnection);
                        adapter.InsertCommand = myCommand;
                        DataSet ds_cards = new DataSet();
                        DataTable cards = ds_cards.Tables.Add("cards");
                        cards.Columns.Add(new DataColumn("id", System.Type.GetType("System.Int32")));
                        cards.Columns.Add(new DataColumn("bank_account", System.Type.GetType("System.String")));
                        cards.Columns.Add(new DataColumn("rest", System.Type.GetType("System.Double")));
                        cards.Columns.Add(new DataColumn("status", System.Type.GetType("System.String")));
                        cards.Columns.Add(new DataColumn("user_id", System.Type.GetType("System.Int32")));
                        cards.Columns.Add(new DataColumn("created_admin_id", System.Type.GetType("System.Int32")));
                        cards.Columns.Add(new DataColumn("created_date", System.Type.GetType("System.DateTime")));
                        cards.Columns.Add(new DataColumn("blocked_id", System.Type.GetType("System.Int32")));
                        cards.Columns.Add(new DataColumn("blocked_date", System.Type.GetType("System.DateTime")));

                        MySqlDataReader MyDataReader = myCommand.ExecuteReader();
                        while (MyDataReader.Read())
                        {
                            cards.Rows.Add(MyDataReader.GetValue(0), MyDataReader.GetValue(1),
                                MyDataReader.GetValue(2), MyDataReader.GetValue(3), MyDataReader.GetValue(4),
                                MyDataReader.GetValue(5), MyDataReader.GetValue(6), MyDataReader.GetValue(7), MyDataReader.GetValue(8));

                        }
                        myConnection.Close();
                        writeMessage(sql, "Загрузка всех записей счетов прошла успешно");
                        writeMessage(string.Format("Успех."));
                        return ds_cards;
                    }
                }
            }
            catch (Exception e)
            {
                viewError(e);
                return null;
            }
        }

        /// <summary>
        /// Возвращает информацию о счете
        /// </summary>
        /// <returns></returns>
        public DataSet getCardInfo(string bankAccount)
        {
            try
            {
                writeMessage(string.Format("Запрос информации о счете {0}...", bankAccount));
                string sql = "select id, bank_account, rest, status, user_id, created_admin_id, created_date, blocked_id, blocked_date from cards where bank_account = @bankAccount";
                using (MySqlConnection myConnection = new MySqlConnection(connectionString))
                {
                    using (MySqlCommand myCommand = new MySqlCommand(sql, myConnection))
                    {
                        myCommand.Parameters.Add("@bankAccount", MySqlDbType.VarChar).Value = bankAccount;
                        myConnection.Open();
                        MySqlDataAdapter adapter = new MySqlDataAdapter(sql, myConnection);
                        adapter.InsertCommand = myCommand;
                        DataSet ds_cards = new DataSet();
                        DataTable cards = ds_cards.Tables.Add("cards");
                        cards.Columns.Add(new DataColumn("id", System.Type.GetType("System.Int32")));
                        cards.Columns.Add(new DataColumn("bank_account", System.Type.GetType("System.String")));
                        cards.Columns.Add(new DataColumn("rest", System.Type.GetType("System.Double")));
                        cards.Columns.Add(new DataColumn("status", System.Type.GetType("System.String")));
                        cards.Columns.Add(new DataColumn("user_id", System.Type.GetType("System.Int32")));
                        cards.Columns.Add(new DataColumn("created_admin_id", System.Type.GetType("System.Int32")));
                        cards.Columns.Add(new DataColumn("created_date", System.Type.GetType("System.DateTime")));
                        cards.Columns.Add(new DataColumn("blocked_id", System.Type.GetType("System.Int32")));
                        cards.Columns.Add(new DataColumn("blocked_date", System.Type.GetType("System.DateTime")));

                        MySqlDataReader MyDataReader = myCommand.ExecuteReader();
                        while (MyDataReader.Read())
                        {
                            cards.Rows.Add(MyDataReader.GetValue(0), MyDataReader.GetValue(1),
                                MyDataReader.GetValue(2), MyDataReader.GetValue(3), MyDataReader.GetValue(4),
                                MyDataReader.GetValue(5), MyDataReader.GetValue(6), MyDataReader.GetValue(7), MyDataReader.GetValue(8));

                        }
                        myConnection.Close();
                        writeMessage(sql, "Загрузка всех записей счетов прошла успешно");
                        writeMessage(string.Format("Информация получена."));
                        return ds_cards;
                    }
                }
            }
            catch (Exception e)
            {
                viewError(e);
                return null;
            }
        }

        /// <summary>
        /// Возвращает все счета определенного пользователя
        /// </summary>
        /// <params name="id">id пользователя</params>
        /// <returns></returns>
        public DataSet getAllCards(int user_id)
        {
            try
            {
                writeMessage(string.Format("Запрос всех записей счетов пользователя (id: {0})", user_id));
                string sql = "select id, bank_account, rest, status, user_id, created_admin_id, created_date, blocked_id, blocked_date from cards where user_id = @user_id";
                using (MySqlConnection myConnection = new MySqlConnection(connectionString))
                {
                    using (MySqlCommand myCommand = new MySqlCommand(sql, myConnection))
                    {
                        myCommand.Parameters.Add("@user_id", MySqlDbType.Int32).Value = user_id;
                        myConnection.Open();
                        MySqlDataAdapter adapter = new MySqlDataAdapter(sql, myConnection);
                        adapter.InsertCommand = myCommand;
                        DataSet ds_cards = new DataSet();
                        DataTable cards = ds_cards.Tables.Add("cards");
                        cards.Columns.Add(new DataColumn("id", System.Type.GetType("System.Int32")));
                        cards.Columns.Add(new DataColumn("bank_account", System.Type.GetType("System.String")));
                        cards.Columns.Add(new DataColumn("rest", System.Type.GetType("System.Double")));
                        cards.Columns.Add(new DataColumn("status", System.Type.GetType("System.String")));
                        cards.Columns.Add(new DataColumn("user_id", System.Type.GetType("System.Int32")));
                        cards.Columns.Add(new DataColumn("created_admin_id", System.Type.GetType("System.Int32")));
                        cards.Columns.Add(new DataColumn("created_date", System.Type.GetType("System.DateTime")));
                        cards.Columns.Add(new DataColumn("blocked_id", System.Type.GetType("System.Int32")));
                        cards.Columns.Add(new DataColumn("blocked_date", System.Type.GetType("System.DateTime")));

                        MySqlDataReader MyDataReader = myCommand.ExecuteReader();
                        while (MyDataReader.Read())
                        {
                            cards.Rows.Add(MyDataReader.GetValue(0), MyDataReader.GetValue(1),
                                MyDataReader.GetValue(2), MyDataReader.GetValue(3), MyDataReader.GetValue(4),
                                MyDataReader.GetValue(5), MyDataReader.GetValue(6), MyDataReader.GetValue(7), MyDataReader.GetValue(8));

                        }
                        myConnection.Close();
                        writeMessage(sql, "Загрузка всех записей счетов прошла успешно");
                        writeMessage(string.Format("Успех."));
                        return ds_cards;
                    }
                }
            }
            catch (Exception e)
            {
                viewError(e);
                return null;
            }
        }

        /// <summary>
        /// Обновляет некоторые поля счета
        /// </summary>
        /// <param name="bankAccount">Номер счета</param>
        /// <param name="rest">Остаток на счете</param>
        /// <param name="status">Статус счета</param>
        /// <param name="userId">Владелец счета</param>
        /// <param name="blockedId">Заблокировал</param>
        /// <param name="user_name">Кто вносит изменения</param>
        public void updateCard(string bankAccount, double rest, string status, int userId, string user_name)
        {
            try
            {
                writeMessage(string.Format("Обновление полей записи счета {0}...", bankAccount));
                string sql = "UPDATE `cards` SET `rest`=@rest,`status`=@status,`user_id`=@userId, `blocked_id`=@blockedId,`blocked_date`=@blockedDate WHERE bank_account = @bankAccount";
                using (MySqlConnection myConnection = new MySqlConnection(connectionString))
                {
                    using (MySqlCommand myCommand = new MySqlCommand(sql, myConnection))
                    {
                        myCommand.Parameters.Add("@bankAccount", MySqlDbType.VarChar).Value = bankAccount;
                        myCommand.Parameters.Add("@rest", MySqlDbType.Double).Value = rest;
                        myCommand.Parameters.Add("@status", MySqlDbType.VarChar).Value = status;
                        myCommand.Parameters.Add("@userId", MySqlDbType.Int32).Value = userId;
                        myCommand.Parameters.Add("@blockedId", MySqlDbType.Int32).Value = getUserId(user_name);
                        myCommand.Parameters.Add("@blockedDate", MySqlDbType.DateTime).Value = DateTime.Now;
                        myConnection.Open();
                        myCommand.ExecuteNonQuery();
                        myConnection.Close();
                        writeMessage(sql, string.Format("Некоторые поля счета {0} успешно изменены\nИнициатор: {1}", bankAccount, user_name));
                        writeMessage(string.Format("Запись счета обновлена успешно."));
                    }
                }
            }
            catch (Exception e)
            {
                viewError(e);
            }
        }

        /// <summary>
        /// Изменить статус счета
        /// </summary>
        /// <param name="bankAccount">Номер счета</param>
        /// <param name="status">Статус счета</param>
        /// <param name="userId">Владелец счета</param>
        /// <param name="user_name">Кто вносит изменения</param>
        public void updateCard(string bankAccount, string status, int userId, string user_name)
        {
            try
            {
                writeMessage(string.Format("Изменение статуса счета {0}...", bankAccount));
                string sql = "UPDATE `cards` SET `status`=@status,`user_id`=@userId, `blocked_id`=@blockedId,`blocked_date`=@blockedDate WHERE bank_account = @bankAccount";
                using (MySqlConnection myConnection = new MySqlConnection(connectionString))
                {
                    using (MySqlCommand myCommand = new MySqlCommand(sql, myConnection))
                    {
                        myCommand.Parameters.Add("@bankAccount", MySqlDbType.VarChar).Value = bankAccount;
                        myCommand.Parameters.Add("@status", MySqlDbType.VarChar).Value = status;
                        myCommand.Parameters.Add("@userId", MySqlDbType.Int32).Value = userId;
                        myCommand.Parameters.Add("@blockedId", MySqlDbType.Int32).Value = getUserId(user_name);
                        myCommand.Parameters.Add("@blockedDate", MySqlDbType.DateTime).Value = DateTime.Now;
                        myConnection.Open();
                        myCommand.ExecuteNonQuery();
                        myConnection.Close();
                        writeMessage(sql, string.Format("Изменение статуса счета {0} на {2} произведено успешно\nИнициатор: {1}", bankAccount, user_name, status));
                        writeMessage(string.Format("Статус счета изменился на {0}.", status));
                    }
                }
            }
            catch (Exception e)
            {
                viewError(e);
            }
        }

        /// <summary>
        /// Изменить остаток на счете
        /// </summary>
        /// <param name="bankAccount">Номер счета</param>
        /// <param name="rest">Остаток на счете</param>
        public void updateCard(string bankAccount, double rest)
        {
            try
            {
                writeMessage(string.Format("Изменение остатка на счете {0}...", bankAccount));
                string sql = "UPDATE `cards` SET `rest`=@rest WHERE bank_account = @bankAccount";
                using (MySqlConnection myConnection = new MySqlConnection(connectionString))
                {
                    using (MySqlCommand myCommand = new MySqlCommand(sql, myConnection))
                    {
                        myCommand.Parameters.Add("@bankAccount", MySqlDbType.VarChar).Value = bankAccount;
                        myCommand.Parameters.Add("@rest", MySqlDbType.Double).Value = rest;
                        myConnection.Open();
                        myCommand.ExecuteNonQuery();
                        myConnection.Close();
                        writeMessage(sql, string.Format("Изменение остатка {1} на счете {0}\n", bankAccount, rest));
                        writeMessage(string.Format("Остаток на счете = {0}", rest));
                    }
                }
            }
            catch (Exception e)
            {
                viewError(e);
            }
        }

        /// <summary>
        /// Удаление счета
        /// </summary>
        /// <param name="bankAccount">Номер удаляемого счета</param>
        /// <param name="reason">Основание</param>
        /// <param name="admin_name">Кто удалил</param>
        public void deleteCard(string bankAccount, string reason, string admin_name)
        {
            try
            {
                writeMessage(string.Format("Удаление счета {0}...", bankAccount));
                string sql = "DELETE FROM `cards` WHERE bank_account = @bankAccount";
                using (MySqlConnection myConnection = new MySqlConnection(connectionString))
                {
                    using (MySqlCommand myCommand = new MySqlCommand(sql, myConnection))
                    {
                        myCommand.Parameters.Add("@bankAccount", MySqlDbType.VarChar).Value = bankAccount;
                        myConnection.Open();
                        myCommand.ExecuteNonQuery();
                        myConnection.Close();
                        writeMessage(sql, string.Format("Основание: {0}\nИнициатор: {1}\nСчет удален", reason, admin_name));
                        writeMessage(string.Format("Счет удален."));
                    }
                }
            }
            catch (Exception e)
            {
                viewError(e);
            }
        }

        /// <summary>
        /// Возвращает все номера счетов из БД
        /// </summary>
        /// <returns></returns>
        public string[] getAllBankAccounts()
        {
            try
            {
                writeMessage(string.Format("Запрос всех номеров счетов..."));
                string sql = "select bank_account from cards";
                using (MySqlConnection myConnection = new MySqlConnection(connectionString))
                {
                    using (MySqlCommand myCommand = new MySqlCommand(sql, myConnection))
                    {
                        myConnection.Open();
                        MySqlDataReader MyDataReader = myCommand.ExecuteReader();
                        List<string> ba = new List<string>();
                        while (MyDataReader.Read())
                        {
                            ba.Add(MyDataReader.GetString(0));
                        }
                        myConnection.Close();
                        writeMessage(sql, ba.ToArray());
                        writeMessage(string.Format("Успех."));
                        return ba.ToArray();
                    }
                }
            }
            catch (Exception e)
            {
                viewError(e);
                return null;
            }
        }

        /// <summary>
        /// Возвращает все платежи из БД
        /// </summary>
        /// <returns></returns>
        public DataSet getAllPayments()
        {
            try
            {
                writeMessage(string.Format("Запрос всех платежей..."));
                string sql = "select id, payment_sum, payment_date, commit, card_id from payments";
                using (MySqlConnection myConnection = new MySqlConnection(connectionString))
                {
                    using (MySqlCommand myCommand = new MySqlCommand(sql, myConnection))
                    {
                        myConnection.Open();
                        MySqlDataAdapter adapter = new MySqlDataAdapter(sql, myConnection);
                        adapter.InsertCommand = myCommand;
                        DataSet ds_payments = new DataSet();
                        DataTable payments = ds_payments.Tables.Add("payments");
                        payments.Columns.Add(new DataColumn("id", System.Type.GetType("System.Int32")));
                        payments.Columns.Add(new DataColumn("payment_sum", System.Type.GetType("System.Double")));
                        payments.Columns.Add(new DataColumn("payment_date", System.Type.GetType("System.DateTime")));
                        payments.Columns.Add(new DataColumn("commit", System.Type.GetType("System.Boolean")));
                        payments.Columns.Add(new DataColumn("card_id", System.Type.GetType("System.Int32")));

                        MySqlDataReader MyDataReader = myCommand.ExecuteReader();
                        while (MyDataReader.Read())
                        {
                            payments.Rows.Add(MyDataReader.GetValue(0), MyDataReader.GetValue(1),
                                MyDataReader.GetValue(2), MyDataReader.GetValue(3), MyDataReader.GetValue(4)
                                );
                        }
                        myConnection.Close();
                        writeMessage(sql, "Загрузка всех записей платежей прошла успешно");
                        writeMessage(string.Format("Успех."));
                        return ds_payments;
                    }
                }
            }
            catch (Exception e)
            {
                viewError(e);
                return null;
            }
        }

        /// <summary>
        /// Возвращает все платежи определенного пользователя из БД
        /// </summary>
        /// <returns></returns>
        public DataSet getAllPaymentsByUser(int user_id)
        {
            try
            {
                writeMessage(string.Format("Запрос всех платежей пользователя (id: {0})...", user_id));
                string sql = "select p.id, p.payment_sum, p.payment_date, p.commit, p.card_id from payments p, cards c where c.id = p.card_id and c.user_id = @user_id order by p.payment_date";
                using (MySqlConnection myConnection = new MySqlConnection(connectionString))
                {
                    using (MySqlCommand myCommand = new MySqlCommand(sql, myConnection))
                    {
                        myCommand.Parameters.Add("@user_id", MySqlDbType.Int32).Value = user_id;
                        myConnection.Open();
                        MySqlDataAdapter adapter = new MySqlDataAdapter(sql, myConnection);
                        adapter.InsertCommand = myCommand;
                        DataSet ds_payments = new DataSet();
                        DataTable payments = ds_payments.Tables.Add("payments");
                        payments.Columns.Add(new DataColumn("id", System.Type.GetType("System.Int32")));
                        payments.Columns.Add(new DataColumn("payment_sum", System.Type.GetType("System.Double")));
                        payments.Columns.Add(new DataColumn("payment_date", System.Type.GetType("System.DateTime")));
                        payments.Columns.Add(new DataColumn("commit", System.Type.GetType("System.Boolean")));
                        payments.Columns.Add(new DataColumn("card_id", System.Type.GetType("System.Int32")));

                        MySqlDataReader MyDataReader = myCommand.ExecuteReader();
                        while (MyDataReader.Read())
                        {
                            payments.Rows.Add(MyDataReader.GetValue(0), MyDataReader.GetValue(1),
                                MyDataReader.GetValue(2), MyDataReader.GetValue(3), MyDataReader.GetValue(4)
                                );
                        }
                        myConnection.Close();
                        writeMessage(sql, "Загрузка всех записей платежей прошла успешно");
                        writeMessage(string.Format("Успех."));
                        return ds_payments;
                    }
                }
            }
            catch (Exception e)
            {
                viewError(e);
                return null;
            }
        }

        /// <summary>
        /// Возвращает все платежи по определенному счету из БД
        /// </summary>
        /// <returns></returns>
        public DataSet getAllPaymentsByCard(int card_id)
        {
            try
            {
                writeMessage(string.Format("Запрос всех платежей по счету {0}...", card_id));
                string sql = "select p.id, p.payment_sum, p.payment_date, p.commit, p.card_id from payments p, cards c where c.id = p.card_id and c.id = @card_id";
                using (MySqlConnection myConnection = new MySqlConnection(connectionString))
                {
                    using (MySqlCommand myCommand = new MySqlCommand(sql, myConnection))
                    {
                        myCommand.Parameters.Add("@card_id", MySqlDbType.Int32).Value = card_id;
                        myConnection.Open();
                        MySqlDataAdapter adapter = new MySqlDataAdapter(sql, myConnection);
                        adapter.InsertCommand = myCommand;
                        DataSet ds_payments = new DataSet();
                        DataTable payments = ds_payments.Tables.Add("payments");
                        payments.Columns.Add(new DataColumn("id", System.Type.GetType("System.Int32")));
                        payments.Columns.Add(new DataColumn("payment_sum", System.Type.GetType("System.Double")));
                        payments.Columns.Add(new DataColumn("payment_date", System.Type.GetType("System.DateTime")));
                        payments.Columns.Add(new DataColumn("commit", System.Type.GetType("System.Boolean")));
                        payments.Columns.Add(new DataColumn("card_id", System.Type.GetType("System.Int32")));

                        MySqlDataReader MyDataReader = myCommand.ExecuteReader();
                        while (MyDataReader.Read())
                        {
                            payments.Rows.Add(MyDataReader.GetValue(0), MyDataReader.GetValue(1),
                                MyDataReader.GetValue(2), MyDataReader.GetValue(3), MyDataReader.GetValue(4)
                                );

                        }
                        myConnection.Close();
                        writeMessage(sql, "Загрузка всех записей платежей прошла успешно");
                        writeMessage(string.Format("Успех."));
                        return ds_payments;
                    }
                }
            }
            catch (Exception e)
            {
                viewError(e);
                return null;
            }
        }

        /// <summary>
        /// Создает платеж
        /// </summary>
        /// <param name="cardId"></param>
        public void createPayment(int cardId, double sum)
        {
            try
            {
                writeMessage(string.Format("Создание записи платежа..."));
                string sql = "INSERT INTO payments (payment_sum, payment_date, commit, card_id ) VALUES (@sum, @date, @commit, @cardId)";
                using (MySqlConnection myConnection = new MySqlConnection(connectionString))
                {
                    using (MySqlCommand myCommand = new MySqlCommand(sql, myConnection))
                    {
                        myCommand.Parameters.Add("@sum", MySqlDbType.Double).Value = sum;
                        myCommand.Parameters.Add("@date", MySqlDbType.DateTime).Value = DateTime.Now;
                        myCommand.Parameters.Add("@commit", MySqlDbType.Int16).Value = 1; //1 - true, 0 - false
                        myCommand.Parameters.Add("@cardId", MySqlDbType.Int32).Value = cardId;
                        myConnection.Open();
                        myCommand.ExecuteNonQuery();
                        myConnection.Close();
                        writeMessage(sql, "Платеж произведен успешно");
                        writeMessage(string.Format("Плажет на сумму {0} по счету (id: {1}) создан.", sum, cardId));
                    }
                }
            }
            catch (Exception e)
            {
                viewError(e);
            }
        }

        /// <summary>
        /// Получает id счета
        /// </summary>
        /// <param name="name">ФИО</param>
        /// <returns></returns>
        public int getCardId(string bankAccount)
        {
            try
            {
                writeMessage(string.Format("Запрос id счета {0}", bankAccount));
                string sql = "select id from cards where bank_account = @bankAccount";
                using (MySqlConnection myConnection = new MySqlConnection(connectionString))
                {
                    using (MySqlCommand myCommand = new MySqlCommand(sql, myConnection))
                    {
                        myCommand.Parameters.Add("@bankAccount", MySqlDbType.VarChar).Value = bankAccount;
                        myConnection.Open();
                        MySqlDataReader MyDataReader = myCommand.ExecuteReader();
                        int id = -1;
                        while (MyDataReader.Read())
                        {
                            id = MyDataReader.GetInt32(0);
                            writeMessage(sql, id.ToString());
                        }
                        myConnection.Close();
                        writeMessage(string.Format("Успех."));
                        return id;
                    }
                }
            }
            catch (Exception e)
            {
                viewError(e);
                return -1;
            }
        }

        /// <summary>
        /// Получает номер счета
        /// </summary>
        /// <param name="name">ФИО</param>
        /// <returns></returns>
        public string getCardBankAccount(int id)
        {
            try
            {
                writeMessage(string.Format("Запрос номера счета (id: {0})...", id));
                string sql = "select bank_account from cards where id = @id";
                using (MySqlConnection myConnection = new MySqlConnection(connectionString))
                {
                    using (MySqlCommand myCommand = new MySqlCommand(sql, myConnection))
                    {
                        myCommand.Parameters.Add("@id", MySqlDbType.Int32).Value = id;
                        myConnection.Open();
                        MySqlDataReader MyDataReader = myCommand.ExecuteReader();
                        string bankAccount = "";
                        while (MyDataReader.Read())
                        {
                            bankAccount = MyDataReader.GetString(0);
                            writeMessage(sql, bankAccount.ToString());
                        }
                        myConnection.Close();
                        writeMessage(string.Format("Успех."));
                        return bankAccount;
                    }
                }
            }
            catch (Exception e)
            {
                viewError(e);
                return "";
            }
        }

    }
}
