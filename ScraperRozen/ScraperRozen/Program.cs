using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Text.RegularExpressions;
using System.Net;

namespace PcPartPickerScraper
{
	class Program
	{
		static void Main(string[] args)
		{
			GetHtmlAsync();
			Console.ReadLine();
		}

		private static async void GetHtmlAsync()
		{
			int Items = 1;
			StringBuilder csvcontent = new StringBuilder();
			csvcontent.AppendLine("1,2,3,4,5,6");

			for (int i = 0; i < 7; i++)
			{
				int PageItems = Items * (i + 1);

				var firstUrl = "";// left blank intentionally

				//time out
				System.Threading.Thread.Sleep(2000);

				var Url = firstUrl.Replace("[]", PageItems.ToString());

				var httpClient = new HttpClient();
				var html = await httpClient.GetStringAsync(Url);

				var htmlDocument = new HtmlDocument();
				htmlDocument.LoadHtml(html);


				var ProductsHtml = htmlDocument.DocumentNode.Descendants("div")
				 .Where(node => node.GetAttributeValue("class", "")
				 .Equals("col-main--container")).ToList();

				var ProductListItems = ProductsHtml[0].Descendants("div")
				.Where(node => node.GetAttributeValue("class", "")
				.Contains("col-sm-3 shopproduct subcatproduct ")).ToList();

				Console.WriteLine();

				foreach (var ProductListItem in ProductListItems)
				{

					//get productname
					string ProductName = ProductListItem.Descendants("img")
					.FirstOrDefault().GetAttributeValue("title", "").Trim('\r', '\n', '\t');


					//get price of product					
					string ProductPrice =
						Regex.Match
						(ProductListItem.Descendants("span")
						.Where(node => node.GetAttributeValue("class", "")
						.Equals("price")).First().InnerText.Trim('\r', '\n', '\t').Replace(",", "."), @"\d+.+\d").ToString();


					// product image url
					string ProductImageUrl = ProductListItem.Descendants("button")
						.FirstOrDefault().GetAttributeValue("product-image", "");

					//product id
					string ProductId = ProductListItem.Descendants("button")
						.FirstOrDefault().GetAttributeValue("product-id", "");

					// product url
					string ProductUrl = ProductListItem.Descendants("a")
						.FirstOrDefault().GetAttributeValue("href", "");




					//// Het ophalen van de product description url per item 
					string ProductInnerDescriptionUrl = ProductListItem.Descendants("a")
						.FirstOrDefault().GetAttributeValue("href", "").Trim('\r', '\n', '\t');


					//// Het aanmaken van een nieuwe html client
					var singleProductHTTPClient = new HttpClient();
					var singleProductHTML = await httpClient.GetStringAsync(ProductInnerDescriptionUrl);

					var singleProductDocument = new HtmlDocument();
					singleProductDocument.LoadHtml(singleProductHTML);


					// ophalen van de eerste div genaamd head-img
					var GetHeadImg = singleProductDocument.DocumentNode.Descendants("div")
					.Where(node => node.GetAttributeValue("id", "")
					.Contains("head-img")).ToList();

					//Console.WriteLine();

					// ophalen van de div waar de img inzit genaamd amlabeldiv
					var GetInnerImage = GetHeadImg[0].Descendants("div")
					.Where(node => node.GetAttributeValue("class", "")
					.Contains("amlabel-div")).ToList();

					Console.WriteLine();


					//// ophalen van de img src url(base64)					
					string GetImageItSelf = GetInnerImage[0].Descendants("img")
					.FirstOrDefault().GetAttributeValue("src", "");

					Console.WriteLine();




					//byte[] imageData;

					////download image
					//using (WebClient webClient = new WebClient())
					//{
					//	imageData = webClient.DownloadData(ProductImageUrl);
					//	File.WriteAllBytes(@"D:\images\" + ProductId + ".png", imageData);


					//}


					string Data = ProductId + "," + ProductName + "," + ProductPrice + "," + GetImageItSelf;
					Console.WriteLine(Data);

					Console.WriteLine();

					//Console.ReadKey();


					// create csv file						
					csvcontent.AppendLine(Data);


				}

			}
			string csvPath = "D:\\RozenV2.csv";
			File.WriteAllText(csvPath, csvcontent.ToString()
							 );
		}
	}
}



