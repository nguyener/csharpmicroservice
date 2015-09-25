using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Net;

namespace CSharpMicroService
{
    class HTTPProcessor
    {
        private HttpListenerContext context;
        public HTTPProcessor(HttpListenerContext context)
        {
            this.context = context;
        }

        public void ProcessRequest(object state)
        {
            HttpListenerRequest request = context.Request;
            string raw_url = request.RawUrl;
            if (raw_url.IndexOf("/index") == 0 || raw_url == "/")
            {
                DisplayIndex();
                return;
            }

            string id = "";
            string[] data = raw_url.Split('/');
            string collection = data[1];
            if (data.Length > 2)
            {
                id = data[2];
            }
            switch (request.HttpMethod.ToLower())
            {
                case "get":
                    ProcessGET(collection, id);
                    break;
                case "post":
                    ProcessPOST(collection);
                    break;
                case "put":
                    ProcessPUT(collection, id);
                    break;
                case "delete":
                    ProcessDELETE(collection, id);
                    break;
                default:
                    ErrorResponse("Method Not Allowed", 405);
                    break;
            }
        }
        private List<Product> loadData(string collection)
        {
            List<Product> fruits;
            using (StreamReader file = File.OpenText(@"fruits.json"))
            {
                JsonSerializer serializer = new JsonSerializer();
                fruits = (List<Product>)serializer.Deserialize(file, typeof(List<Product>));
            }
            return fruits;

        }

        private void saveData(List<Product> fruits)
        {
            using (StreamWriter file = File.CreateText(@"fruits.json.new"))
            {
                JsonSerializer serializer = new JsonSerializer();
                serializer.Serialize(file, fruits);
            }
            File.Replace(@"fruits.json.new", @"fruits.json", @"fruits.json.bck", false);

        }
        public void ProcessGET(string collection, string id)
        {

            // Obtain a response object.
            HttpListenerResponse response = context.Response;
            // Construct a response.

            List<Product> fruits = loadData(collection);
            string responseString;
            if (id == "" || id == null)
            {
                responseString = JsonConvert.SerializeObject(fruits);
            }
            else
            {
                var fruit = fruits.Find(x => x.Name == id);
                if (fruit == null)
                {
                    responseString = "<HTML><BODY> NOT FOUND</BODY></HTML>";
                }
                else
                {
                    responseString = JsonConvert.SerializeObject(fruit);
                }

            }



            byte[] buffer = System.Text.Encoding.UTF8.GetBytes(responseString);
            // Get a response stream and write the response to it.
            //response.ContentType = "application/json";


            response.ContentLength64 = buffer.Length;
            System.IO.Stream output = response.OutputStream;
            output.Write(buffer, 0, buffer.Length);
            // You must close the output stream.
            output.Close();
        }

        public void ProcessPOST(string collection)
        {
            HttpListenerRequest request = context.Request;
            string text;

            using (var reader = new StreamReader(request.InputStream,
                                     request.ContentEncoding))
            {
                text = reader.ReadToEnd();
            }


            // Obtain a response object.
            HttpListenerResponse response = context.Response;
            List<Product> fruits = loadData(collection);

            fruits.Add(JsonConvert.DeserializeObject<Product>(text));

            string responseString = JsonConvert.SerializeObject(fruits);
            byte[] buffer = System.Text.Encoding.UTF8.GetBytes(responseString);
            // Get a response stream and write the response to it.
            //response.ContentType = "application/json";


            response.ContentLength64 = buffer.Length;
            System.IO.Stream output = response.OutputStream;
            output.Write(buffer, 0, buffer.Length);
            // You must close the output stream.
            output.Close();
            saveData(fruits);
        }

        public void ProcessPUT(string collection, string id)
        {
            //check for input id
            if (id == "" || id == null)
            {
                ErrorResponse("Bad Request. Id is not specified ", 400);
                return;
            }

            //loading up the collection and look for specified item
            List<Product> fruits = loadData(collection);
            var fruit = fruits.Find(x => x.Name == id);
            if (fruit == null)
            {
                ErrorResponse("Specified item is not found", 404);
                return;
            }

           
            //get input data from request
            HttpListenerRequest request = context.Request;
            string data_input;

            using (var reader = new StreamReader(request.InputStream, request.ContentEncoding))
            {
                data_input = reader.ReadToEnd();
            }

            
            var item_input = JsonConvert.DeserializeObject<Product>(data_input);
            fruit.Price = item_input.Price;

            saveData(fruits);

            // Obtain a response object.
            HttpListenerResponse response = context.Response;
            // Construct a response.
            string responseString;
            responseString = "<HTML><BODY>UPDATED</BODY></HTML>";
            
            byte[] buffer = System.Text.Encoding.UTF8.GetBytes(responseString);
            // Get a response stream and write the response to it.
            //response.ContentType = "application/json";


            response.ContentLength64 = buffer.Length;
            System.IO.Stream output = response.OutputStream;
            output.Write(buffer, 0, buffer.Length);
            // You must close the output stream.
            output.Close();
        }


        public void ProcessDELETE(string collection, string id)
        {

            //check for input id
            if (id == "" || id == null)
            {
                ErrorResponse("Bad Request. Id is not specified ", 400);
                return;
            }

            //loading up the collection and look for specified item
            List<Product> fruits = loadData(collection);
            var fruit = fruits.Find(x => x.Name == id);
            if (fruit == null)
            {
                ErrorResponse("Specified item is not found", 404);
                return;
            }


            fruits.RemoveAll(item => item.Name == id);
            saveData(fruits);

            // Obtain a response object.
            HttpListenerResponse response = context.Response;
            // Construct a response.
            string responseString;
            responseString = "<HTML><BODY> UPDATED</BODY></HTML>";
            
            byte[] buffer = System.Text.Encoding.UTF8.GetBytes(responseString);
            // Get a response stream and write the response to it.
            //response.ContentType = "application/json";

            response.ContentLength64 = buffer.Length;
            System.IO.Stream output = response.OutputStream;
            output.Write(buffer, 0, buffer.Length);
            // You must close the output stream.
            output.Close();
        }

        public void DisplayIndex()
        {
            HttpListenerRequest request = context.Request;
            // Obtain a response object.
            HttpListenerResponse response = context.Response;
            // Construct a response.
            string responseString = "<HTML><BODY> Hello world!</BODY></HTML>";
            byte[] buffer = System.Text.Encoding.UTF8.GetBytes(responseString);
            // Get a response stream and write the response to it.
            response.ContentLength64 = buffer.Length;
            System.IO.Stream output = response.OutputStream;
            output.Write(buffer, 0, buffer.Length);
            // You must close the output stream.
            output.Close();

        }

        public void ErrorResponse(string error_msg, int error_code)
        {
            // Obtain a response object.
            HttpListenerResponse response = context.Response;
            context.Response.StatusCode = error_code;
            context.Response.StatusDescription = "Error";
            // Construct a response.
           
            byte[] buffer = System.Text.Encoding.UTF8.GetBytes(error_msg);
            // Get a response stream and write the response to it.
            response.ContentLength64 = buffer.Length;
            System.IO.Stream output = response.OutputStream;
            output.Write(buffer, 0, buffer.Length);
            // You must close the output stream.
            output.Close();
        }
    }
}
