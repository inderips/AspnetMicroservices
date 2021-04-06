using System;
using System.Threading.Tasks;
using Discount.Grpc.Entities;
using Microsoft.AspNetCore.Mvc;

namespace Discount.Grpc.Repositories
{
    public interface IDiscountRepository
    {
       
        Task<Coupon> GetDiscount(string productName);

        Task<bool> DeleteDiscount(string productName);

        Task<bool> UpdateDiscount(Coupon coupon);

        Task<bool> InsertDiscount(Coupon coupon);
    }
}
