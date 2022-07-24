using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public static class ExtensionEnumerable
{

    public static T GrabOne<T>(this IList<T> ts)
    {
        return ts[RandomIndex(ts)];
    }

    public static int RandomIndex<T>(this IList<T> ts)
    {
        return Random.Range(0, ts.Count);
    }

    //public static IEnumerable<T> Combine<T,J>(T t, Func<int, J> getAt, )

}

