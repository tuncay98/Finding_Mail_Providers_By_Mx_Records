using DnsClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MxRecords.Models
{
    public class MxOperations
    {
        public static async Task<List<string>> SetListForSearchingAsync(List<string> mails)
        {
            List<string> MxRecords = new List<string>();

            foreach (var mail in mails)
            {
                int indexOfAt = mail.IndexOf('@');
                string afterAt = mail.Substring(indexOfAt + 1);
                var lookup = new LookupClient();
                var result = await lookup.QueryAsync(afterAt, QueryType.MX);

                var record = result.Answers.MxRecords().FirstOrDefault();
                MxRecords.Add(record.Exchange.Original);
            }
            return MxRecords;
        }
    }
}
