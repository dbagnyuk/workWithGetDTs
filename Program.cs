using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel;
using workWithGetDTs.ISca;

namespace workWithGetDTs
{
    class Program
    {
        static string srv;
        static DateTime dateTo;
        static DateTime dateFrom;

        static void Main(string[] args)
        {
            long inputPA = 277300065848;
            string lookedSrv = "TEST666";
            List<long> tdcWithLookedSrv = new List<long>();

            ScaClient scaClient = new ScaClient("ConfigurationService_ISca", new EndpointAddress("http://msk-dev-foris:8106/SCA"));

            var scaOutput = scaClient.GetTDs(new GetTDsInput() { PANumber = inputPA });

            Console.WriteLine($"TDIds with services for PA {inputPA}:\n");
            foreach (var tdcs in scaOutput.TDs)
            {
                Console.Write("\n\n-----------\n\n");

                long tdid = tdcs.TdId;
                Console.WriteLine(tdid);
                string srvs = tdcs.Services;
                Console.WriteLine(srvs);

                Console.Write("\n\n-----------\n\n");

                string [] srvRow = tdcs.Services.Split('|');
                foreach(var service in srvRow)
                {
                    if (!service.Contains(","))
                    {
                        srv = service;
                        dateTo = Convert.ToDateTime("2100-01-01 00:00:00");
                        dateFrom = Convert.ToDateTime("1900-01-01 00:00:00");
                    }
                    else if (service.Contains(","))
                    {
                        string[] srvCol = service.Split(',');
                        int idx = 0;
                        foreach (var oneField in srvCol)
                        {
                            ++idx;

                            if (!String.IsNullOrEmpty(oneField) && idx % 3 == 1)
                                srv = oneField;

                            if (!String.IsNullOrEmpty(oneField) && idx % 3 == 2)
                                dateTo = Convert.ToDateTime(oneField);
                            else if (String.IsNullOrEmpty(oneField) && idx % 3 == 2)
                                dateTo = Convert.ToDateTime("2100-01-01 00:00:00");

                            if (!String.IsNullOrEmpty(oneField) && idx % 3 == 0)
                                dateFrom = Convert.ToDateTime(oneField);
                            else if (String.IsNullOrEmpty(oneField) && idx % 3 == 0)
                                dateFrom = Convert.ToDateTime("1900-01-01 00:00:00");

                            //if (idx % 3 == 0)
                            //    Console.Write("\n");
                        }
                    }
                    if (srv == lookedSrv && dateFrom <= DateTime.Now && dateTo >= DateTime.Now)
                        tdcWithLookedSrv.Add(tdid);

                    Console.Write("\t" + srv + ", ");
                    Console.Write(dateFrom + ", ");
                    Console.Write(dateTo + ", ");
                    if (dateFrom <= DateTime.Now && dateTo >= DateTime.Now)
                        Console.WriteLine("Valid");
                    else Console.WriteLine("NoValid");
                }
            }
            Console.WriteLine($"\n\nThe TDs who have the valid service {lookedSrv}:");
            tdcWithLookedSrv.ForEach(i => Console.Write("{0}\n", i));

            Console.Read();
        }
    }
}
