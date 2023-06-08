using DogsTest.Contexts;
using DogsTest.Models;
using MySql.Data.MySqlClient;
using System.Net;
using System.Web.Http;

namespace DogsTest.Contexts
{
    public class DogContext
    {
        public string ConnectionString { get; set; }
        private DateTime _lastRequestTime;
        private int _requestCount;

        public DogContext(string connectionString)
        {
            ConnectionString = connectionString;
        }

        private MySqlConnection GetConnection()
        {
            return new MySqlConnection(ConnectionString);
        }

        private bool CanHandleRequest()
        {
            DateTime now = DateTime.Now;
            TimeSpan elapsed = now - _lastRequestTime;

            const int requestLimit = 1;
            const int secondLimit = 1;

            if (elapsed.TotalSeconds > secondLimit)
            {
                _lastRequestTime = now;
                _requestCount = 1;
                return true;
            }

            if (_requestCount < requestLimit)
            {
                _requestCount++;
                return true;
            }

            return false;
        }

        public async Task<List<Dog>> GetAllAsync(string? attribute, string? order, int? page, int? pageSize)
        {

            if (!CanHandleRequest())
            {
                throw new HttpResponseException((HttpStatusCode)429);
            }
            List<Dog> list = new List<Dog>();
            using (MySqlConnection connectionDb = GetConnection())
            {
                await connectionDb.OpenAsync();
                var command = "select * from dogs ";
                if (attribute is not null && order is not null)
                {
                    command += $"order by {attribute} {order} ";
                }
                if (page is not null && pageSize is not null)
                {
                    command += $"limit {page * pageSize}, {pageSize} ";
                }

                MySqlCommand cmd = new MySqlCommand(command, connectionDb);

                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        list.Add(new Dog()
                        {
                            Name = reader["name"].ToString(),
                            Color = reader["color"].ToString(),
                            Tail_Length = Convert.ToInt32(reader["tail_length"]),
                            Weight = Convert.ToInt32(reader["weight"]),
                        });
                    }
                }
            }
            return list;
        }

        public async Task AddDogAsync(Dog dog)
        {
            if (!CanHandleRequest())
            {
                throw new HttpResponseException((HttpStatusCode)429);
            }
            using (MySqlConnection connectionDb = GetConnection())
            {
                await connectionDb.OpenAsync();
                string insertQuery = "INSERT INTO dogs (name, color, tail_length, weight) VALUES (@value1, @value2, @value3, @value4)";

                using (MySqlCommand command = new MySqlCommand(insertQuery, connectionDb))
                {
                    command.Parameters.AddWithValue("@value1", dog.Name);
                    command.Parameters.AddWithValue("@value2", dog.Color);
                    command.Parameters.AddWithValue("@value3", dog.Tail_Length);
                    command.Parameters.AddWithValue("@value4", dog.Weight);

                    await command.ExecuteNonQueryAsync();
                }
            }
        }
    }
}
