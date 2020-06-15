using NPOI.SS.Formula.Functions;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using static Xunit.Assert;

namespace VotingSystem.Tests
{
	public class CounterTests
	{
		public const string CounterName = "Counter Name";
		public Counter _counter = new Counter { Name = CounterName, Count = 5 };

		[Fact]
		public void HasName()
		{
			Equal(CounterName, _counter.Name);
		}

		[Fact]
		public void GetStatistics_IncludesCounterName()
		{
			var statistics = _counter.GetStatistics(5);
			Equal(CounterName, statistics.Name);
		}

		[Fact]
		public void GetStatistics_IncludesCounter()
		{
			var statistics = _counter.GetStatistics(10);
			Equal(5, statistics.Count);
		}

		[Theory]
		[InlineData(5, 10, 50)]
		[InlineData(1, 3, 33.33)]
		[InlineData(2, 3, 66.67)]
		[InlineData(2, 8, 25)]
		public void GetStatistics_ShowsPercentageUpToTwoDecimalsBasedOnTotalCount(int count, int total, double expected)
		{
			_counter.Count = count;
			var statistics = _counter.GetStatistics(total);
			Equal(expected, statistics.Percent);
		}

		[Fact]
		public void ResolveExcess_DoesntAddExcessWhenAllCountersAreEqual()
		{ 
			var counter1 = new Counter { Percent = 33.33};
			var counter2 = new Counter { Percent = 33.33};
			var counter3 = new Counter { Percent = 33.33};
			var counters = new List<Counter> { counter1, counter2, counter3};

			new CounterManager().ResolveExcess(counters);

			Equal(33.33, counter1.Percent);
			Equal(33.33, counter2.Percent);
			Equal(33.33, counter3.Percent);
		}

		[Theory]
		[InlineData(66.66, 66.67, 33.33)]
		[InlineData(66.65, 66.67, 33.33)]
		[InlineData(66.66, 66.68, 33.32)]
		public void ResolveExcess_AddsExcessToHighestCounter(double initial, double expected, double lowest) 
		{
			var counter1 = new Counter { Percent = initial };
			var counter2 = new Counter { Percent = lowest };
			var counters = new List<Counter> { counter1, counter2 };

			new CounterManager().ResolveExcess(counters);

			Equal(expected, counter1.Percent);
			Equal(lowest, counter1.Percent);
		}

		[Theory]
		[InlineData(11.11, 11.12, 44.44)]
		public void ResolveExcess_AddsExcessToLowestCounterWhenMoreThanOneHighestCounters(double initial, double expected, double highest)
		{
			var counter1 = new Counter { Percent = highest };
			var counter2 = new Counter { Percent = highest };
			var counter3 = new Counter { Percent = initial };
			var counters = new List<Counter> { counter1, counter2, counter3 };

			new CounterManager().ResolveExcess(counters);

			Equal(highest, counter1.Percent);
			Equal(highest, counter2.Percent);
			Equal(expected, counter3.Percent);
		}

		[Fact]
		public void ResolveExcess_DoesntAddExcessIfTotalPercentIs100()
		{
			var counter1 = new Counter { Percent = 80 };
			var counter2 = new Counter { Percent = 20 };
			var counters = new List<Counter> { counter1, counter2 };

			new CounterManager().ResolveExcess(counters);

			Equal(80, counter1.Percent);
			Equal(20, counter1.Percent);
		}
	}	

	public class Counter
	{
		public string Name { get; set; }
		public int Count { get; set; }
		public double Percent { get; set; }

		internal Counter GetStatistics(int totalCount)
		{
			Percent =  Math.Round( Count * 100.0 / totalCount, 2);
			return this; 
		}
	}

	public class CounterManager
	{
		private double lowestPercent;

		public void ResolveExcess(List<Counter> counters)
		{
			var totalPercent = counters.Sum(x => x.Percent);
			if (totalPercent == 100) return;

			var excess = 100 - totalPercent;

			//var highestPercent = counters.Max(x => x.Percent);
			//var highestCounters = counters.Where(x => x.Percent == highestPercent).ToList();


			if (highestCounters.Count == 1)
			{
				counters[0].Percent += excess;
			}
			else if (highestCounters.Count < counters.Count)
			{
				var lowersPercent = counters.Min(x => x.Percent);
				counters.First(x => x.Percent == lowestPercent).Percent += excess;
			}

		}
	}
}
