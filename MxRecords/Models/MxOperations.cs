using DnsClient;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace MxRecords.Models
{
    public class MxOperations
    {
        public static async Task<List<string>> SetListForSearchingAsync(List<string> mails)
        {
            List<string> MxRecords = new List<string>();
            int index = 0;
            int countofmails = mails.Count;

            foreach (var mail in mails)
            {
                index++;
                if (countofmails > 75)
                    if (index % 50 == 0)
                        await Task.Delay(1000);
                try
                {

                    int indexOfAt = mail.IndexOf('@');
                    string afterAt = mail.Substring(indexOfAt + 1);
                    var lookup = new LookupClient() { 
                    UseCache = true,
                    Retries = 50,
                    ContinueOnDnsError = true,
                    };
                    var result = await lookup.QueryAsync(afterAt, QueryType.MX);

                    var record = result.Answers.MxRecords().FirstOrDefault();
                    if (record != null)
                        MxRecords.Add(record.Exchange.Original);
                    else
                        MxRecords.Add("");

                }
                catch (DnsResponseException)
                {
                    MxRecords.Add(" ");

                }
            }
            return MxRecords;
        }

    }
}
