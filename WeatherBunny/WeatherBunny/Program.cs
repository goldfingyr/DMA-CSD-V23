// Module 02, Network, Exercise
// Based on
//   https://scrapingant.com/blog/html-parsing-libraries-c-sharp
//   https://dotnetfiddle.net/HoXY9S
//   https://www.w3schools.com/xml/xpath_syntax.asp


using HtmlAgilityPack;
using System.Net;

Console.WriteLine("Hi, I will be your weatherbunny today");
var url = "https://www.timeanddate.no/vaer/?continent=europe&low=c";
var web = new HtmlWeb();
var dom = web.Load(url);
var cities = dom.DocumentNode.SelectNodes("//td");
for (int index=0; index<cities.Count; index += 4)
{
    string temperature = cities[index + 3].InnerText;
    int posAmbersand = temperature.IndexOf('&');
    Console.WriteLine(cities[index].InnerText + "   " + cities[index+3].InnerText.Remove(posAmbersand, 6));
}
