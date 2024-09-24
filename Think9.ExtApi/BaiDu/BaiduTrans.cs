﻿using Newtonsoft.Json;
using System;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace Think9.TransAPI
{
	public class BaiduTrans
	{
		/// <summary>
		/// 将中文翻译为英文
		/// </summary>
		/// <param name="source"></param>
		/// <returns></returns>
		public static string translation(string appId, string password, string source, bool bSpace)
		{
			string sReturn = "";
			string str;
			string strSpace = "";
			if (bSpace)
			{
				strSpace = " ";
			}

			TranslationResult result = GetTranslationFromBaiduFanyi(appId, password, source, Language.zh, Language.en);
			if (result != null)
			{
				//判断是否出错
				if (result.Error_code == null)
				{
					string[] arr = GetStrArrayBlankSpace(result.Trans_result[0].Dst);

					for (int i = 0; i < arr.GetLength(0); i++)
					{
						if (arr[i] != null)
						{
							str = arr[i].ToString().Trim();
							if (str.Length > 0)
							{
								sReturn += str.Substring(0, 1).ToUpper() + str.Substring(1, str.Length - 1) + strSpace;
							}
						}
					}
				}
				else
				{
					sReturn = "ERR：" + result.Error_code + " " + result.Error_msg;
				}
			}
			else
			{
				sReturn = "network error or other";
			}

			return sReturn.Trim();
		}

		public enum Language
		{
			//百度翻译API官网提供了多种语言，这里只列了几种
			auto = 0,

			zh = 1,
			en = 2,
			cht = 3,
		}

		//对字符串做md5加密
		private static string GetMD5WithString(string input)
		{
			if (input == null)
			{
				return null;
			}
			MD5 md5Hash = MD5.Create();
			//将输入字符串转换为字节数组并计算哈希数据
			byte[] data = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(input));
			//创建一个 Stringbuilder 来收集字节并创建字符串
			StringBuilder sBuilder = new StringBuilder();
			//循环遍历哈希数据的每一个字节并格式化为十六进制字符串
			for (int i = 0; i < data.Length; i++)
			{
				sBuilder.Append(data[i].ToString("x2"));
			}
			//返回十六进制字符串
			return sBuilder.ToString();
		}

		/// <summary>
		/// 调用百度翻译API进行翻译
		/// 详情可参考http://api.fanyi.baidu.com/api/trans/product/apidoc
		/// </summary>
		/// <param name="q">待翻译字符</param>
		/// <param name="from">源语言</param>
		/// <param name="to">目标语言</param>
		/// <returns></returns>
		private static TranslationResult GetTranslationFromBaiduFanyi(string appId, string password, string q, Language from, Language to)
		{
			//可以直接到百度翻译API的官网申请
			//一定要去申请，不然程序的翻译功能不能使用

			string jsonResult = String.Empty;
			//源语言
			string languageFrom = from.ToString().ToLower();
			//目标语言
			string languageTo = to.ToString().ToLower();
			//随机数
			string randomNum = System.DateTime.Now.Millisecond.ToString();
			//md5加密
			string md5Sign = GetMD5WithString(appId + q + randomNum + password);
			//url
			string url = String.Format("http://api.fanyi.baidu.com/api/trans/vip/translate?q={0}&from={1}&to={2}&appid={3}&salt={4}&sign={5}",
				HttpUtility.UrlEncode(q, Encoding.UTF8),
				languageFrom,
				languageTo,
				appId,
				randomNum,
				md5Sign
				);

			WebClient wc = new WebClient();
			try
			{
				jsonResult = wc.DownloadString(url);
			}
			catch
			{
				jsonResult = string.Empty;
			}

			//解析json
			//JavaScriptSerializer jss = new JavaScriptSerializer();
			//TranslationResult result = jss.Deserialize<TranslationResult>(jsonResult);
			//return result;

			//.net framework下
			//JavaScriptSerializer serializer = new JavaScriptSerializer();
			//var res = serializer.Serialize(YourObject);

			//.net core 下使用Newtonsoft.Json JsonConvert.SerializeObject(YourObject)
			//string str = JsonConvert.SerializeObject(jsonResult);
			//var res = JsonConvert.DeserializeObject<TranslationResult>(str);

			return JsonConvert.DeserializeObject<TranslationResult>(jsonResult);
		}

		/// <summary>
		/// 以空格分割
		/// </summary>
		public static string[] GetStrArrayBlankSpace(string sOutput)
		{
			string[] stringSeparators = new string[] { " " };
			string[] stReturn = sOutput.Split(stringSeparators, StringSplitOptions.RemoveEmptyEntries);
			return stReturn;
		}
	}
}