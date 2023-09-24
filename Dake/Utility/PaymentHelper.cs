using Dake.DAL;
using Dake.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Dake.Utility
{
	public static class PaymentHelper
	{
		public const string TerminalId = "24088739";
		public const string MerchantId = "140333498";
		public const string MerchantKey = "azuXOm5p545cHNfwfTgrxWUoE4HHBwBQ";
		public const string PurchasePage = "https://sadad.shaparak.ir";

		public static async Task< PayResultData> SendRequest(int orderid , long Amount , string ReturnUrl)
		{
		    PaymentRequest request = new PaymentRequest();
			request.OrderId = orderid.ToString();
			var dataBytes = Encoding.UTF8.GetBytes(string.Format("{0};{1};{2}", TerminalId, request.OrderId, Amount));
			var symmetric = SymmetricAlgorithm.Create("TripleDes");
			symmetric.Mode = CipherMode.ECB;
			symmetric.Padding = PaddingMode.PKCS7;
			var encryptor = symmetric.CreateEncryptor(Convert.FromBase64String(MerchantKey), new byte[8]);
			request.SignData = Convert.ToBase64String(encryptor.TransformFinalBlock(dataBytes, 0, dataBytes.Length));
			request.ReturnUrl = ReturnUrl;
			var ipgUri = string.Format("{0}/api/v0/Request/PaymentRequest", PurchasePage);
			var data = new
			{
				TerminalId,
				MerchantId,
				Amount,
				request.SignData,
				request.ReturnUrl,
				LocalDateTime = DateTime.Now,
				orderid
			};
			var res = CallApi<PayResultData>(ipgUri, data);
			res.Wait();
			return await res;		
		
		}
		public static async Task<T> CallApi<T>(string apiUrl, object value)
		{
			using (var client = new HttpClient())
			{
				client.BaseAddress = new Uri(apiUrl);
				client.DefaultRequestHeaders.Accept.Clear();
				var w = client.PostAsJsonAsync(apiUrl, value);
				w.Wait();
				HttpResponseMessage response = w.Result;
				if (response.IsSuccessStatusCode)
				{
					var result = response.Content.ReadAsAsync<T>();
					result.Wait();
					return result.Result;
				}
				return default(T);
			}
		}
	}
}
