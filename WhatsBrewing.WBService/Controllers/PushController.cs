using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using Microsoft.Azure;
using Microsoft.Azure.NotificationHubs;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using System.Web.Http.Cors;
using WhatsBrewing.WBService.Models;

namespace WhatsBrewing.WBService.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*", SupportsCredentials = true)]
    public class PushController : ApiController
    {
        string _tableName = "pushmessages";
        string _notificationHub = "wb-mobile-servicehub2";

        [HttpPost]
        //[ResponseType(typeof(void))]        
        public async Task<IHttpActionResult> SendMessage([FromBody]PushMessage jsonMsg)
        {
            var ok = false;
            
            //if (!string.IsNullOrEmpty(value))
            if (jsonMsg != null)
            {
                //var jsonMsg = JsonConvert.DeserializeObject<dynamic>(value);


                // Send ios push
                var iosPushMsg = string.Format(@"{{""aps"":{{""alert"":""{0}"",""category"":""{1}""}}}}", jsonMsg.Message, jsonMsg.Category);
                string connectionString = CloudConfigurationManager.GetSetting("Microsoft.ServiceBus.ConnectionString");

                var hub = NotificationHubClient.CreateClientFromConnectionString(connectionString, _notificationHub);
                
                var result = await hub.SendAppleNativeNotificationAsync(iosPushMsg);

                ok = result.State == NotificationOutcomeState.Enqueued;

                if(ok)
                {
                    CloudStorageAccount storageAccount = CloudStorageAccount.Parse(CloudConfigurationManager.GetSetting("StorageConnectionString"));

                    // Create the table client.
                    CloudTableClient tableClient = storageAccount.CreateCloudTableClient();

                    // Create the table if it doesn't exist.
                    CloudTable table = tableClient.GetTableReference(_tableName);
                    table.CreateIfNotExists();

                    var dTime = DateTime.UtcNow;

                    var msg = new PushMessage() { PartitionKey = dTime.Day.ToString(), RowKey = dTime.ToString("u"), Message = jsonMsg.Message, Category = jsonMsg.Category };

                    // Create the TableOperation object that inserts the PushMessage entity.
                    TableOperation insertOperation = TableOperation.Insert(msg);

                    // Execute the insert operation.
                    try
                    {
                        table.Execute(insertOperation);
                        ok = true;
                    }
                    catch (Exception e) 
                    {
                        ok = false;
                    }
                }
            }

            if (ok)
            {
                return Ok("Cheers! Message was sent!");
            }
            else
            {
                return InternalServerError();
            }
        }

        [HttpGet]
        //[ResponseType(typeof(IEnumerable<dynamic>))]
        public IHttpActionResult GetMessages([FromUri]int skip = 0, [FromUri]int take = 1000)
        {
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(CloudConfigurationManager.GetSetting("StorageConnectionString"));

            // Create the table client.
            CloudTableClient tableClient = storageAccount.CreateCloudTableClient();

            // Create the table if it doesn't exist.
            CloudTable table = tableClient.GetTableReference(_tableName);

            if (table.Exists())
            {
                var dTime = DateTime.UtcNow;

                // Create the table query.
                var rangeQuery = new TableQuery<PushMessage>();

                return Ok(table.ExecuteQuery(rangeQuery)
                   .OrderByDescending(p => p.RowKey).Skip(skip).Take(take)
                   .Select(p => new
                       {
                           Category = p.Category,
                           Message = p.Message,
                           Sent = p.RowKey
                       })
                   );

            }
            else
            {
                return InternalServerError();
            }            
        }
    }
}
