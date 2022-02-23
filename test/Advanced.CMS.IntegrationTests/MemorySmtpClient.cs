using System.Collections.Generic;
using System.Threading.Tasks;
using EPiServer.Notification.Internal;
using MimeKit;

namespace Advanced.CMS.IntegrationTests
{
    public class MemorySmtpClient : ISmtpClient
    {
        private static readonly IList<MimeMessage> _messages = new List<MimeMessage>();

        public IList<MimeMessage> ReceivedMessages => _messages;

        public Task SendAsync(MimeMessage message)
        {
            _messages.Add(message);
            return Task.CompletedTask;
        }
    }
}
