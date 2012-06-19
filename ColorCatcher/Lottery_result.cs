using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ColorCatcher
{
	class Lottery_result
	{
		public DateTime Date
		{
			get { return date; }
			set { date = value; }
		}
		public int Id
		{
			get { return id; }
			set { id = value; }
		}
		public int[] Red_nums
		{
			get { return red_nums; }
			set { red_nums = value; }
		}
		public int Blue_num
		{
			get { return blue_num; }
			set { blue_num = value; }
		}

		public override string ToString()
		{
			string num = "";
			foreach (var item in red_nums)
			{
				num += string.Format("{0:D2}, ", item);
			}
			num += string.Format(" {0:D2}", blue_num);
			return string.Format("{0:yyyy-MM-dd} {1} {2}", date, id, num);
		}

		private DateTime date;
		private int id;
		private int[] red_nums = new int[6];
		private int blue_num;
	}
}
