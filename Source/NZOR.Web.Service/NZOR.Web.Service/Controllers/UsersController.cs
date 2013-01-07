using NZOR.Admin.Data.Entities;
using NZOR.Admin.Data.Sql.Repositories;
using NZOR.Publish.Model.Users;
using System;
using System.Net;
using System.Net.Mail;
using System.Web.Http;

namespace NZOR.Web.Service.APIs.Controllers
{
    public class UsersController : ApiController
    {
        private readonly AdminRepository _adminRepository;

        public UsersController(AdminRepository adminRepository)
        {
            _adminRepository = adminRepository;
        }

        //[RequireHttps()]
        [HttpGet]
        public RegistrationResponse RegisterUser(string email, string name, string password, string organisation = "")
        {
            RegistrationResponse rr = new RegistrationResponse();
            rr.Email = email;

            User u = _adminRepository.GetUserByEmail(email);
            if (u != null)
            {
                rr.UserId = u.UserId.ToString();
                rr.Message = "User with this email already exists";
                rr.Status = UserStatus.Unknown.ToString();
            }
            else
            {
                try
                {
                    var encryptor = new System.Security.Cryptography.SHA1CryptoServiceProvider();
                    u = new User();
                    u.UserId = Guid.NewGuid();
                    u.Email = email;
                    u.Name = name;
                    u.Organisation = organisation;
                    u.Password = encryptor.ComputeHash(System.Text.UTF8Encoding.UTF8.GetBytes(password));
                    u.Status = UserStatus.Requested.ToString();
                    u.APIKey = Guid.NewGuid().ToString();
                    u.AddedDate = DateTime.Now;
                    u.ModifiedDate = DateTime.Now;
                    u.State = Entity.EntityState.Added;

                    _adminRepository.Users.Clear();
                    _adminRepository.Users.Add(u);
                    _adminRepository.Save();

                    rr.Status = UserStatus.Requested.ToString();
                    rr.UserId = u.UserId.ToString();

                    string body =
                    @"<table>
                        <tr>
                            <td>Hello,</td>
                        </tr>
                        <tr>
                            <td><br/>Please click the following link to complete your registration with NZOR.<br/><br/>
                            <a href='" + System.Configuration.ConfigurationManager.AppSettings["BaseWebSiteUrl"] + "users/completeRegistration?userId=" + u.UserId.ToString() + @"'>Complete registration</a><br/<br/>
                            </td>
                        </tr>
                        <tr>
                            <td>Regards,</td>
                        </tr>
                        <tr>
                            <td>The NZOR website administrator.</td>
                        </tr>
                    </table>";

                    SendEmailMessage(email, "NZOR User Registration", body);

                    rr.Message = "Registration requested.  An email with instructions to complete the registration has been sent.";

                    u.Status = UserStatus.EmailSent.ToString();
                    u.State = Entity.EntityState.Modified;
                    _adminRepository.Save();

                    rr.Status = UserStatus.EmailSent.ToString();
                }
                catch (Exception)
                {
                    rr.Message = "Internal server error.";
                    rr.Status = "Error";

                    throw new HttpResponseException(HttpStatusCode.InternalServerError);
                }
            }

            return rr;
        }

        [HttpGet]
        public RegistrationResponse Complete(string userId)
        {
            RegistrationResponse rr = new RegistrationResponse();
            try
            {
                User u = _adminRepository.GetUser(new Guid(userId));
                if (u != null)
                {
                    u.Status = UserStatus.Registered.ToString();
                    u.State = Entity.EntityState.Modified;
                    _adminRepository.Users.Clear();
                    _adminRepository.Users.Add(u);
                    _adminRepository.Save();

                    string body = @"<table>
                        <tr>
                            <td>Hello,</td>
                        </tr>
                        <tr>
                            <td><br/>Your NZOR registration is complete.<br/><br/>
                            When using the NZOR web service, please use the following API Key: " + u.APIKey + @"<br/><br/>
                            </td>
                        </tr>
                        <tr>
                            <td>Regards,</td>
                        </tr>
                        <tr>
                            <td>The NZOR website administrator.</td>
                        </tr>
                    </table>";

                    SendEmailMessage(u.Email, "NZOR User Registration", body);

                    rr.Message = "Registration complete.  Please refer to your email.";
                    rr.UserId = userId;
                    rr.Status = UserStatus.Registered.ToString();
                }
            }
            catch (Exception)
            {
                rr.Message = "Internal Server Error.";
                throw new HttpResponseException(HttpStatusCode.InternalServerError);
            }

            return rr;
        }

        [HttpGet]
        public RegistrationResponse ResendApiKey(string userId)
        {
            RegistrationResponse rr = new RegistrationResponse();
            try
            {
                User u = _adminRepository.GetUser(new Guid(userId));
                if (u == null)
                {
                    rr.Message = "User does not exist";
                    throw new HttpResponseException(System.Net.HttpStatusCode.NotFound);
                }
                else
                {
                    string body = @"<table>
                        <tr>
                            <td>Hello,</td>
                        </tr>
                        <tr>
                            <td><br/>Here is your API key for use with the NZOR Web service.<br/>" + u.APIKey + @"<br/><br/>
                            </td>
                        </tr>
                        <tr>
                            <td>Regards,</td>
                        </tr>
                        <tr>
                            <td>The NZOR website administrator.</td>
                        </tr>
                    </table>";

                    SendEmailMessage(u.Email, "NZOR API Key", body);

                    rr.Message = "API Key has been resent.  Please refer to your email.";
                    rr.UserId = userId;
                    rr.Status = UserStatus.Registered.ToString();
                }
            }
            catch (Exception)
            {
                rr.Message = "Internal Server Error.";
                throw new HttpResponseException(HttpStatusCode.InternalServerError);
            }

            return rr;
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
