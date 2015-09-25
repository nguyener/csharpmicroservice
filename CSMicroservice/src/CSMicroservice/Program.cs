using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using System.Collections; 
using System.IO; 
using System.Net;  
using System.Threading;



namespace CSMicroService
{
    public class HTTPServer
    {
        public void Main(string[] args)
        {
            HTTPServer server = new HTTPServer(80);
        }


        protected int port;
        HttpListener listener;


        public HTTPServer(int port)
        {
            this.port = port;
            listener = new HttpListener();
        }

        public HTTPServer()
        {
            this.port = 80;
        }

        public void start()
        {
            while (true)
            {
                HttpClient s = listener.GetContext();
                HTTPProcessor processor = new HTTPProcessor(s, this);
                Thread thread = new Thread(new ThreadStart(processor.process));
                thread.Start();
                Thread.Sleep(1);
            }

        }

        public void handleGET(HTTPProcessor processor)
        {
            processor.outputStream.WriteLine("<html><body><h1>Hello There</h1></body></html>");
        }

    }

    public class HTTPProcessor
    {
        TcpClient client;
        HTTPServer server;
        public StreamWriter outputStream;
        public HTTPProcessor(TcpClient client, HTTPServer server)
        {
            this.client = client;
            this.server = server;
        }

        public void process()
        {
            outputStream = new StreamWriter(new BufferedStream(client.GetStream()));
            server.handleGET(this);
            client.Close();
        }
    }
}
