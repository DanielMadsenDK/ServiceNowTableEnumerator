using System.Collections.ObjectModel;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace SN.WidEnum;

public partial class MainPage : ContentPage
{
	public int AmountOfExtractedTables { get; set; }

	public ObservableCollection<Table> Tables { get; set; }
	public Dictionary<string, string> Headers { get; set; }

	public MainPage()
	{
		InitializeComponent();

		var currentDirectory = Directory.GetCurrentDirectory();
		if (File.Exists(currentDirectory + @"\tables.cfg"))
		{
			var tablesList = File.ReadAllText(currentDirectory + @"\tables.cfg").Split("\n");
			Tables = new ObservableCollection<Table>();
			foreach (var table in tablesList)
			{
				Tables.Add(new Table() { Name = table });
			}
		}
		else
		{
			//setup defalut tables if cfg file is not found
			Tables = new ObservableCollection<Table> {
				new() { Name = "sys_attachment" },
				new() { Name = "survey_question_new" },
				new() { Name = "sc_cat_item" },
				new() { Name = "cmdb_model" },
				new() { Name = "cmn_department" },
				new() { Name = "kb_knowledge" },
				new() { Name = "licensable_app" },
				new() { Name = "alm_asset" },
				new() { Name = "oauth_entity" },
				new() { Name = "cmn_cost_center" },
				new() { Name = "sn_admin_center_application" },
				new() { Name = "cmn_company" },
				new() { Name = "sys_email_attachment" },
				new() { Name = "cmn_notif_device" },
				new() { Name = "incident" },
				new() { Name = "task" },
				new() { Name = "sc_category" }
			};
		}

		Headers = new Dictionary<string, string> {
			{"Content-Type", "application/json"},
			{"Accept", "application/json"}
		};

		BindingContext = this;
	}

	private async void OnScanClicked(object sender, EventArgs e)
	{
		if (await DisplayAlert("Warning", "You should only use this tool to scan your own instance, or an instance you have permission to scan.", "Proceed", "Abort"))
		{
			ScanBtn.IsEnabled = false;
			//scan
			AmountOfExtractedTables = 0;

			var cookie = "";
			var htmlContent = "";
			var gCkValue = "";

			using (var httpClient = new HttpClient())
			{
				httpClient.DefaultRequestHeaders.Clear();
				foreach (var header in Headers)
				{
					httpClient.DefaultRequestHeaders.TryAddWithoutValidation(header.Key, header.Value);
				}

				var url = InstanceTxt.Text + "/login.do";
				var setupResponse = httpClient.GetAsync(url).Result;
				if (setupResponse.IsSuccessStatusCode)
				{
					cookie = setupResponse.Headers.GetValues("Set-Cookie").FirstOrDefault();
					htmlContent = setupResponse.Content.ReadAsStringAsync().Result;

					//Find the gCk value
					var pattern = @"var\s+g_ck\s*=\s*'([^']+)';";
					var match = Regex.Match(htmlContent, pattern);
					if (match.Success)
					{
						gCkValue = match.Groups[1].Value.Replace(" ", "");
					}
					else
					{
						await DisplayAlert("Alert", "It was not possible to get the token required to proceed. The instance does not seem to be vulnerable. Aborting.", "OK");
						ScanBtn.IsEnabled = true;
						return;
					}
				}

				var tableUrl = InstanceTxt.Text + "/api/now/sp/widget/widget-simple-list?t=";
				httpClient.DefaultRequestHeaders.TryAddWithoutValidation("X-UserToken", gCkValue);
				httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Cookie", cookie);

				foreach (var table in Tables)
				{
					var result = await httpClient.PostAsync(tableUrl + table.Name, null);
					if (result.IsSuccessStatusCode)
					{
						try {
						var resultText = await result.Content.ReadAsStringAsync();
						var deserializedResponse = JsonDocument.Parse(resultText).RootElement;
						var entities = deserializedResponse.GetProperty("result").GetProperty("data").GetProperty("count").GetInt32();
						if (entities > 0)
							table.ScanResult = true;
							AmountOfExtractedTables++;
						} catch(Exception ex) {
							table.ScanResult = false;
						}
					}
					else
					{
						table.ScanResult = false;
					}
				}
			}
			if(AmountOfExtractedTables > 0) {
				await DisplayAlert("Alert", $"{AmountOfExtractedTables} tables have been found to be leaking data. Please review the list and adjust the security levels for these tables to prevent data leaking. The tablenames marked with red were found to be leaking data.", "OK");
				
			} else {
				await DisplayAlert("Information", "No tables were found to be leaking data.", "OK");
			}
			ScanBtn.IsEnabled = true;
		}
	}
}