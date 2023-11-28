﻿namespace BicycleApp.Services.Services.Orders
{
    using BicicleApp.Common.Providers.Contracts;
    using BicycleApp.Data;
    using BicycleApp.Data.Models.EntityModels;
    using BicycleApp.Services.Contracts.Factory;
    using BicycleApp.Services.Contracts.OrderContracts;
    using BicycleApp.Services.HelperClasses.Contracts;
    using BicycleApp.Services.Models.Order;
    using static BicycleApp.Common.ApplicationGlobalConstants;

    using Microsoft.EntityFrameworkCore;

    using System.Text;
    using BicycleApp.Services.Models.Order.Contracts;

    public class OrderUserService : IOrderUserService
    {
        private readonly BicycleAppDbContext _db;
        private readonly IStringManipulator _stringManipulator;
        private readonly IOrderFactory _orderFactory;
        private readonly IDateTimeProvider _dateTimeProvider;

        public OrderUserService(BicycleAppDbContext db, 
                                IStringManipulator stringManipulator,
                                IOrderFactory orderFactory,
                                IDateTimeProvider dateTimeProvider)
        {
            _db = db;
            _stringManipulator = stringManipulator;
            _orderFactory = orderFactory;
            _dateTimeProvider = dateTimeProvider;
        }


        /// <summary>
        /// Creating order in database.
        /// </summary>
        /// <param name="order"></param>
        /// <returns>Task<bool></returns>
        public async Task<bool> CreateOrderByUserAsync(IUserOrderDto order)
        {
            try
            { 
                var orderToSave = _orderFactory.CreateUserOrder();
                orderToSave.ClientId = order.ClientId;
                orderToSave.DateCreated = _dateTimeProvider.Now;
                orderToSave.StatusId = 1;

                decimal totalAmount = 0M;
                decimal totalDiscount = 0M;
                decimal totalVAT = 0M;

                var vatCategory = await _db.VATCategories.AsNoTracking().FirstAsync(v => v.Id == order.VATId);

                foreach (var orderPart in order.OrderParts)
                {
                    var currentPart = await _db.Parts.FirstAsync(p => p.Id == orderPart.PartId);
                    decimal currentProductTotalPrice = Math.Round(currentPart.SalePrice * order.OrderQuantity, 2);
                    totalAmount += currentProductTotalPrice;
                    decimal currentProductTotalDiscount = Math.Round(currentPart.Discount * order.OrderQuantity, 2);                   
                    totalDiscount += currentProductTotalDiscount;
                    if (currentProductTotalDiscount > currentProductTotalPrice)
                    {
                        return false;
                    }
                    totalVAT += Math.Round(((currentProductTotalPrice - currentProductTotalDiscount) * vatCategory.VATPercent) / (100 + vatCategory.VATPercent), 2);
                }

                orderToSave.Discount = totalDiscount;
                orderToSave.FinalAmount = totalAmount - totalDiscount;
                orderToSave.VAT = totalVAT;
                orderToSave.SaleAmount = totalAmount - totalDiscount - totalVAT;

                await _db.Orders.AddAsync(orderToSave);
                await _db.SaveChangesAsync();

                ICollection<OrderPartEmployee> orderPartEmployeeCollection = new List<OrderPartEmployee>();

                string serialNumber = _stringManipulator.SerialNumberGenerator();

                foreach (var part in order.OrderParts)
                {
                    var ope = new OrderPartEmployee()
                    {
                        OrderId = orderToSave.Id,
                        PartId = part.PartId,
                        PartPrice = part.PricePerUnit,
                        PartQuantity = part.Quantity,
                        PartName = part.PartName,
                        Description = _stringManipulator.GetTextFromProperty(order.Description),
                        SerialNumber = serialNumber
                    };

                    orderPartEmployeeCollection.Add(ope);
                }

                await _db.OrdersPartsEmployees.AddRangeAsync(orderPartEmployeeCollection);
                await _db.SaveChangesAsync();

                return true;
            }
            catch (Exception)
            {
            }
            return false;
        }

        /// <summary>
        /// Takes all unfinished orders for a specific user and returns information about the workmanship of the parts.
        /// </summary>
        /// <param name="userId"></param>
        /// <returns>Task<ICollection<OrderProgretionDto>></returns>
        public async Task<ICollection<OrderProgretionDto>> GetOrdersProgresions(string userId)
        {
            return await _db.Orders
                            .Include(o => o.OrdersPartsEmployees)
                                .ThenInclude(ope => ope.Employee)
                            .Include(o => o.OrdersPartsEmployees)
                                .ThenInclude(ope => ope.Part)
                            .ThenInclude(part => part.Category)
                            .Where(o => o.ClientId == userId && o.IsDeleted == false && o.DateFinish == null)
                            .Select(o => new OrderProgretionDto()
                            {
                                OrderId = o.Id,
                                SerialNumber = o.OrdersPartsEmployees.Select(sn => sn.SerialNumber).FirstOrDefault(),
                                DateCreated = o.DateCreated.ToString(DefaultDateFormat),
                                OrderStates = o.OrdersPartsEmployees
                                               .Select(ope => new OrderStateDto()
                                               {
                                                   IsProduced = ope.IsCompleted,
                                                   NameOfEmplоyeeProducedThePart = _stringManipulator.ReturnFullName(ope.Employee.FirstName, ope.Employee.LastName),
                                                   PartModel = ope.Part.Name,
                                                   PartType = ope.Part.Category.Name,
                                                   PartId = ope.PartId
                                                   
                                               }).ToList()
                            })
                            .ToListAsync();
        }
        public async Task<ICollection<OrderProgretionDto>> AllPendingApprovalOrder(string userId)
        {
            return await _db.Orders
                            .Where(o => o.DateUpdated.Equals(null))
                            .Select(o => new OrderProgretionDto()
                            {
                                DateCreated = o.DateCreated.ToString(DefaultDateFormat),
                                OrderId = o.Id,
                                SerialNumber = o.OrdersPartsEmployees.Select(sn => sn.SerialNumber).FirstOrDefault()
                            })
                            .ToListAsync();
        }
                
    }
}
