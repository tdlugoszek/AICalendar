using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AICalendar.Shared.Models
{
    internal class ChatMessage
    {
        public string Content { get; set; } = string.Empty;
        public bool IsFromUser { get; set; }
        public DateTime Timestamp { get; set; }
        public string? EventData { get; set; }
    }

    internal class ChatRequest
    {
        public string Message { get; set; } = string.Empty;
        public string UserId { get; set; } = string.Empty;
    }
}
