using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Xunit;

namespace LinqKit.Tests.Net452
{
    public static class TestExtensions
    {
        [Expandable(nameof(GenericImpl1))]
        public static string Generic1<T>(this T t) => throw new NotImplementedException();

        [Expandable(nameof(GenericImpl2))]
        public static string Generic2<T1, T2>(this T1 t1, T2 t2) => throw new NotImplementedException();

        static Expression<Func<T, string>> GenericImpl1<T>()
        {
            return t => t.ToString() + "_extended";
        }

        static Expression<Func<T1, T2, string>> GenericImpl2<T1, T2>()
        {
            return (t1, t2) => t1 + "_extended_" + t2;
        }
    }

    
    public class ExpandableAttributeTests
    {
        class MainClass
        {
            public int Id { get; set; }
            public int IntValueProp { get; set; }
            public string ValueProp { get; set; }

            public override string ToString()
            {
                return ValueProp;
            }

            public List<DetailClass> Details { get; } = new List<DetailClass>();

            static Func<MainClass, IEnumerable<DetailClass>> _filteredDetailsImpl;

            [Expandable(nameof(FilteredDetailsImpl))]
            public IEnumerable<DetailClass> FilteredDetails
            {
                get
                {
                    if (_filteredDetailsImpl == null)
                        _filteredDetailsImpl = FilteredDetailsImpl().Compile();
                    return _filteredDetailsImpl(this);
                }
            }

            [Expandable(nameof(FilteredDetailsImpl))]
            public IEnumerable<DetailClass> FilteredDetailsNoImpl => throw new NotImplementedException();

            static Expression<Func<MainClass, IEnumerable<DetailClass>>> FilteredDetailsImpl()
            {
                return e => e.Details.Where(d => d.Id % 4 == 0);
            }

            static Expression<Func<MainClass, string>> NonGenericImpl()
            {
                return t => t + "_non_generic_extended";
            }

            static Expression<Func<MainClass, string, string>> NonGenericImplStr()
            {
                return (t, str) => t + "_non_generic_extended_" + str;
            }

            [Expandable(nameof(NonGenericImpl))]
            public string ExpandableProp
            {
                get => throw new NotImplementedException();
                set => throw new NotImplementedException();
            }

            [Expandable(nameof(NonGenericImpl))]
            public string ExpandableMethod() => throw new NotImplementedException();

            [Expandable(nameof(NonGenericImplStr))]
            public string ExpandableMethod(string str) => throw new NotImplementedException();

        }

        class DetailClass
        {
            public int Id { get; set; }
            public int MasterId { get; set; }

            public string DetailValue { get; set; }
        }

        IQueryable<MainClass> GenerateTestData()
        {
            var items = Enumerable.Range(1, 2).Select(i => new MainClass { ValueProp = "Value" + i }).ToArray();
            var details = Enumerable.Range(1, 20)
                .Select(i => new DetailClass { DetailValue = "Detail" + i, Id = i, MasterId = i % 2 + 1 })
                .ToArray();

            foreach (var detail in details)
            {
                items[detail.MasterId - 1].Details.Add(detail);
            }

            return items.AsQueryable();
        }

        [Fact]
        public void ExpandableAttribute_Property()
        {
            // Assign
            var query = GenerateTestData();

            // Act
            var items = query.AsExpandable()
                .Where(e => e.ExpandableProp.EndsWith("_non_generic_extended") && e.ExpandableProp.StartsWith("Value"))
                .ToArray();

            // Assert
            Assert.Equal(2, items.Length);
        }

        [Fact]
        public void ExpandableAttribute_NonGenericMethod()
        {
            // Assign
            var query = GenerateTestData();

            // Act
            var items = query.AsExpandable()
                .Where(e => e.ExpandableMethod().EndsWith("_non_generic_extended") && e.ExpandableMethod().StartsWith("Value"))
                .ToArray();

            // Assert
            Assert.Equal(2, items.Length);
        }

        [Fact]
        public void ExpandableAttribute_NonGenericMethodStr()
        {
            // Assign
            var query = GenerateTestData();

            // Act
            var items = query.AsExpandable()
                .Where(e => e.ExpandableMethod("end.").EndsWith("_non_generic_extended_end.") && e.ExpandableMethod().StartsWith("Value"))
                .ToArray();

            // Assert
            Assert.Equal(2, items.Length);
        }

        [Fact]
        public void ExpandableAttribute_Generic1()
        {
            // Assign
            var query = GenerateTestData();

            // Act
            var items = query.AsExpandable()
                .Where(e => e.Generic1().EndsWith("_extended") && e.Generic1().StartsWith("Value"))
                .ToArray();

            // Assert
            Assert.Equal(2, items.Length);
        }

        [Fact]
        public void ExpandableAttribute_Generic2()
        {
            // Assign
            var query = GenerateTestData();

            // Act
            var items = query.AsExpandable()
                .Where(e => e.Generic2("end.").EndsWith("_extended_end.") && e.Generic2("end.").StartsWith("Value"))
                .ToArray();

            // Assert
            Assert.Equal(2, items.Length);
        }

        [Fact]
        public void ExpandableAttribute_FilteredDetails()
        {
            // Assign
            var query = GenerateTestData();

            // Act
            var actual = query.AsExpandable()
                .Select(e => new
                {
                    e.Id,
                    e.FilteredDetails
                }).ToArray();

            var expected = query.AsEnumerable()
                .Select(e => new
                {
                    e.Id,
                    e.FilteredDetails
                }).ToArray();

            // Assert
            Assert.Equal(expected.Length, actual.Length);

            for (var i = 0; i < actual.Length; i++)
            {
                var a = actual[i];
                var e = expected[i];
                Assert.Equal(e.FilteredDetails.Count(), a.FilteredDetails.Count());
            }
        }

        [Fact]
        public void ExpandableAttribute_FilteredDetailsNoImpl()
        {
            // Assign
            var query = GenerateTestData();

            // Act
            
            var actual = query.AsExpandable()
                .Select(e => new
                {
                    e.Id,
                    e.FilteredDetailsNoImpl
                }).ToArray();

            Assert.Throws<NotImplementedException>(() =>
            {
                _ = query.AsEnumerable()
                    .Select(e => new
                    {
                        e.Id,
                        e.FilteredDetailsNoImpl
                    }).ToArray();
            });

            // Assert
            Assert.Equal(2, actual.Length);
        }

    }
}