using PaymentsWPF;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using System.Data;
using System.Text;

namespace TestProject2
{
    
    
    /// <summary>
    ///Это класс теста для DBRequestsTest, в котором должны
    ///находиться все модульные тесты DBRequestsTest
    ///</summary>
    [TestClass()]
    public class DBRequestsTest
    {


        private TestContext testContextInstance;

        /// <summary>
        ///Получает или устанавливает контекст теста, в котором предоставляются
        ///сведения о текущем тестовом запуске и обеспечивается его функциональность.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region Дополнительные атрибуты теста
        // 
        //При написании тестов можно использовать следующие дополнительные атрибуты:
        //
        //ClassInitialize используется для выполнения кода до запуска первого теста в классе
        //[ClassInitialize()]
        //public static void MyClassInitialize(TestContext testContext)
        //{
        //}
        //
        //ClassCleanup используется для выполнения кода после завершения работы всех тестов в классе
        //[ClassCleanup()]
        //public static void MyClassCleanup()
        //{
        //}
        //
        //TestInitialize используется для выполнения кода перед запуском каждого теста
        //[TestInitialize()]
        //public void MyTestInitialize()
        //{
        //}
        //
        //TestCleanup используется для выполнения кода после завершения каждого теста
        //[TestCleanup()]
        //public void MyTestCleanup()
        //{
        //}
        //
        #endregion

        /// <summary>
        ///Тест для checkBankAccount
        ///</summary>
        [TestMethod()]
        public void checkBankAccountTest()
        {
            bool logging_http = false; 
            bool logging_debug = false; 
            DBRequests target = new DBRequests(logging_http, logging_debug, null, null, null); // TODO: инициализация подходящего значения
            string bankAccount = string.Empty; 
            bool expected = false; 
            bool actual;
            actual = target.checkBankAccount(bankAccount);
            Assert.AreEqual(expected, actual);
            bankAccount = "61475554995445097398";//взял из БД (тест может не пройти, если кто-нибудь удалил запись этого счета) (ниже аналогично будет)
            expected = true;
            actual = target.checkBankAccount(bankAccount);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///Тест для checkConnection
        ///</summary>
        [TestMethod()]
        public void checkConnectionTest()
        {
            bool logging_http = false; // TODO: инициализация подходящего значения
            bool logging_debug = false; // TODO: инициализация подходящего значения
            StreamWriter error = null; // TODO: инициализация подходящего значения
            StreamWriter http = null; // TODO: инициализация подходящего значения
            StreamWriter debug = null; // TODO: инициализация подходящего значения
            DBRequests target = new DBRequests(logging_http, logging_debug, error, http, debug); // TODO: инициализация подходящего значения
            target.checkConnection();
            Assert.AreEqual(1, 1);
        }

        /// <summary>
        ///Тест для checkUser
        ///</summary>
        [TestMethod()]
        public void checkUserTest()
        {
            bool logging_http = false; // TODO: инициализация подходящего значения
            bool logging_debug = false; // TODO: инициализация подходящего значения
            StreamWriter error = null; // TODO: инициализация подходящего значения
            StreamWriter http = null; // TODO: инициализация подходящего значения
            StreamWriter debug = null; // TODO: инициализация подходящего значения
            DBRequests target = new DBRequests(logging_http, logging_debug, error, http, debug); // TODO: инициализация подходящего значения
            string name = string.Empty; // TODO: инициализация подходящего значения
            bool expected = false; // TODO: инициализация подходящего значения
            bool actual;
            actual = target.checkUser(name);
            Assert.AreEqual(expected, actual);
            name = "Пискарев Николай Сергеевич";//также (далее комментировать не буду)
            expected = true;
            actual = target.checkUser(name);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///Тест для checkUser
        ///</summary>
        [TestMethod()]
        public void checkUserTest1()
        {
            bool logging_http = false; // TODO: инициализация подходящего значения
            bool logging_debug = false; // TODO: инициализация подходящего значения
            StreamWriter error = null; // TODO: инициализация подходящего значения
            StreamWriter http = null; // TODO: инициализация подходящего значения
            StreamWriter debug = null; // TODO: инициализация подходящего значения
            DBRequests target = new DBRequests(logging_http, logging_debug, error, http, debug); // TODO: инициализация подходящего значения
            string name = string.Empty; // TODO: инициализация подходящего значения
            string password = string.Empty; // TODO: инициализация подходящего значения
            bool expected = false; // TODO: инициализация подходящего значения
            bool actual;
            actual = target.checkUser(name, password);
            Assert.AreEqual(expected, actual);
            name = "Пискарев Николай Сергеевич";
            password = "2f2522f0adfe0bee91d9c6b67c164aff";
            expected = true;
            actual = target.checkUser(name, password);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///Тест для createUser
        ///</summary>
        [TestMethod()]
        public void createUserTest()
        {
            bool logging_http = false; // TODO: инициализация подходящего значения
            bool logging_debug = false; // TODO: инициализация подходящего значения
            StreamWriter error = null; // TODO: инициализация подходящего значения
            StreamWriter http = null; // TODO: инициализация подходящего значения
            StreamWriter debug = null; // TODO: инициализация подходящего значения
            DBRequests target = new DBRequests(logging_http, logging_debug, error, http, debug); // TODO: инициализация подходящего значения
            string name = "test"; // TODO: инициализация подходящего значения
            string password = "test"; // TODO: инициализация подходящего значения
            string role = "Пользователь"; // TODO: инициализация подходящего значения
            string status = DBRequests.STATUS_OF_USER[0]; // TODO: инициализация подходящего значения
            int admin_id = 1; // TODO: инициализация подходящего значения
            target.createUser(name, password, role, status, admin_id);
            DataSet user = target.getUserInfo(name, "098f6bcd4621d373cade4e832627b4f6");
            int i = user.Tables["users"].Rows.Count;
            string newname = user.Tables["users"].Rows[0].ItemArray[1].ToString();
            string newcrypted_password = user.Tables["users"].Rows[0].ItemArray[2].ToString();
            if (name.Equals(newname) && "098f6bcd4621d373cade4e832627b4f6".Equals(newcrypted_password))
                Assert.AreEqual(1, 1);
            else
                Assert.AreEqual(1, 0);
        }

        /// <summary>
        ///Тест для createCard
        ///</summary>
        [TestMethod()]
        public void createCardTest()
        {
            bool logging_http = false; // TODO: инициализация подходящего значения
            bool logging_debug = false; // TODO: инициализация подходящего значения
            StreamWriter error = null; // TODO: инициализация подходящего значения
            StreamWriter http = null; // TODO: инициализация подходящего значения
            StreamWriter debug = null; // TODO: инициализация подходящего значения
            DBRequests target = new DBRequests(logging_http, logging_debug, error, http, debug); // TODO: инициализация подходящего значения
            Admin a = new Admin(logging_http, logging_debug, error, http, debug);
            a.generateBankAccount();
            string bankAccount = a.textBoxBankAccount.Text;// TODO: инициализация подходящего значения
            string status = DBRequests.STATUS_OF_CARD[0]; // TODO: инициализация подходящего значения
            int userId = target.getUserId("test"); // TODO: инициализация подходящего значения
            int createdAdminId = 1; // TODO: инициализация подходящего значения
            target.createCard(bankAccount, status, userId, createdAdminId);

            DataSet card = target.getCardInfo(bankAccount);
            string ba = card.Tables["cards"].Rows[0].ItemArray[1].ToString();
            Assert.AreEqual(ba, bankAccount);
        }
        
        /// <summary>
        ///Тест для deleteCard
        ///</summary>
        [TestMethod()]
        public void deleteCardTest()
        {
            bool logging_http = false; // TODO: инициализация подходящего значения
            bool logging_debug = false; // TODO: инициализация подходящего значения
            StreamWriter error = null; // TODO: инициализация подходящего значения
            StreamWriter http = null; // TODO: инициализация подходящего значения
            StreamWriter debug = null; // TODO: инициализация подходящего значения
            DBRequests target = new DBRequests(logging_http, logging_debug, error, http, debug); // TODO: инициализация подходящего значения

            Admin a = new Admin(logging_http, logging_debug, error, http, debug);
            a.generateBankAccount();
            string bankAccount = a.textBoxBankAccount.Text;
            target.createCard(bankAccount, "", -1, -1);
            target.deleteCard(bankAccount, "", "");
            int id = target.getCardId(bankAccount);
            if (id == -1)
                Assert.AreEqual(1, 1);
            else
                Assert.AreEqual(1, 0);
               
        }

        /// <summary>
        ///Тест для deleteUser
        ///</summary>
        [TestMethod()]
        public void deleteUserTest()
        {
            bool logging_http = false; // TODO: инициализация подходящего значения
            bool logging_debug = false; // TODO: инициализация подходящего значения
            StreamWriter error = null; // TODO: инициализация подходящего значения
            StreamWriter http = null; // TODO: инициализация подходящего значения
            StreamWriter debug = null; // TODO: инициализация подходящего значения
            DBRequests target = new DBRequests(logging_http, logging_debug, error, http, debug); // TODO: инициализация подходящего значения
            string name = "test"; // TODO: инициализация подходящего значения
            string reason = string.Empty; // TODO: инициализация подходящего значения
            string admin_name = string.Empty; // TODO: инициализация подходящего значения
            target.deleteUser(name, reason, admin_name);
            int id = target.getUserId(name);
            if (id == -1)
                Assert.AreEqual(1, 1);
            else
                Assert.AreEqual(1, 0);
        }
        
        /// <summary>
        ///Тест для getMD5
        ///</summary>
        [TestMethod()]
        public void getMD5Test()
        {
            bool logging_http = false; // TODO: инициализация подходящего значения
            bool logging_debug = false; // TODO: инициализация подходящего значения
            StreamWriter error = null; // TODO: инициализация подходящего значения
            StreamWriter http = null; // TODO: инициализация подходящего значения
            StreamWriter debug = null; // TODO: инициализация подходящего значения
            DBRequests target = new DBRequests(logging_http, logging_debug, error, http, debug); // TODO: инициализация подходящего значения
            string message = "1234"; // TODO: инициализация подходящего значения
            string expected = "81dc9bdb52d04dc20036dbd8313ed055"; // TODO: инициализация подходящего значения
            string actual;
            actual = target.getMD5(message);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///Тест для getUserId
        ///</summary>
        [TestMethod()]
        public void getUserIdTest()
        {
            bool logging_http = false; // TODO: инициализация подходящего значения
            bool logging_debug = false; // TODO: инициализация подходящего значения
            StreamWriter error = null; // TODO: инициализация подходящего значения
            StreamWriter http = null; // TODO: инициализация подходящего значения
            StreamWriter debug = null; // TODO: инициализация подходящего значения
            DBRequests target = new DBRequests(logging_http, logging_debug, error, http, debug); // TODO: инициализация подходящего значения
            string name = "Пискарев Николай Сергеевич"; // TODO: инициализация подходящего значения
            int expected = 1; // TODO: инициализация подходящего значения
            int actual;
            actual = target.getUserId(name);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///Тест для getUserInfo
        ///</summary>
        [TestMethod()]
        public void getUserInfoTest()
        {
            bool logging_http = false; // TODO: инициализация подходящего значения
            bool logging_debug = false; // TODO: инициализация подходящего значения
            StreamWriter error = null; // TODO: инициализация подходящего значения
            StreamWriter http = null; // TODO: инициализация подходящего значения
            StreamWriter debug = null; // TODO: инициализация подходящего значения
            DBRequests target = new DBRequests(logging_http, logging_debug, error, http, debug); // TODO: инициализация подходящего значения
            string name = "Пискарев Николай Сергеевич"; // TODO: инициализация подходящего значения
            string password = "2f2522f0adfe0bee91d9c6b67c164aff"; // TODO: инициализация подходящего значения
            DataSet actual;
            actual = target.getUserInfo(name, password);
            int idU = (int)actual.Tables["users"].Rows[0].ItemArray[0];
            if (idU == 1)
                Assert.AreEqual(1, 1);
            else
                Assert.AreEqual(1, 0);
        }

        /// <summary>
        ///Тест для getUserInfo
        ///</summary>
        [TestMethod()]
        public void getUserInfoTest1()
        {
            bool logging_http = false; // TODO: инициализация подходящего значения
            bool logging_debug = false; // TODO: инициализация подходящего значения
            StreamWriter error = null; // TODO: инициализация подходящего значения
            StreamWriter http = null; // TODO: инициализация подходящего значения
            StreamWriter debug = null; // TODO: инициализация подходящего значения
            DBRequests target = new DBRequests(logging_http, logging_debug, error, http, debug); // TODO: инициализация подходящего значения
            string name = "Пискарев Николай Сергеевич"; // TODO: инициализация подходящего значения
            DataSet actual;
            actual = target.getUserInfo(name);
            int idU = (int)actual.Tables["users"].Rows[0].ItemArray[0];
            if (idU == 1)
                Assert.AreEqual(1, 1);
            else
                Assert.AreEqual(1, 0);
        }

        /// <summary>
        ///Тест для getUserName
        ///</summary>
        [TestMethod()]
        public void getUserNameTest()
        {
            bool logging_http = false; // TODO: инициализация подходящего значения
            bool logging_debug = false; // TODO: инициализация подходящего значения
            StreamWriter error = null; // TODO: инициализация подходящего значения
            StreamWriter http = null; // TODO: инициализация подходящего значения
            StreamWriter debug = null; // TODO: инициализация подходящего значения
            DBRequests target = new DBRequests(logging_http, logging_debug, error, http, debug); // TODO: инициализация подходящего значения
            int id = 1; // TODO: инициализация подходящего значения
            string expected = "Пискарев Николай Сергеевич"; // TODO: инициализация подходящего значения
            string actual;
            actual = target.getUserName(id);
            Assert.AreEqual(expected, actual);
        }

    }
}
