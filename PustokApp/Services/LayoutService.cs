using PustokApp.Data;
using PustokApp.Models;

namespace PustokApp.Services
{
    public class LayoutService
        (PustokAppDbContext pustokAppDbContext)
    {
        private readonly PustokAppDbContext _pustokAppDbContext = pustokAppDbContext;
        public Dictionary<string,string> GetSettings()
        {
            return _pustokAppDbContext.Settings.ToDictionary(s => s.Key, s => s.Value);
        }
    }
}
