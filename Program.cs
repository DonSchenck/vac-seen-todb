using Confluent.Kafka;
using System;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Collections.Generic;
using Marten;

namespace vac_seen_todb
{
    class Program
    {
        private const string KafkaTopic = "us";

        static void Main(string[] args)
        {
            // Consume Kafka events and write them to Console
            Console.WriteLine("vac-seen-todb started.");

            write_events();

        }

        static void write_events()
        {
            //DocumentStore docstore = DocumentStore.For("Host=postgresql;Username=postgres;Password=7f986df431344327b52471df0142e520;");
            try
            {

                Console.WriteLine("Beginning to write Vaccination Events to permanent data store...");
                DocumentStore docstore = DocumentStore.For(Environment.GetEnvironmentVariable("MARTEN_CONNECTION_STRING"));

                bool cancelled = false;
                CancellationTokenSource source = new CancellationTokenSource();
                CancellationToken token = source.Token;

                Console.WriteLine("config.BootstrapServers: {0}", Environment.GetEnvironmentVariable("BOOTSTRAPSERVERS"));
                Console.WriteLine("SECURITY_PROTOCOL ENV: {0}", Environment.GetEnvironmentVariable("SECURITY_PROTOCOL"));
                Console.WriteLine("SASL_MECHANICM ENV: {0}", Environment.GetEnvironmentVariable("SASL_MECHANISM"));
                Console.WriteLine("config.SaslUsername: {0}", Environment.GetEnvironmentVariable("CLIENT_ID"));
                Console.WriteLine("config.SaslPassword: {0}", Environment.GetEnvironmentVariable("CLIENT_SECRET"));


                ConsumerConfig config = new ConsumerConfig();
                config.GroupId = "ustodb";
                config.AutoOffsetReset = AutoOffsetReset.Latest;
                config.BootstrapServers = Environment.GetEnvironmentVariable("BOOTSTRAPSERVERS");
                config.SecurityProtocol = ToSecurityProtocol(Environment.GetEnvironmentVariable("SECURITY_PROTOCOL"));
                config.SaslMechanism = ToSaslMechanism(Environment.GetEnvironmentVariable("SASL_MECHANISM"));
                config.SaslUsername = Environment.GetEnvironmentVariable("CLIENT_ID");
                config.SaslPassword = Environment.GetEnvironmentVariable("CLIENT_SECRET");


                Console.WriteLine("config.GroupId: {0}", config.GroupId);
                Console.WriteLine("config.BootstrapServers: {0}", config.BootstrapServers);
                Console.WriteLine("SASL_PROTOCOL ENV: {0}", Environment.GetEnvironmentVariable("SECURITY_PROTOCOL"));
                Console.WriteLine("SASL_MECHANICM ENV: {0}", Environment.GetEnvironmentVariable("SASL_MECHANISM"));
                Console.WriteLine("config.SaslUsername: {0}", config.SaslUsername);
                Console.WriteLine("config.SaslPassword: {0}", config.SaslPassword);



                using (var consumer = new ConsumerBuilder<Ignore, string>(config).Build())
                {
                    consumer.Subscribe("us");
                    int vaxcount = 0;
                        foreach (var msg in consumer.Consume())
                        {
                            VaccinationEvent ve = JsonConvert.DeserializeObject<VaccinationEvent>(msg);
                            Console.WriteLine("Message offset: {0}", msg.Offset);
                            using (var session = docstore.LightweightSession())
                            {
                                // Write to database
                                session.Store(ve);
                                session.SaveChanges();
                            }
                            vaxcount++;
                        }

                    consumer.Close();
                    Console.WriteLine("FINISHED writing Vaccination Events to permanent data store.");

                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
        public static SecurityProtocol ToSecurityProtocol(string bindingValue) => bindingValue switch
        {
            "SASL_SSL" => SecurityProtocol.SaslSsl,
            "PLAIN" => SecurityProtocol.Plaintext,
            "SASL_PLAINTEXT" => SecurityProtocol.SaslPlaintext,
            "SSL" => SecurityProtocol.Ssl,
            _ => throw new ArgumentOutOfRangeException(bindingValue, $"Not expected SecurityProtocol value: {bindingValue}"),
        };
        public static SaslMechanism ToSaslMechanism(string bindingValue) => bindingValue switch
        {
            "GSSAPI" => SaslMechanism.Gssapi,
            "PLAIN" => SaslMechanism.Plain,
            "SCRAM-SHA-256" => SaslMechanism.ScramSha256,
            "SCRAM-SHA-512" => SaslMechanism.ScramSha512,
            _ => throw new ArgumentOutOfRangeException(bindingValue, $"Not expected SaslMechanism value: {bindingValue}"),
        };
    }
}
