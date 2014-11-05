using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.Web.Script.Serialization;

using RestSharp;

using CCLR.Market.Source.Messages;

namespace CCLR.Market.Source.Bittrex
{
	public class BittrexMarketSource : IMarketSource
	{
		private const String marketCode = "BTC";
		private String apiKey = "";
		private String apiSecret = "";

		public BittrexMarketSource(String apiKey, String apiSecret)
		{
			this.apiKey = apiKey;
			this.apiSecret = apiSecret;
		}

		public TransactionsResponse GetTransactions(TransactionsRequest request)
		{
			throw new NotImplementedException();
		}

		public CurrenciesResponse GetCurrencies(GetCurrenciesRequest request)
		{
			RestClient restClient = new RestClient("https://bittrex.com/api/v1.1/");
			DateTime theNow = DateTime.Now;

			String uri = "https://bittrex.com/api/v1.1/public/getmarkets?apikey=" + apiKey + "&nonce=" + theNow.ToString();
			String apisign = HashIt(uri);

			// client.Authenticator = new HttpBasicAuthenticator(username, password);

			RestRequest restRequest = new RestRequest("public/getmarkets", Method.GET);
			restRequest.AddParameter("apikey", apiKey); // adds to POST or URL querystring based on Method
			restRequest.AddParameter("nonce", theNow); // replaces matching token in request.Resource

			// easily add HTTP Headers
			restRequest.AddHeader("apisign", apisign);

			// execute the request
			RestResponse restResponse = (RestResponse)restClient.Execute(restRequest);
			var content = restResponse.Content; // raw content as string*/

			MarketResponse marketResponse = new JavaScriptSerializer().Deserialize<MarketResponse>(content);

			List<String> currencyNames = new List<String>();
			foreach(Market market in marketResponse.result)
			{
				currencyNames.Add(market.MarketName);
			}

			CurrenciesResponse response = new CurrenciesResponse();
			response.CurrencyCodes = currencyNames.AsReadOnly();
			response.IsSuccess = true;
			return response;
		}

		private String HashIt(string message)
		{
			var keyByte = Encoding.UTF8.GetBytes(apiSecret);
			using (HMACSHA256 hmacsha256 = new HMACSHA256(keyByte))
			{
				hmacsha256.ComputeHash(Encoding.UTF8.GetBytes(message));

				return ByteToString(hmacsha256.Hash);
			}
		}

		private String ByteToString(byte[] buff)
		{
			String sbinary = "";
			for (int i = 0; i < buff.Length; i++)
				sbinary += buff[i].ToString("X2"); /* hex format */
			return sbinary;
		}


		public MarketsResponse GetMarkets()
		{
			// The bitrex market source is a single market source....
			MarketsResponse response = new MarketsResponse();
			response.Markets = new String[1] { marketCode };
			response.IsSuccess = true;
			return response;
		}
	}
}
