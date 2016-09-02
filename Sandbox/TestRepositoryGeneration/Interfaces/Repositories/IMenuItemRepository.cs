﻿using System;
using System.Collections.Generic;
using YumaPos.FrontEnd.Infrastructure.Repositories;
using YumaPos.Server.Infrastructure.DataObjects;

namespace YumaPos.Server.Infrastructure.Repositories
{
    interface IMenuItemRepository : IRepository<MenuItem>
    {
        void Insert(MenuItem menuItem);

        void UpdateByItemId(MenuItem itemId);

        void RemoveByItemId(MenuItem itemId);

        IEnumerable<MenuItem> GetByItemId(Guid itemId, bool? isDeleted);

        IEnumerable<MenuItem> GetAll(bool? isDeleted);
    }
}