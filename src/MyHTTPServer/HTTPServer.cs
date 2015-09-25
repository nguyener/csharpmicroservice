using System;
using System.Net;
using System.Threading;

namespace CSharpMicroService
{
    public class HTTPServer
    {
        HttpListener listener;

        public HTTPServer()
        {

            string[] prefixes = { "http://localhost:8080/" };

            // Create a listener.
            listener = new HttpListener();
            // Add the prefixes.
            foreach (string s in prefixes)
            {
                listener.Prefixes.Add(s);
            }
        }

        public void Start()
        {
            listener.Start();
            //IAsyncResult result = listener.BeginGetContext(new AsyncCallback(ListenerCallback), listener);
            //IAsyncResult result = listener.BeginGetContext(ListenerCallback, listener);
            Console.WriteLine("Listening...");
            
            while (true)
            {
                //result.AsyncWaitHandle.WaitOne();
                HttpListenerContext ctx = listener.GetContext();
                //new Thread(new HTTPProcessor(ctx).ProcessRequest).Start();
                ThreadPool.QueueUserWorkItem(new WaitCallback(new HTTPProcessor(ctx).ProcessRequest));
            }
            //Console.WriteLine("Request processed asynchrously");

            //listener.Close();
            //Console.ReadLine();

        }

        void ThreadProc(Object state)
        {
            Console.WriteLine("Processing request");
        }


        public static void ListenerCallback(IAsyncResult result)
        {
            HttpListener listener = (HttpListener)result.AsyncState;

            if (!listener.IsListening)
            {
                Console.WriteLine("NOT Listening...");

                return;
            }

            // Note: The GetContext method blocks while waiting for a request. 
            HttpListenerContext context = listener.EndGetContext(result);
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



    }
}