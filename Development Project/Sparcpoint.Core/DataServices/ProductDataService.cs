﻿using Sparcpoint.Models;
using Sparcpoint.SqlServer.Abstractions;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Threading.Tasks;

namespace Sparcpoint.DataServices
{
    //EVAL: with more time I'd want to refactor this to use the ISQLExecutor and pull out repeated code to lesson repition
    public class ProductDataService : IProductDataService
    {
        private string _dbConn;

        public ProductDataService(string dbConn) => _dbConn = dbConn;

        public async Task<List<Product>> GetProducts()
        {
            var productList = new List<Product>();

            string queryString = "SELECT * FROM [Instances].[Products];";
            using (SqlConnection conn = new SqlConnection(_dbConn))
            {
                SqlCommand command = new SqlCommand(queryString, conn);
                conn.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var timestamp = DateTime.Parse(reader["CreatedTimestamp"].ToString());

                        var product = new Product()
                        {
                            InstanceId = Convert.ToInt32(reader["InstanceId"].ToString()),
                            Name = reader["Name"].ToString(),
                            Description = reader["Description"].ToString(),
                            ProductImageUris = reader["ProductImageUris"].ToString(),
                            ValidSkus = reader["ValidSkus"].ToString(),
                            CreatedTimestamp = timestamp
                        };

                        productList.Add(product);
                    }
                }

                conn.Close();
            }

            return productList;
        }

        public async Task<int> CreateProductAsync(Product newProduct)
        {
            var productList = new List<Product>();
            int createdId;

            string commandText = "INSERT [Instances].[Products] (Name, Description, ProductImageUris, ValidSkus, CreatedTimestamp) VALUES (@Name, @Description, @ProductImageUris, @ValidSkus, @CreatedTimestamp)";

            SqlParameter parameterName = new SqlParameter("@Name", newProduct.Name);
            SqlParameter parameterDescription = new SqlParameter("@Description", newProduct.Description);
            SqlParameter parameterImages = new SqlParameter("@ProductImageUris", newProduct.ProductImageUris);
            SqlParameter parameterSkus = new SqlParameter("@ValidSkus", newProduct.ValidSkus);
            SqlParameter parameterCreatedOn = new SqlParameter("@CreatedTimestamp", newProduct.CreatedTimestamp);

            using (SqlConnection conn = new SqlConnection(_dbConn))
            {
                using (SqlCommand cmd = new SqlCommand(commandText, (SqlConnection)conn))
                {
                    SqlCommand command = new SqlCommand(commandText, conn);

                    command.Parameters.Add(parameterName);
                    command.Parameters.Add(parameterDescription);
                    command.Parameters.Add(parameterImages);
                    command.Parameters.Add(parameterSkus);
                    command.Parameters.Add(parameterCreatedOn);

                    conn.Open();
                    createdId = (int)command.ExecuteScalar();

                    conn.Close();
                }
            }

            return createdId;
        }

        public async Task AddAttributeToProduct(int productId, KeyValuePair<string,string> attribute)
        {
            string commandText = "INSERT [Instances].[ProductAttributes] ([InstanceId], [Key], [Value]) VALUES (@InstanceId, @Key, @Value)";

            SqlParameter parameterProductId = new SqlParameter("@InstanceId", productId);
            SqlParameter parameterKey = new SqlParameter("@Key", attribute.Key);
            SqlParameter parameterValue = new SqlParameter("@Value", attribute.Value);

            using (SqlConnection conn = new SqlConnection(_dbConn))
            {
                using (SqlCommand cmd = new SqlCommand(commandText, (SqlConnection)conn))
                {
                    SqlCommand command = new SqlCommand(commandText, conn);

                    command.Parameters.Add(parameterProductId);
                    command.Parameters.Add(parameterKey);
                    command.Parameters.Add(parameterValue);

                    conn.Open();
                    command.ExecuteNonQuery();

                    conn.Close();
                }
            }
        }

        public async Task AddProductToCategory(int categoryId, int productId)
        {
            string commandText = "INSERT [Instances].[CategoryCategories] (InstanceId, CategoryInstanceId) VALUES (@InstanceId, @CategoryInstanceId)";

            SqlParameter parameterProductId = new SqlParameter("@InstanceId", productId);
            SqlParameter parameterCategoryId = new SqlParameter("@CategoryInstanceId", categoryId);

            using (SqlConnection conn = new SqlConnection(_dbConn))
            {
                using (SqlCommand cmd = new SqlCommand(commandText, (SqlConnection)conn))
                {
                    SqlCommand command = new SqlCommand(commandText, conn);

                    command.Parameters.Add(parameterProductId);
                    command.Parameters.Add(parameterCategoryId);

                    conn.Open();
                    command.ExecuteNonQuery();

                    conn.Close();
                }
            }
        }

        public async Task<List<Product>> SearchProducts(string keyword, List<string> searchBy, string orderBy, string orderDirection, int page, int pageCount)
        {
            return new List<Product>();
        }

        public async Task<List<KeyValuePair<string, string>>> GetAttributesForProduct(int productId)
        {
            var attributeList = new List<KeyValuePair<string, string>>();

            string queryString = "SELECT * FROM [Instances].[ProductAttributes] Where InstanceId = @InstanceId;";
            SqlParameter parameterProductId = new SqlParameter("@InstanceId", productId);
            using (SqlConnection conn = new SqlConnection(_dbConn))
            {
                SqlCommand command = new SqlCommand(queryString, conn);
                command.Parameters.Add(parameterProductId);

                conn.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var attribute = new KeyValuePair<string, string>(reader["Key"].ToString(), reader["Value"].ToString());

                        attributeList.Add(attribute);
                    }
                }

                conn.Close();
            }

            return attributeList;
        }
    }
}
