using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Text.RegularExpressions;
using ColorCatcher.LotteryTableAdapters;
using System.Data.SQLite;

namespace ColorCatcher
{
	public class Data
	{
		private static void Main(string[] args)
		{
			var p = new Data();

			Lottery.CheckAndFix();

			while (p.Run()) { }
		}

		public List<Lottery_result> Lottery_list
		{
			get
			{
				if (lottery_list == null)
				{
					Lottery.CheckAndFix();

					lottery_list = new List<Lottery_result>();
					Init_lottery_result_list(lottery_list);

					if (lottery_list.Count == 0)
					{
						lottery_list = Get_all_results();
						Save_all_results(lottery_list);
					}
				}
				return lottery_list;
			}
		}

		private bool Run()
		{
			Console.Write(@"
*****************************************
1. Analyse;
2. Get base data.
Choose: ");

			string re = Console.ReadLine();
			switch (re)
			{
				case "1":
					lottery_list = new List<Lottery_result>();
					Init_lottery_result_list(lottery_list);
					Analyse(lottery_list);
					return true;

				case "2":
					lottery_list = Get_all_results();
					Save_all_results(lottery_list);
					Console.WriteLine("Getting base data done.");
					return true;

				default:
					return false;
			}
		}

		private List<Lottery_result> Get_all_results()
		{
			string history_data_url = "http://kaijiang.zhcw.com/zhcw/html/ssq/list_{0}.html";

			WebClient wc = new WebClient();
			string page_html = wc.DownloadString(string.Format(history_data_url, 1));

			int page_count = int.Parse(Regex.Match(page_html, @"class=""pg"".+?(?<num>\d+)").Groups["num"].Value);

			List<Lottery_result> lr_list = new List<Lottery_result>();
			for (int i = 1; i <= page_count; i++)
			{
				Console.WriteLine("Parsing page : " + i);
				page_html = wc.DownloadString(string.Format(history_data_url, i));
				MatchCollection mc = Regex.Matches(page_html, @"<td align=""center"">\d[\s\S]+?</em></td>");

				foreach (Match m in mc)
				{
					Lottery_result lr = new Lottery_result();

					MatchCollection mc_d = Regex.Matches(m.Groups[0].Value, @"<td align=""center"">(?<d>.+?)</td>");
					lr.Date = DateTime.Parse(mc_d[0].Groups["d"].Value);
					lr.Id = int.Parse(mc_d[1].Groups["d"].Value);

					MatchCollection mc_n = Regex.Matches(m.Groups[0].Value, @"<em.*>(?<n>\d+)</em>");

					int n;
					for (n = 0; n < 6; n++)
					{
						lr.Red_nums[n] = int.Parse(mc_n[n].Groups["n"].Value);
					}
					lr.Blue_num = int.Parse(mc_n[n].Groups["n"].Value);

					lr_list.Add(lr);
				}
			}

			return lr_list;
		}

		private void Save_all_results(List<Lottery_result> lr_list)
		{
			Lottery_resultTableAdapter adpter = new Lottery_resultTableAdapter();
			adpter.Adapter.DeleteCommand = adpter.Connection.CreateCommand();
			adpter.Adapter.DeleteCommand.CommandText = "delete from [Lottery_result] where 1";

			adpter.Connection.Open();

			SQLiteTransaction transaction = adpter.Connection.BeginTransaction();

			adpter.Adapter.DeleteCommand.ExecuteNonQuery();

			foreach (Lottery_result item in lr_list)
			{
				adpter.Insert(
					item.Id,
					item.Date,
					item.Red_nums[0],
					item.Red_nums[1],
					item.Red_nums[2],
					item.Red_nums[3],
					item.Red_nums[4],
					item.Red_nums[5],
					item.Blue_num
				);
			}

			transaction.Commit();

			adpter.Connection.Close();
		}

		private void Init_lottery_result_list(List<Lottery_result> lr_list)
		{
			Lottery_resultTableAdapter adpter = new Lottery_resultTableAdapter();
			Lottery.Lottery_resultDataTable table = adpter.GetData();
			foreach (Lottery.Lottery_resultRow row in table.Rows)
			{
				Lottery_result lr = new Lottery_result()
				{
					Date = row.Date,
					Id = row.Id,
					Red_nums = new int[]
					{
						row.Red_nums01,
						row.Red_nums02,
						row.Red_nums03,
						row.Red_nums04,
						row.Red_nums05,
						row.Red_nums06
					},
					Blue_num = row.Blue_num
				};
				lr_list.Add(lr);
			}
		}

		private void Analyse(List<Lottery_result> lr_list)
		{
			Console.WriteLine("\nLatest: " + lr_list[0]);

			int[] hist_red = new int[33];
			int[] hist_blue = new int[16];
			foreach (var item in lr_list)
			{
				foreach (var red in item.Red_nums)
				{
					hist_red[red - 1]++;
				}
				hist_blue[item.Blue_num - 1]++;
			}

			Console.WriteLine("\n--- Red ---");
			foreach (var item in array2dict(hist_red).OrderBy(h => h.Value))
			{
				Console.WriteLine("{0:D2} : {1}", item.Key + 1, item.Value);
			}

			Console.WriteLine("\n--- Blue ---");
			foreach (var item in array2dict(hist_blue).OrderBy(h => h.Value))
			{
				Console.WriteLine("{0:D2} : {1}", item.Key + 1, item.Value);
			}
		}

		private Dictionary<int, int> array2dict(int[] arr)
		{
			Dictionary<int, int> dict = new Dictionary<int, int>();
			for (int i = 0; i < arr.Length; i++)
			{
				dict.Add(i, arr[i]);
			}
			return dict;
		}

		private List<Lottery_result> lottery_list;
	}
}
