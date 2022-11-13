using LCW.Demo.Data;
using Microsoft.PowerPlatform.Dataverse.Client;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;

namespace LCW.Demo.Helper
{
    public class ContactHelper
    {
        public static EntityCollection GetContact(ContactRequest request, ServiceClient serviceClient)
        {
            var query = ContactQuery(request);
            return GetAllData(serviceClient, query);
        }

        static EntityCollection GetAllData(ServiceClient service, QueryExpression query)
        {
            query.PageInfo = new PagingInfo();
            query.PageInfo.Count = 5000;
            query.PageInfo.PageNumber = 1;
            var pageResult = new EntityCollection();
            var result = new EntityCollection();

            do
            {
                pageResult = service.RetrieveMultiple(query);

                if (pageResult.Entities != null)
                {
                    result.Entities.AddRange(pageResult.Entities);
                }

                query.PageInfo.PageNumber++;
                query.PageInfo.PagingCookie = pageResult.PagingCookie;

            } while (pageResult.MoreRecords);

            return result;

        }
        static QueryExpression ContactQuery(ContactRequest request)
        {
            QueryExpression query = new QueryExpression("contact")
            {
                ColumnSet = new ColumnSet("firstname", "lastname", "mobilephone", "emailaddress1"),
                Criteria = new FilterExpression(LogicalOperator.Or)
                {
                    Conditions = 
                    {
                        new ConditionExpression("mobilephone",ConditionOperator.Equal,request.PhoneNumber),
                        new ConditionExpression("emailaddress1",ConditionOperator.Equal,request.Email)
                    }
                },
            };
            return query;
        }

        public static Entity ContactMapping(ContactRequest request, Guid entityId)
        {
            Entity entity = new Entity("contact", entityId)
            {
                ["firstname"] = !string.IsNullOrEmpty(request.Name) ? request.Name.Trim() : string.Empty,
                ["lastname"] = !string.IsNullOrEmpty(request.Surname) ? request.Surname.Trim() : string.Empty,
                ["emailaddress1"] = !string.IsNullOrEmpty(request.Email) ? request.Email.Trim() : string.Empty,
                ["mobilephone"] = !string.IsNullOrEmpty(request.PhoneNumber) ? request.PhoneNumber.Trim() : string.Empty
            };
            return entity;
        }
    }
}
