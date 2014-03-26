using System;
using log4net;
using log4net.Config;

namespace MongoTalk
{
    public class MongoSandbox
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof (MongoSandbox));
        
        public static void Main(string[] args)
        {
            XmlConfigurator.Configure();
            Log.Info("Starting Mongo Sandbox");

            Log.Info("Done running Mongo Sandbox");
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
        }
    }
}
