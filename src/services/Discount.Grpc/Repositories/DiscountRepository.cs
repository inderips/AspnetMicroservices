﻿using System;
using System.Threading.Tasks;
using Dapper;
using Discount.Grpc.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Npgsql;

namespace Discount.Grpc.Repositories
{
    public class DiscountRepository :IDiscountRepository
    {
        
        public readonly IConfiguration _configuration;

        public DiscountRepository( IConfiguration configuration)
        {
          
            _configuration = configuration;
        }

        public async Task<bool> DeleteDiscount(string productName)
        {
            using (var connection = new Npgsql.NpgsqlConnection(_configuration.GetValue<string>("DatabaseSettings.ConnectionString")))
            {
                var retVal =  await connection.ExecuteAsync("Delete from \"Coupon\" WHERE \"ProductName\"=@ProductName",new {ProductName=productName });
                if(retVal == 1)
                {
                    return true;
                }
                return false;
            }
        }

        public async Task<Coupon> GetDiscount(string productName)
        {
            using (var connection = new NpgsqlConnection(_configuration.GetValue<string>("DatabaseSettings:ConnectionString")))
            {
                var coupon = await connection.QueryFirstOrDefaultAsync<Coupon>
                    ("SELECT * FROM \"Coupon\" WHERE \"ProductName\"=@ProductName", new {ProductName=productName});

                if(coupon == null)
                {
                    return new Coupon {ProductName="No Discount", Description = "No Description", Amount = 0 };
                }

                return coupon;
            }
        }

        public async Task<bool> InsertDiscount(Coupon coupon)
        {
            using (var connection = new NpgsqlConnection(_configuration.GetValue<string>("DatabaseSettings:ConnectionString")))
            {
               var affected =  await connection.ExecuteAsync("INSERT INTO \"COUPON'\" (\"ProductName\", \"Description\", \"Amount\") " +
                   "VALUES (@ProductName, @Description, @Amount) "
                   ,new { ProductName=coupon.ProductName, Description= coupon.Description, Amount=coupon.Amount});

                if(affected == 0)
                {
                    return false;
                }
                return true;
            }
        }

        public async Task<bool> UpdateDiscount(Coupon coupon)
        {
            using (var connection = new NpgsqlConnection(_configuration.GetValue<string>("DatabaseSettings:ConnectionString")))
            {
                var affected = await connection.ExecuteAsync("UPDATE  COUPON  SET \"ProductName\"=@ProductName, \"Description\"=@Description, \"Amount\"=@Amount) ",new { ProductName = coupon.ProductName, Description = coupon.Description, Amount = coupon.Amount });

                if (affected == 0)
                {
                    return false;
                }
                return true;
            }
        }
    }
}
