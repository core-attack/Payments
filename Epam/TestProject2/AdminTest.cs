﻿using PaymentsWPF;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using System.Windows.Markup;
using System.Windows;
using System.Windows.Controls;

namespace TestProject2
{
    
    
    /// <summary>
    ///Это класс теста для AdminTest, в котором должны
    ///находиться все модульные тесты AdminTest
    ///</summary>
    [TestClass()]
    public class AdminTest
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
        ///Тест для generateBankAccount
        ///</summary>
        [TestMethod()]
        [DeploymentItem("PaymentsWPF.exe")]
        public void generateBankAccountTest()
        {
            Admin target = new Admin(false, false, null, null, null); // TODO: инициализация подходящего значения
            target.generateBankAccount();
            if (target.textBoxBankAccount.Text.Length == 20) 
                Assert.AreEqual(0, 0);
            else
                Assert.AreEqual(0, 1);
        }

    }
}
