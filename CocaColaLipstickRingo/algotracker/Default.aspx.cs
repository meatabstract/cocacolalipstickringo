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
using Tools.DataAccess;
using System.Data.Common;
using System.Data;








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




		//var client = new RestClient("https://bittrex.com/api/v1.1/");
		//DateTime theNow = DateTime.Now;

		//string uri = "https://bittrex.com/api/v1.1/public/getmarkets?apikey=" + System.Configuration.ConfigurationManager.AppSettings["apikey"] + "&nonce=" + theNow.ToString();
		//string HashMajik = HashIt(uri);


		//// client.Authenticator = new HttpBasicAuthenticator(username, password);

		//var request = new RestRequest("public/getmarkets", Method.GET);
		//request.AddParameter("apikey", System.Configuration.ConfigurationManager.AppSettings["apikey"]); // adds to POST or URL querystring based on Method
		//request.AddParameter("nonce", theNow); // replaces matching token in request.Resource

		//// easily add HTTP Headers
		//request.AddHeader("apisign", HashMajik);

		// add files to upload (works with compatible verbs)


		// execute the request
		//RestResponse response = (RestResponse)client.Execute(request);
		//var content = response.Content; // raw content as string*/

		//marketresponse m_r = new JavaScriptSerializer().Deserialize<marketresponse>(content);


		marketresponse m_r = MarketTools.GetMarkets();



		DAL Dal = Tools.DataAccess.Utils.GetDAL();
		DAL.Parameters Params = new DAL.Parameters(Dal.ProviderFactory);
		int m_id = 0;


		int i = 0;
		results.Text = "";
		foreach (market m in m_r.result)
		{

			Params.Clear();
			Params.Add("pname", m.MarketName);
			Params.Add("pkey", m.MarketName.ToUpper());

			Dal.ExecNonQuery("Market_Save", CommandType.StoredProcedure, Params);

			Params.Clear();
			Params.Add("pkey", m.MarketName.ToUpper());

			DbDataReader dr = Dal.ExecDataReader("Market_getOnKey", CommandType.StoredProcedure, Params);
			//TODO: Fix
			if (dr.Read())
			{
				//results.Text += dr.FieldCount + " - > " + dr[0].ToString() + "< - ";


				m_id = Convert.ToInt32(dr["uniqueId"].ToString());
			}

			dr.Close();

			//return the id
			//theNow = DateTime.Now;
			results.Text += m_id.ToString() + " " + m.MarketName + "<br/>";

			//https://bittrex.com/api/v1.1/public/getmarkethistory?market=

			//uri = "https://bittrex.com/api/v1.1/public/getmarkethistory?market=" + m.MarketName + "&apikey=" + System.Configuration.ConfigurationManager.AppSettings["apikey"] + "&nonce=" + theNow.ToString();
			//HashMajik = HashIt(uri);

			//request = new RestRequest("public/getmarkethistory", Method.GET);
			//request.AddParameter("apikey", System.Configuration.ConfigurationManager.AppSettings["apikey"]); // adds to POST or URL querystring based on Method
			//request.AddParameter("nonce", theNow); // replaces matching token in request.Resource
			//request.AddParameter("market", m.MarketName); // replaces matching token in request.Resource

			//// easily add HTTP Headers
			//request.AddHeader("apisign", HashMajik);


			//// easily add HTTP Headers
			//request.AddHeader("apisign", HashMajik);
			//response = (RestResponse)client.Execute(request);
			//content = response.Content; // raw content as string*/
			//markethistoryresponse mh_r = new JavaScriptSerializer().Deserialize<markethistoryresponse>(content);

			markethistoryresponse mh_r = MarketTools.GetMarketHistory(m.MarketName);




			if (mh_r.success && m_id > 0)
			{

				foreach (markethistory mh in mh_r.result)
				{
					//Trade_Save
					Params.Clear();
					Params.Add("pmarket", m_id);
					Params.Add("ptradtype", mh.OrderType);
					Params.Add("pprice", mh.Price);
					Params.Add("pqty", mh.Quantity);
					Params.Add("pkey", mh.Total);
					Params.Add("pfiltype", mh.FillType);

					Dal.ExecNonQuery("Trade_Save", CommandType.StoredProcedure, Params);
				}
			}

			//i++;





		}


		Dal.Dispose();






		//results.Text = content;



	}




}