using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
using System.Security.Cryptography;
using RestSharp;
using System.Web.Script.Serialization;


public enum OrderType { UNKNOWN = 0, BUY = 1, SELL = 2 }
public enum FillType { UNKNOWN = 0, FILL = 1, PARTIAL_FILL = 2 }
public class markethistory
{

	public string Id { get; set; }
	public string TimeStamp { get; set; }
	public string Quantity { get; set; }
	public string Price { get; set; }
	public string Total { get; set; }
	public FillType FillType { get; set; }
	public OrderType OrderType { get; set; }
}

public class markethistoryresponse
{
	public bool success { get; set; }
	public string message { get; set; }
	public List<markethistory> result { get; set; }
}





public class marketresponse
{
	public bool success { get; set; }
	public string message { get; set; }
	public List<market> result { get; set; }
}



public class market
{
	public string MarketCurrency { get; set; }
	public string BaseCurrency { get; set; }
	public string MarketCurrencyLong { get; set; }
	public string BaseCurrencyLong { get; set; }
	public string MinTradeSize { get; set; }
	public string MarketName { get; set; }
	public string IsActive { get; set; }
	public string Created { get; set; }
	public string Notice { get; set; }
	public string IsSponsored { get; set; }
	public string LogoUrl { get; set; }

}



/// <summary>
/// Summary description for Helper
/// </summary>
public class Helper
{
	public Helper()
	{
		//
		// TODO: Add constructor logic here
		//
	}
}


public static class MarketTools
{


	public static marketresponse GetMarkets()
	{
		var client = new RestClient(System.Configuration.ConfigurationManager.AppSettings["apiurl"]);
		DateTime theNow = DateTime.Now;

		string uri = System.Configuration.ConfigurationManager.AppSettings["apiurl"] + "/public/getmarkets?apikey=" + System.Configuration.ConfigurationManager.AppSettings["apikey"] + "&nonce=" + theNow.ToString();
		
		var request = new RestRequest("public/getmarkets", Method.GET);
		request.AddParameter("apikey", System.Configuration.ConfigurationManager.AppSettings["apikey"]); 
		request.AddParameter("nonce", theNow); 

		// easily add HTTP Headers
		request.AddHeader("apisign", MakeHash(uri));

		RestResponse response = (RestResponse)client.Execute(request);
		var content = response.Content; 
			

		return new JavaScriptSerializer().Deserialize<marketresponse>(content);
	}


	public static markethistoryresponse GetMarketHistory(string marketKey)
	{
		var client = new RestClient(System.Configuration.ConfigurationManager.AppSettings["apiurl"]);
		DateTime theNow = DateTime.Now;


		string uri = System.Configuration.ConfigurationManager.AppSettings["apiurl"] + "/public/getmarkethistory?market=" + marketKey + "&apikey=" + System.Configuration.ConfigurationManager.AppSettings["apikey"] + "&nonce=" + theNow.ToString();
		//HashMajik = HashIt(uri);

		var request = new RestRequest("public/getmarkethistory", Method.GET);
		//request = new RestRequest("public/getmarkethistory", Method.GET);
		request.AddParameter("apikey", System.Configuration.ConfigurationManager.AppSettings["apikey"]); // adds to POST or URL querystring based on Method
		request.AddParameter("nonce", theNow); // replaces matching token in request.Resource
		request.AddParameter("market", marketKey); // replaces matching token in request.Resource

		//// easily add HTTP Headers
		request.AddHeader("apisign", MakeHash(uri));

		RestResponse response = (RestResponse)client.Execute(request);
		var content = response.Content; // raw content as string*/
		return new JavaScriptSerializer().Deserialize<markethistoryresponse>(content);
	}




	private static readonly Encoding encoding = Encoding.UTF8;
	private static string MakeHash(string message)
	{
		var keyByte = encoding.GetBytes(System.Configuration.ConfigurationManager.AppSettings["apisecret"]);
		using (var hmacsha256 = new HMACSHA256(keyByte))
		{
			hmacsha256.ComputeHash(encoding.GetBytes(message));

			return ByteToString(hmacsha256.Hash);
		}
	}
	private static string ByteToString(byte[] buff)
	{
		string sbinary = "";
		for (int i = 0; i < buff.Length; i++)
			sbinary += buff[i].ToString("X2"); /* hex format */
		return sbinary;
	}



}
