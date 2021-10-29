using Confluent.Kafka;
using System;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Collections.Generic;
using KubeServiceBinding;
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

        static async void write_events()
        {
            //DocumentStore docstore = DocumentStore.For("Host=postgresql;Username=postgres;Password=7f986df431344327b52471df0142e520;");
            try {

                Console.WriteLine("Beginning to write Vaccination Events to permanent data store...");
                DocumentStore docstore = DocumentStore.For(Environment.GetEnvironmentVariable("CONNECTION_STRING"));

                Dictionary<string, string> bindingsKVP = GetDotnetServiceBindings();
                bool cancelled = false;
                CancellationTokenSource source = new CancellationTokenSource();
                CancellationToken token = source.Token;
                var config = new ConsumerConfig
                {
                    BootstrapServers = bindingsKVP["bootstrapservers"],
                    GroupId = "todb",
                    AutoOffsetReset = AutoOffsetReset.Latest,
                    SecurityProtocol = ToSecurityProtocol(bindingsKVP["securityProtocol"]),
                    SaslMechanism = SaslMechanism.Plain,
                    SaslUsername = bindingsKVP["user"],
                    SaslPassword = bindingsKVP["password"],
                };

                using (var consumer = new ConsumerBuilder<Ignore, string>(config).Build())
                {
                    consumer.Subscribe("us");
                    int vaxcount = 0;
                    while (!cancelled)
                    {
                        var consumeResult = consumer.Consume(token);

                        VaccinationEvent ve = JsonConvert.DeserializeObject<VaccinationEvent>(consumeResult.Message.Value);
                        DEBUGGING: Console.WriteLine("Message offset: {0}", consumeResult.Offset);
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
            } catch(Exception e) {
                Console.WriteLine(e.Message);
            }
        }

        private static Dictionary<string, string> GetDotnetServiceBindings()
        {
            int count = 0;
            int maxTries = 999;
            while (true)
            {
                try
                {
                    DotnetServiceBinding sc = new DotnetServiceBinding();
                    Dictionary<string, string> d = sc.GetBindings("kafka");
                    return d;
                    // At this point, we have the information needed to bind to our Kafka
                    // bootstrap server.
                }
                catch (Exception e)
                {
                    // handle exception
                    System.Threading.Thread.Sleep(1000);
                    if (++count == maxTries) throw e;
                }
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
