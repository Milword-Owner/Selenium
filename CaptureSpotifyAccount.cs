using System;
using System.Text.RegularExpressions;
using ikvm.extensions;
using Newtonsoft.Json.Linq;
using PluginFramework;
using PluginFramework.Attributes;
using RuriLib;
using RuriLib.LS;

namespace SpotifyPlugin
{
	// Token: 0x02000003 RID: 3
	public class CaptureSpotifyAccount : BlockBase
	{
		// Token: 0x06000004 RID: 4 RVA: 0x00002083 File Offset: 0x00000283
		public CaptureSpotifyAccount()
		{
			base.Label = "CaptureSpotifyAccount";
		}

		// Token: 0x17000001 RID: 1
		// (get) Token: 0x06000005 RID: 5 RVA: 0x00002096 File Offset: 0x00000296
		public string Name
		{
			get
			{
				return "CaptureSpotifyAccount";
			}
		}

		// Token: 0x17000002 RID: 2
		// (get) Token: 0x06000006 RID: 6 R

		// Token: 0x17000003 RID: 3
		// (get) Token: 0x06000007 RID: 7 RVA: 0x000020C7 File Offset: 0x000002C7
		public bool LightForeground
		{
			get
			{
				return false;
			}
		}

		// Token: 0x17000004 RID: 4
		// (get) Token: 0x06000008 RID: 8 RVA: 0x000020CA File Offset: 0x000002CA
		// (set) Token: 0x06000009 RID: 9 RVA: 0x000020D2 File Offset: 0x000002D2
		[Text("VariableName:", "capture")]
		public string VariableName
		{
			get
			{
				return this.variableName;
			}
			set
			{
				this.variableName = value;
				this.OnPropertyChanged("VariableName");
			}
		}

		// Token: 0x17000005 RID: 5
		// (get) Token: 0x0600000A RID: 10 RVA: 0x000020E6 File Offset: 0x000002E6
		// (set) Token: 0x0600000B RID: 11 RVA: 0x000020EE File Offset: 0x000002EE
		[Text("Input:", "")]
		public string Input
		{
			get
			{
				return this.input;
			}
			set
			{
				this.input = value;
				this.OnPropertyChanged("Input");
			}
		}

		// Token: 0x0600000C RID: 12 RVA: 0x00002104 File Offset: 0x00000304
		public override void Process(BotData data)
		{
			base.Process(data);
			string loginResult = BlockBase.ReplaceValues(this.Input, data);
			string capturedData;
			if (loginResult.Contains("student"))
			{
				capturedData = "Student";
			}
			else if (loginResult.Contains("hulu"))
			{
				capturedData = "Hulu";
			}
			else if (loginResult.Contains("duo"))
			{
				capturedData = "Duo";
			}
			else if (loginResult.Contains("family") && loginResult.Contains("sub"))
			{
				capturedData = "Family Member";
			}
			else if (loginResult.Contains("family") && loginResult.Contains("master"))
			{
				capturedData = "Family Owner, ";
				string value = Regex.Match(ExtensionMethods.split(loginResult, "-lol-")[1], "(?<=canonical_username: \")(.*)(?=\")").Value;
				string token = Regex.Match(ExtensionMethods.split(loginResult, "-lol-")[1], "(?<=reusable_auth_credentials: \")(.*)(?=\")").Value;
				JObject jobject = JObject.Parse(puff.GetCapture(value, token));
				string address = ExtensionMethods.toString(jobject["address"]);
				string inviteToken = ExtensionMethods.toString(jobject["inviteToken"]);
				ExtensionMethods.toString(jobject["maxCapacity"]);
				string country = ExtensionMethods.toString(JArray.Parse(ExtensionMethods.toString(jobject["members"]))[0]["country"]);
				capturedData = string.Concat(new string[]
				{
					capturedData,
					address,
					":",
					inviteToken,
					":",
					country
				});
			}
			else if (loginResult.Contains("pr:premium"))
			{
				capturedData = "Premium";
			}
			else
			{
				capturedData = "Other";
			}
			BlockBase.InsertVariable(data, true, capturedData, this.VariableName, "", "", false, true);
		}

		// Token: 0x0600000D RID: 13 RVA: 0x000022C0 File Offset: 0x000004C0
		public override string ToLS(bool indent = true)
		{
			BlockWriter writer = new BlockWriter(base.GetType(), indent, base.Disabled);
			writer.Label(base.Label).Token("CaptureSpotifyAccount", "").Literal(this.Input, "");
			if (!writer.CheckDefault(this.VariableName, "VariableName"))
			{
				writer.Arrow().Token("VAR", "").Literal(this.VariableName, "").Indent(1);
			}
			return writer.ToString();
		}

		// Token: 0x0600000E RID: 14 RVA: 0x00002354 File Offset: 0x00000554
		public override BlockBase FromLS(string line)
		{
			string inp = line.Trim();
			if (inp.StartsWith("#"))
			{
				base.Label = LineParser.ParseLabel(ref inp);
			}
			this.Input = LineParser.ParseLiteral(ref inp, "Input", false, null);
			try
			{
				this.VariableName = LineParser.ParseToken(ref inp, (TokenType)2, true, true);
			}
			catch
			{
				throw new ArgumentException("Variable name not specified");
			}
			return this;
		}

		// Token: 0x04000002 RID: 2
		private string variableName;

		// Token: 0x04000003 RID: 3
		private string input;
	}
}
