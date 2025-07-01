using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Task1.DoNotChange;

namespace Task1
{
    public static class LinqTask
    {
        public static IEnumerable<Customer> Linq1(IEnumerable<Customer> customers, decimal limit)
        {
            if(customers == null || limit == null) 
                throw new ArgumentNullException();
            
            var customerQuery = from c in customers
                                where c.Orders.Count() > limit
                                select c;

            return customerQuery;
        }

        public static IEnumerable<(Customer customer, IEnumerable<Supplier> suppliers)> Linq2(
            IEnumerable<Customer> customers,
            IEnumerable<Supplier> suppliers
        )
        {
            if (customers == null || suppliers == null)
                throw new ArgumentNullException();

            var customerQuery = from c in customers
                                select (customer: c,
                                        suppliers: suppliers.Where(s => s.City == c.City && s.Country == c.Country));

            return customerQuery;
        }

        public static IEnumerable<(Customer customer, IEnumerable<Supplier> suppliers)> Linq2UsingGroup(
            IEnumerable<Customer> customers,
            IEnumerable<Supplier> suppliers
        )
        {
            if (customers == null || suppliers == null)
                throw new ArgumentNullException();

            var customerQuery = customers.Select(c => (
                customer: c,
                suppliers: suppliers.Where(s => s.City == c.City && s.Country == c.Country)
            ));

            return customerQuery;
        }

        public static IEnumerable<Customer> Linq3(IEnumerable<Customer> customers, decimal limit)
        {
            if (customers == null || limit == null)
                throw new ArgumentNullException();

            var customerQuery = customers
                .Where(c => c.Orders.Any() && c.Orders.Sum(o => o.Total) > limit);

            return customerQuery;
        }

        public static IEnumerable<(Customer customer, DateTime dateOfEntry)> Linq4(
            IEnumerable<Customer> customers
        )
        {
            if (customers == null)
                throw new ArgumentNullException();

            var customerQuery = customers
                                .Where(c => c.Orders.Any()) // Include only customers with orders
                                .Select(c => (customer: c, 
                                              dateOfEntry: c.Orders.Min(o => o.OrderDate)));

            return customerQuery;
        }

        public static IEnumerable<(Customer customer, DateTime dateOfEntry)> Linq5(
            IEnumerable<Customer> customers
        )
        {
            if (customers == null)
                throw new ArgumentNullException();

            var customerQuery = customers
                                .Where(c => c.Orders.Any()) // Filter customers with orders
                                .Select(c => (customer: c,
                                              dateOfEntry: c.Orders.Min(o => o.OrderDate))) // Extract date of entry and customer
                                .OrderBy(o => o.dateOfEntry.Year) // Order by year (ascending)
                                .ThenBy(o => o.dateOfEntry.Month) // Then by month (ascending)
                                .ThenByDescending(o => o.customer.Orders.Sum(o => o.Total)) // Then by turnover (descending)
                                .ThenBy(o => o.customer.CompanyName); // Then by customer name (alphabetical)
            return customerQuery;
        }

        public static IEnumerable<Customer> Linq6(IEnumerable<Customer> customers)
        {
            if (customers == null)
                throw new ArgumentNullException();

            var customerQuery = customers.Where(c =>
                                // a. Postal code contains non-digit characters
                                (c.PostalCode != null && !Regex.IsMatch(c.PostalCode, @"^\d+$")) ||

                                // b. Region is undefined
                                string.IsNullOrEmpty(c.Region) ||

                                // c. Phone does not contain an operator code (parentheses)
                                (c.Phone == null || (!c.Phone.Contains("(") && !c.Phone.Contains(")")))
            );

            return customerQuery;
        }

        public static IEnumerable<Linq7CategoryGroup> Linq7(IEnumerable<Product> products)
        {
            /* example of Linq7result

             category - Beverages
	            UnitsInStock - 39
		            price - 18.0000
		            price - 19.0000
	            UnitsInStock - 17
		            price - 18.0000
		            price - 19.0000
             */
            if (products == null)
                throw new ArgumentNullException();
            var categoryGroups = products
                                .GroupBy(p => p.Category) 
                                .Select(categoryGroup => new Linq7CategoryGroup
                                {
                                    Category = categoryGroup.Key,
                                    UnitsInStockGroup = categoryGroup
                                        .GroupBy(p => p.UnitsInStock)
                                        .Select(unitsInStockGroup => new Linq7UnitsInStockGroup
                                        {
                                            UnitsInStock = unitsInStockGroup.Key,
                                            Prices = unitsInStockGroup.Select(p => p.UnitPrice).OrderBy(price => price)
                                        })
                                });

            return categoryGroups;
        }

        public static IEnumerable<(decimal category, IEnumerable<Product> products)> Linq8(
            IEnumerable<Product> products,
            decimal cheap,
            decimal middle,
            decimal expensive
        )
        {
            if (products == null || cheap == null || middle == null || expensive == null)
                throw new ArgumentNullException();

            var groupedProducts = new[]
            {
                    (
                        category: cheap,
                        products: products.Where(p => p.UnitPrice <= cheap)
                    ),
                    (
                        category: middle,
                        products: products.Where(p => p.UnitPrice > cheap && p.UnitPrice <= middle)
                    ),
                    (
                        category: expensive,
                        products: products.Where(p => p.UnitPrice > middle)
                    )
                };
            return groupedProducts;
        }

        public static IEnumerable<(string city, int averageIncome, int averageIntensity)> Linq9(
            IEnumerable<Customer> customers
        )
        {
            if (customers == null)
                throw new ArgumentNullException();
            var groupedData = customers
                            .GroupBy(c => c.City) // Group by City
                            .Select(cityGroup =>
                            {
                                var customersInCity = cityGroup.ToList();

                                var averageIncome = customersInCity
                                    .Select(c => c.Orders.Sum(o => o.Total)) 
                                    .DefaultIfEmpty(0) 
                                    .Average();

                                var averageIntensity = customersInCity
                                    .Select(c => c.Orders.Count())
                                    .DefaultIfEmpty(0)
                                    .Average();

                                return (
                                    city: cityGroup.Key,
                                    averageIncome: (int)Math.Round(averageIncome),
                                    averageIntensity: (int)Math.Round(averageIntensity)
                                );
                            });

            return groupedData;
        }

        public static string Linq10(IEnumerable<Supplier> suppliers)
        {
            if (suppliers == null)
                throw new ArgumentNullException();

            var uniqueCountries = suppliers
                                .Select(s => s.Country)
                                .Distinct()
                                .OrderBy(c => c.Length)
                                .ThenBy(c => c); 
            
            return string.Join("", uniqueCountries);
        }
    }
}