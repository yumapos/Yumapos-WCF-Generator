using System;
using System.Collections.Generic;
using YumaPos.FrontEnd.Infrastructure.Configuration;
using YumaPos.Server.Infrastructure.DataObjects;
using YumaPos.Server.Infrastructure.Repositories;

namespace YumaPos.Server.Data.Sql.Menu
{
    class MenuCategoryRepository : RepositoryBase, IMenuCategoryRepository
    {
        MenuCategoryСacheRepository _menuCategoryСacheRepository;
        MenuCategoryVersionRepository _menuCategoryVersionRepository;

        public MenuCategoryRepository(IDataAccessService dataAccessService) : base(dataAccessService)
        {
            _menuCategoryVersionRepository = new MenuCategoryVersionRepository(dataAccessService);
            _menuCategoryСacheRepository = new MenuCategoryСacheRepository(dataAccessService);
        }

        public Guid Insert(MenuCategory menuCategory)
        {
            menuCategory.Modified = DateTimeOffset.Now;

            menuCategory.MenuCategoryVersionId = _menuCategoryVersionRepository.Insert(menuCategory);
            _menuCategoryСacheRepository.Insert(menuCategory);

            return menuCategory.MenuCategoryVersionId;
        }

        public Guid Update(MenuCategory menuCategory)
        {
            menuCategory.Modified = DateTimeOffset.Now;

            menuCategory.MenuCategoryVersionId = _menuCategoryVersionRepository.Insert(menuCategory);
            _menuCategoryСacheRepository.Update(menuCategory);

            return menuCategory.MenuCategoryVersionId;
        }

        //todo GUID!!
        public Guid Remove(MenuCategory menuCategory)
        {
            menuCategory.IsDeleted = true;
            menuCategory.Modified = DateTimeOffset.Now;

            menuCategory.MenuCategoryVersionId = _menuCategoryVersionRepository.Insert(menuCategory);
            _menuCategoryСacheRepository.Update(menuCategory);

            return menuCategory.MenuCategoryVersionId;
        }

        public MenuCategory GetByMenuCategoryId(System.Guid menuCategoryId, bool? isDeleted = false)
        {
            return _menuCategoryСacheRepository.GetByMenuCategoryId(menuCategoryId);
        }

        public IEnumerable<MenuCategory> GetAll(bool? isDeleted = false)
        {
            return _menuCategoryСacheRepository.GetAll();
        }

    }
}
