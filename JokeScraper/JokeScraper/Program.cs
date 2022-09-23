using AngleSharp;
using Newtonsoft.Json;
using System.Net;

var data = await ReadData();
var json = JsonConvert.SerializeObject(data, Formatting.Indented);
File.WriteAllText("jokes.json", json);

async static Task<Dictionary<string, List<string>>> ReadData()
{
    var data = new Dictionary<string, List<string>>();
    var categoryPage = GetHtml("https://fun.dir.bg/categories");
    var config = Configuration.Default;
    using var context = BrowsingContext.New(config);
    using var doc = await context.OpenAsync(req => req.Content(categoryPage));
    var categoryLinks = doc.QuerySelectorAll(".category_link");
    foreach (var categoryLink in categoryLinks)
    {
        var category = categoryLink.QuerySelector(".category_title").TextContent;
        var href = categoryLink.GetAttribute("href");
        var currentCategoryPage = GetHtml($"https://fun.dir.bg{href}");
        var jokes = await GetJokesFromCategory(currentCategoryPage);
        foreach (var joke in jokes)
        {
            if (data.ContainsKey(category))
            {
                data[category].Add(joke);
            }
            else
            {
                data.Add(category, new List<string> { joke });
            }
        }
    }
    return data;
}

async static Task<List<string>> GetJokesFromCategory(string page)
{
    var result = new List<string>();
    var config = Configuration.Default;
    using var context = BrowsingContext.New(config);
    using var doc = await context.OpenAsync(req => req.Content(page));
    var allJokes = doc.QuerySelectorAll(".joke_text");
    foreach (var joke in allJokes)
    {
        result.Add(joke.TextContent);
    }
    var pages = doc.QuerySelectorAll(".pagination li a");
    foreach (var paginationElement in pages)
    {
        if (paginationElement.TextContent == "1") continue;
        var link = paginationElement.GetAttribute("href");
        
        using var nextPageDoc = await context.OpenAsync(req => req.Content(GetHtml(link)));
        var allJokesOnCurrPage = nextPageDoc.QuerySelectorAll(".joke_text");
        foreach (var joke in allJokesOnCurrPage)
        {
            result.Add(joke.TextContent);
        }
    }
    return result;
}

static String GetHtml(string url)
{

    HttpWebRequest myRequest = (HttpWebRequest)WebRequest.Create(url);
    myRequest.Method = "GET";
    WebResponse myResponse = myRequest.GetResponse();
    StreamReader sr = new StreamReader(myResponse.GetResponseStream(), System.Text.Encoding.UTF8);
    string result = sr.ReadToEnd();
    sr.Close();
    myResponse.Close();

    return result;
}