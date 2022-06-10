using Kingsoft.Utils.Http;
using Kingsoft.Utils.TypeExtensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibHostServer.Modules
{
    public static class DbModule
    {
        public static void Init(HostServer _server)
        {
            _server.Server.GetRoutes.Add(new Route("/db/get/<var>", (args, self, WriteRes, req) => // -> /db/<command>/<key>
            {
                Dictionary<string, string> query = HttpServer.Utils.DecodeQuery(
                    req.RawUrl.Split('?').Length >= 2 ? req.RawUrl.Split('?')[1] : "?error=true");
                string status = "500";

                ServerRole _role = _server.data.Self.ServerType;

                ServerInfo _info;
                string result;

                switch (_role)
                {
                    case ServerRole.standby:
                        break;
                    case ServerRole.input:
                        _info = _server.data.ProcessingServers[0];
                        result = _info.Request($"/db/get/{args[0]}");
                        break;
                    case ServerRole.processing:
                        _info = _server.data.OutputServer;
                        result = _server.dataBase[args[0]];
                        result = _info.Request($"/db/add/{query[_server.keys[0]]}_res/{result}");
                        if ((string)result.jobj<Dictionary<string, object>>()["status"] == "300")
                            result = _info.Request($"/db/set/res_{query[_server.keys[0]]}/{result}");
                        status = (string)result.jobj<Dictionary<string, object>>()["status"];
                        break;
                    case ServerRole.backup:
                        _info = _server.data.OutputServer;
                        result = _server.dataBase[args[0]];
                        result = _info.Request($"/db/add/{query[_server.keys[0]]}_res/{result}");
                        if ((string)result.jobj<Dictionary<string, object>>()["status"] == "300")
                            result = _info.Request($"/db/set/res_{query[_server.keys[0]]}/{result}");
                        status = (string)result.jobj<Dictionary<string, object>>()["status"];
                        break;
                    case ServerRole.output:
                        break;
                    case ServerRole.lonely:
                        _server.dataBase[$"res_{args[0]}"] = args[1];
                        break;
                    case ServerRole.inoutput:
                        _info = _server.data.ProcessingServers[0];
                        if (_server.dataBase.ContainsKey(args[0])) result = _info.Request($"/db/get/{args[0]}");
                        break;
                    default:
                        
                        break;
                }

                string response = new Dictionary<string, object>
                {
                    ["status"] = status
                }.json();

                byte[] dataBuffer = Encoding.UTF8.GetBytes(response);
                WriteRes(self, (dataBuffer, Encoding.UTF8, "application/json", dataBuffer.LongLength));
                return dataBuffer;
            }));

            _server.Server.GetRoutes.Add(new Route("/db/set/<var>/<var>", (args, self, WriteRes, req) => // -> /db/<command>/<key>/<value>
            {
                Dictionary<string, string> body = HttpServer.Utils.GetFPostBody(req);
                string status = "300";

                if (_server.dataBase.ContainsKey(args[0]))
                {
                    _server.dataBase[args[0]] = args[1];
                    status = "200";
                }

                string response = new Dictionary<string, object>
                {
                    ["status"] = status
                }.json();

                byte[] dataBuffer = Encoding.UTF8.GetBytes(response);
                WriteRes(self, (dataBuffer, Encoding.UTF8, "application/json", dataBuffer.LongLength));
                return dataBuffer;
            }));

            _server.Server.GetRoutes.Add(new Route("/db/add/<var>/<var>", (args, self, WriteRes, req) => // -> /db/<command>/<key>/<value>
            {
                Dictionary<string, string> body = HttpServer.Utils.GetFPostBody(req);
                string status = "300";

                if (!_server.dataBase.ContainsKey(args[0]))
                {
                    _server.dataBase[args[0]] = args[1];
                    status = "200";
                }

                string response = new Dictionary<string, object>
                {
                    ["status"] = status
                }.json();

                byte[] dataBuffer = Encoding.UTF8.GetBytes(response);
                WriteRes(self, (dataBuffer, Encoding.UTF8, "application/json", dataBuffer.LongLength));
                return dataBuffer;
            }));
        }
    }
}
