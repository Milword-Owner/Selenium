using System;
using System.IO;
using Extreme.Net;
using Newtonsoft.Json.Linq;
using ProtoBuf;

namespace SpotifyPlugin
{
	// Token: 0x02000004 RID: 4
	internal class puff
	{
		// Token: 0x0600000F RID: 15 RVA: 0x000023C8 File Offset: 0x000005C8
		public static string GetCapture(string username, string key)
		{
			string result;
			try
			{
				puff.main main = new puff.main();
				puff.one one = new puff.one();
				puff.two two = new puff.two();
				one.Line1 = "65b708073fc0480ea92a077233ca87bd";
				one.Line2 = "S-1-5-21-3906878023-3586315189-2161068791";
				two.Line1 = username;
				two.Line2 = key;
				main.one = one;
				main.two = two;
				MemoryStream stream = new MemoryStream();
				Serializer.Serialize<puff.main>(stream, main);
				CookieDictionary cookies = new CookieDictionary(false);
				using (HttpRequest request = new HttpRequest
				{
					UseCookies = true,
					Cookies = cookies
				})
				{
					puff.Responsemain response = Serializer.Deserialize<puff.Responsemain>(new MemoryStream(request.Post("https://login5.spotify.com/v3/login", stream.ToArray(), "application/x-protobuf").ToBytes()));
					request.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/66.0.3000.145 Safari/537.36";
					request.Post("https://www.spotify.com/token-bounce/?url=/redirect/account-page", "oauth_token=" + response.one.Line2, "application/x-www-form-urlencoded");
					result = JObject.Parse(request.Get("https://www.spotify.com/us/home-hub/api/v1/family/home/", null).ToString()).ToString();
				}
			}
			catch (Exception)
			{
				result = null;
			}
			return result;
		}

		// Token: 0x02000007 RID: 7
		[ProtoContract]
		private class Responsemain
		{
			// Token: 0x1700000A RID: 10
			// (get) Token: 0x0600001D RID: 29 RVA: 0x00002FEC File Offset: 0x000011EC
			// (set) Token: 0x0600001E RID: 30 RVA: 0x00002FF4 File Offset: 0x000011F4
			[ProtoMember(1)]
			public puff.response one { get; set; }
		}

		// Token: 0x02000008 RID: 8
		[ProtoContract]
		private class response
		{
			// Token: 0x1700000B RID: 11
			// (get) Token: 0x06000020 RID: 32 RVA: 0x00003005 File Offset: 0x00001205
			// (set) Token: 0x06000021 RID: 33 RVA: 0x0000300D File Offset: 0x0000120D
			[ProtoMember(1)]
			public string Line1 { get; set; }

			// Token: 0x1700000C RID: 12
			// (get) Token: 0x06000022 RID: 34 RVA: 0x00003016 File Offset: 0x00001216
			// (set) Token: 0x06000023 RID: 35 RVA: 0x0000301E File Offset: 0x0000121E
			[ProtoMember(2)]
			public string Line2 { get; set; }

			// Token: 0x1700000D RID: 13
			// (get) Token: 0x06000024 RID: 36 RVA: 0x00003027 File Offset: 0x00001227
			// (set) Token: 0x06000025 RID: 37 RVA: 0x0000302F File Offset: 0x0000122F
			[ProtoMember(3)]
			public string Line3 { get; set; }
		}

		// Token: 0x02000009 RID: 9
		[ProtoContract]
		private class main
		{
			// Token: 0x1700000E RID: 14
			// (get) Token: 0x06000027 RID: 39 RVA: 0x00003040 File Offset: 0x00001240
			// (set) Token: 0x06000028 RID: 40 RVA: 0x00003048 File Offset: 0x00001248
			[ProtoMember(1)]
			public puff.one one { get; set; }

			// Token: 0x1700000F RID: 15
			// (get) Token: 0x06000029 RID: 41 RVA: 0x00003051 File Offset: 0x00001251
			// (set) Token: 0x0600002A RID: 42 RVA: 0x00003059 File Offset: 0x00001259
			[ProtoMember(100)]
			public puff.two two { get; set; }
		}

		// Token: 0x0200000A RID: 10
		[ProtoContract]
		private class one
		{
			// Token: 0x17000010 RID: 16
			// (get) Token: 0x0600002C RID: 44 RVA: 0x0000306A File Offset: 0x0000126A
			// (set) Token: 0x0600002D RID: 45 RVA: 0x00003072 File Offset: 0x00001272
			[ProtoMember(1)]
			public string Line1 { get; set; }

			// Token: 0x17000011 RID: 17
			// (get) Token: 0x0600002E RID: 46 RVA: 0x0000307B File Offset: 0x0000127B
			// (set) Token: 0x0600002F RID: 47 RVA: 0x00003083 File Offset: 0x00001283
			[ProtoMember(2)]
			public string Line2 { get; set; }
		}

		// Token: 0x0200000B RID: 11
		[ProtoContract]
		private class two
		{
			// Token: 0x17000012 RID: 18
			// (get) Token: 0x06000031 RID: 49 RVA: 0x00003094 File Offset: 0x00001294
			// (set) Token: 0x06000032 RID: 50 RVA: 0x0000309C File Offset: 0x0000129C
			[ProtoMember(1)]
			public string Line1 { get; set; }

			// Token: 0x17000013 RID: 19
			// (get) Token: 0x06000033 RID: 51 RVA: 0x000030A5 File Offset: 0x000012A5
			// (set) Token: 0x06000034 RID: 52 RVA: 0x000030AD File Offset: 0x000012AD
			[ProtoMember(2)]
			public string Line2 { get; set; }
		}
	}
}
