using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.OData;
using Microsoft.WindowsAzure.Mobile.Service;
using dotnetdb3Service.DataObjects;
using ServiceStack.Redis;
using System;
using Newtonsoft.Json.Linq;
using ServiceStack.Redis.Generic;
using System.Net.Http;
using System.Net;
using System.Collections.Generic;


namespace dotnetdb3Service.Controllers
{
    [RoutePrefix("Tables")]
    public class TodoItemController : TableController<TodoItem>
    {        
        private IRedisTypedClient<TodoItem> redisItems;
        private IRedisList incompleteItemsIds;
        public TodoItemController(IRedisClient redisClient)
        {                                    
            this.redisItems = redisClient.As<TodoItem>();
            this.incompleteItemsIds = redisClient.Lists["urn:TodoItem:IncompleteItemIds"];
        }
        
        // GET tables/TodoItem
        public IQueryable<TodoItem> GetAllTodoItems()
        {
            System.Diagnostics.Debug.WriteLine("foo");
            return this.redisItems.GetAll().AsQueryable();
        }

        [Route("IncompleteTodoItems")]
        public IEnumerable<TodoItem> GetIncompleteTodoItems()
        {
            var ids = this.incompleteItemsIds.GetAll();
            return redisItems.GetByIds(ids);
        }

        // GET tables/TodoItem/48D68C86-6EA6-4C25-AA33-223FC9A27959
        public TodoItem GetTodoItem(string id)
        {
            return this.redisItems.GetById(id);
        }

        // PATCH tables/TodoItem/48D68C86-6EA6-4C25-AA33-223FC9A27959
        public TodoItem PatchTodoItem(string id, Delta<TodoItem> patch)
        {            
            var item = this.redisItems.GetById(id);
            patch.CopyChangedValues(item);
            this.redisItems.Store(item);

            if (item.Complete)
            {
                this.incompleteItemsIds.Remove(item.Id);
            }
            else
            {
                this.incompleteItemsIds.Add(item.Id);
            }
            
            return item;
        }

        // POST tables/TodoItem/48D68C86-6EA6-4C25-AA33-223FC9A27959
        public IHttpActionResult PostTodoItem(TodoItem item)
        {
            item.Id = item.Id ?? Guid.NewGuid().ToString();
            this.redisItems.Store(item);

            if(item.Complete == false)
            {
                this.incompleteItemsIds.Add(item.Id);
            }

            //return this.Request.CreateResponse(HttpStatusCode.Created, item);            
            return CreatedAtRoute("Tables", new { id = 0 }, item);
        }

        // DELETE tables/TodoItem/48D68C86-6EA6-4C25-AA33-223FC9A27959
        public void DeleteTodoItem(string id)
        {
            this.incompleteItemsIds.Remove(id);
            this.redisItems.DeleteById(id);            
        }

    }
}