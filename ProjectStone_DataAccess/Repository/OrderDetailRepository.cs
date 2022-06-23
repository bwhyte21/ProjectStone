﻿using ProjectStone_DataAccess.Data;
using ProjectStone_DataAccess.Repository.IRepository;
using ProjectStone_Models;

namespace ProjectStone_DataAccess.Repository;

public class OrderDetailRepository : Repository<OrderDetail>, IOrderDetailRepository
{
    private readonly ApplicationDbContext _db;

    public OrderDetailRepository(ApplicationDbContext db) : base(db)
    {
        _db = db;
    }

    public void Update(OrderDetail obj)
    {
        _db.OrderDetail.Update(obj);
    }
}