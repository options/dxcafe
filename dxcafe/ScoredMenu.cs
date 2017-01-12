using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace dxcafe
{
    public class ScoredMenu
    {
        public ScoredMenu(double americano, double cafeRatte, double cafuChinno, double espresso, string topScoredMenu)
        {
            Americano = americano;
            CafeRatte = cafeRatte;
            CafuChino = cafuChinno;
            Espresso = espresso;
            TopScoredMenu = topScoredMenu;
        }

        public SortedDictionary<string, double> GetSortedMenuList()
        {

            SortedDictionary<string, double> sortedMenuList = new SortedDictionary<string, double>();
            sortedMenuList.Add("Americano", Americano);
            sortedMenuList.Add("Cafe Latte", CafeRatte);
            sortedMenuList.Add("Cappucino", CafuChino);
            sortedMenuList.Add("Espresso", Espresso);
            return sortedMenuList;
        }

        public static ScoredMenu Empty
        {
            get
            {
                return new ScoredMenu(0, 0, 0, 0, "unknown");
            }
        }

        public double Americano { get; private set; }
        public double CafeRatte { get; private set; }
        public double CafuChino { get; private set; }
        public double Espresso { get; private set; }

        public string TopScoredMenu { get; private set; }
    }
    public enum Gender
    {
        Male,
        Femail
    }
    public static class MenuRecommender
    {
        public static async Task<ScoredMenu> GetTopScoredMenu(string name, int age, Gender gender)
        {
            ScoredMenu menuScore = ScoredMenu.Empty;

            var scoreRequest = new
            {
                Inputs = new Dictionary<string, List<Dictionary<string, string>>>() {
                        {
                            "input1",
                            new List<Dictionary<string, string>>(){new Dictionary<string, string>(){
                                            {
                                                "name", name
                                            },
                                            {
                                                "age", age.ToString()
                                            },
                                            {
                                                "date", ""
                                            },
                                            {
                                                "gender", gender == Gender.Male ? "M" : "F"
                                            },
                                            {
                                                "prodcts", ""
                                            },
                                }
                            }
                        },
                    },
                GlobalParameters = new Dictionary<string, string>()
                {
                }
            };

            using (var client = new System.Net.Http.HttpClient())
            {
                const string apiKey = "6EwIb5Om7prnsvyGpIXjsNnBzryfC5/tBcVEK/K+gxAtq9rF6Nt182fG9V6IW/Kn2iaxACcJGk0jmaDGh2cc7g=="; // Replace this with the API key for the web service
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);
                client.BaseAddress = new Uri("https://japaneast.services.azureml.net/subscriptions/123d6582561440979417b9712cc9255b/services/866a9f6da9374e799256415921ff6656/execute?api-version=2.0&format=swagger");
                // WARNING: The 'await' statement below can result in a deadlock
                // if you are calling this code from the UI thread of an ASP.Net application.
                // One way to address this would be to call ConfigureAwait(false)
                // so that the execution does not attempt to resume on the original context.
                // For instance, replace code such as:
                //      result = await DoSomeTask()
                // with the following:
                //      result = await DoSomeTask().ConfigureAwait(false)

                HttpResponseMessage response = await client.PostAsJsonAsync("", scoreRequest);

                if (response.IsSuccessStatusCode)
                {
                    string result = await response.Content.ReadAsStringAsync();
                    JObject o = JObject.Parse(result);
                    var obj = o["Results"]["output1"][0];

                    menuScore = new ScoredMenu(
                        americano: obj["Scored Probabilities for Class \"Americano\""].Value<double>(),
                        cafeRatte: obj["Scored Probabilities for Class \"cafe ratte\""].Value<double>(),
                        cafuChinno: obj["Scored Probabilities for Class \"cafu chino\""].Value<double>(),
                        espresso: obj["Scored Probabilities for Class \"Espresso\""].Value<double>(),
                        topScoredMenu: obj["Scored Labels"].Value<string>()
                    );
                }
            }

            return menuScore;
        }
    }
}
