using System;
using System.Collections.Generic;
//using System.Collections.Generic;
//using System.Linq;
using System.Net;
//using System.Threading.Tasks;
using System.Threading;
using Newtonsoft.Json;
using System.IO;

namespace CSharpMicroService
{
    class Product
    {
        private string name;
        private int price;

        public String Name
        {
            get { return name; }
            set { name = value; }

        }

        public int Price
        {
            get { return price; }
            set { price = value; }

        }
    }
}