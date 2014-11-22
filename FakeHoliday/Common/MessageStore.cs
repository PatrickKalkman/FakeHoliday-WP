using FakeHoliday.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FakeHoliday.Common
{
    public class MessageStore : Dictionary<string, SelectedForegroundMessage>, IMessageStorage
    {
        public void Store(string key, SelectedForegroundMessage value)
        {
            this[key] = value;
        }

        public SelectedForegroundMessage Get(string key)
        {
            return this[key];
        }
    }
}
