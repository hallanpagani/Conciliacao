using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Conciliacao.App_Helpers.Componentes
{
    public static class NotificacoesExtensions
    {
        private static IDictionary<String, String> NotificationKey = new Dictionary<String, String>
            {
                { "Erro",       "App.Notifications.Erro" },
                { "Alerta",     "App.Notifications.Alerta" },
                { "Sucesso",    "App.Notifications.Sucesso" },
                { "Informação", "App.Notifications.Informação" }
            };

        public static void AddNotification(this ControllerBase controller, String message, String notificationType)
        {
            string NotificationKey = getNotificationKeyByType(notificationType);
            ICollection<String> messages = controller.TempData[NotificationKey] as ICollection<String>;

            if (messages == null)
            {
                controller.TempData[NotificationKey] = (messages = new HashSet<String>());
            }

            messages.Add(message);
        }

        public static IEnumerable<String> GetNotifications(this HtmlHelper htmlHelper, String notificationType)
        {
            string NotificationKey = getNotificationKeyByType(notificationType);
            return htmlHelper.ViewContext.Controller.TempData[NotificationKey] as ICollection<String> ?? null;
        }

        private static string getNotificationKeyByType(string notificationType)
        {
            try
            {
                return NotificationKey[notificationType];
            }
            catch (IndexOutOfRangeException e)
            {
                ArgumentException exception = new ArgumentException("Key is invalid", "notificationType", e);
                throw exception;
            }
        }
    }

    public static class NotificationType
    {
        public const string Erro = "Erro";
        public const string Alerta = "Alerta";
        public const string Sucesso = "Sucesso";
        public const string Informação = "Informação";

    }
}