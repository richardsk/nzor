using NZOR.Admin.Data.Entities;
using NZOR.Admin.Data.Sql.Repositories;
using NZOR.Publish.Data.Repositories;
using NZOR.Publish.Model.Names;
using System;
using System.Collections.Generic;
using System.Net.Mail;
using System.Web;
using System.Web.Http;

namespace NZOR.Web.Service.APIs
{
    public class FeedbackController : ApiController
    {
        private readonly FeedbackRepository _feedbackRepository;
        private readonly NZOR.Admin.Data.Sql.Repositories.ProviderRepository _providerRepository;
        private readonly NameRepository _nameRepository;

        public FeedbackController(FeedbackRepository feedbackRepository, NZOR.Admin.Data.Sql.Repositories.ProviderRepository providerRepository, NameRepository nameRepository)
        {
            _feedbackRepository = feedbackRepository;
            _providerRepository = providerRepository;
            _nameRepository = nameRepository;
        }

        public void GetSubmitFeedback(string sender = "", string senderEmail = "", string message = "", string nameId = "", string providersToEmail = "")
        {
            Guid nameGuid = Guid.Empty;
            if (!Guid.TryParse(nameId, out nameGuid)) return;

            string emailedTo = "";
            string fbSentTo = "";
            List<Provider> providersEmail = new List<Provider>();
            if (nameGuid != Guid.Empty)
            {
                providersEmail = _providerRepository.GetProvidersForName(nameGuid);

                foreach (Provider p in providersEmail)
                {
                    if ((providersToEmail == null || providersToEmail.ToLower().Contains(p.Code.ToLower())) &&
                        p.ContactEmail != null && p.ContactEmail.Length > 0)
                    {
                        emailedTo += " Data provider:<br/> - " + p.Name + "<br/> Contact email:<br/> - " + p.ContactEmail + "<br/>";
                        fbSentTo += "Data provider: " + p.Name + ", Contact email: " + p.ContactEmail + ";";
                    }
                }
            }

            Feedback fb = new Feedback();
            fb.FeedbackId = Guid.NewGuid();
            fb.NameId = nameGuid;
            fb.Message = message;
            fb.Status = FeedbackStatus.Created.ToString();
            fb.Sender = sender;
            fb.SenderEmail = senderEmail;
            fb.SentTo = fbSentTo;
            fb.AddedDate = DateTime.Now;
            fb.State = Entity.EntityState.Added;

            _feedbackRepository.Feedbacks.Clear();

            _feedbackRepository.Feedbacks.Add(fb);
            _feedbackRepository.Save();

            Name name = null;

            if (fb.NameId.HasValue) name = _nameRepository.SingleOrDefault(fb.NameId.Value);

            //email nzor admin
            string adminBody = @"
<table>
    <tr>
        <td>Hello,</td>
    </tr>
    <tr>
        <td><br/>
        Feedback has been submitted for an NZOR name.<br/><br/>
        Submitter: " + fb.Sender + @"<br/>
        Submitter email: " + fb.SenderEmail + @"<br/><br/>";

            if (emailedTo.Length > 0)
            {
                adminBody += "Email sent to data providers: <br/>" + emailedTo + "<br/>";
            }

            adminBody += " NZOR Name: " + (name == null ? "not provided" : HttpUtility.HtmlDecode(name.FormattedFullName)) + "<br/>";
            if (name != null)
            {
                adminBody += "Name Id: " + name.NameId.ToString() + "<br/>";
                string url = System.Configuration.ConfigurationManager.AppSettings["BaseWebSiteUrl"] + "names/" + name.NameId.ToString();
                adminBody += "Url: <a href='" + url + "'>" + url + "</a><br/>";
            }
            adminBody += "Message: " + fb.Message + @"<br/><br/>
        </td>
    </tr>
</table>
";

            string adminEmail = System.Configuration.ConfigurationManager.AppSettings["SenderAddress"];
            SendEmailMessage(adminEmail, "NZOR Feedback Message", adminBody);

            //email providers
            if (name != null)
            {
                foreach (Provider p in providersEmail)
                {
                    if ((providersToEmail == null || providersToEmail.ToLower().Contains(p.Code.ToLower())) &&
                        p.ContactEmail != null && p.ContactEmail.Length > 0)
                    {
                        string body =
@"<table>
    <tr>
        <td>Hello,</td>
    </tr>
    <tr>
        <td><br/>Feedback has been submitted for an NZOR name that " + p.Name + @" is a provider for.<br/><br/>
        Please review this feedback and either resolve the issue in the source dataset or communicate with the submitter about the feedback.<br/>
        Please include (cc) the NZOR adminstrator email in all email conversation.<br/><br/>
        Submitter: " + fb.Sender + @"<br/>
        Submitter email: " + fb.SenderEmail + @"<br/>
        NZOR Name: " + HttpUtility.HtmlDecode(name.FormattedFullName) + @"<br/>
        Name Id: " + name.NameId.ToString() + "<br/>";
                        string url = System.Configuration.ConfigurationManager.AppSettings["BaseWebSiteUrl"] + "names/" + name.NameId.ToString();
                        body += "Url: <a href='" + url + "'>" + url + @"</a><br/>
        Message: " + fb.Message + @"<br/><br/>

        </td>
    </tr>
    <tr>
        <td>Regards,</td>
    </tr>
    <tr>
        <td>The NZOR administrator.</td>
    </tr>
</table>";

                        SendEmailMessage(p.ContactEmail, "NZOR Feedback Message", body);
                    }
                }
            }

            //email sender

            string senderBody = @"
<table>
    <tr>
        <td>Hello,</td>
    </tr>
    <tr>
        <td><br/>
        Thank you for your feedback to NZOR.<br/><br/>
        The feedback will be submitted to the relevant reviewers.  When this feedback has been addressed any updates will be incorporated into NZOR.<br/><br/>
        </td>
    </tr>
    <tr>
        <td>Regards,</td>
    </tr>
    <tr>
        <td>The NZOR administrator.</td>
    </tr>
</table>
";
            SendEmailMessage(senderEmail, "NZOR Feedback Message", senderBody);

        }

        private void SendEmailMessage(string to, string subject, string body)
        {
            try
            {
                string from = System.Configuration.ConfigurationManager.AppSettings["SenderAddress"];

                using (var client = new SmtpClient())
                {
                    using (MailMessage message = new MailMessage(from, to, subject, body))
                    {
                        message.DeliveryNotificationOptions = DeliveryNotificationOptions.OnFailure;
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
    }
}