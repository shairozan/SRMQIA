using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using SRMQ;
using SRMQ.Jobs;
using SRMQ.Send;

namespace SRMQInAction
{
    class Program
    {
        public static async Task Main(string[] args)
        {         

            //Gives RabbitMQ service time to start if we're in docker-compose
            Thread.Sleep(5000);

            List<IMessageHandler> Jobs = new List<IMessageHandler>();

            var type = typeof(IMessageHandler);
            var types = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(s => s.GetTypes())
                .Where(p => type.IsAssignableFrom(p)  
                    && p.Namespace.Contains("Jobs")); // <-- Specifies the namespace in which you are adding your jobs. Jobs must inherit IMessageHandler

            //Reflect all the jobs into the class list
            foreach(var t in types)
            {
                Jobs.Add(
                        (IMessageHandler)Activator.CreateInstance(t, new object[] {
                            new QueueConfiguration()
                            {
                                Host = Environment.GetEnvironmentVariable("Host"),
                                Queue = t.Name
                            }
                        })
                    );
            }

            //Set first message into queue
            using(PostMaster pm = new PostMaster(new QueueConfiguration(){
                Host = Environment.GetEnvironmentVariable("Host"),
                MessageConfiguration = new MessageConfiguration(){
                    RoutingKey = "SRMQ"
                },
                Queue = "Say"
                })){
                    pm.Send("Next");
            }

            
            while (true)
            {
                    
                Thread.Sleep(1000);
            }

        }

    }
}
