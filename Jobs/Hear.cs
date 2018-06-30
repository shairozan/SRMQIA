using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using SRMQ.Jobs;
using SRMQ;
using System;
using System.Collections.Generic;
using System.Text;
using SRMQ.Send;
using System.Threading;

namespace SRMQInAction.Jobs
{
    class Hear : IJobProcessor, IMessageHandler
    {
        public QueueComponents Components {get; set;}

        public Hear(QueueConfiguration c)
        {
            Components = QueueChef.Configure(c);


            /*
            Here we tell the sy stem that whenever a method is received to
            pass it to the Process method
            */
            Components.Consumer.Received += (model, ea) =>
            {       
                Process(ea); 
            };

            //Build the consumer. This triggers actions based on the message
            Components.Model.BasicConsume(queue: Components.Config.Queue,
                                 autoAck: true,
                                 consumer: Components.Consumer);

            //We'll also build a postmaster component so we can send messages to
            //other queues
        }

        //This is where we actually dot he work
        public void Process(BasicDeliverEventArgs Event)
        {
            var body = Event.Body;
            var message = Encoding.UTF8.GetString(body);
            string Saying = Guid.NewGuid().ToString();
            Console.WriteLine($"I heard {message}!");

            using(PostMaster pm = new PostMaster(new QueueConfiguration(){
                Host = Environment.GetEnvironmentVariable("Host"),
                MessageConfiguration = new MessageConfiguration(){
                    RoutingKey = "SRMQ"
                },
                Queue = "Say"
            })){
                pm.Send("Next");
            }

            
        }

    }
}