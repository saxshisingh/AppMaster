using MailKit.Security;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using MimeKit;
using Newtonsoft.Json;
using PingaUnitBooking.Core.Domain;
using PingaUnitBooking.Infrastructure.Interfaces;
using System.Collections;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.RegularExpressions;
using static System.Net.WebRequestMethods;


namespace PingaUnitBooking.Infrastructure.Helpers
{
    public class Notification : INotificationService
    {
        private IHostingEnvironment _Environment;
        private readonly IUnitInterface _UnitInterface;
        private readonly IBookingInterface _bookingInterface;
        private readonly ITemplateInterface _templateinterface;
        private readonly IMailConfigureInterface _mailConfigureInterface;
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly IConfiguration _configuration;
        public const string MatchEmailPattern = @"\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*";
        public Notification(IMailConfigureInterface mailConfigureInterface, ITemplateInterface templateinterface,
            IHostingEnvironment hostingEnvironment, IConfiguration configuration, IBookingInterface bookingInterface,
            IUnitInterface UnitInterface, IHostingEnvironment environment)
        {
            _Environment = environment;
            _configuration = configuration;
            _templateinterface = templateinterface;
            _mailConfigureInterface = mailConfigureInterface;
            _hostingEnvironment = hostingEnvironment;
            _bookingInterface = bookingInterface;
            _UnitInterface = UnitInterface;
        }
        public async Task SendNotifiction(decimal GroupID, int UserID, int BookingID, string ProcessType)
        {
            try
            {

                ResponseDataResults<List<MailConfigure>> MailConfigure = await _mailConfigureInterface.GetMailConfigure(GroupID);
                if (!MailConfigure.IsSuccess)
                {
                    return;
                }
                ResponseDataResults<Communication> Communicationobj = await _mailConfigureInterface.GetCustomerUnitDetail(BookingID);
                if (!Communicationobj.IsSuccess)
                {
                    return;
                }
                ResponseDataResults<List<Template>> responseDataResults = await _mailConfigureInterface.GetNotificationTemplate(GroupID, Communicationobj.Data.UnitDetail.projectID, ProcessType);
                if (!responseDataResults.IsSuccess)
                {
                    return;
                }

                MailConfigure EmailConfig = MailConfigure.Data.ToList().Find(x => x.ConfigureType.ToUpper() == "EMAIL" && x.IsActive == true);
                if (EmailConfig == null)
                {
                    return;
                }
                else
                {
                    Template template = responseDataResults.Data.ToList().Find(x => x.TemplateType.ToUpper() == "EMAIL");
                    if (template == null)
                    {
                        return;
                    }
                    string body = Replace(template.TemplateMsg, Communicationobj.Data);
                    List<string> filePathList = await this.GetAttachments(GroupID, Communicationobj.Data.BookingID, Communicationobj.Data.PayPlanID, Communicationobj.Data.ApplicationType, ProcessType);
                    string status = await SendEmail(GroupID, UserID, Communicationobj.Data.BookingID, Communicationobj.Data.Email, body, ProcessType, EmailConfig, filePathList);
                }
                MailConfigure WhatsAppConfig = MailConfigure.Data.ToList().Find(x => x.ConfigureType.ToUpper() == "WHATSAPP" && x.IsActive == true);
                if (WhatsAppConfig == null)
                {
                    return;
                }
                else
                {
                    Template template = responseDataResults.Data.ToList().Find(x => x.TemplateType.ToUpper() == "WHATSAPP");
                    if (template == null)
                    {
                        return;
                    }

                    Dictionary<string, string> parameterValues = new Dictionary<string, string>();
                    int MessageLineNo = 0;
                    string[] tempstr = template.TemplateMsg.Split('[');
                    foreach (string str in tempstr)
                    {
                        if (str.StartsWith("|") && str.Contains("|]"))
                        {
                            string paramName = str.Substring(0, str.IndexOf(']'));
                            string paramValue = "";
                            if (paramName == "|UnitNo|")
                                paramValue = Communicationobj.Data.UnitDetail.unitNo;
                            else if (paramName == "|Email|")
                                paramValue = Communicationobj.Data.Email;
                            else if (paramName == "|MobileNo|")
                                paramValue = Communicationobj.Data.MobileNo;
                            else if (paramName == "|BookingDate|")
                                paramValue = Convert.ToString(Communicationobj.Data.BookingDate);
                            else if (paramName == "|ProjectName|")
                                paramValue = Convert.ToString(Communicationobj.Data.UnitDetail.projectName);
                            else if (paramName == "|ProjectAddress|")
                                paramValue = Convert.ToString(Communicationobj.Data.UnitDetail.projectAddress);
                            else if (paramName == "|ApplicationType|")
                                paramValue = Convert.ToString(Communicationobj.Data.ApplicationType);
                            else if (paramName == "|TowerName|")
                                paramValue = Convert.ToString(Communicationobj.Data.UnitDetail.towerName);
                            else if (paramName == "|FloorName|")
                                paramValue = Convert.ToString(Communicationobj.Data.UnitDetail.floorName);
                            else if (paramName == "|Area|")
                                paramValue = Convert.ToString(Communicationobj.Data.UnitDetail.area);
                            else if (paramName == "|Rate|")
                                paramValue = Convert.ToString(Communicationobj.Data.UnitDetail.rate);
                            else if (paramName == "|BasicAmount|")
                                paramValue = Convert.ToString(Communicationobj.Data.UnitDetail.basicAmount);
                            else if (paramName == "|AdditionalAmount|")
                                paramValue = Convert.ToString(Communicationobj.Data.UnitDetail.additionalCharge);
                            else if (paramName == "|DiscountAmount|")
                                paramValue = Convert.ToString(Communicationobj.Data.UnitDetail.discountAmount);
                            else if (paramName == "|CarpetArea|")
                                paramValue = Convert.ToString(Communicationobj.Data.UnitDetail.unitCarpetArea);
                            else if (paramName == "|CarpetAreaRate|")
                                paramValue = Convert.ToString(Communicationobj.Data.UnitDetail.unitCarpetAreaRate);
                            else if (paramName == "|UnitBalconyArea|")
                                paramValue = Convert.ToString(Communicationobj.Data.UnitDetail.unitBalconyArea);
                            else if (paramName == "|UnitBalconyAreaRate|")
                                paramValue = Convert.ToString(Communicationobj.Data.UnitDetail.unitBalconyAreaRate);
                            else if (paramName == "|UnitCarpetArea|")
                                paramValue = Convert.ToString(Communicationobj.Data.UnitDetail.unitCarpetArea);
                            else if (paramName == "|UnitCarpetAreaRate|")
                                paramValue = Convert.ToString(Communicationobj.Data.UnitDetail.unitCarpetAreaRate);
                            else if (paramName == "|BookingAmount|")
                                paramValue = Convert.ToString(Communicationobj.Data.BookingAmount);
                            else if (paramName == "|SalesPerson|")
                                paramValue = Convert.ToString(Communicationobj.Data.SalesPerson);
                            else if (paramName == "|BookingUrl|")
                                paramValue = Communicationobj.Data.BookingUrl;
                            else if (paramName == "|UserName|")
                                paramValue = Communicationobj.Data.Email;
                            else if (paramName == "|Password|")
                                paramValue = Communicationobj.Data.Password;

                            if (!string.IsNullOrEmpty(paramName))
                            {
                                parameterValues.Add(MessageLineNo.ToString(), paramValue);
                                MessageLineNo++;
                            }
                        }
                    }
                    string status = await SendWhatsApp(GroupID, UserID, Communicationobj.Data.BookingID, Communicationobj.Data.MobileNo, parameterValues, template.VendorTemplateID, EmailConfig);
                }

                MailConfigure smsConfig = MailConfigure.Data.ToList().Find(x => x.ConfigureType.ToUpper() == "SMS" && x.IsActive == true);
                if (smsConfig == null)
                {
                    return;
                }
                else
                {
                    Template template = responseDataResults.Data.ToList().Find(x => x.TemplateType.ToUpper() == "SMS");
                    if (template == null)
                    {
                        return;
                    }
                    string body = Replace(template.TemplateMsg, Communicationobj.Data);
                    string status = await SendSms(GroupID, UserID, Communicationobj.Data.BookingID, Communicationobj.Data.MobileNo, body, template.VendorTemplateID, smsConfig);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<ResponseDataResults<string>> TestMailConfigure(TestMail testMail)
        {
            string status = string.Empty;

            if (testMail.Type.ToUpper() == "SMS")
            {
                ResponseDataResults<List<MailConfigure>> MailConfigure = await _mailConfigureInterface.GetMailConfigure(testMail.GroupID);
                MailConfigure smsConfig = MailConfigure.Data.ToList().Find(x => x.ConfigureType.ToUpper() == "SMS" && x.MailConfigureID == testMail.MailConfigureID);
                status = await SendSms(testMail.GroupID, testMail.UserID, 0, testMail.ToMobileNo, testMail.Message, testMail.TemplateID, smsConfig);
            }
            else if (testMail.Type.ToUpper() == "WHATSAPP")
            {
                ResponseDataResults<List<MailConfigure>> MailConfigure = await _mailConfigureInterface.GetMailConfigure(testMail.GroupID);
                MailConfigure whatsAppConfig = MailConfigure.Data.ToList().Find(x => x.ConfigureType.ToUpper() == "WHATSAPP" && x.MailConfigureID == testMail.MailConfigureID);
                Dictionary<string, string> parameterValues = new Dictionary<string, string>();
                parameterValues.Add("0", testMail.Message);
                status = await SendWhatsApp(testMail.GroupID, testMail.UserID, 0, testMail.ToMobileNo, parameterValues, testMail.TemplateID, whatsAppConfig, "TEMPLATE");
            }
            else
            {
                ResponseDataResults<List<MailConfigure>> MailConfigure = await _mailConfigureInterface.GetMailConfigure(testMail.GroupID);
                MailConfigure emailConfig = MailConfigure.Data.ToList().Find(x => x.ConfigureType.ToUpper() == "EMAIL" && x.MailConfigureID == testMail.MailConfigureID);
                status = await SendEmail(testMail.GroupID, testMail.UserID, 0, testMail.ToEmail, testMail.Message, "Test Mail from SS-GROUP", emailConfig, null);

            }
            return new ResponseDataResults<string>
            {
                IsSuccess = true,
                Message = "Send Successfully!",
                Data = status
            };

        }




        public async Task<ResponseDataResults<string>> AlertMail(TestMail testMail)
        {
            string status = string.Empty;
            ResponseDataResults<List<MailConfigure>> MailConfigure = await _mailConfigureInterface.GetMailConfigure(testMail.GroupID);
            MailConfigure emailConfig = MailConfigure.Data.ToList().Find(x => x.ConfigureType.ToUpper() == "EMAIL" && x.MailConfigureID == testMail.MailConfigureID);
            string[] emails = testMail.ToEmail.Trim().Split(",");
            for (int i = 0; i < emails.Length; i++)
            {
                status = await SendEmail(testMail.GroupID, testMail.UserID, 0, emails[i], testMail.Message, testMail.Subject, emailConfig, null);
            }

            return new ResponseDataResults<string>
            {
                IsSuccess = true,
                Message = "Send Successfully!",
                Data = status
            };

        }

        public async Task<ResponseDataResults<string>> SendReallocationMail(TestMail testMail)
        {
            string status = string.Empty;

           
                ResponseDataResults<List<MailConfigure>> MailConfigure = await _mailConfigureInterface.GetMailConfigure(testMail.GroupID);
                MailConfigure emailConfig = MailConfigure.Data.ToList().Find(x => x.ConfigureType.ToUpper() == "EMAIL" && x.MailConfigureID == testMail.MailConfigureID);
                status = await SendEmail(testMail.GroupID, testMail.UserID, testMail.UbmID, testMail.ToEmail, testMail.Message , testMail.Subject, emailConfig, null);

            return new ResponseDataResults<string>
            {
                IsSuccess = true,
                Message = "Send Successfully!",
                Data = status
            };

        }



        private async Task<string> SendSms(decimal GroupID, int UserID, int ubmId, string MobileNo, string Message, string TemplateID, MailConfigure smsConfig)
        {
            string status = string.Empty;
            string SMSUserName;
            string SMSPassword;
            string SMSFrom;
            string SMSAPI;
            SMSUserName = smsConfig.UserName; //c.Decrypt(SMSUserName);
            SMSFrom = smsConfig.SenderName; //c.Decrypt(SMSFrom);
            SMSPassword = smsConfig.Password; //c.Decrypt(SMSPassword);
            SMSAPI = smsConfig.SmsUrl; //c.Decrypt(SMSAPI);
            SMSAPI = SMSAPI.Replace("=username", "=" + SMSUserName);
            SMSAPI = SMSAPI.Replace("=password", "=" + SMSPassword);
            SMSAPI = SMSAPI.Replace("=number", "=91" + MobileNo);
            SMSAPI = SMSAPI.Replace("=message", "=" + Message);
            SMSAPI = SMSAPI.Replace("=templateid", "=" + TemplateID);
            Uri objURI = new Uri(SMSAPI);
            WebRequest objWebRequest = WebRequest.Create(objURI);
            WebResponse objWebResponse = objWebRequest.GetResponse();
            System.IO.Stream objStream = objWebResponse.GetResponseStream();
            System.IO.StreamReader objStreamReader = new System.IO.StreamReader(objStream);
            status = objStreamReader.ReadToEnd();


            if (ubmId > 0)
            {
                MailHistory mailHistory = new MailHistory();
                mailHistory.MailConfigureID = smsConfig.MailConfigureID;
                mailHistory.GroupID = smsConfig.GroupID;
                mailHistory.UbmID = ubmId;
                mailHistory.ToEmail = MobileNo;
                mailHistory.Subject = "WhatsApp";
                mailHistory.Body = Message;
                mailHistory.CreatedBy = UserID;
                await _mailConfigureInterface.SaveMailHistory(mailHistory);
            }


            return await Task.FromResult<string>(status);
        }
        private async Task<string> SendWhatsApp(decimal GroupID, int UserID, int ubmId, string MobileNo, Dictionary<string, string> parameterValues, string TemplateID, MailConfigure whatsappConfigure, string TempType = "", string FileName = "", string FileUrl = "")
        {
            string status = string.Empty;

            //Send whats App Message
            string token = whatsappConfigure.TokenWA.ToString();
            var httpRequest = (HttpWebRequest)WebRequest.Create(whatsappConfigure.SmsUrl);
            httpRequest.Method = "POST";
            httpRequest.Headers["Authentication"] = "Bearer " + token;
            httpRequest.ContentType = "application/json";

            Dictionary<string, object> content = new Dictionary<string, object>();
            Dictionary<string, object> template = new Dictionary<string, object>();
            if (TempType == "TEMPLATE")
            {
                //Create Text-Template Data
                template.Add("templateId", TemplateID);
                template.Add("parameterValues", parameterValues);
                content.Add("preview_url", false);
                content.Add("shorten_url", false);
                content.Add("type", "TEMPLATE");
                content.Add("text", null);
                content.Add("attachment", null);
                content.Add("template", template);
                content.Add("mediaTemplate", null);
            }
            else if (parameterValues != null && !string.IsNullOrEmpty(FileName))
            {
                template.Add("templateId", TemplateID);
                template.Add("bodyParameterValues", parameterValues);

                Dictionary<string, string> media = new Dictionary<string, string>();
                if (TempType == "DOCUMENT/PDF")
                {
                    media.Add("type", "document");
                    media.Add("mimeType", "document/pdf");
                    media.Add("fileName", FileName + ".pdf");
                }
                else if (TempType == "IMAGE/PNG")
                {
                    media.Add("type", "image");
                    media.Add("mimeType", "image/png");
                    media.Add("fileName", FileName + ".png");
                }
                else if (TempType == "IMAGE/JPEG")
                {
                    media.Add("type", "image");
                    media.Add("mimeType", "image/jpeg");
                    media.Add("fileName", FileName + ".jpeg");
                }
                string base64String = "";
                using (WebClient client = new WebClient())
                {
                    var bytes = client.DownloadData(FileUrl);
                    base64String = Convert.ToBase64String(bytes);
                }
                media.Add("attachmentData", base64String);
                template.Add("media", media);

                content.Add("preview_url", false);
                content.Add("shorten_url", false);
                content.Add("type", "MEDIA_TEMPLATE");
                content.Add("text", null);
                content.Add("attachment", null);
                content.Add("template", null);
                content.Add("mediaTemplate", template);
            }

            Dictionary<string, string> recipient = new Dictionary<string, string>();
            recipient.Add("to", MobileNo.Length < 12 ? "91" + MobileNo : MobileNo);
            recipient.Add("recipient_type", "individual");
            Dictionary<string, string> senderFrom = new Dictionary<string, string>();
            senderFrom.Add("from", whatsappConfigure.PhoneWA.Length < 12 ? "91" + whatsappConfigure.PhoneWA : whatsappConfigure.PhoneWA);
            Dictionary<string, string> preferences = new Dictionary<string, string>();
            preferences.Add("webHookDNId", "1001");

            Dictionary<string, object> message = new Dictionary<string, object>();
            message.Add("channel", "WABA");
            message.Add("content", content);
            message.Add("recipient", recipient);
            message.Add("sender", senderFrom);
            message.Add("preferences", preferences);

            Dictionary<string, string> metaData = new Dictionary<string, string>();
            metaData.Add("version", "v1.0.9");

            Dictionary<string, object> body = new Dictionary<string, object>();
            body.Add("message", message);
            body.Add("metaData", metaData);

            string strinputdata = JsonConvert.SerializeObject(body);
            var inputdata = JsonConvert.SerializeObject(body);

            using (var streamWriter = new StreamWriter(httpRequest.GetRequestStream()))
            {
                streamWriter.Write(inputdata);
            }
            string result = string.Empty;
            HttpWebResponse httpResponse = (HttpWebResponse)httpRequest.GetResponse();
            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                result = streamReader.ReadToEnd();
            }

            if (httpResponse.StatusCode != HttpStatusCode.OK)
            {
                throw new Exception(httpResponse.StatusCode + ": " + httpResponse.StatusDescription);
            }
            if (httpResponse.StatusCode == HttpStatusCode.OK)
            {
                if (result.Contains("Successfully Accepted"))
                {
                    status = "Successfully Accepted";
                }
            }

            if (ubmId > 0)
            {
                MailHistory mailHistory = new MailHistory();
                mailHistory.MailConfigureID = whatsappConfigure.MailConfigureID;
                mailHistory.GroupID = whatsappConfigure.GroupID;
                mailHistory.UbmID = ubmId;
                mailHistory.ToEmail = MobileNo;
                mailHistory.Subject = "WhatsApp";
                mailHistory.Body = inputdata;
                mailHistory.CreatedBy = UserID;
                await _mailConfigureInterface.SaveMailHistory(mailHistory);
            }
            return await Task.FromResult<string>(status);
        }
        private async Task<string> SendEmail(decimal GroupID, int UserID, int ubmId, string ToEmail, string body, string Subject, MailConfigure mailConfigure, List<string> attachmentstring)
        {
            string status = string.Empty;

            if (mailConfigure.BasedOn.ToUpper() == "TLS")
            {
                var builder = new BodyBuilder();
                int port1 = Convert.ToInt32(mailConfigure.PortNo) != 0 ? Convert.ToInt32(mailConfigure.PortNo) : 25;
                var email = new MimeMessage();
                var smtpTLS = new MailKit.Net.Smtp.SmtpClient();
                smtpTLS.Connect(mailConfigure.SMTPServer, port1, SecureSocketOptions.Auto);
                if (mailConfigure.SenderName.Trim() != "" && IsEmail(mailConfigure.SenderName))
                {
                    email.From.Add(MailboxAddress.Parse(mailConfigure.SenderName));
                }
                else
                {
                    return "";
                }
                if (IsEmail(ToEmail.Trim()))
                {

                    email.To.Add(MailboxAddress.Parse(ToEmail.Trim()));
                }
                else
                {
                    return "";
                }

                email.From.Add(MailboxAddress.Parse(mailConfigure.SenderName));
                email.Subject = Subject;
                body = body.Replace("&nbsp;", "");
                builder.HtmlBody = body;
                List<LinkedResource> listLinkedResource = new List<LinkedResource>();
                string messageStr = body;
                if (messageStr.Contains("src"))
                {
                    string[] msgArray = messageStr.Split(' ');
                    msgArray = msgArray.Where(itemObject => itemObject.Contains("src")).ToArray<string>();
                    int k = 1;
                    foreach (string m in msgArray)
                    {
                        if (m.Contains("src"))
                        {
                            string srcpath = m;
                            srcpath = srcpath.Replace("src=", "").Replace("\"", "");

                            string webRootPath = _hostingEnvironment.WebRootPath;
                            string contentRootPath = _hostingEnvironment.ContentRootPath;

                            string ImagePath = webRootPath + "\n" + contentRootPath + "\n" + srcpath;
                            LinkedResource LinkedImage = new LinkedResource(ImagePath);
                            LinkedImage.ContentId = "Image" + k;
                            messageStr = messageStr.Replace(m, "src=cid:" + LinkedImage.ContentId);
                            listLinkedResource.Add(LinkedImage);
                            k = k + 1;
                        }
                    }
                }

                AlternateView htmlView = AlternateView.CreateAlternateViewFromString(messageStr, null, "text/html");
                if (listLinkedResource.Count > 0)
                {
                    foreach (LinkedResource linkResource in listLinkedResource)
                    {
                        htmlView.LinkedResources.Add(linkResource);
                    }
                }
                foreach (var path in attachmentstring)
                {
                    //foreach (var attachment in attachments)
                    //{
                    //    //builder.Attachments.Add(attachment);
                    //    byte[] fileBytes;
                    //    if (attachment.Length > 0)
                    //    {
                    //        using (var ms = new MemoryStream())
                    //        {
                    //            attachment.CopyTo(ms);
                    //            fileBytes = ms.ToArray();
                    //        }
                    //        builder.Attachments.Add(attachment.FileName, fileBytes, ContentType.Parse(attachment.ContentType));
                    //    }
                    //}
                    //  Attachment t = Attachment.CreateAttachmentFromString(attachmentstring.ToString(), "doc.html");
                    builder.Attachments.Add(path);
                }
                email.Body = builder.ToMessageBody();

                smtpTLS.Timeout = 2147483647;
                smtpTLS.Authenticate(mailConfigure.UserName, mailConfigure.Password);
                smtpTLS.Send(email);
                email.Dispose();
                status = "Email sent!";

            }

            //// start mtalk

            else if (mailConfigure.Provider.ToUpper() == "MTALK")
            {
                MailMessage objMM = new MailMessage();
                // System.Net.Mail.SmtpClient mailClient = new System.Net.Mail.SmtpClient(SmtpServer);
                System.Net.Mail.SmtpClient mailClient;

                if (Convert.ToString(mailConfigure.PortNo) == "")
                {
                    mailClient = new System.Net.Mail.SmtpClient(mailConfigure.SMTPServer);
                }
                else
                {
                    mailClient = new System.Net.Mail.SmtpClient(mailConfigure.SMTPServer, Convert.ToInt32(mailConfigure.PortNo));
                }
                System.Net.NetworkCredential Auth = new System.Net.NetworkCredential(mailConfigure.UserName, mailConfigure.Password);
                mailClient.UseDefaultCredentials = false;
                mailClient.Credentials = Auth;
                //if (IsSSL == true)
                //{
                //    mailClient.EnableSsl = IsSSL;
                //}


                if (mailConfigure.SenderName.Trim() != "" && IsEmail(mailConfigure.SenderName))
                {
                    MailAddress from = new MailAddress(mailConfigure.SenderName);
                    objMM.From = new MailAddress(from.ToString());
                }
                else
                {
                    return "";
                }
                if (IsEmail(ToEmail.Trim()))
                {
                    objMM.To.Add(new MailAddress(ToEmail.ToString()));
                }
                else
                {
                    return "";
                }
                objMM.Subject = Subject;// "Notification";

                objMM.Priority = MailPriority.High;
                objMM.IsBodyHtml = true;
                string msg = body;
                //Session["FailedAddress"] = FailedAddress;
                msg = msg.Replace("&nbsp;", "");
                objMM.Body = msg;

                List<LinkedResource> listLinkedResource = new List<LinkedResource>();
                string messageStr = msg;
                if (messageStr.Contains("src"))
                {
                    string[] msgArray = messageStr.Split(' ');
                    msgArray = msgArray.Where(itemObject => itemObject.Contains("src")).ToArray<string>();
                    int k = 1;
                    foreach (string m in msgArray)
                    {
                        if (m.Contains("src"))
                        {
                            string srcpath = m;
                            srcpath = srcpath.Replace("src=", "").Replace("\"", "");
                            string webRootPath = _hostingEnvironment.WebRootPath;
                            string contentRootPath = _hostingEnvironment.ContentRootPath;
                            string ImagePath = webRootPath + "\n" + contentRootPath + "\n" + srcpath;
                            LinkedResource LinkedImage = new LinkedResource(ImagePath);
                            LinkedImage.ContentId = "Image" + k;
                            messageStr = messageStr.Replace(m, "src=cid:" + LinkedImage.ContentId);
                            listLinkedResource.Add(LinkedImage);
                            k = k + 1;
                        }
                    }
                }

                AlternateView htmlView = AlternateView.CreateAlternateViewFromString(messageStr, null, "text/html");
                if (listLinkedResource.Count > 0)
                {
                    foreach (LinkedResource linkResource in listLinkedResource)
                    {
                        htmlView.LinkedResources.Add(linkResource);
                    }
                }

                mailClient.Send(objMM);
                objMM.Dispose();
                status = "Email sent!";
            }
            else
            {
                System.Net.Mail.SmtpClient mailClient;

                if (Convert.ToString(mailConfigure.PortNo) == "")
                {
                    //  mailClient = new SmtpClient(SmtpServer);
                    mailClient = new System.Net.Mail.SmtpClient(mailConfigure.SMTPServer);
                }
                else
                {
                    //  mailClient = new SmtpClient(SmtpServer, Convert.ToInt32(port));
                    mailClient = new System.Net.Mail.SmtpClient(mailConfigure.SMTPServer, Convert.ToInt32(mailConfigure.PortNo));
                }

                System.Net.NetworkCredential Auth = new System.Net.NetworkCredential(mailConfigure.UserName, mailConfigure.Password);
                mailClient.UseDefaultCredentials = false;
                mailClient.Credentials = Auth;
                if (mailConfigure.BasedOn == "SSL")
                {
                    mailClient.EnableSsl = true;
                }

                if (mailConfigure.SenderName.Trim() != "" && IsEmail(mailConfigure.SenderName))
                {
                    MailAddress mfrom = new MailAddress(mailConfigure.SenderName);
                }
                else
                {
                    return "";
                }

                MailAddress to;
                if (IsEmail(ToEmail.ToString()))//validate TO eMail ID
                {
                    to = new MailAddress(ToEmail.ToString());
                }
                else
                {

                    return "";
                }

                MailAddress from = new MailAddress(mailConfigure.SenderName);

                MailMessage objMM = new MailMessage(from, to);


                objMM.Subject = Subject;

                objMM.Priority = MailPriority.High;
                objMM.IsBodyHtml = true;
                string msg = body;
                //Session["FailedAddress"] = FailedAddress;
                msg = msg.Replace("&nbsp;", "");
                objMM.Body = msg;
                List<LinkedResource> listLinkedResource = new List<LinkedResource>();
                string messageStr = msg;
                if (messageStr.Contains("src"))
                {
                    string[] msgArray = messageStr.Split(' ');
                    msgArray = msgArray.Where(itemObject => itemObject.Contains("src")).ToArray<string>();
                    int k = 1;
                    foreach (string m in msgArray)
                    {
                        if (m.Contains("src"))
                        {
                            string srcpath = m;
                            srcpath = srcpath.Replace("src=", "").Replace("\"", "");
                            string webRootPath = _hostingEnvironment.WebRootPath;
                            string contentRootPath = _hostingEnvironment.ContentRootPath;
                            string ImagePath = webRootPath + "\n" + contentRootPath + "\n" + srcpath;
                            LinkedResource LinkedImage = new LinkedResource(ImagePath);
                            LinkedImage.ContentId = "Image" + k;
                            messageStr = messageStr.Replace(m, "src=cid:" + LinkedImage.ContentId);
                            listLinkedResource.Add(LinkedImage);
                            k = k + 1;
                        }
                    }
                }

                AlternateView htmlView = AlternateView.CreateAlternateViewFromString(messageStr, null, "text/html");
                if (listLinkedResource.Count > 0)
                {
                    foreach (LinkedResource linkResource in listLinkedResource)
                    {
                        htmlView.LinkedResources.Add(linkResource);
                    }
                }
                //validate(bypass) x509 certificate
                ServicePointManager.ServerCertificateValidationCallback = delegate (object s, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors) { return true; };
                objMM.AlternateViews.Add(htmlView);
                mailClient.Timeout = 2147483647;
                mailClient.Send(objMM);
                objMM.Dispose();
                status = "Email sent!";
            }

            if (ubmId > 0)
            {
                MailHistory mailHistory = new MailHistory();
                mailHistory.MailConfigureID = mailConfigure.MailConfigureID;
                mailHistory.GroupID = mailConfigure.GroupID;
                mailHistory.UbmID = ubmId;
                mailHistory.ToEmail = ToEmail;
                mailHistory.Subject = Subject;
                mailHistory.Body = body;
                mailHistory.CreatedBy = UserID;
                await _mailConfigureInterface.SaveMailHistory(mailHistory);
            }

            return await Task.FromResult<string>(status);
        }
        private string Replace(string original, Communication bookingdetail)
        {
            string tempstring = original;
            tempstring = ReplaceEx(tempstring, "[|UnitNo|]", bookingdetail.UnitDetail.unitNo);
            tempstring = ReplaceEx(tempstring, "[|Email|]", bookingdetail.Email);
            tempstring = ReplaceEx(tempstring, "[|MobileNo|]", bookingdetail.MobileNo);
            tempstring = ReplaceEx(tempstring, "[|BookingDate|]", Convert.ToString(bookingdetail.BookingDate));
            tempstring = ReplaceEx(tempstring, "[|ProjectName|]", bookingdetail.UnitDetail.projectName);
            tempstring = ReplaceEx(tempstring, "[|ProjectAddress|]", bookingdetail.UnitDetail.projectAddress);
            tempstring = ReplaceEx(tempstring, "[|ApplicationType|]", bookingdetail.ApplicationType);
            tempstring = ReplaceEx(tempstring, "[|TowerName|]", bookingdetail.UnitDetail.towerName);
            tempstring = ReplaceEx(tempstring, "[|FloorName|]", bookingdetail.UnitDetail.floorName);
            tempstring = ReplaceEx(tempstring, "[|Area|]", Convert.ToString(bookingdetail.UnitDetail.area));
            tempstring = ReplaceEx(tempstring, "[|Rate|]", Convert.ToString(bookingdetail.UnitDetail.rate));
            tempstring = ReplaceEx(tempstring, "[|BasicAmount|]", Convert.ToString(bookingdetail.UnitDetail.basicAmount));
            tempstring = ReplaceEx(tempstring, "[|AdditionalAmount|]", Convert.ToString(bookingdetail.UnitDetail.additionalCharge));
            tempstring = ReplaceEx(tempstring, "[|DiscountAmount|]", Convert.ToString(bookingdetail.UnitDetail.discountAmount));
            tempstring = ReplaceEx(tempstring, "[|CarpetArea|]", Convert.ToString(bookingdetail.UnitDetail.unitCarpetArea));
            tempstring = ReplaceEx(tempstring, "[|CarpetAreaRate|]", Convert.ToString(bookingdetail.UnitDetail.unitCarpetAreaRate));
            tempstring = ReplaceEx(tempstring, "[|UnitBalconyArea|]", Convert.ToString(bookingdetail.UnitDetail.unitBalconyArea));
            tempstring = ReplaceEx(tempstring, "[|UnitBalconyAreaRate|]", Convert.ToString(bookingdetail.UnitDetail.unitBalconyAreaRate));
            tempstring = ReplaceEx(tempstring, "[|UnitCarpetArea|]", Convert.ToString(bookingdetail.UnitDetail.unitCarpetArea));
            tempstring = ReplaceEx(tempstring, "[|UnitCarpetAreaRate|]", Convert.ToString(bookingdetail.UnitDetail.unitCarpetAreaRate));
            tempstring = ReplaceEx(tempstring, "[|BookingAmount|]", Convert.ToString(bookingdetail.BookingAmount));
            tempstring = ReplaceEx(tempstring, "[|SalesPerson|]", bookingdetail.SalesPerson);
            tempstring = ReplaceEx(tempstring, "[|BookingUrl|]", bookingdetail.BookingUrl);
            tempstring = ReplaceEx(tempstring, "[|UserName|]", bookingdetail.Email);
            tempstring = ReplaceEx(tempstring, "[|Password|]", bookingdetail.Password);
            return tempstring;
        }
        private string ReplaceEx(string original, string pattern, string replacement)
        {
            int count, position0, position1;
            count = position0 = position1 = 0;
            string upperString = original.ToUpper();
            string upperPattern = pattern.ToUpper();
            int inc = (original.Length / pattern.Length) *
                      (replacement.Length - pattern.Length);
            char[] chars = new char[original.Length + Math.Max(0, inc)];
            while ((position1 = upperString.IndexOf(upperPattern,
                                              position0)) != -1)
            {
                for (int i = position0; i < position1; ++i)
                    chars[count++] = original[i];
                for (int i = 0; i < replacement.Length; ++i)
                    chars[count++] = replacement[i];
                position0 = position1 + pattern.Length;
            }
            if (position0 == 0) return original;
            for (int i = position0; i < original.Length; ++i)
                chars[count++] = original[i];
            return new string(chars, 0, count);
        }
        public static bool IsEmail(string email)
        {
            /// <summary>
            /// Checks whether the given Email-Parameter is a valid E-Mail address.
            /// </summary>
            /// <param name="email">Parameter-string that contains an E-Mail address.</param>
            /// <returns>True, when Parameter-string is not null and 
            /// contains a valid E-Mail address;
            /// otherwise false.</returns>
            if (email != null && email != "") return System.Text.RegularExpressions.Regex.IsMatch(email, MatchEmailPattern);
            else return false;
        }

        public async Task<List<string>> GetAttachments(decimal? GroupID, int BookingID, int PayPlanID, string BookingType, string ProcessType)
        {
            List<string> filePathList = new List<string>();
            if (ProcessType.ToUpper() == "INITIAT BOOKING")
            {
                StringBuilder doc = new StringBuilder();
                var responseDocument = await _bookingInterface.GetApplicationDocument(GroupID, BookingType);
                if (responseDocument.IsSuccess)
                {

                    doc.Append("<!DOCTYPE html><html><head><title>Application Booking Required Documents</title></head><style>");
                    doc.Append("body {vertical-align: middle;}");
                    doc.Append("table { font-family: arial, sans-serif; border-collapse: collapse; width: 100%;}");
                    doc.Append("td, th { border: 1px solid #dddddd; text-align: left; padding: 8px;}");
                    doc.Append("tr:nth-child(even) { background-color: #dddddd;}");
                    doc.Append(".table-container { max-height: 550px; overflow-y: auto; }");
                    doc.Append("</style></head><body>");
                    doc.Append("<h2>Documents</h2>");
                    doc.Append("<div class='table-container'>");
                    doc.Append("<table><tr><th>S. No.</th><th>Document Name</th></tr>");
                    if (responseDocument.Data.Count > 0)
                    {
                        for (int i = 0; i < responseDocument.Data.Count; i++)
                        {
                            doc.Append("<tr>");
                            doc.Append("<td>" + i + "</td>");
                            doc.Append("<td>" + responseDocument.Data[i] + "</td>");
                            doc.Append("</tr>");
                        }
                    }
                    else
                    {
                        doc.Append("<tr>");
                        doc.Append("<td colspan='2'></td>");
                        doc.Append("</tr>");
                    }
                    doc.Append("</table>");
                    doc.Append("</div>");
                    doc.Append("</body></html>");

                    string wwwPath = this._Environment.WebRootPath;
                    var path = Path.Combine(wwwPath, "AttachmentFile");
                    if (!Directory.Exists(path))
                        Directory.CreateDirectory(path);
                    var filepath = Path.Combine(path, BookingID + "_Documents.html");
                    System.IO.File.WriteAllText(filepath, doc.ToString());
                    filePathList.Add(filepath);
                }
                var responsePayplan = await _UnitInterface.GetPaymentPlan(PayPlanID);
                if (responsePayplan.IsSuccess)
                {

                    doc.Append("<!DOCTYPE html><html><head><title>Payment Plan</title></head><style>");
                    doc.Append("body {vertical-align: middle;}");
                    doc.Append("table { font-family: arial, sans-serif; border-collapse: collapse; width: 100%;}");
                    doc.Append("td, th { border: 1px solid #dddddd; text-align: left; padding: 8px;}");
                    doc.Append("tr:nth-child(even) { background-color: #dddddd;}");
                    doc.Append(".table-container { max-height: 550px; overflow-y: auto; }");
                    doc.Append("</style></head><body>");
                    doc.Append("<h2>Payment Plan</h2>");
                    doc.Append("<div class='table-container'>");
                    doc.Append("<table><tr><th>S. No.</th><th>Stage Name</th><th>Charge Name</th><th>Due %</th></tr>");
                    if (responsePayplan.Data.Count > 0)
                    {
                        var prestageId = 0;
                        for (int i = 0; i < responsePayplan.Data.Count; i++)
                        {
                            doc.Append("<tr>");
                            if (prestageId != responsePayplan.Data[i].StageID)
                            {
                                doc.Append("<td>" + i + "</td>");
                                doc.Append("<td>" + responsePayplan.Data[i].StageName + "</td>");
                                doc.Append("<td>" + responsePayplan.Data[i].ChargeName + "</td>");
                                doc.Append("<td>" + responsePayplan.Data[i].DuePercentage + "</td>");
                                doc.Append("</tr>");
                            }
                            else
                            {
                                doc.Append("<td></td>");
                                doc.Append("<td></td>");
                                doc.Append("<td>" + responsePayplan.Data[i].ChargeName + "</td>");
                                doc.Append("<td>" + responsePayplan.Data[i].DuePercentage + "</td>");
                                doc.Append("</tr>");
                            }

                            prestageId = responsePayplan.Data[i].StageID;
                        }
                    }
                    else
                    {
                        doc.Append("<tr>");
                        doc.Append("<td colspan='4'></td>");
                        doc.Append("</tr>");
                    }
                    doc.Append("</table>");
                    doc.Append("</div>");
                    doc.Append("</body></html>");
                    string wwwPath = this._Environment.WebRootPath;
                    var path = Path.Combine(wwwPath, "AttachmentFile");
                    if (!Directory.Exists(path))
                        Directory.CreateDirectory(path);
                    var filepath = Path.Combine(path, BookingID + "_PaymentPlan.html");
                    System.IO.File.WriteAllText(filepath, doc.ToString());
                    filePathList.Add(filepath);
                    // Attachment t = System.Net.Mail.Attachment.CreateAttachmentFromString(paymentPlan.ToString(), "application/vnd.html");
                }
            }

            return filePathList;
        }
    }
}

