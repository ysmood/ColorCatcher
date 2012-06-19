namespace ColorCatcher {
	
	
	public partial class Lottery
	{

		public static void CheckAndFix()
		{
			LotteryTableAdapters.Lottery_resultTableAdapter da = new LotteryTableAdapters.Lottery_resultTableAdapter();

			da.Adapter.SelectCommand = da.Connection.CreateCommand();
			da.Adapter.SelectCommand.CommandText =
@"create table if not exists ""Lottery_result"" (
""Id""  int NOT NULL,
""Date""  datetime  int NOT NULL,
""Red_nums01""  int NOT NULL,
""Red_nums02""  int NOT NULL,
""Red_nums03""  int NOT NULL,
""Red_nums04""  int NOT NULL,
""Red_nums05""  int NOT NULL,
""Red_nums06""  int NOT NULL,
""Blue_num""  int NOT NULL,
PRIMARY KEY (""Id"" ASC)
)";
			da.Connection.Open();
			da.Adapter.SelectCommand.ExecuteNonQuery();
			da.Connection.Close();
		}
	}
}
