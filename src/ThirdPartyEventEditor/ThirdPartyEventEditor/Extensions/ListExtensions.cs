using System.Collections.Generic;
using ThirdPartyEventEditor.Models;

namespace ThirdPartyEventEditor.Extensions
{
    public static class ListExtensions
    {
        public static int GenerateId(this List<ThirdPartyEvent> events)
        {
            for (int i = 1; i <= int.MaxValue; i++)
            {
                if (!events.Exists(e => e.Id == i))
                {
                    return i;
                }
            }

            return 1;
        }
    }
}