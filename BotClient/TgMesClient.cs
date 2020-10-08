using System;
using System.Diagnostics;
using System.IO;
using Telegram.Bot.Args;
using Telegram.Bot;
using Telegram.Bot.Requests;
using RestSharp;
using Telegram.Bot.Types.InputFiles;
using System.Runtime.CompilerServices;
using System.Linq.Expressions;

namespace BotClient
{
    class TgMesClient
    {
        

        private string token; 
        private MainWindow w;
        private TelegramBotClient bot;
         
       
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
       
        /// <summary>
        /// Конструктор клиента
        /// </summary>
        /// <param name="W">вызывающее окно</param>
        public TgMesClient(MainWindow W)
        {
            try
            { 
                this.token = File.ReadAllText(@"token.txt");
                
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Токен не может быть прочитан " + ex.Message);
                throw;
            }   

            
           

            this.MessageLog = new MessageHistory();

            this.ContactList = new BotContactList();

            this.Catalog = new FileCatalog();
            
            
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
            
           this.w.Dispatcher.Invoke(async() =>
            {
                string botReply = "Спасибо. ";
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
                    try
                    {
                        
                        DownLoad(IdentifiedFile(e));
                    }
                    catch (Exception ex)
                    {

                       botReply += ex.Message;
                    }
                    finally
                    {
                        await bot.SendTextMessageAsync(e.Message.Chat.Id, botReply);
                        MessageLog.Add(new MessageRec(DateTime.Now.ToLongTimeString(), 0, "Bot", botReply, "Text"));
                    }
                    
                }
 
            });

        }

        
        /// <summary>
        /// Загружает файл из телеграмм чата
        /// </summary>
        /// <param name="f">атрибуты полученного файла</param>
        public async void DownLoad(FileProperties f)
        {
            
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
                
                Debug.WriteLine("Ошибка загрузки файла: " + ex.Message);
                throw;
                
            }
            finally
            {
                Catalog.Add(f);
                
            }
        }

        /// <summary>
        /// Обрабатывает текстовый запрос пользователя и отвечает на него в чат
        /// </summary>
        /// <param name="text">текст запроса</param>
        /// <param name="ChatId">ID пользователя</param>
        public async void ReplyOnText(string text, long ChatId)
        {
            string botReply = "";
            
            switch (text)
            {

                case "/ковид":  //обработка запроса на информацию по Covid19: отправка фото и данных 
                    
                    SendPhoto("coronavirus-5018466_640.jpg", ChatId);
                    try
                    {   
                        
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
                        await bot.SendTextMessageAsync(ChatId, botReply);
                        MessageLog.Add(new MessageRec(DateTime.Now.ToLongTimeString(), 0, "Bot", botReply, "Text"));
                    }
                    break;

               
                case "/файл":   //отправка файла по запросу

                    SendFile("covid-19-4938932_640.png", ChatId);
                    
                    break;

                default:    // меню 
                    botReply = "Доступные команды:\n" +
                           "/ковид - получить актуальную информацию о распространении коронавирусной инфекции в мире;\n" +
                           "/файл - получить котика.\n" +
                           "Боту можно отправить файл, картинку, видео и звук.";
                    
                    await bot.SendTextMessageAsync(ChatId, botReply);
                    MessageLog.Add(new MessageRec(DateTime.Now.ToLongTimeString(), 0, "Bot", botReply, "Text"));
                    break;
            }
                                   
        }

        // Два метода ниже практически дублируют друг друга, наверное, можно переписать их (используя делегаты?)

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
        public FileProperties IdentifiedFile(MessageEventArgs e)
        {
            string fullPath;
            string fileName;
            FileProperties f;
            Exception unknownType = new Exception("Неизвестный тип файла. Не знаю, что с ним делать.");

            switch (e.Message.Type)
            {
                
                case Telegram.Bot.Types.Enums.MessageType.Photo:
                    
                    fileName = "photo" + Guid.NewGuid() + ".jpg";
                    fullPath = $@"{Catalog.PathToUserFiles}\{e.Message.Type}\{fileName}";
                    f = new FileProperties(fullPath, e.Message.Photo[e.Message.Photo.Length - 1].FileId, e.Message.Type.ToString(),
                                            DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss"), e.Message.Chat.FirstName, e.Message.Chat.Id);
                    break;

                case Telegram.Bot.Types.Enums.MessageType.Audio:

                    fileName = "audio" + Guid.NewGuid() + ".mp3";
                    fullPath = $@"{Catalog.PathToUserFiles}\{e.Message.Type}\{fileName}";
                    f = new FileProperties(fullPath, e.Message.Audio.FileId, e.Message.Type.ToString(),
                                            DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss"), e.Message.Chat.FirstName, e.Message.Chat.Id);
                    break;

                case Telegram.Bot.Types.Enums.MessageType.Video:

                    fullPath = $@"{Catalog.PathToUserFiles}\{e.Message.Type}\{e.Message.Document.FileName}";
                    f = new FileProperties(fullPath, e.Message.Video.FileId, e.Message.Type.ToString(),
                                            DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss"), e.Message.Chat.FirstName, e.Message.Chat.Id);
                    break;
                
                case Telegram.Bot.Types.Enums.MessageType.Document:
                   
                    fullPath = $@"{Catalog.PathToUserFiles}\{e.Message.Type}\{e.Message.Document.FileName}";
                    f = new FileProperties(fullPath, e.Message.Document.FileId, e.Message.Type.ToString(),
                                            DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss"), e.Message.Chat.FirstName, e.Message.Chat.Id);
                    break;
                
                default:

                    throw unknownType;
                    
            }


            //switch (e.Message.Type.ToString())
            //{
            //    case "Document":    // формирование пути для сохранения файла с именем, полученным от пользователя
                    
            //        fullPath = $@"{Catalog.PathToUserFiles}\{e.Message.Type}\{e.Message.Document.FileName}";

            //        f = new FileProperties(fullPath, e.Message.Document.FileId, e.Message.Type.ToString(), 
            //                        DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss"), e.Message.Chat.FirstName, e.Message.Chat.Id);
            //        break;

            //    case "Photo":   // формирование пути для сохранения фотографии с уникальным именем
                    
            //        fileName = "photo" + Guid.NewGuid() + ".jpg";
            //        fullPath = $@"{Catalog.PathToUserFiles}\{e.Message.Type}\{fileName}";

            //        f = new FileProperties(fullPath, e.Message.Photo[e.Message.Photo.Length - 1].FileId, e.Message.Type.ToString(), 
            //                        DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss"), e.Message.Chat.FirstName, e.Message.Chat.Id);
            //        break;
            //    case "Audio": // формирование пути для сохранения аудиофайла с уникальным именем
                    
            //        fileName = "audio" + Guid.NewGuid() + ".mp3";
            //        fullPath = $@"{Catalog.PathToUserFiles}\{e.Message.Type}\{fileName}";

            //        f = new FileProperties(fullPath, e.Message.Audio.FileId, e.Message.Type.ToString(), 
            //                        DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss"), e.Message.Chat.FirstName, e.Message.Chat.Id);
            //        break;

            //    case "Video":   // формирование пути для сохранения файла с именем, полученным от пользователя
            //        fullPath = $@"{Catalog.PathToUserFiles}\{e.Message.Type}\{e.Message.Document.FileName}";

            //        f = new FileProperties(fullPath, e.Message.Video.FileId, e.Message.Type.ToString(), 
            //                        DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss"), e.Message.Chat.FirstName, e.Message.Chat.Id);
            //        break;
            //    default:
                    
            //        throw unknownType;
            //}

            return f;
        }
        
    }
}
