using System;

namespace Karata.Shared.Models
{
    public abstract class Message<T>
    {
        public string ConnectionId { get; set; }
        public T Content { get; set; }
        public DateTimeOffset DateSent { get; set; }
    }
}
