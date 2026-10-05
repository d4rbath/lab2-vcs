using System;
using System.Collections.Generic;
using System.Windows;
using System.Xml.Linq;

namespace IPPRPO_lab1
{
    public class User
    {
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string TelegramHandle { get; set; } = string.Empty;
    }

    public interface INotifier
    {
        void Send(User user, string message);
    }


    public class EmailNotifier : INotifier
    {
        private readonly string _smtpServer;
        private readonly Action<string> _logger;

        public EmailNotifier(string smtpServer, Action<string> logger)
        {
            _smtpServer = smtpServer;
            _logger = logger;
        }

        public void Send(User user, string message)
        {
            _logger($"[EMAIL]  Отправка на {user.Email} (сервер {_smtpServer}): {message}");
        }
    }

    public class SmsNotifier : INotifier
    {
        private readonly string _gateway;
        private readonly Action<string> _logger;

        public SmsNotifier(string gateway, Action<string> logger)
        {
            _gateway = gateway;
            _logger = logger;
        }

        public void Send(User user, string message)
        {
            _logger($"[SMS]    Отправка на номер {user.Phone} через {_gateway}: {message}");
        }
    }

    public class PushNotifier : INotifier
    {
        private readonly string _appKey;
        private readonly Action<string> _logger;

        public PushNotifier(string appKey, Action<string> logger)
        {
            _appKey = appKey;
            _logger = logger;
        }

        public void Send(User user, string message)
        {
            _logger($"[PUSH]   Отправка пользователю {user.Name} (Key: {_appKey}): {message}");
        }
    }

    public class TelegramNotifier : INotifier
    {
        private readonly string _botToken;
        private readonly Action<string> _logger;

        public TelegramNotifier(string botToken, Action<string> logger)
        {
            _botToken = botToken;
            _logger = logger;
        }

        public void Send(User user, string message)
        {
            _logger($"[TELEGRAM] Отправка в {user.TelegramHandle} (BotToken: {_botToken}): {message}");
        }
    }


    public class NotificationService
    {
        private readonly IEnumerable<INotifier> _notifiers;

        public NotificationService(IEnumerable<INotifier> notifiers)
        {
            _notifiers = notifiers ?? throw new ArgumentNullException(nameof(notifiers));
        }

        public void NotifyAll(User user, string message)
        {
            foreach (var notifier in _notifiers)
            {
                notifier.Send(user, message);
            }
        }
    }


    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void BtnSend_Click(object sender, RoutedEventArgs e)
        {
            var user = new User
            {
                Name = TxtName.Text,
                Email = TxtEmail.Text,
                Phone = TxtPhone.Text,
                TelegramHandle = TxtTelegram.Text
            };

            string message = TxtMessage.Text;

            Action<string> logAction = (logMessage) =>
            {
                LstLog.Items.Add($"[{DateTime.Now:HH:mm:ss}] {logMessage}");
            };

            var activeNotifiers = new List<INotifier>();

            if (ChkEmail.IsChecked == true)
                activeNotifiers.Add(new EmailNotifier("smtp.mail.com", logAction));

            if (ChkSms.IsChecked == true)
                activeNotifiers.Add(new SmsNotifier("api.sms.by", logAction));

            if (ChkPush.IsChecked == true)
                activeNotifiers.Add(new PushNotifier("Firebase_Key_12345", logAction));

            if (ChkTelegram.IsChecked == true)
                activeNotifiers.Add(new TelegramNotifier("BOT_TOKEN_99887766", logAction));

            if (activeNotifiers.Count == 0)
            {
                MessageBox.Show("Выберите хотя бы один канал отправки!", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var notificationService = new NotificationService(activeNotifiers);

            LstLog.Items.Add($"=== СТАРТ РАССЫЛКИ ДЛЯ {user.Name.ToUpper()} ===");
            notificationService.NotifyAll(user, message);
            LstLog.Items.Add("=== РАССЫЛКА УСПЕШНО ЗАВЕРШЕНА ===\n");

            if (LstLog.Items.Count > 0)
                LstLog.ScrollIntoView(LstLog.Items[LstLog.Items.Count - 1]);
        }

        private void BtnClearLog_Click(object sender, RoutedEventArgs e)
        {
            LstLog.Items.Clear();
        }
    }
}