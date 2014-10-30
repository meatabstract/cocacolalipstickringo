using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text;
using System.Security.Cryptography;
using System.Web.Script.Serialization;


// Test 3

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


public partial class _Default : System.Web.UI.Page
{

	private static readonly Encoding encoding = Encoding.UTF8;



	protected void Page_Load(object sender, EventArgs e)
	{

	}
	protected void action_Click(object sender, EventArgs e)
	{
		/*$apikey='xxx';
$apisecret='xxx';
$nonce=time();
$uri='https://bittrex.com/api/v1.1/market/getopenorders?apikey='.$apikey.'&nonce='.$nonce;
$sign=hash_hmac('sha512',$uri,$apisecret);
$ch = curl_init($uri);
curl_setopt($ch, CURLOPT_HTTPHEADER, array('apisign:'.$sign));
$execResult = curl_exec($ch);
$obj = json_decode($execResult);*/




		var client = new RestClient("https://bittrex.com/api/v1.1/");
		DateTime theNow = DateTime.Now;

		string uri = "https://bittrex.com/api/v1.1/public/getmarkets?apikey=" + System.Configuration.ConfigurationManager.AppSettings["apikey"] + "&nonce=" + theNow.ToString();
		string HashMajik = HashIt(uri);



		// client.Authenticator = new HttpBasicAuthenticator(username, password);

		var request = new RestRequest("public/getmarkets", Method.GET);
		request.AddParameter("apikey", System.Configuration.ConfigurationManager.AppSettings["apikey"]); // adds to POST or URL querystring based on Method
		request.AddParameter("nonce", theNow); // replaces matching token in request.Resource

		// easily add HTTP Headers
		request.AddHeader("apisign", HashMajik);

		// add files to upload (works with compatible verbs)


		// execute the request
		RestResponse response = (RestResponse)client.Execute(request);
		var content = response.Content; // raw content as string*/

		marketresponse m_r = new JavaScriptSerializer().Deserialize<marketresponse>(content);


		int i = 0;
		results.Text = "";
		foreach (market m in m_r.result)
		{
			theNow = DateTime.Now;
			results.Text += "<br/>" + m.MarketName;

			//https://bittrex.com/api/v1.1/public/getmarkethistory?market=

			uri = "https://bittrex.com/api/v1.1/public/getmarkethistory?market=" + m.MarketName + "&apikey=" + System.Configuration.ConfigurationManager.AppSettings["apikey"] + "&nonce=" + theNow.ToString();
			HashMajik = HashIt(uri);

			request = new RestRequest("public/getmarkethistory", Method.GET);
			request.AddParameter("apikey", System.Configuration.ConfigurationManager.AppSettings["apikey"]); // adds to POST or URL querystring based on Method
			request.AddParameter("nonce", theNow); // replaces matching token in request.Resource
			request.AddParameter("market", m.MarketName); // replaces matching token in request.Resource

			// easily add HTTP Headers
			request.AddHeader("apisign", HashMajik);


			// easily add HTTP Headers
			request.AddHeader("apisign", HashMajik);
			response = (RestResponse)client.Execute(request);
			content = response.Content; // raw content as string*/
			results.Text += "<br/>" + content;

			i++;


			if (i > 10) return;


		}






		//results.Text = content;



	}


	protected string HashIt(string message)
	{
		var keyByte = encoding.GetBytes(System.Configuration.ConfigurationManager.AppSettings["apisecret"]);
		using (var hmacsha256 = new HMACSHA256(keyByte))
		{
			hmacsha256.ComputeHash(encoding.GetBytes(message));

			return ByteToString(hmacsha256.Hash);
		}
	}
	protected string ByteToString(byte[] buff)
	{
		string sbinary = "";
		for (int i = 0; i < buff.Length; i++)
			sbinary += buff[i].ToString("X2"); /* hex format */
		return sbinary;
	}



}