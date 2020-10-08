using System;
using System.Diagnostics;
using System.IO;
using Telegram.Bot.Args;
using Telegram.Bot;
using Telegram.Bot.Requests;
using RestSharp;
using Telegram.Bot.Types.InputFiles;

namespace BotClient
{
    class TgMesClient
    {
        #region Поля

        private string token; 
        private MainWindow w;
        private TelegramBotClient bot;
        
        //текущее сообщение в чате
        //private MessageRec message;
        ////текущий контакт
        //private BotContact botContact;
        #endregion 

        #region Автосвойства
        /// <summary>
        /// Путь к файлу с историей сообщений
        /// </summary>
        public string HistoryPath { get { return $@"{Directory.GetCurrentDirectory()}\messagelog.json"; } } 

        /// <summary>
        /// Путь к файлу списка контактов
        /// </summary>
        public string ContactPath { get { return $@"{Directory.GetCurrentDirectory()}\contacts.json"; } } 

        /// <summary>
        /// Путь к файлу каталога полученных файлов
        /// </summary>
        public string CatalogPath { get { return $@"{Directory.GetCurrentDirectory()}\catalog.json"; } }

        /// <summary>
        /// Лог сообщений
        /// </summary>
        public MessageHistory MessageLog { get; set; }
        
        /// <summary>
        /// Каталог присланных файлов
        /// </summary>
        public FileCatalog Catalog { get; set; }
        
        /// <summary>
        /// Список контактов бота
        /// </summary>
        public BotContactList ContactList { get; set; }
        #endregion

        /// <summary>
        /// Конструктор клиента
        /// </summary>
        /// <param name="W">вызывающее окно</param>
        public TgMesClient(MainWindow W)
        {
            this.token = File.ReadAllText(@"token.txt");
           

            if (File.Exists(HistoryPath))
                this.MessageLog = new MessageHistory(HistoryPath);
            else
                this.MessageLog = new MessageHistory();

            if (File.Exists(ContactPath))
                this.ContactList = new BotContactList(ContactPath);
            else
                this.ContactList = new BotContactList();

            if (File.Exists(CatalogPath))
                this.Catalog = new FileCatalog(CatalogPath);

            else
                this.Catalog = new FileCatalog();
            
            if (!Directory.Exists(Catalog.PathToUserFiles))
            {
                try
                {
                    Directory.CreateDirectory(Catalog.PathToUserFiles);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("Невозможно создать папку каталога! " + ex.Message);
                    throw ex;
                }
            }
                
          

            this.w = W;

            bot = new TelegramBotClient(this.token);

            bot.OnMessage += MessageListener;

            bot.StartReceiving();

        }

        /// <summary>
        /// Получает сообщения в фоновом режиме
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void MessageListener(object sender, Telegram.Bot.Args.MessageEventArgs e)
        {
            
           this.w.Dispatcher.Invoke(() =>
            {
               
                MessageRec message = new MessageRec(DateTime.Now.ToLongTimeString(), e.Message.Chat.Id, e.Message.Chat.FirstName, e.Message.Text, e.Message.Type.ToString());
                MessageLog.Add(message);

                BotContact botContact = new BotContact(e.Message.Chat.FirstName, e.Message.Chat.Id);

                if (!ContactList.Contains(botContact))
                    ContactList.Add(botContact);

                if (e.Message.Type.ToString() == "Text")
                {
                    string messageText = e.Message.Text.ToLower();
                    ReplyOnText(messageText, e.Message.Chat.Id);
                }
                else
                {
                     DownLoad(IdentifiedFile(e));

                }

           });

        }

        
        /// <summary>
        /// Загружает файл из телеграмм чата
        /// </summary>
        /// <param name="f">атрибуты полученного файла</param>
        public async void DownLoad(MyFile f)
        {
            string botReply = "Спасибо. ";
            try
            {
                if (!Directory.Exists($@"{Catalog.PathToUserFiles}\{f.FileType}"))
                {
                    Directory.CreateDirectory($@"{Catalog.PathToUserFiles}\{f.FileType}");
                }
                var file = await bot.GetFileAsync(f.FileId);
                FileStream fs = new FileStream(f.FilePath, FileMode.Create);
             
                await bot.DownloadFileAsync(file.FilePath, fs);
               
                fs.Close();

                fs.Dispose();
                f.IsDownloaded = true; // отметка о том, что файл был успешно загружен
               
            }
            catch (Exception ex)
            {
                botReply += "Возможно, мне неизвестен тип этого файла или произошла ошибка при загрузке.";
                Debug.WriteLine("Ошибка загрузки файла: " + ex.Message);
                
            }
            finally
            {
                Catalog.Add(f);
                await bot.SendTextMessageAsync(f.ChatId, botReply);
                MessageLog.Add(new MessageRec(DateTime.Now.ToLongTimeString(), 0, "Bot", botReply, "Text"));
            }
        }

        /// <summary>
        /// Обрабатывает текстовый запрос пользователя и отвечает на него в чат
        /// </summary>
        /// <param name="text">текст запроса</param>
        /// <param name="chatID">ID пользователя</param>
        public async void ReplyOnText(string text, long chatID)
        {
            string botReply = "";
            
            switch (text)
            {

                case "/ковид":  //обработка запроса на информацию по Covid19: отправка фото и данных 
                    
                    SendPhoto("coronavirus-5018466_640.jpg", chatID);
                    try
                    {   
                        
                        ////запрос через RapidAPI агрегатор, бесплатный режим с ограничениями по количеству запросов
                        //    var client = new RestClient("https://covid-19-data.p.rapidapi.com/totals?format=json");
                        //    var request = new RestRequest(Method.GET);
                        //    request.AddHeader("x-rapidapi-host", "covid-19-data.p.rapidapi.com");
                        //    request.AddHeader("x-rapidapi-key", "d93af22c62msh260c18d52dc8569p147315jsn68b6d709f561"); //k
                        //    IRestResponse response = client.Execute(request);

                        //    botReply = "Всего в мире: \n";
                        //    Newtonsoft.Json.Linq.JArray o = Newtonsoft.Json.Linq.JArray.Parse(response.Content);
                        //    var confirmed = (string)(o[0]["confirmed"]);
                        //    var recovered = (string)o[0]["recovered"];
                        //    var critical = (string)o[0]["critical"];
                        //    var deaths = (string)o[0]["deaths"];
                        //    var lastUpdate = (string)o[0]["lastUpdate"];
                        //    botReply += $"Подтвержденных случаев {confirmed}\nВыздоровeло {recovered}\nВ критическом состоянии {critical}\n" +
                        //                $"Умерло {deaths}\nИнформация обновлена {lastUpdate}";

                        //    await bot.SendTextMessageAsync(chatID, botReply);
                        //    Console.WriteLine($"{DateTime.Now.ToLongTimeString()} | Bot | {chatID}: {botReply} | Text");

                        //альтернативный источник информации о Covid 19 - без токена.
                        var client = new RestClient("https://api.covid19api.com/world/total");
                        client.Timeout = -1;
                        var request = new RestRequest(Method.GET);
                        IRestResponse response = client.Execute(request);
                        botReply = "Всего в мире: \n";
                        var o = Newtonsoft.Json.Linq.JObject.Parse(response.Content);
                        string confirmed = o.SelectToken("TotalConfirmed").ToString();
                        string deaths = o.SelectToken("TotalDeaths").ToString();
                        string recovered = o.SelectToken("TotalRecovered").ToString();
                        botReply += $"подтвержденных случаев  {confirmed}\nсмертей  {deaths}\nвыздоровело  {recovered}";
                        
                    }
                    catch 
                    {
                        botReply = "Извините, произошла ошибка. Возможно, Covid 19 data center не отвечает.";
                        
                    }
                    finally
                    {
                        await bot.SendTextMessageAsync(chatID, botReply);
                        MessageLog.Add(new MessageRec(DateTime.Now.ToLongTimeString(), 0, "Bot", botReply, "Text"));
                    }
                    break;

               
                case "/файл":   //отправка файла по запросу

                    SendFile("covid-19-4938932_640.png", chatID);
                    
                    break;

                default:    // меню 
                    botReply = "Доступные команды:\n" +
                           "/ковид - получить актуальную информацию о распространении коронавирусной инфекции в мире;\n" +
                           "/файл - получить котика.\n" +
                           "Боту можно отправить файл, картинку, видео и звук.";
                    
                    await bot.SendTextMessageAsync(chatID, botReply);
                    MessageLog.Add(new MessageRec(DateTime.Now.ToLongTimeString(), 0, "Bot", botReply, "Text"));
                    break;
            }
                                   
        }

        // Два метода ниже практически дублируют друг друга, можно переписать их используя делегаты!

        /// <summary>
        /// Отправляет фото в телеграмм чат
        /// </summary>
        /// <param name="path">путь для сохранения на компьютере</param>
        /// <param name="chatId">ID чата</param>
        async void SendPhoto(string path, long chatId)
        {
            try
            {
                using (FileStream fs = File.OpenRead(path))
                {
                    InputOnlineFile inputOnlineFile = new InputOnlineFile(fs, path);
                    await bot.SendPhotoAsync(chatId, inputOnlineFile);
                    MessageLog.Add(new MessageRec(DateTime.Now.ToLongTimeString(), 0, "Bot", "", "Photo"));
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Ошибка чтения/отправки фото: " + ex.Message);
            }

        }

        /// <summary>
        /// Отправляет файл в телеграмм чат
        /// </summary>
        /// <param name="path">путь к файлу на компьютере</param>
        /// <param name="chatId">ID чата</param>
        async void SendFile(string path, long chatId)
        {
            try
            {
                using (FileStream fs = File.OpenRead(path))
                {
                    InputOnlineFile inputOnlineFile = new InputOnlineFile(fs, path);
                    await bot.SendDocumentAsync(chatId, inputOnlineFile);
                    MessageLog.Add(new MessageRec(DateTime.Now.ToLongTimeString(), 0, "Bot", "", "Document"));
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Ошибка чтения/отправки файла: " + ex.Message);
            }

        }
  

        /// <summary>
        /// Обрабатывает информацию об отправленном пользователем файле
        /// </summary>
        /// <param name="e">сообщение от бота</param>
        /// <returns>Атрибуты полученного файла для записи в каталог</returns>
        public MyFile IdentifiedFile(MessageEventArgs e)
        {
            string fullPath;
            string fileName;
            MyFile f;
            
            switch (e.Message.Type.ToString())
            {
                case "Document":    // формирование пути для сохранения файла с именем, полученным от пользователя
                    
                    fullPath = $@"{Catalog.PathToUserFiles}\{e.Message.Type}\{e.Message.Document.FileName}";

                    f = new MyFile(fullPath, e.Message.Document.FileId, e.Message.Type.ToString(), 
                                    DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss"), e.Message.Chat.FirstName, e.Message.Chat.Id);
                    break;

                case "Photo":   // формирование пути для сохранения фотографии с уникальным именем
                    
                    fileName = "photo" + Guid.NewGuid() + ".jpg";
                    fullPath = $@"{Catalog.PathToUserFiles}\{e.Message.Type}\{fileName}";

                    f = new MyFile(fullPath, e.Message.Photo[e.Message.Photo.Length - 1].FileId, e.Message.Type.ToString(), 
                                    DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss"), e.Message.Chat.FirstName, e.Message.Chat.Id);
                    break;
                case "Audio": // формирование пути для сохранения аудиофайла с уникальным именем
                    
                    fileName = "audio" + Guid.NewGuid() + ".mp3";
                    fullPath = $@"{Catalog.PathToUserFiles}\{e.Message.Type}\{fileName}";

                    f = new MyFile(fullPath, e.Message.Audio.FileId, e.Message.Type.ToString(), 
                                    DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss"), e.Message.Chat.FirstName, e.Message.Chat.Id);
                    break;

                case "Video":   // формирование пути для сохранения файла с именем, полученным от пользователя
                    fullPath = $@"{Catalog.PathToUserFiles}\{e.Message.Type}\{e.Message.Document.FileName}";

                    f = new MyFile(fullPath, e.Message.Video.FileId, e.Message.Type.ToString(), 
                                    DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss"), e.Message.Chat.FirstName, e.Message.Chat.Id);
                    break;
                default:
                    f = new MyFile("", "", e.Message.Type.ToString(), 
                                    DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss"), e.Message.Chat.FirstName, e.Message.Chat.Id);
                    
                    break;
            }

            return f;
        }
        
    }
}
