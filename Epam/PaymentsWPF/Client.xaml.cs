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
using System.Globalization;
using System.IO;

namespace PaymentsWPF
{
    /// <summary>
    /// Логика взаимодействия для Client.xaml
    /// </summary>
    public partial class Client : Window
    {
        DBRequests DB;
        /// <summary>
        /// инициализировались ли все компоненты
        /// </summary>
        bool isInit = false;
        /// <summary>
        /// id клиента
        /// </summary>
        int clientID = 0;
        /// <summary>
        /// Текущий счет
        /// </summary>
        DataSet card;

        /// <summary>
        /// Заблокирован ли выбранный счет
        /// </summary>
        bool isCurrentCardStatusOk = false;

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

        public Client(int id, bool logging_http, bool logging_debug, StreamWriter error, StreamWriter http, StreamWriter debug)
        {
            InitializeComponent();

            this.logging_http = logging_http;
            this.logging_debug = logging_debug;
            log_debug = debug;
            log_error = error;
            log_http = http;

            isInit = true;
            clientID = id;
            DB = new DBRequests(logging_http, logging_debug, error, http, debug);
            loading();

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
        /// Загружает всё, что нужно
        /// </summary>
        void loading()
        {
            loadUserCards();
            loadAllPayments();
        }

        /// <summary>
        /// Загружает счета пользователей из БД
        /// </summary>
        void loadUserCards()
        {
            writeMessage("Загружаем счета пользователей...");
            listBoxCards.Items.Clear();
            DataSet cards = DB.getAllCards(clientID);
            for (int i = 0; i < cards.Tables["cards"].Rows.Count; i++)
            {
                listBoxCards.Items.Add(cards.Tables["cards"].Rows[i].ItemArray[1]);
            }
            listBoxCards.SelectedIndex = 0;
            comboBoxChengeRestType.Items.Clear();
            foreach (string type in DBRequests.TYPE_OF_OPERATION)
                comboBoxChengeRestType.Items.Add(type);
            writeMessage("Счета загружены.");
        }

        /// <summary>
        /// Загружает информацию о счете из БД
        /// </summary>
        void loadCardInfo(string bankAccount)
        {
            try
            {
                writeMessage("Загружаем информацию о счете...");
                card = DB.getCardInfo(bankAccount);
                int id = (int)card.Tables["cards"].Rows[0].ItemArray[0];
                double rest = (double)card.Tables["cards"].Rows[0].ItemArray[2];
                string status = card.Tables["cards"].Rows[0].ItemArray[3].ToString();
                buttonBlockCard.Content = status.Equals("Не заблокирован") ? "Заблокировать" : "Разблокировать";
                buttonBlockCard.IsEnabled = status.Equals("Не заблокирован") ? true : false;
                isCurrentCardStatusOk = status.Equals("Не заблокирован") ? true : false;
                string user = DB.getUserName((int)card.Tables["cards"].Rows[0].ItemArray[4]);

                labelSelectedCardRest.Content = "Остаток на счете: " + rest.ToString();
                labelSelectedCardStatus.Content = "Статус счета: " + status;

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
                    labelSelectedCardBlockedUser.Content = "Заблокировал: " + DB.getUserName(blocked_admin_id);
                }
                else
                    labelSelectedCardBlockedUser.Content = "";

                if (card.Tables["cards"].Rows[0].ItemArray[8].GetType() != System.Type.GetType("System.DBNull"))
                {
                    DateTime block_date = (DateTime)card.Tables["cards"].Rows[0].ItemArray[8];
                    labelSelectedCardBlockedDate.Content = "Дата блокировки: " + block_date.ToShortDateString() + " " + block_date.ToShortTimeString();
                }
                else
                    labelSelectedCardBlockedDate.Content = "";
                writeMessage("Успешно.");
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message + e.StackTrace);
            }
        }

        private void buttonBlockCard_Click(object sender, RoutedEventArgs e)
        {
            if (listBoxCards.SelectedIndex >= 0)
            {
                if (buttonBlockCard.Content.ToString().Equals("Заблокировать"))
                {
                    DB.updateCard(listBoxCards.Items[listBoxCards.SelectedIndex].ToString(), DBRequests.STATUS_OF_CARD[1], clientID, labelClientName.Content.ToString());
                    buttonBlockCard.Content = "Разблокировать";
                    buttonBlockCard.IsEnabled = false;
                    labelSelectedCardStatus.Content = "Статус счета: " + DBRequests.STATUS_OF_CARD[1];
                }
                else
                {
                    DB.updateCard(listBoxCards.Items[listBoxCards.SelectedIndex].ToString(), DBRequests.STATUS_OF_CARD[0], clientID, labelClientName.Content.ToString());
                    buttonBlockCard.Content = "Заблокировать";
                    labelSelectedCardStatus.Content = "Статус счета: " + DBRequests.STATUS_OF_CARD[0];
                    writeMessage("Пользователь заблокировал счет: " + listBoxCards.Items[listBoxCards.SelectedIndex].ToString());
                }
            }
            else
            {
                MessageBox.Show("Выберите счет!");
            }
        }

        private void listBoxCards_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (listBoxCards.SelectedIndex >= 0 && listBoxCards.SelectedIndex < listBoxCards.Items.Count)
                loadCardInfo(listBoxCards.Items[listBoxCards.SelectedIndex].ToString());
        }

        private void buttonChangeRest_Click(object sender, RoutedEventArgs e)
        {
            if (listBoxCards.SelectedIndex >= 0)
            {
                if (isCurrentCardStatusOk)
                    if (textBoxChangeRestSum.Text != "" && comboBoxChengeRestType.SelectedIndex >= 0)
                    {
                        double rest = (double)card.Tables["cards"].Rows[0].ItemArray[2];
                        double sum = Convert.ToDouble(textBoxChangeRestSum.Text);
                        if (comboBoxChengeRestType.Items[comboBoxChengeRestType.SelectedIndex].ToString().Equals(DBRequests.TYPE_OF_OPERATION[0]))
                        {
                            if (sum <= rest)
                            {
                                DB.createPayment(DB.getCardId(listBoxCards.Items[listBoxCards.SelectedIndex].ToString()), sum);
                                DB.updateCard(listBoxCards.Items[listBoxCards.SelectedIndex].ToString(), rest - sum);
                                labelSelectedCardRest.Content = "Остаток на счете: " + (rest - sum);
                                writeMessage(string.Format("Уменьшение средств на счете {0} на {1} единиц. Текущий остаток на счете: {2}", listBoxCards.Items[listBoxCards.SelectedIndex].ToString(), sum, rest - sum));
                                loadAllPayments();

                            }
                            else
                            {
                                MessageBox.Show("Недостаточно средств для совершения данной операции!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Stop);
                                writeMessage(string.Format("Попытка уменьшения остатка на счете {0}. Отклонено: недостаточно средств для совершения данной операции.", listBoxCards.Items[listBoxCards.SelectedIndex].ToString()));
                            }
                        }
                        else
                        {
                            DB.updateCard(listBoxCards.Items[listBoxCards.SelectedIndex].ToString(), rest + sum);
                            labelSelectedCardRest.Content = "Остаток на счете: " + (rest + sum);
                            writeMessage(string.Format("Увеличение средств на счете {0} на {1} единиц. Текущий остаток на счете: {2}", listBoxCards.Items[listBoxCards.SelectedIndex].ToString(), sum, rest - sum));
                            loadAllPayments();
                        }
                        if (listBoxCards.SelectedIndex >= 0)
                            loadCardInfo(listBoxCards.Items[listBoxCards.SelectedIndex].ToString());
                    }
                    else
                    {
                        MessageBox.Show("Выберите тип операции и введите сумму!", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
                        writeMessage(string.Format("Попытка изменения остатка на счете {0}. Отклонено: не выбран тип операции и не введена сумма.", listBoxCards.Items[listBoxCards.SelectedIndex].ToString()));
                    }
                else
                {
                    MessageBox.Show("Невозможно выполнить операцию из-за блокировки счета!", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
                    writeMessage(string.Format("Попытка изменения остатка на счете {0}. Отклонено: счет заблокирован.", listBoxCards.Items[listBoxCards.SelectedIndex].ToString()));
                }
            }
            else
                MessageBox.Show("Выберите счет!");
        }

        private void textBoxSum_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (regex.IsMatch(textBoxChangeRestSum.Text))
            {
                label5.Content = "Введите сумму";
                label5.Foreground = Brushes.Black;
                buttonChangeRest.IsEnabled = true;
            }
            else
            {
                label5.Content = "Сумма введена некорректно!";
                label5.Foreground = Brushes.Red;
                buttonChangeRest.IsEnabled = false;
            }
        }

        void loadAllPayments()
        {
            writeMessage("Загрузка всех счетов пользователя...");
            listBoxPayments.Items.Clear();
            DataSet payments = DB.getAllPaymentsByUser(clientID);
            for (int i = 0; i < payments.Tables["payments"].Rows.Count; i++)
            {
                listBoxPayments.Items.Add(string.Format(
                    "Дата: {0}\tСумма: {1}\tСчет: {2}\tСтатус: {3}",
                    ((DateTime)payments.Tables["payments"].Rows[i].ItemArray[2]).ToString(),
                    ((double)payments.Tables["payments"].Rows[i].ItemArray[1]).ToString(),
                    (string)DB.getCardBankAccount((int)payments.Tables["payments"].Rows[i].ItemArray[4]),
                    (Boolean)payments.Tables["payments"].Rows[i].ItemArray[3] ? "Прошел" : "Ожидание"
                    ));
            }
            writeMessage("Успшно.");
        }

        
    }
}
