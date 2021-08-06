using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Models
{
    public class Convert<T>
    {
        public Convert() { }
        public Convert(T data)
        {
            Data = data;
        }
        public T Data { get; set; }
    }
}
