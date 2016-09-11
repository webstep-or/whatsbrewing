using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Cors;
using Microsoft.Azure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using WhatsBrewing.WBService.Models;

namespace WhatsBrewing.WBService.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*", SupportsCredentials = true)]
    public class ChatController : ApiController
    {
        string _tableName = "chatmessages";
        
        [HttpPost]
        //[ResponseType(typeof(void))]        
        public async Task<IHttpActionResult> SendMessage([FromBody]ChatMessage jsonMsg)
        {
            var ok = false;

            if(jsonMsg!= null)
            {

                CloudStorageAccount storageAccount = CloudStorageAccount.Parse(CloudConfigurationManager.GetSetting("StorageConnectionString"));

                // Create the table client.
                CloudTableClient tableClient = storageAccount.CreateCloudTableClient();

                // Create the table if it doesn't exist.
                CloudTable table = tableClient.GetTableReference(_tableName);
                table.CreateIfNotExists();

                var dTime = DateTime.UtcNow;

                var msg = new ChatMessage() { PartitionKey = dTime.Day.ToString(), RowKey = dTime.ToString("u"), Message = jsonMsg.Message, Color = jsonMsg.Color, Nick = jsonMsg.Nick };

                // Create the TableOperation object that inserts the PushMessage entity.
                TableOperation insertOperation = TableOperation.Insert(msg);

                // Execute the insert operation.
                try
                {
                    var result = table.Execute(insertOperation);

                    ok = result.HttpStatusCode == 204;
                }
                catch (Exception e) { }
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
                var rangeQuery = new TableQuery<ChatMessage>();

                return Ok(table.ExecuteQuery(rangeQuery)
                    .OrderByDescending(p => p.RowKey).Skip(skip).Take(take)
                    .Select(p =>new
                          {
                              Nick = p.Nick,
                              Message = p.Message,
                              Color = p.Color,
                              Sent = p.RowKey
                          })
                    );
            }
            else
            {
                return InternalServerError();
            }            
        }

        [HttpGet]
        //[ResponseType(typeof(IEnumerable<dynamic>))]
        public IHttpActionResult GetAllWords()
        {
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(CloudConfigurationManager.GetSetting("StorageConnectionString"));

            // Create the table client.
            CloudTableClient tableClient = storageAccount.CreateCloudTableClient();

            // Create the table if it doesn't exist.
            CloudTable table = tableClient.GetTableReference(_tableName);

            if (table.Exists())
            {
                // Create the table query.
                var rangeQuery = new TableQuery<ChatMessage>();

                var builder = new StringBuilder();

                var messages = table.ExecuteQuery(rangeQuery)
                    .OrderByDescending(p => p.RowKey)
                    .Select(p => p.Message).ToList();

                messages.ForEach(p => builder.AppendFormat("{0} ",p));

                return Ok(builder.ToString());
            }
            else
            {
                return InternalServerError();
            }
        }
    }
}
