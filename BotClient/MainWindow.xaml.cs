using System;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Win32;

namespace BotClient
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        // Создать UI для бота из ДЗ 9
        // Предусмотреть возможность сохранения истории сообщений, присланных боту в JSON-файл 
        
        
        TgMesClient client;
        
        public MainWindow()
        {
            InitializeComponent();
            client = new TgMesClient(this);
            listViewF.ItemsSource = client.Catalog.Files;
            Messages.ItemsSource = client.MessageLog.Messages;
            Contacts.ItemsSource = client.ContactList.Contacts;
            StatusInfo.Text = $"Клиент успешно стартовал {DateTime.Now.ToShortDateString()} в {DateTime.Now.ToLongTimeString()}";
        }

        private void DataWindow_Closing(object sender, CancelEventArgs e)
        {
            client.MessageLog.Save();
            client.ContactList.Save();
            client.Catalog.Save();
            Application.Current.Shutdown();
        }

                       
        private void SaveHistory_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog f = new SaveFileDialog();
            if (f.ShowDialog() == true)
            {
                client.MessageLog.Save(f.FileName);
                StatusInfo.Text = $"История сообщений выгружена в файл {f.FileName}";
            }
        }

        private void SaveContacts_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog f = new SaveFileDialog();
            if (f.ShowDialog() == true)
            {
                client.ContactList.Save(f.FileName);
                StatusInfo.Text = $"Контакты выгружены в файл {f.FileName}";

            }
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void About_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Простой телеграм-бот (домашнее задание 10), Автор: К. Польская");
        }


    }
}
