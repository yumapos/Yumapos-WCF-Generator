using System;
using System.Collections.Generic;
using VersionedRepositoryGeneration.Interfaces;
using VersionedRepositoryGeneration.Interfaces.Repositories;
using VersionedRepositoryGeneration.Models;

namespace VersionedRepositoryGeneration.Repositories
{
    class MenuCategoryRepository : RepositoryBase, IMenuCategoryRepository
    {
        MenuCategoryCashRepository _menuCategoryCashRepository;
        MenuCategoryVersionRepository _menuCategoryVersionRepository;

        public MenuCategoryRepository(IDataAccessService dataAccessService) : base(dataAccessService)
        {
            _menuCategoryVersionRepository = new MenuCategoryVersionRepository(dataAccessService);
            _menuCategoryCashRepository = new MenuCategoryCashRepository(dataAccessService);
        }

        public Guid Insert(Models.MenuCategory menuCategory)
        {
            menuCategory.Modified = DateTimeOffset.Now;

            menuCategory.MenuCategoryVersionId = _menuCategoryVersionRepository.Insert(menuCategory);
            _menuCategoryCashRepository.Insert(menuCategory);

            return menuCategory.MenuCategoryVersionId;
        }

        public Guid Update(Models.MenuCategory menuCategory)
        {
            menuCategory.Modified = DateTimeOffset.Now;

            menuCategory.MenuCategoryVersionId = _menuCategoryVersionRepository.Insert(menuCategory);
            _menuCategoryCashRepository.Update(menuCategory);

            return menuCategory.MenuCategoryVersionId;
        }

        //todo GUID!!
        public Guid Remove(MenuCategory menuCategory)
        {
            menuCategory.IsDeleted = true;
            menuCategory.Modified = DateTimeOffset.Now;

            menuCategory.MenuCategoryVersionId = _menuCategoryVersionRepository.Insert(menuCategory);
            _menuCategoryCashRepository.Update(menuCategory);

            return menuCategory.MenuCategoryVersionId;
        }

        public Models.MenuCategory GetByMenuCategoryId(System.Guid menuCategoryId, bool? isDeleted = false)
        {
            return _menuCategoryCashRepository.GetByMenuCategoryId(menuCategoryId);
        }

        public IEnumerable<Models.MenuCategory> GetAll(bool? isDeleted = false)
        {
            return _menuCategoryCashRepository.GetAll();
        }

    }
}
