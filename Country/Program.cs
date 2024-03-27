using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Text;

class Program
{
    static void Main()
    {

        Console.OutputEncoding = Encoding.UTF8;

        string connectionString = ConfigurationManager.ConnectionStrings["CountriesDB"].ConnectionString;

        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            connection.Open();

            DataTable countriesTable = new DataTable();
            using (SqlDataAdapter adapter = new SqlDataAdapter("SELECT * FROM Countries", connection))
            {
                adapter.Fill(countriesTable);
            }

            var allCountries = countriesTable.AsEnumerable();
            foreach (var country in allCountries)
                Console.WriteLine(
                    $"Id: {country.Field<int>("id_country"),-4}" +
                    $"Name: {country.Field<string>("country_name"),-11}" +
                    $"Population: {country.Field<long>("country_population"),-12}" +
                    $"Area: {country.Field<double>("country_area"),-10}" +
                    $"Is in EU: {country.Field<bool>("is_in_eu"),-7}" +
                    $"Continent Id: {country.Field<int>("id_continent"),-10}"
                );
            Console.WriteLine();

            var countryNames = allCountries
                .Select(row => row.Field<string>("country_name"))
                .ToList();
            foreach (var name in countryNames)
                Console.WriteLine($"Country Name: {name}");
            Console.WriteLine();

            DataTable citiesTable = new DataTable();
            using (SqlDataAdapter adapter = new SqlDataAdapter("SELECT * FROM Cities WHERE is_capital = 1", connection))
            {
                adapter.Fill(citiesTable);
            }

            var capitalNames = citiesTable.AsEnumerable()
                .Select(row => row.Field<string>("city_name"))
                .ToList();
            foreach (var name in capitalNames)
                Console.WriteLine($"Capital Name: {name}");
            Console.WriteLine();

            string countryName = "Ukraine";

            DataTable bigCitiesTable = new DataTable();
            using (SqlDataAdapter adapter = new SqlDataAdapter($"SELECT * FROM Cities WHERE city_population > 100000 AND id_region IN (SELECT id_region FROM Regions WHERE id_country IN (SELECT id_country FROM Countries WHERE country_name = '{countryName}'))", connection))
            {
                adapter.Fill(bigCitiesTable);
            }

            var bigCitiesOfCountry = bigCitiesTable.AsEnumerable()
                .Select(row => row.Field<string>("city_name"))
                .ToList();
            foreach (var name in bigCitiesOfCountry)
                Console.WriteLine($"Big City Name: {name}");
            Console.WriteLine();

            var bigCapitals = citiesTable.AsEnumerable()
                .Where(row => row.Field<long>("city_population") > 5000000
                && row.Field<bool>("is_capital"))
                .Select(row => row.Field<string>("city_name"))
                .ToList();
            foreach (var name in bigCapitals)
                Console.WriteLine($"Big Capital Name: {name}");
            Console.WriteLine();

            var europeanCountries = countriesTable.AsEnumerable()
                .Where(row => row.Field<bool>("is_in_eu"))
                .Select(row => row.Field<string>("country_name"))
                .ToList();
            foreach (var name in europeanCountries)
                Console.WriteLine($"European Country Name: {name}");
            Console.WriteLine();

            double areaRange = 480000;

            var largeCountries = countriesTable.AsEnumerable()
                .Where(row => row.Field<double>("country_area") > areaRange)
                .Select(row => row.Field<string>("country_name"))
                .ToList();
            foreach (var name in largeCountries)
                Console.WriteLine($"Large Country name: {name}");
            Console.WriteLine();




            var capitalsWithAandP = citiesTable.AsEnumerable()
                .Where(row => row.Field<string>("city_name")
                    .Contains("a", StringComparison.OrdinalIgnoreCase)
                && row.Field<string>("city_name")
                    .Contains("p", StringComparison.OrdinalIgnoreCase))
                .ToList();
            foreach (var capital in capitalsWithAandP)
                Console.WriteLine($"Capital name: {capital.Field<string>("city_name")}");
            Console.WriteLine();

            var capitalsStartingWithK = citiesTable.AsEnumerable()
                .Where(row => row.Field<string>("city_name")
                    .StartsWith("k", StringComparison.OrdinalIgnoreCase))
                .ToList();
            foreach (var capital in capitalsStartingWithK)
                Console.WriteLine($"Capital name: {capital.Field<string>("city_name")}");
            Console.WriteLine();

          
           

            double minRange = 200000;
            double maxRange = 900000;

            var countriesInAreaRange = countriesTable.AsEnumerable()
                .Where(row => row.Field<double>("country_area") >= minRange
                && row.Field<double>("country_area") <= maxRange)
                .ToList();
            foreach (var country in countriesInAreaRange)
                Console.WriteLine($"Country Name: {country.Field<string>("country_name")}");
            Console.WriteLine();

            long populationRange = 50000000;

            var countriesWithLargePopulation = countriesTable.AsEnumerable()
                .Where(row => row.Field<long>("country_population") > populationRange)
                .ToList();
            foreach (var country in countriesWithLargePopulation)
                Console.WriteLine($"Country Name: {country.Field<string>("country_name")}");
            Console.WriteLine();


            var top5CountriesByArea = countriesTable.AsEnumerable()
               .OrderByDescending(row => row.Field<double>("country_area"))
               .Take(5)
               .ToList();
            foreach (var country in top5CountriesByArea)
                Console.WriteLine(
                    $"Country Name: {country.Field<string>("country_name"),-11}" +
                    $"Area: {country.Field<double>("country_area")}"
                );
            Console.WriteLine();

            using (SqlDataAdapter adapter = new SqlDataAdapter("SELECT * FROM Cities WHERE is_capital = 1", connection))
            {
                adapter.Fill(citiesTable);
            }

            var top5CapitalsByPopulation = citiesTable.AsEnumerable()
                .OrderByDescending(row => row.Field<long>("city_population"))
                .Take(5)
                .ToList();
            foreach (var capital in top5CapitalsByPopulation)
                Console.WriteLine(
                    $"Capital Name: {capital.Field<string>("city_name"),-8}" +
                    $"Population: {capital.Field<long>("city_population")}"
                );
            Console.WriteLine();

            var countryWithLargestArea = countriesTable.AsEnumerable()
                .OrderByDescending(row => row.Field<double>("country_area"))
                .First();
            Console.WriteLine(
                $"Country with Largest Area: {countryWithLargestArea.Field<string>("country_name"),-8}" +
                $"Area: {countryWithLargestArea.Field<double>("country_area")}\n"
            );

            var capitalWithLargestPopulation = citiesTable.AsEnumerable()
                .Where(row => row.Field<bool>("is_capital"))
                .OrderByDescending(row => row.Field<long>("city_population"))
                .First();
            Console.WriteLine(
                $"Capital with Largest Population: {capitalWithLargestPopulation.Field<string>("city_name"),-8}" +
                $"Population: {capitalWithLargestPopulation.Field<long>("city_population")}\n"
            );

            var smallestEuropeanCountry = countriesTable.AsEnumerable()
                .Where(row => row.Field<bool>("is_in_eu"))
                .OrderBy(row => row.Field<double>("country_area"))
                .First();
            Console.WriteLine(
                $"Smallest European Country: {smallestEuropeanCountry.Field<string>("country_name"),-9}" +
                $"Area: {smallestEuropeanCountry.Field<double>("country_area")}\n"
            );

            var averageAreaOfEuropeanCountries = countriesTable.AsEnumerable()
                .Where(row => row.Field<bool>("is_in_eu"))
                .Average(row => row.Field<double>("country_area"));
            Console.WriteLine(
                $"Average Area of European Countries: {Math.Round(averageAreaOfEuropeanCountries, 2)}\n"
            );


            DataTable citiesOfCountryTable = new DataTable();
            using (SqlDataAdapter adapter = new SqlDataAdapter($"SELECT * FROM Cities WHERE id_region IN (SELECT id_region FROM Regions WHERE id_country IN (SELECT id_country FROM Countries WHERE country_name = '{countryName}'))", connection))
            {
                adapter.Fill(citiesOfCountryTable);
            }

            var top3CitiesOfCountry = citiesOfCountryTable.AsEnumerable()
                .OrderByDescending(row => row.Field<long>("city_population"))
                .Take(3)
                .ToList();
            foreach (var city in top3CitiesOfCountry)
                Console.WriteLine($"City Name: {city.Field<string>("city_name"),-6}" +
                    $"Population: {city.Field<long>("city_population")}\n"
                );

            var totalNumberOfCountries = countriesTable.AsEnumerable()
                .Count();
            Console.WriteLine(
                $"Total Number of Countries: {totalNumberOfCountries}\n"
            );

            DataTable continentsTable = new DataTable();
            using (SqlDataAdapter adapter = new SqlDataAdapter("SELECT * FROM Continents", connection))
            {
                adapter.Fill(continentsTable);
            }

            var continentWithMostCountries = continentsTable.AsEnumerable()
                .OrderByDescending(row => countriesTable.AsEnumerable()
                    .Count(c => c.Field<int>("id_continent") == row.Field<int>("id_continent")))
                .First();
            Console.WriteLine(
                $"Continent with Most Countries: {continentWithMostCountries.Field<string>("continent_name")}\n"
            );

            foreach (var continent in continentsTable.AsEnumerable())
            {
                var numOfCountriesInContinent = countriesTable.AsEnumerable()
                    .Count(c => c.Field<int>("id_continent") == continent.Field<int>("id_continent"));
                Console.WriteLine(
                    $"Continent: {continent.Field<string>("continent_name"),-15}" +
                    $"Number of Countries: {numOfCountriesInContinent}"
                );
            }
            Console.WriteLine();
        }
    }
    
}