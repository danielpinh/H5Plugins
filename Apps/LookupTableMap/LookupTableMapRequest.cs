using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace H5Plugins
{
    /// <summary>
    /// First, this is the enumeration of each command triggered in the MainWindow (modeless dialog).
    /// </summary>
    public enum RequestId : int
{
    None = 0,
    Leitos = 1,
    Eletrocalhas = 2,
    Eletrodutos = 3,    
    Perfilados = 4,
    Dutos = 5,
    DetalhesTipicos = 6,
    Tubos = 7,
    Sistemas = 8,
    Septos = 9,
}

/// <summary>
/// This is the class that will take the user command by the RequestId enum 
/// and make the request once the RequestHandler identifies it while the external event is being raised.
/// </summary>
public class LookupTableMapRequest
{
    private int m_request = (int)RequestId.None;

    public RequestId Take()
    {
        return (RequestId)Interlocked.Exchange(ref m_request, (int)RequestId.None);
    }

    public void Make(RequestId request)
    {
        Interlocked.Exchange(ref m_request, (int)request);
    }
}
}
