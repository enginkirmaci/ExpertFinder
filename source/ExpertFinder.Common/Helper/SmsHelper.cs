using Common.Utilities;
using Common.Utilities.Converters;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Text.Encodings.Web;

namespace ExpertFinder.Common.Helper
{
    public class SmsHelper
    {
        static public string SendSMS(string phone, string smsBody)
        {
            if (string.IsNullOrWhiteSpace(phone))
                return null;

            try
            {
                var xmlMessage = @"
<request>
	<authentication>
		<username>{0}</username>
		<password>{1}</password>
	</authentication>
	<order>
		<sender>{2}</sender>
		<sendDateTime>{3}</sendDateTime>
		<message>
			<text><![CDATA[{4}]]></text>
			<receipents>
				<number>{5}</number>
			</receipents>
		</message>
	</order>
</request>";

                var username = "05457780708";
                var password = "10123654";
                var sender = HtmlEncoder.Default.Encode("TEKLIFCEPTE");

                var text = HtmlEncoder.Default.Encode(Converter.String.TurkishCharsToEnglish(smsBody));
                phone = Utils.CorrectPhoneNumber(phone);

                ////TODO test
                //text = HttpUtility.HtmlEncode(ConvertTurkishChars("Tebrikler, Kayıt işlemini neredeyse tamamladınız. Üyeliğinizi aktif etmek için lütfen linke tıklayınız."));
                //var t2 = ConvertTurkishChars("Tebrikler, Kayıt işlemini neredeyse tamamladınız. Üyeliğinizi aktif etmek için lütfen linke tıklayınız.");
                //to = CorrectPhoneNumber("5332211868");
                //var to2 = CorrectPhoneNumber("05332211868");

                var now = DateTime.Now;
                var dateTime = string.Format("{0}/{1}/{2} {3}:{4}",
                    now.Day, now.Month, now.Year, now.Hour, now.Minute);

                var data = string.Format(
                    xmlMessage,
                    username,
                    password,
                    sender,
                    dateTime,   // DateTime.Now.ToString("dd/MM/yyyy hh:mm"),
                    text,
                    phone);

                byte[] buffer = Encoding.UTF8.GetBytes(data);
                HttpWebRequest WebReq = (HttpWebRequest)WebRequest.Create("http://api.iletimerkezi.com/v1/send-sms");
                WebReq.Timeout = 5 * 60 * 1000;

                WebReq.Method = "POST";
                WebReq.ContentType = "application/x-www-form-urlencoded";
                WebReq.ContentLength = buffer.Length;
                WebReq.CookieContainer = new CookieContainer();

                var ReqStream = WebReq.GetRequestStream();
                ReqStream.Write(buffer, 0, buffer.Length);
                ReqStream.Close();

                var WebRes = WebReq.GetResponse();
                var ResStream = WebRes.GetResponseStream();
                var ResReader = new StreamReader(ResStream);

                return ResReader.ReadToEnd();
            }
            catch (Exception ex)
            {
                //Util.Log(HttpContext.Current.Server.MapPath("~/data/log"), "sms-error", ex.ToString());
            }

            return null;
        }

        static public string SendBulkSMS(List<string> phoneList, string smsBody)
        {
            if (phoneList == null || phoneList.Count == 0)
                return null;

            try
            {
                var xmlMessage = @"
<request>
	<authentication>
		<username>{0}</username>
		<password>{1}</password>
	</authentication>
	<order>
		<sender>{2}</sender>
		<sendDateTime>{3}</sendDateTime>
		<message>
			<text><![CDATA[{4}]]></text>
			<receipents>
				{5}
			</receipents>
		</message>
	</order>
</request>";

                var username = "05457780708";
                var password = "10123654";
                var sender = HtmlEncoder.Default.Encode("TEKLIFCEPTE");

                var text = HtmlEncoder.Default.Encode(Converter.String.TurkishCharsToEnglish(smsBody));
                StringBuilder phones = new StringBuilder();
                foreach (var item in phoneList)
                {
                    phones.Append(string.Format("<number>{0}</number>", Utils.CorrectPhoneNumber(item)));
                }

                ////TODO test
                //text = HttpUtility.HtmlEncode(ConvertTurkishChars("Tebrikler, Kayıt işlemini neredeyse tamamladınız. Üyeliğinizi aktif etmek için lütfen linke tıklayınız."));
                //var t2 = ConvertTurkishChars("Tebrikler, Kayıt işlemini neredeyse tamamladınız. Üyeliğinizi aktif etmek için lütfen linke tıklayınız.");
                //to = CorrectPhoneNumber("5332211868");
                //var to2 = CorrectPhoneNumber("05332211868");

                var now = DateTime.Now;
                var dateTime = string.Format("{0}/{1}/{2} {3}:{4}",
                    now.Day, now.Month, now.Year, now.Hour, now.Minute);

                var data = string.Format(
                    xmlMessage,
                    username,
                    password,
                    sender,
                    dateTime,   // DateTime.Now.ToString("dd/MM/yyyy hh:mm"),
                    text,
                    phones.ToString());

                byte[] buffer = Encoding.UTF8.GetBytes(data);
                HttpWebRequest WebReq = (HttpWebRequest)WebRequest.Create("http://api.iletimerkezi.com/v1/send-sms");
                WebReq.Timeout = 5 * 60 * 1000;

                WebReq.Method = "POST";
                WebReq.ContentType = "application/x-www-form-urlencoded";
                WebReq.ContentLength = buffer.Length;
                WebReq.CookieContainer = new CookieContainer();

                var ReqStream = WebReq.GetRequestStream();
                ReqStream.Write(buffer, 0, buffer.Length);
                ReqStream.Close();

                var WebRes = WebReq.GetResponse();
                var ResStream = WebRes.GetResponseStream();
                var ResReader = new StreamReader(ResStream);

                return ResReader.ReadToEnd();
            }
            catch (Exception ex)
            {
                //Util.Log(HttpContext.Current.Server.MapPath("~/data/log"), "sms-error", ex.ToString());
            }

            return null;
        }
    }
}