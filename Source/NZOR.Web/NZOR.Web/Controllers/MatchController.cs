using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;
using NZOR.Publish.Model.Matching;
using NZOR.Web.Service.Client;
using NZOR.Web.ViewModels.Matching;

namespace NZOR.Web.Controllers
{
    public class MatchController : Controller
    {
        private readonly ServiceClient _service;

        public MatchController()
        {
            _service = new ServiceClient("", System.Configuration.ConfigurationManager.AppSettings["ServiceUrl"]);
        }

        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Results(HttpPostedFileBase dwcFile)
        {
            if (dwcFile != null && dwcFile.ContentLength > 0)
            {
                try
                {
                    string content = String.Empty;

                    using (StreamReader reader = new StreamReader(dwcFile.InputStream))
                    {
                        content = reader.ReadToEnd();
                    }

                    var results = _service.SubmitMatchList(content);

                    var viewModel = new MatchViewModel();

                    if (results != null)
                    {
                        viewModel.NameMatchResults = results;
                    }

                    TempData["content"] = content;

                    return View(viewModel);
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError(String.Empty, "Error: " + ex.Message);
                    return View();
                }
            }

            ModelState.AddModelError(String.Empty, "A file is required.");

            return View();
        }

        [HttpPost]
        public ActionResult Download(string emailAddress)
        {
            try
            {
                string csvData = TempData["content"] as string;

                bool doExternalLookup = false;
                if (Request.Form["doExternalLookup"] == "on") doExternalLookup = true;

                _service.SubmitMatchList(csvData, emailAddress, doExternalLookup);
                
                //send email
                string body =
                    @"<table>
                        <tr>
                            <td>Hello,</td>
                        </tr>
                        <tr>
                            <td><br/>Your submitted batch match file has been added to the queue for processing. 
                            You will receive an email when the job has been completed including a URL to retrieve the results.<br/><br/></td>
                        </tr>
                        <tr>
                            <td>Regards,</td>
                        </tr>
                        <tr>
                            <td>The NZOR website administrator.</td>
                        </tr>
                    </table>";
                SendEmailMessage(emailAddress, "NZOR Batch Match", body);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(String.Empty, ex.Message);
            }

            return View();
        }

        private void SendEmailMessage(string to, string subject, string body)
        {
            try
            {
                string from = System.Configuration.ConfigurationManager.AppSettings["SenderAddress"];

                using (SmtpClient client = new SmtpClient())
                {
                    using (MailMessage message = new MailMessage(from, to, subject, body))
                    {
                        message.DeliveryNotificationOptions = DeliveryNotificationOptions.OnSuccess | DeliveryNotificationOptions.OnFailure;
                        message.Priority = MailPriority.High;
                        message.IsBodyHtml = true;

                        client.Send(message);
                    }
                }
            }
            catch (SmtpException ex)
            {
                throw new ApplicationException("SmtpException has occured: " + ex.Message);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public ActionResult BrokerNames()
        {
            return View();
        }

        [HttpPost]
        public ActionResult BrokerNames(HttpPostedFileBase namesFile, string emailAddress, string apiKey)
        {
            if (namesFile != null && namesFile.ContentLength > 0)
            {
                try
                {
                    string content = String.Empty;

                    using (StreamReader reader = new StreamReader(namesFile.InputStream))
                    {
                        content = reader.ReadToEnd();
                    }

                    _service.SubmitBrokeredNames(content, emailAddress, apiKey);
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError(String.Empty, ex.Message);
                }
            }

            return View("index");
        }
    }
}
