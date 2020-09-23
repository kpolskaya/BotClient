using System;
using System.Collections.Generic;
using System.Collections;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Args;
using Telegram.Bot;
using Telegram.Bot.Requests;
using RestSharp;
using Telegram.Bot.Types.InputFiles;
using System.Collections.ObjectModel;

namespace BotClient
{
    class TgMesClient
    {
        private string token; 
        private MainWindow w;
        private string historyPath = $@"{Directory.GetCurrentDirectory()}\messagelog.json"; //! hard-coded
        private string contactPath = $@"{Directory.GetCurrentDirectory()}\contacts.json"; //! hard-coded

        private TelegramBotClient bot;
        //public ObservableCollection<MessageRec> BotMessageLog { get; set; }
        public MessageHistory botMessageLog { get; set; }
        MessageRec message;
        public FileCatalog catalog { get; set; }
        MyFile f;
        public ContactList botContactList { get; set; }
        BotContact botContact;

        public void MessageListener(object sender, Telegram.Bot.Args.MessageEventArgs e)
        {
            string text = $"{DateTime.Now.ToLongTimeString()} | {e.Message.Chat.FirstName} | {e.Message.Chat.Id}: {e.Message.Text} | {e.Message.Type}";
            Debug.WriteLine(text);
            

           w.Dispatcher.Invoke(() =>
            {
               
                message = new MessageRec(DateTime.Now.ToLongTimeString(), e.Message.Chat.Id, e.Message.Chat.FirstName, e.Message.Text, e.Message.Type.ToString());
                botMessageLog.Add(message);

                botContact = new BotContact(e.Message.Chat.FirstName, e.Message.Chat.Id);

                if (!botContactList.Contains(botContact))
                    botContactList.Add(botContact);

                if (!Directory.Exists(catalog.CatPath))
                {
                    Directory.CreateDirectory(catalog.CatPath);
                }
                if (e.Message.Type.ToString() == "Text")
                {
                    string messageText = e.Message.Text.ToLower();
                    ReplyOnText(messageText, e.Message.Chat.Id);
                }
                else
                {
                    ReplyOnFile(e);
                    DownLoad(f);
                   

                }

                //    BotMessageLog.Add(
                //    new MessageRec(
                //        DateTime.Now.ToLongTimeString(), e.Message.Chat.Id, e.Message.Chat.FirstName, e.Message.Text, e.Message.Type.ToString()));

               
            });



        }

        public TgMesClient(MainWindow W)
        {
            this.token = File.ReadAllText(@"token.txt");

            if (File.Exists(historyPath))
                this.botMessageLog = new MessageHistory(historyPath);
            else
                this.botMessageLog = new MessageHistory();

            if (File.Exists(contactPath))
                this.botContactList = new ContactList(contactPath);
            else
                this.botContactList = new ContactList();

            if (File.Exists($@"{Directory.GetCurrentDirectory()}\catalog.json"))
                this.catalog = new FileCatalog($@"{Directory.GetCurrentDirectory()}\catalog.json");
            
            else 
               this.catalog = new FileCatalog();

            this.w = W;
            

            //bot = new TelegramBotClient(File.ReadAllText(PathToken));
            bot = new TelegramBotClient(this.token);

            bot.OnMessage += MessageListener;

            bot.StartReceiving();
           
        }
        /// <summary>
        /// Загружает файл из телеграмм чата
        /// </summary>
        /// <param name="fileId">ID файла</param>
        /// <param name="path">путь для сохранения на компьютере</param>
        public async void DownLoad(MyFile f)
        {
            try
            {
                if (!Directory.Exists($@"{catalog.CatPath}\{f.FileType}"))
                {
                    Directory.CreateDirectory($@"{catalog.CatPath}\{f.FileType}");
                }
                var file = await bot.GetFileAsync(f.FileId);
                FileStream fs = new FileStream(f.FilePath, FileMode.Create);
             
                await bot.DownloadFileAsync(file.FilePath, fs);
               
                fs.Close();

                fs.Dispose();
                f.IsDownloaded = true;
                //catalog.Files[catalog.Files.Count-1].IsDownloaded = true;                        //!!! был глюк с гифом - не проставилась отметка о записи - проверить!

                // Catalog.AddFile(path, type, chatId);

            }
            catch (Exception ex)
            {
                Debug.WriteLine("Ошибка загрузки файла: " + ex.Message);////????
                
            }
            finally
            {
                catalog.Add(f);

            }
        }

        /// <summary>
        /// Обрабатывает текстовый запрос пользователя и отвечает на него в чат
        /// </summary>
        /// <param name="text">текст запроса</param>
        /// <param name="chatID">ID чата</param>
        public async void ReplyOnText(string text, long chatID)
        {
            string botReply;
            switch (text)
            {

                case "/ковид":  //обработка запроса на информацию по Covid19: отправка фото и данных с covid19api.com в телеграмм чат
                                //запрос через RapidAPI агрегатор, бесплатный режим с ограничениями по количеству запросов
                    SendPhoto("coronavirus-5018466_640.jpg", chatID);
                    Console.WriteLine($"{DateTime.Now.ToLongTimeString()} | Bot | {chatID}: coronavirus-5018466_640.jpg | Photo");
                    //try
                    //{
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

                    //    ////альтернативный источник информации о Covid 19 - без токена.
                    //    //var client = new RestClient("https://api.covid19api.com/world/total");
                    //    //client.Timeout = -1;
                    //    //var request = new RestRequest(Method.GET);
                    //    //IRestResponse response = client.Execute(request);
                    //    //botReply = "Всего в мире: \n";
                    //    //var o = Newtonsoft.Json.Linq.JObject.Parse(response.Content);
                    //    //string confirmed = o.SelectToken("TotalConfirmed").ToString();
                    //    //string deaths = o.SelectToken("TotalDeaths").ToString();
                    //    //string recovered = o.SelectToken("TotalRecovered").ToString();
                    //    //botReply += $"подтвержденных случаев  {confirmed}\nсмертей  {deaths}\nвыздоровело  {recovered}";
                    //    //await bot.SendTextMessageAsync(chatID, botReply);
                    //    //Console.WriteLine($"{DateTime.Now.ToLongTimeString()} | Bot | {chatID}: {botReply} | Text");
                    //}
                    //catch (Exception)
                    //{
                    //    await bot.SendTextMessageAsync(chatID,
                    //   $"Ошибка запроса. Covid 19 data center не отвечает.");
                    //    Console.WriteLine($"{DateTime.Now.ToLongTimeString()} | Bot | {chatID}: Ошибка запроса. Covid 19 data center не отвечает. | Text");
                    //}
                    break;


                case "/файл":   //отправка файла по запросу
                    SendFile("covid-19-4938932_640.png", chatID);
                    Console.WriteLine($"{DateTime.Now.ToLongTimeString()} | Bot | {chatID}: covid-19-4938932_640.png | Document");
                    break;

                default:    // меню 
                    botReply = "Доступные команды:\n" +
                           "/ковид - получить актуальную информацию о распространении коронавирусной инфекции в мире;\n" +
                           "/файл - получить котика.\n" +
                           "Боту можно отправить файл, картинку, видео и звук.";
                    await bot.SendTextMessageAsync(chatID, botReply);
                    Console.WriteLine($"{DateTime.Now.ToLongTimeString()} | Bot | {chatID}: {botReply} | Text");
                    break;
            }

            /// <summary>
            /// Загружает фото из телеграмм чата
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
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Ошибка загрузки файла: " + ex.Message);
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
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Ошибка чтения/отправки файла: " + ex.Message);
                }

            }
        }

       /// <summary>
       /// Принимает файл от пользователя, определяет путь для сохранения файла, формирует строку каталога
       /// </summary>
       /// <param name="e">сообщение от бота</param>
       /// <returns>строка каталога файлов</returns>
       public MyFile ReplyOnFile(MessageEventArgs e)
        {
            string FullPath;
            bot.SendTextMessageAsync(e.Message.Chat.Id, "Спасибо за файл");
            switch (e.Message.Type.ToString())
            {
                case "Document":    // получение файла, сохранение в директории, установленной ранее, с именем, полученным от пользователя
                    FullPath = $@"{catalog.CatPath}\{e.Message.Type}\{e.Message.Document.FileName}";
                    f = new MyFile(FullPath, e.Message.Document.FileId, e.Message.Type.ToString(), DateTime.Now.ToLongTimeString(),e.Message.Chat.FirstName, e.Message.Chat.Id);
                    break;

                case "Photo":   // получение файла фотографии, сохранение в директории, установленной ранее, с уникальным идентификатором
                    string namep = "photo" + Guid.NewGuid();
                    FullPath = $@"{catalog.CatPath}\{e.Message.Type}\{namep}";
                    f = new MyFile(FullPath, e.Message.Photo[e.Message.Photo.Length - 1].FileId, e.Message.Type.ToString(), DateTime.Now.ToLongTimeString(), e.Message.Chat.FirstName, e.Message.Chat.Id);
                    break;
                case "Audio": // получение аудио файла, сохранение в директории, установленной ранее, с уникальным идентификатором
                    string namea = "audio" + Guid.NewGuid();
                    FullPath = $@"{catalog.CatPath}\{e.Message.Type}\{namea}";
                    f = new MyFile(FullPath, e.Message.Audio.FileId, e.Message.Type.ToString(), DateTime.Now.ToLongTimeString(), e.Message.Chat.FirstName, e.Message.Chat.Id);
                    break;

                case "Video":   // получение видео файла, сохранение в директории, установленной ранее, с именем, полученным от пользователя
                    FullPath = $@"{catalog.CatPath}\{e.Message.Type}\{e.Message.Document.FileName}";
                    f = new MyFile(FullPath, e.Message.Video.FileId, e.Message.Type.ToString(), DateTime.Now.ToLongTimeString(), e.Message.Chat.FirstName, e.Message.Chat.Id);
                    break;
                default:
                    f = new MyFile("", "", e.Message.Type.ToString(), DateTime.Now.ToLongTimeString(), e.Message.Chat.FirstName, e.Message.Chat.Id);
                    bot.SendTextMessageAsync(e.Message.Chat.Id, "Файл неизвестного типа. Не знаю, что с ним делать.");
                    break;
            }
            return f;
        }

        public void BotStart()
        {
            bot.StartReceiving();
        }
        
    }
}
