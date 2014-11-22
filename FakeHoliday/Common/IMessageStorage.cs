using FakeHoliday.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FakeHoliday.Common
{
    public interface IMessageStorage
    {
        void Store(string key, SelectedForegroundMessage value);
        SelectedForegroundMessage Get(string key);
    }
}
