using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Karata.Server.Models
{
    public abstract class Message<T>
    {
        public string Content { get; set; }
    }
}
