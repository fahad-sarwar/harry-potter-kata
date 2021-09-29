using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Xunit;

namespace Tests
{
    public class TestSpec
    {
        [Theory]
        [InlineData("1", 8)]
        [InlineData("2", 8)]
        [InlineData("3", 8)]
        [InlineData("4", 8)]
        [InlineData("5", 8)]
        public void BuyingOneBookReturnsEightPounds(string book, double total)
            => ScanMultipleBooks(new[] { book }).Should().Be(total);

        [Theory]
        [InlineData(new[] { "1", "1" }, 16)]
        [InlineData(new[] { "2", "2" }, 16)]
        [InlineData(new[] { "3", "3", "3" }, 24)]
        [InlineData(new[] { "4", "4", "4", "4" }, 32)]
        [InlineData(new[] { "5", "5", "5", "5", "5" }, 40)]
        public void BuyingMultipleOfSameBookHasNoDiscountIsApplied(IEnumerable<string> books, double total)
            => ScanMultipleBooks(books).Should().Be(total);

        [Theory]
        [InlineData(new[] { "1", "2" }, 8 * 2 * 0.95)]
        [InlineData(new[] { "1", "5" }, 8 * 2 * 0.95)]
        [InlineData(new[] { "1", "2", "3" }, 8 * 3 * 0.90)]
        [InlineData(new[] { "3", "4", "5" }, 8 * 3 * 0.90)]
        [InlineData(new[] { "1", "2", "3", "4" }, 8 * 4 * 0.80)]
        [InlineData(new[] { "2", "3", "4", "5" }, 8 * 4 * 0.80)]
        [InlineData(new[] { "1", "2", "3", "4", "5" }, 8 * 5 * 0.75)]
        public void BuyingDifferentBooksHasAPercentDiscountApplied(IEnumerable<string> books, double total)
            => ScanMultipleBooks(books).Should().Be(total);

        [Theory]
        [InlineData(new[] { "1", "2", "1" }, 8 * 2 * 0.95 + 8)]
        public void BuyingSeveralDifferentBooks(IEnumerable<string> books, double total)
            => ScanMultipleBooks(books).Should().Be(total);

        private static double ScanMultipleBooks(IEnumerable<string> books)
        {
            var scanner = new Scanner();

            books.ToList().ForEach(book => scanner.Scan(book));

            return scanner.GetTotal();
        }
    }

    public class Scanner
    {
        private const int BasePrice = 8;
        private readonly List<string> _items = new();

        private readonly Dictionary<int, double> _discountRules = new Dictionary<int, double>
        {
            { 1, 1 },
            { 2, 0.95 },
            { 3, 0.90 },
            { 4, 0.80 },
            { 5, 0.75 }
        };

        public Scanner Scan(string book)
        {
            _items.Add(book);

            return this;
        }

        public double GetTotal()
        {
            var distinctItemCount = _items
                .GroupBy(i => i)
                .Select(g => new
                {
                    Title = g.Key,
                    Count = g.Count()
                });

            var discountToApply = _discountRules[distinctItemCount.Count()];

            return BasePrice * _items.Count * discountToApply;
        }
    }
}
