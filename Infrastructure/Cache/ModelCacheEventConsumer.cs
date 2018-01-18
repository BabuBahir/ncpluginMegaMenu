using Nop.Core.Caching;
using Nop.Core.Domain.Configuration;
using Nop.Core.Events;
using Nop.Core.Infrastructure;
using Nop.Services.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Widgets.MegaMenu.Infrastructure.Cache
{
        public partial class ModelCacheEventConsumer :
                IConsumer<EntityInserted<Setting>>,
                IConsumer<EntityUpdated<Setting>>,
                IConsumer<EntityDeleted<Setting>>
        {
            /// <summary>
            /// Key for caching
            /// </summary>
            /// <remarks>
            /// {0} : picture id
            /// </remarks>
            public const string PICTURE_URL_MODEL_KEY = "Nop.plugins.widgets.megamenu.pictureurl-{0}";
            public const string PICTURE_URL_PATTERN_KEY = "Nop.plugins.widgets.megamenu";   //nivosrlider

        private readonly ICacheManager _cacheManager;

            public ModelCacheEventConsumer()
            {
                //TODO inject static cache manager using constructor
                this._cacheManager = EngineContext.Current.ContainerManager.Resolve<ICacheManager>("nop_cache_static");
            }

            public void HandleEvent(EntityInserted<Setting> eventMessage)
            {
                _cacheManager.RemoveByPattern(PICTURE_URL_PATTERN_KEY);
            }
            public void HandleEvent(EntityUpdated<Setting> eventMessage)
            {
                _cacheManager.RemoveByPattern(PICTURE_URL_PATTERN_KEY);
            }
            public void HandleEvent(EntityDeleted<Setting> eventMessage)
            {
                _cacheManager.RemoveByPattern(PICTURE_URL_PATTERN_KEY);
            }
        }
}
