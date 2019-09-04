using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetworkCheckers
{
    class CachedUserNameProvider : IUserNameProvider
    {
        private readonly IUserNameProvider nameProvider;
        private string name;
        public CachedUserNameProvider(IUserNameProvider nameProvider)
        {
            this.nameProvider = nameProvider;
            name = nameProvider.GetName();
        }
        public string GetName()
        {
            return name;
        }

        public void SaveName(string name)
        {
            this.name = name;
            nameProvider.SaveName(name);
        }
    }
}
