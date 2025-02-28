using PanguSpacing;

string text = """
	Sephiroth见到他这等神情，也是悚然一惊：“此人来历不小啊！不知我这太极拳是否对付得了？”
	张无忌道：“Tifa，你待孩儿恩重如山，孩儿便粉身碎骨，也不足以报太师父和Red XIII的大恩。我武当派功夫虽不敢说天下无敌，但也不致输于西域少林的手下。太师父尽管放心。”
	변환된123
	烫+烫-烫*烫/烫%烫^烫+烫−烫×烫÷烫±烫=烫≠烫≈烫
	更多⋯⋯文本“︁全角引号”︁引文“halfwidth”“︀quotes”︀引文。
	""";

Console.OutputEncoding = System.Text.Encoding.UTF8;
Console.WriteLine(text);
Console.WriteLine();
Console.WriteLine(Pangu.Spacing(text));
