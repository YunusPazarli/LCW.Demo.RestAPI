using LCW.Demo.RestAPI.Helper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using Newtonsoft.Json;
using System.Runtime.CompilerServices;
using System;
using LCW.Demo.Data;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.PowerPlatform.Dataverse.Client;
using LCW.Demo.Helper;

namespace LCW.Demo.RestAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ContactController : ControllerBase
    {
        private string cS = $"AuthType=ClientSecret;Url=https://org12332edf.crm4.dynamics.com;ClientId=083d1a78-559d-4b06-a558-fce4e88f6de3;ClientSecret=Vsk8Q~UKO_WhRNXoopOZyzOLi5udXxKmdkqnXbB8;";

        [HttpPost]
        public ContactResponse CreateContact(ContactRequest request)
        {
            try
            {
                var serviceClient = new ServiceClient(cS);
                EntityCollection contact = ContactHelper.GetContact(request, serviceClient);
                if (string.IsNullOrEmpty(request.Email)&&string.IsNullOrEmpty(request.PhoneNumber))
                {
                    throw new Exception("Telefon ya da E-Posta Adresi Zorunludur.");
                }
                if (contact.Entities.Count == 0)
                {
                    Entity entity = ContactHelper.ContactMapping(request, Guid.Empty);
                    serviceClient.Create(entity);
                    var response = new ContactResponse();
                    response.Response = "00";
                    response.Message = $"{request.Name.Trim()} {request.Surname.Trim()} isimli Contact kaydı başarıyla oluşturuldu";
                    return response;
                }
                else
                {
                    var response = new ContactResponse();
                    response.Response = "02";
                    response.Message = $"{contact.Entities[0]["firstname"]} {contact.Entities[0]["lastname"]} isimli Contact kaydı mevcut"; 
                    return response;
                }
            }
            catch (Exception ex)
            {
                var response = new ContactResponse();
                response.Response = "01";
                response.Message = ex.Message;
                return response;
            }

        }
        [HttpPut]
        public ContactResponse UpdateContact(ContactRequest request)
        {
            try
            {
                var serviceClient = new ServiceClient(cS);
                EntityCollection contacts = ContactHelper.GetContact(request, serviceClient);
                Entity entity = null;
                var response = new ContactResponse();
                if (string.IsNullOrEmpty(request.Email) && string.IsNullOrEmpty(request.PhoneNumber))
                {
                    throw new Exception("Telefon ya da E-Posta Adresi Zorunludur.");
                }
                if (contacts.Entities.Count > 0)
                {
                    entity = ContactHelper.ContactMapping(request, contacts.Entities[0].Id);
                    serviceClient.Update(entity);
                    response.Response = "00";
                    response.Message = $"{contacts.Entities[0]["firstname"]} {contacts.Entities[0]["lastname"]} isimli Contact kaydı başarıyla güncellendi";
                    return response;
                }
                else
                {
                    response.Response = "02";
                    response.Message = $"{request.Name.Trim()} {request.Surname.Trim()} isimli Contact kaydı bulunamadı";
                    return response;
                }
            }
            catch (Exception ex)
            {
                var response = new ContactResponse();
                response.Response = "01";
                response.Message = ex.Message;
                return response;
            }

        }

        [HttpDelete]
        public ContactResponse DeleteContact(ContactRequest request)
        {
            try
            {
                var serviceClient = new ServiceClient(cS);
                EntityCollection contact = ContactHelper.GetContact(request, serviceClient);
                if (string.IsNullOrEmpty(request.Email) && string.IsNullOrEmpty(request.PhoneNumber))
                {
                    throw new Exception("Telefon ya da E-Posta Adresi Zorunludur.");
                }
                if (contact.Entities.Count > 0)
                {
                    serviceClient.Delete(contact.Entities[0].LogicalName, contact.Entities[0].Id);
                    var response = new ContactResponse();
                    response.Response = "00";
                    response.Message = $"{contact.Entities[0]["firstname"]} {contact.Entities[0]["lastname"]} isimli Contact kaydı başarıyla silindi";
                    return response;
                }
                else
                {
                    var response = new ContactResponse();
                    response.Response = "02";
                    response.Message = $"{request.Name.Trim()} {request.Surname.Trim()} isimli Contact kaydı bulunamadı";
                    return response;
                }



            }
            catch (Exception ex)
            {
                var response = new ContactResponse();
                response.Response = "01";
                response.Message = ex.Message;
                return response;
            }

        }
    }
}
