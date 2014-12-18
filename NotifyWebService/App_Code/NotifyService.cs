using System;
using System.IO;
using System.Web.Services;
using System.Web.Services.Protocols;

[WebService(Namespace = "http://tempuri.org/")]
[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
public class NotifyService : WebService
{

    [SoapDocumentMethod(Action = "http://schemas.microsoft.com/TeamFoundation/2005/06/Services/Notification/03/Notify",
        RequestNamespace = " http://schemas.microsoft.com/TeamFoundation/2005/06/Services/Notification/03")]
    [WebMethod(MessageName = "Notify")]
    public async void Notify(string eventXml)
    {
        using (var file = new StreamWriter(string.Format(@"F:\Temp\Notifications\{0}", DateTime.Now.ToString("yyyyMMdd-hhmmss.txt")), true))
        {
            await file.WriteAsync(eventXml);
        }
    }

}
